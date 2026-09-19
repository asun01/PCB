namespace Asun.UI.Viewports;

public readonly record struct ViewportPresentationExecutionStatistics(
    long Executed,
    long Presented,
    long Superseded,
    long Cancelled,
    long Deferred,
    long Failed,
    long RenderedUnits,
    long? LastGeneration,
    long? LastSequence);

public readonly record struct ViewportPresentationExecutionResult<TTile>(
    bool Executed,
    bool Presented,
    bool Superseded,
    ViewportPresentationPacket<TTile>? Packet,
    ViewportRenderDeliveryResult Delivery)
{
    public bool Cancelled =>
        Executed &&
        Delivery.Status == ViewportRenderDeliveryStatus.Cancelled;

    public long Generation =>
        Packet?.Token.Generation ?? Delivery.Generation;
}

public sealed class ViewportPresentationExecutionRuntime<TTile>
{
    private readonly ViewportPresentationQueueRuntime<TTile> _queue;
    private readonly ViewportPresentationBufferRuntime _buffers;
    private readonly ViewportRenderSurfaceRuntime _surface;
    private readonly ViewportRenderDeliveryTracker _delivery;
    private readonly object _statisticsSync = new();
    private long _executed;
    private long _presented;
    private long _superseded;
    private long _cancelled;
    private long _deferred;
    private long _failed;
    private long _renderedUnits;
    private long? _lastGeneration;
    private long? _lastSequence;

    public ViewportPresentationExecutionRuntime(
        ViewportPresentationQueueRuntime<TTile> queue,
        ViewportPresentationBufferRuntime buffers,
        ViewportRenderSurfaceRuntime surface,
        ViewportRenderDeliveryTracker delivery)
    {
        _queue = queue ?? throw new ArgumentNullException(nameof(queue));
        _buffers = buffers ?? throw new ArgumentNullException(nameof(buffers));
        _surface = surface ?? throw new ArgumentNullException(nameof(surface));
        _delivery = delivery ?? throw new ArgumentNullException(nameof(delivery));
    }

    public ViewportPresentationQueueRuntime<TTile> Queue => _queue;

    public ViewportPresentationBufferRuntime Buffers => _buffers;

    public ViewportRenderSurfaceRuntime Surface => _surface;

    public ViewportRenderDeliveryTracker Delivery => _delivery;

    public ViewportPresentationExecutionStatistics Statistics
    {
        get
        {
            lock (_statisticsSync)
            {
                return new(
                    Interlocked.Read(ref _executed),
                    Interlocked.Read(ref _presented),
                    Interlocked.Read(ref _superseded),
                    Interlocked.Read(ref _cancelled),
                    Interlocked.Read(ref _deferred),
                    Interlocked.Read(ref _failed),
                    Interlocked.Read(ref _renderedUnits),
                    _lastGeneration,
                    _lastSequence);
            }
        }
    }

    public void Reset()
    {
        lock (_statisticsSync)
        {
            Interlocked.Exchange(ref _executed, 0);
            Interlocked.Exchange(ref _presented, 0);
            Interlocked.Exchange(ref _superseded, 0);
            Interlocked.Exchange(ref _cancelled, 0);
            Interlocked.Exchange(ref _deferred, 0);
            Interlocked.Exchange(ref _failed, 0);
            Interlocked.Exchange(ref _renderedUnits, 0);
            _lastGeneration = null;
            _lastSequence = null;
        }
    }

    public async ValueTask<ViewportPresentationExecutionResult<TTile>> ExecuteNextAsync(
        IViewportRenderSink<TTile> sink,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sink);

        if (!_queue.TryTakeLatest(out var packet))
            return default;

        ViewportPresentationExecutionResult<TTile> Complete(
            ViewportPresentationExecutionResult<TTile> result)
        {
            Record(result);
            return result;
        }

        ViewportPresentationBufferTransaction? bufferTransaction = null;
        var commitWindowStarted = false;
        using var supersedeCancellation =
            CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                _queue.GetInFlightCancellationToken(packet.Token));

        try
        {
            if (!_queue.IsCurrent(packet.Token))
            {
                _queue.TryCancel(packet.Token);

                return Complete(new(
                    true,
                    false,
                    true,
                    packet,
                    new ViewportRenderDeliveryResult(
                        false,
                        true,
                        false,
                        packet.Frame.Composite.Generation,
                        0,
                        0,
                        Array.Empty<ViewportRenderWorkItem>(),
                        new ViewportPresentationFenceRejectedException(),
                        packet.Frame.Batch.ItemCount,
                        packet.Frame.Batch.RegionCount)));
            }

            var frame = packet.Frame;

            bufferTransaction = _buffers.Begin(
                packet.Token,
                frame.Batch.ItemCount,
                frame.CommandStream.Regions);

            var delivery = await ViewportRenderDeliveryRuntime
                .TryDeliverAsync(
                    frame,
                    sink,
                    _delivery,
                    supersedeCancellation.Token,
                    _surface,
                    () => _queue.IsCommitCurrent(packet.Token),
                    () =>
                    {
                        if (!_queue.TryBeginCommit(packet.Token))
                            return false;

                        commitWindowStarted = true;
                        return true;
                    })
                .ConfigureAwait(false);

            if (delivery.Succeeded)
            {
                if (!_queue.IsCurrent(packet.Token))
                {
                    if (bufferTransaction is ViewportPresentationBufferTransaction staleBuffer)
                        _buffers.Discard(staleBuffer);

                    _queue.TryCancel(packet.Token);

                    return Complete(new(
                        true,
                        false,
                        true,
                        packet,
                        delivery));
                }

                _buffers.Commit(
                    bufferTransaction.Value,
                    delivery.RenderedUnits,
                    frame.CommandStream.Regions,
                    () => _queue.IsCurrent(packet.Token));

                if (_queue.TryCompleteCommit(packet.Token))
                {
                    commitWindowStarted = false;
                    return Complete(new(
                        true,
                        true,
                        false,
                        packet,
                        delivery));
                }

                if (commitWindowStarted)
                {
                    _queue.TryAbortCommit(packet.Token);
                    commitWindowStarted = false;
                }

                _queue.TryCancel(packet.Token);

                return Complete(new(
                    true,
                    false,
                    true,
                    packet,
                    delivery));
            }

            if (bufferTransaction is ViewportPresentationBufferTransaction activeBuffer)
                _buffers.Discard(activeBuffer);

            var superseded =
                delivery.Cancelled &&
                !_queue.IsCurrent(packet.Token);

            if (commitWindowStarted)
            {
                _queue.TryAbortCommit(packet.Token);
                commitWindowStarted = false;
            }

            _queue.TryCancel(packet.Token);

            return Complete(new(
                true,
                false,
                superseded,
                packet,
                delivery));
        }
        catch (Exception exception)
        {
            if (bufferTransaction is ViewportPresentationBufferTransaction activeBuffer)
                _buffers.Discard(activeBuffer);

            if (commitWindowStarted)
            {
                _queue.TryAbortCommit(packet.Token);
                commitWindowStarted = false;
            }

            _queue.TryCancel(packet.Token);

            var frame = packet.Frame;
            var failure = new ViewportRenderDeliveryResult(
                false,
                exception is OperationCanceledException,
                false,
                frame.Composite.Generation,
                0,
                0,
                Array.Empty<ViewportRenderWorkItem>(),
                exception,
                frame.Batch.ItemCount,
                frame.Batch.RegionCount);

            return Complete(new(
                true,
                false,
                false,
                packet,
                failure));
        }
    }

    private void Record(ViewportPresentationExecutionResult<TTile> result)
    {
        if (!result.Executed)
            return;

        Interlocked.Increment(ref _executed);
        Interlocked.Add(ref _renderedUnits, result.Delivery.RenderedUnits);

        lock (_statisticsSync)
        {
            _lastGeneration = result.Generation;
            _lastSequence = result.Packet?.Token.Sequence;
        }

        if (result.Presented)
        {
            Interlocked.Increment(ref _presented);
            return;
        }

        if (result.Superseded)
        {
            Interlocked.Increment(ref _superseded);
            return;
        }

        switch (result.Delivery.Status)
        {
            case ViewportRenderDeliveryStatus.Cancelled:
                Interlocked.Increment(ref _cancelled);
                break;
            case ViewportRenderDeliveryStatus.Deferred:
                Interlocked.Increment(ref _deferred);
                break;
            case ViewportRenderDeliveryStatus.Failed:
                Interlocked.Increment(ref _failed);
                break;
        }
    }

    public async ValueTask RunAsync(
        IViewportRenderSink<TTile> sink,
        Func<ViewportPresentationExecutionResult<TTile>, ValueTask> onCompleted,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sink);
        ArgumentNullException.ThrowIfNull(onCompleted);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var result = await WaitAndExecuteAsync(
                    sink,
                    cancellationToken)
                    .ConfigureAwait(false);

                if (result.Executed)
                    await onCompleted(result).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (
                cancellationToken.IsCancellationRequested)
            {
                break;
            }
        }
    }

    public async ValueTask<ViewportPresentationExecutionResult<TTile>> WaitAndExecuteAsync(
        IViewportRenderSink<TTile> sink,
        CancellationToken cancellationToken = default)
    {
        await _queue
            .WaitForActivityAsync(cancellationToken)
            .ConfigureAwait(false);

        return await ExecuteNextAsync(
            sink,
            cancellationToken)
            .ConfigureAwait(false);
    }
}
