namespace Asun.UI.Viewports;

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

    public async ValueTask<ViewportPresentationExecutionResult<TTile>> ExecuteNextAsync(
        IViewportRenderSink<TTile> sink,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sink);

        if (!_queue.TryTakeLatest(out var packet))
        {
            return default;
        }

        ViewportPresentationBufferTransaction? bufferTransaction = null;
        using var supersedeCancellation =
            CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                _queue.GetInFlightCancellationToken(packet.Token));

        try
        {
            if (!_queue.IsCurrent(packet.Token))
            {
                _queue.TryCancel(packet.Token);

                return new(
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
                        packet.Frame.Batch.RegionCount));
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
                    () => _queue.IsCurrent(packet.Token))
                .ConfigureAwait(false);

            if (delivery.Succeeded)
            {
                if (!_queue.IsCurrent(packet.Token))
                {
                    if (bufferTransaction is ViewportPresentationBufferTransaction staleBuffer)
                        _buffers.Discard(staleBuffer);

                    _queue.TryCancel(packet.Token);

                    return new(
                        true,
                        false,
                        true,
                        packet,
                        delivery);
                }

                _buffers.Commit(
                    bufferTransaction.Value,
                    delivery.RenderedUnits,
                    frame.CommandStream.Regions,
                    () => _queue.IsCurrent(packet.Token));

                if (_queue.TryAcknowledgePresented(packet.Token))
                {
                    return new(
                        true,
                        true,
                        false,
                        packet,
                        delivery);
                }

                _queue.TryCancel(packet.Token);

                return new(
                    true,
                    false,
                    true,
                    packet,
                    delivery);
            }

            if (bufferTransaction is ViewportPresentationBufferTransaction activeBuffer)
                _buffers.Discard(activeBuffer);

            var superseded =
                delivery.Cancelled &&
                !_queue.IsCurrent(packet.Token);

            _queue.TryCancel(packet.Token);

            return new(
                true,
                false,
                superseded,
                packet,
                delivery);
        }
        catch (Exception exception)
        {
            if (bufferTransaction is ViewportPresentationBufferTransaction activeBuffer)
                _buffers.Discard(activeBuffer);

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

            return new(
                true,
                false,
                false,
                packet,
                failure);
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
