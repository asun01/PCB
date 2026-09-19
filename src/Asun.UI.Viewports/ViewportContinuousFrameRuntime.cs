namespace Asun.UI.Viewports;

public readonly record struct ViewportContinuousFrameStatistics(
    long LoopCount,
    long RenderedFrames,
    long SkippedLoops,
    long ProcessedInputs,
    long DeliveryFailures,
    long DeliveryDeferrals,
    long DeliveryCancellations,
    long SupersededFrames,
    long DeliveredUnits);

public sealed class ViewportContinuousFrameRuntime<TTile>
{
    private readonly ViewportRenderPipelineRuntime<TTile> _pipeline;
    private readonly ViewportInputSubmissionRuntime _input;
    private readonly TimeSpan _idleDelay;
    private readonly ViewportCompositeInputRuntime<TTile> _interaction;
    private readonly ViewportRenderDeliveryTracker _delivery = new();
    private readonly ViewportRenderSurfaceRuntime _surface;
    private readonly ViewportPresentationQueueRuntime<TTile> _presentationQueue;
    private readonly ViewportPresentationBufferRuntime _presentationBuffers;
    private readonly ViewportPresentationExecutionRuntime<TTile> _presentationExecution;
    private readonly object _deliveryStateSync = new();
    private ViewportRenderDeliveryResult? _lastDelivery;
    private long _loopCount;
    private long _renderedFrames;
    private long _skippedLoops;
    private long _processedInputs;
    private long _supersededFrames;

    public ViewportContinuousFrameRuntime(
        ViewportRenderPipelineRuntime<TTile> pipeline,
        ViewportInputSubmissionRuntime? input = null,
        TimeSpan? idleDelay = null,
        ViewportRenderSurfaceRuntime? surface = null)
    {
        ArgumentNullException.ThrowIfNull(pipeline);

        _pipeline = pipeline;
        _input = input ?? new ViewportInputSubmissionRuntime();
        _surface = surface ?? new ViewportRenderSurfaceRuntime();
        _presentationQueue = new ViewportPresentationQueueRuntime<TTile>();
        _presentationBuffers = new ViewportPresentationBufferRuntime();
        _presentationExecution = new ViewportPresentationExecutionRuntime<TTile>(
            _presentationQueue,
            _presentationBuffers,
            _surface,
            _delivery);
        _interaction = new ViewportCompositeInputRuntime<TTile>(_pipeline.Composite);

        _idleDelay = idleDelay ?? TimeSpan.FromMilliseconds(4);

        if (_idleDelay <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(idleDelay));
    }

    public ViewportRenderPipelineRuntime<TTile> Pipeline => _pipeline;

    public ViewportInputSubmissionRuntime Input => _input;

    public ViewportRenderDeliveryTracker Delivery => _delivery;

    public ViewportRenderSurfaceRuntime Surface => _surface;

    public ViewportPresentationQueueRuntime<TTile> PresentationQueue =>
        _presentationQueue;

    public ViewportPresentationBufferRuntime PresentationBuffers =>
        _presentationBuffers;

    public ViewportPresentationExecutionRuntime<TTile> PresentationExecution =>
        _presentationExecution;

    public ViewportRenderDeliveryResult? LastDelivery
    {
        get
        {
            lock (_deliveryStateSync)
                return _lastDelivery;
        }
    }

    public ViewportContinuousFrameStatistics Statistics
    {
        get
        {
            var delivery = _delivery.Statistics;
            return new(
                Interlocked.Read(ref _loopCount),
                Interlocked.Read(ref _renderedFrames),
                Interlocked.Read(ref _skippedLoops),
                Interlocked.Read(ref _processedInputs),
                delivery.Failed,
                delivery.Deferred,
                delivery.Cancelled,
                Interlocked.Read(ref _supersededFrames),
                delivery.RenderedUnits);
        }
    }

    public void Reset()
    {
        _input.ResetLifecycle();
        _interaction.Reset();
        _delivery.Reset();
        _surface.Reset();
        _presentationQueue.Reset();
        _presentationBuffers.Reset();

        lock (_deliveryStateSync)
            _lastDelivery = null;

        Interlocked.Exchange(ref _loopCount, 0);
        Interlocked.Exchange(ref _renderedFrames, 0);
        Interlocked.Exchange(ref _skippedLoops, 0);
        Interlocked.Exchange(ref _processedInputs, 0);
        Interlocked.Exchange(ref _supersededFrames, 0);
    }

    public async ValueTask RunAsync(
        IViewportRenderSink<TTile> sink,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sink);

        using var executionCancellation =
            CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken);

        var executionTask = _presentationExecution
            .RunAsync(
                sink,
                HandlePresentationExecutionAsync,
                executionCancellation.Token)
            .AsTask();

        try
        {
            _pipeline.Invalidate(
                ViewportDirtyFlags.All,
                _pipeline.Composite.Generation);

            while (!cancellationToken.IsCancellationRequested)
            {
                if (executionTask.IsFaulted)
                    await executionTask.ConfigureAwait(false);

                Interlocked.Increment(ref _loopCount);

            var processed = ProcessInputs();
            Interlocked.Add(ref _processedInputs, processed);

            if (_pipeline.Scheduler.PendingFlags != ViewportDirtyFlags.None)
            {
                try
                {
                    var frame = await _pipeline
                        .RefreshAsync(
                            DateTimeOffset.UtcNow,
                            includePrefetch: false,
                            cancellationToken)
                        .ConfigureAwait(false);

                    if (frame is not null && frame.Accepted)
                    {
                        var currentGeneration = _pipeline.Composite.Generation;

                        if (frame.Composite.Generation != currentGeneration)
                        {
                            Interlocked.Increment(ref _supersededFrames);

                            var dirty = _pipeline.Composite.DirtyRuntime.Flags;

                            _pipeline.Invalidate(
                                dirty == ViewportDirtyFlags.None
                                    ? ViewportDirtyFlags.All
                                    : dirty,
                                currentGeneration);

                            Interlocked.Increment(ref _skippedLoops);
                            continue;
                        }

                        if (frame.WorkPlan.IsEmpty)
                        {
                            Interlocked.Increment(ref _skippedLoops);

                            var emptyFrameDelay = _pipeline.Scheduler
                                .GetNextFrameDelay(DateTimeOffset.UtcNow);

                            await Task.Delay(
                                emptyFrameDelay > TimeSpan.Zero
                                    ? emptyFrameDelay
                                    : _idleDelay,
                                cancellationToken)
                                .ConfigureAwait(false);

                            continue;
                        }

                        if (!_presentationQueue.TryEnqueue(
                                frame,
                                out _))
                        {
                            Interlocked.Increment(ref _skippedLoops);
                            continue;
                        }
                    }
                    else
                    {
                        Interlocked.Increment(ref _skippedLoops);

                        var pacingDelay = _pipeline.Scheduler
                            .GetNextFrameDelay(DateTimeOffset.UtcNow);

                        await Task.Delay(
                            pacingDelay > TimeSpan.Zero
                                ? pacingDelay
                                : _idleDelay,
                            cancellationToken)
                            .ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException) when (
                    !cancellationToken.IsCancellationRequested)
                {
                    Interlocked.Increment(ref _skippedLoops);
                }
            }
            else
            {
                Interlocked.Increment(ref _skippedLoops);
                await WaitForActivityAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
        }
        }
        finally
        {
            executionCancellation.Cancel();

            try
            {
                await executionTask.ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (
                cancellationToken.IsCancellationRequested)
            {
            }
        }
    }

    private async ValueTask HandlePresentationExecutionAsync(
        ViewportPresentationExecutionResult<TTile> execution)
    {
        if (!execution.Executed)
            return;

        lock (_deliveryStateSync)
            _lastDelivery = execution.Delivery;

        var frame = execution.Packet?.Frame;

        if (execution.Presented && frame is not null)
        {
            _pipeline.MarkPresented(frame);
            Interlocked.Increment(ref _renderedFrames);
            return;
        }

        if (execution.Superseded && frame is not null)
        {
            Interlocked.Increment(ref _supersededFrames);
            _pipeline.Invalidate(
                frame.Submission.DirtyFlags,
                _pipeline.Composite.Generation);
            return;
        }

        if (frame is not null &&
            execution.Delivery.Status is
                ViewportRenderDeliveryStatus.Failed or
                ViewportRenderDeliveryStatus.Deferred)
        {
            _pipeline.RequeueFrame(
                frame,
                execution.Delivery.Deferred
                    ? execution.Delivery.DeferredWorkItems
                    : null);
            _pipeline.Invalidate(
                frame.Submission.DirtyFlags,
                frame.Composite.Generation);
        }

        Interlocked.Increment(ref _skippedLoops);
    }

    private async ValueTask WaitForActivityAsync(
        CancellationToken cancellationToken)
    {
        using var wakeCancellation = CancellationTokenSource
            .CreateLinkedTokenSource(cancellationToken);

        var inputTask = _input
            .WaitForActivityAsync(wakeCancellation.Token)
            .AsTask();

        var renderTask = _pipeline.Scheduler
            .WaitForActivityAsync(wakeCancellation.Token)
            .AsTask();

        var completed = await Task
            .WhenAny(inputTask, renderTask)
            .ConfigureAwait(false);

        wakeCancellation.Cancel();

        try
        {
            await completed.ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (
            cancellationToken.IsCancellationRequested)
        {
        }
        catch (OperationCanceledException)
        {
        }
    }

    public int ProcessInputs(int maxCount = 256)
    {
        var events = _input.Drain(maxCount);
        var processed = 0;

        foreach (var input in events)
        {
            processed++;

            var result = _interaction.Apply(input);

            if (result.DirtyFlags != ViewportDirtyFlags.None)
            {
                _pipeline.Invalidate(
                    result.DirtyFlags,
                    _pipeline.Composite.Generation);
            }
        }

        return processed;
    }
}
