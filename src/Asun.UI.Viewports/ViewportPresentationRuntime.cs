using System.Numerics;

namespace Asun.UI.Viewports;

public sealed class ViewportPresentationRuntime<TTile> : IDisposable
{
    private readonly ViewportRenderPipelineRuntime<TTile> _pipeline;
    private readonly ViewportInputSubmissionRuntime _input;
    private readonly ViewportContinuousFrameRuntime<TTile> _continuous;
    private readonly ViewportPresentationLifecycleRuntime _lifecycle = new();
    private int _disposed;

    public ViewportPresentationRuntime(
        Vector2 imageSize,
        Vector2 viewportSize,
        Vector2 tileSize,
        int prefetchMarginTiles,
        int cacheCapacity,
        int maxConcurrency,
        ITileSource<TTile> tileSource,
        ViewportRenderBudget? budget = null,
        double framesPerSecond = 60,
        TimeSpan? idleDelay = null,
        RoiEditorMode roiMode = RoiEditorMode.Select)
    {
        _pipeline = new ViewportRenderPipelineRuntime<TTile>(
            imageSize,
            viewportSize,
            tileSize,
            prefetchMarginTiles,
            cacheCapacity,
            maxConcurrency,
            tileSource,
            budget,
            framesPerSecond,
            roiMode);

        _input = new ViewportInputSubmissionRuntime();
        _continuous = new ViewportContinuousFrameRuntime<TTile>(
            _pipeline,
            _input,
            idleDelay);
    }

    public ViewportRenderPipelineRuntime<TTile> Pipeline => _pipeline;

    public ViewportInputSubmissionRuntime Input => _input;

    public ViewportContinuousFrameRuntime<TTile> Continuous => _continuous;

    public ViewportCompositeRuntime<TTile> Composite => _pipeline.Composite;

    public ViewportRenderDeliveryTracker Delivery => _continuous.Delivery;

    public ViewportPresentationState State => _lifecycle.State;

    public ViewportPresentationSnapshot Snapshot =>
        _lifecycle.Capture(
            _pipeline.Composite.Generation,
            _input.PendingCount,
            _pipeline.Scheduler.PendingFlags,
            _continuous.Delivery.Statistics,
            _continuous.Statistics);

    public long Submit(
        ViewportInputEventKind kind,
        Vector2 position,
        int wheelDelta = 0,
        ViewportMouseButton button = ViewportMouseButton.Left)
    {
        ThrowIfDisposed();

        return _input.Submit(
            kind,
            position,
            wheelDelta,
            button);
    }

    public bool TrySubmit(
        ViewportInputEventKind kind,
        Vector2 position,
        int wheelDelta = 0,
        ViewportMouseButton button = ViewportMouseButton.Left)
    {
        ThrowIfDisposed();

        return _input.TrySubmit(
            kind,
            position,
            wheelDelta,
            button);
    }

    public async ValueTask RunAsync(
        IViewportRenderSink<TTile> sink,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(sink);

        if (!_lifecycle.TryStart(out var lifecycleToken))
            throw new InvalidOperationException(
                "The presentation runtime is already running.");

        using var linked = CancellationTokenSource.CreateLinkedTokenSource(
            lifecycleToken,
            cancellationToken);

        try
        {
            await _continuous
                .RunAsync(sink, linked.Token)
                .ConfigureAwait(false);
        }
        finally
        {
            _lifecycle.MarkStopped();
        }
    }

    public ViewportContinuousFrameStatistics Statistics =>
        _continuous.Statistics;

    public ViewportRenderDeliveryStatistics DeliveryStatistics =>
        _continuous.Delivery.Statistics;

    public void Stop() => _lifecycle.RequestStop();

    public void Reset()
    {
        ThrowIfDisposed();
        _lifecycle.Reset();
        _continuous.Reset();
        _pipeline.Reset();
    }

    public void CancelInput()
    {
        ThrowIfDisposed();
        _input.Cancel();
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
            return;

        _lifecycle.RequestStop();
        _input.Dispose();
        _pipeline.Dispose();
        _lifecycle.Dispose();
    }

    private void ThrowIfDisposed() =>
        ObjectDisposedException.ThrowIf(_disposed != 0, this);
}
