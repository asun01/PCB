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
    private long _loopCount;
    private long _renderedFrames;
    private long _skippedLoops;
    private long _processedInputs;
    private long _supersededFrames;

    public ViewportContinuousFrameRuntime(
        ViewportRenderPipelineRuntime<TTile> pipeline,
        ViewportInputSubmissionRuntime? input = null,
        TimeSpan? idleDelay = null)
    {
        ArgumentNullException.ThrowIfNull(pipeline);

        _pipeline = pipeline;
        _input = input ?? new ViewportInputSubmissionRuntime();
        _interaction = new ViewportCompositeInputRuntime<TTile>(_pipeline.Composite);

        _idleDelay = idleDelay ?? TimeSpan.FromMilliseconds(4);

        if (_idleDelay <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(idleDelay));
    }

    public ViewportRenderPipelineRuntime<TTile> Pipeline => _pipeline;

    public ViewportInputSubmissionRuntime Input => _input;

    public ViewportRenderDeliveryTracker Delivery => _delivery;

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

        _pipeline.Invalidate(
            ViewportDirtyFlags.All,
            _pipeline.Composite.Generation);

        while (!cancellationToken.IsCancellationRequested)
        {
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

                        var delivery = await ViewportRenderDeliveryRuntime
                            .TryDeliverAsync(
                                frame,
                                sink,
                                _delivery,
                                cancellationToken)
                            .ConfigureAwait(false);

                        if (delivery.Succeeded)
                        {
                            Interlocked.Increment(ref _renderedFrames);
                        }
                        else
                        {
                            if (delivery.Status is
                                ViewportRenderDeliveryStatus.Failed or
                                ViewportRenderDeliveryStatus.Deferred)
                            {
                                _pipeline.RequeueFrame(frame);
                                _pipeline.Invalidate(
                                    frame.Submission.DirtyFlags,
                                    frame.Composite.Generation);
                            }

                            Interlocked.Increment(ref _skippedLoops);
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
                await Task.Delay(
                    _idleDelay,
                    cancellationToken)
                    .ConfigureAwait(false);
            }
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
