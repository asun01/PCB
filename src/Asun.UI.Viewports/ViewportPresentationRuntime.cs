using System.Numerics;

namespace Asun.UI.Viewports;

public sealed class ViewportPresentationRuntime<TTile> : IDisposable
{
    private readonly ViewportRenderPipelineRuntime<TTile> _pipeline;
    private readonly ViewportInputSubmissionRuntime _input;
    private readonly ViewportContinuousFrameRuntime<TTile> _continuous;
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

        await _continuous
            .RunAsync(sink, cancellationToken)
            .ConfigureAwait(false);
    }

    public ViewportContinuousFrameStatistics Statistics =>
        _continuous.Statistics;

    public ViewportRenderDeliveryStatistics DeliveryStatistics =>
        _continuous.Delivery.Statistics;

    public void Reset()
    {
        ThrowIfDisposed();
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

        _input.Dispose();
        _pipeline.Dispose();
    }

    private void ThrowIfDisposed() =>
        ObjectDisposedException.ThrowIf(_disposed != 0, this);
}
