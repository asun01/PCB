namespace Asun.UI.Viewports;

public readonly record struct ViewportContinuousFrameStatistics(
    long LoopCount,
    long RenderedFrames,
    long SkippedLoops,
    long ProcessedInputs);

public sealed class ViewportContinuousFrameRuntime<TTile>
{
    private readonly ViewportRenderPipelineRuntime<TTile> _pipeline;
    private readonly ViewportInputSubmissionRuntime _input;
    private readonly TimeSpan _idleDelay;
    private readonly ViewportCompositeInputRuntime<TTile> _interaction;
    private long _loopCount;
    private long _renderedFrames;
    private long _skippedLoops;
    private long _processedInputs;

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

    public ViewportContinuousFrameStatistics Statistics =>
        new(
            Interlocked.Read(ref _loopCount),
            Interlocked.Read(ref _renderedFrames),
            Interlocked.Read(ref _skippedLoops),
            Interlocked.Read(ref _processedInputs));

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
                        await ViewportRenderAdapterRuntime
                            .RenderAsync(
                                frame,
                                sink,
                                cancellationToken)
                            .ConfigureAwait(false);

                        Interlocked.Increment(ref _renderedFrames);
                    }
                    else
                    {
                        Interlocked.Increment(ref _skippedLoops);
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
