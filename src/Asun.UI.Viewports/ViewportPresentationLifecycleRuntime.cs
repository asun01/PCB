namespace Asun.UI.Viewports;

public enum ViewportPresentationState
{
    Created,
    Running,
    Stopping,
    Stopped,
    Disposed
}

public readonly record struct ViewportPresentationSnapshot(
    ViewportPresentationState State,
    long Generation,
    int PendingInput,
    ViewportDirtyFlags PendingDirtyFlags,
    ViewportInputBackpressureSnapshot Backpressure,
    ViewportRenderSchedulerStatistics Scheduler,
    ViewportRenderDeliveryStatistics Delivery,
    ViewportPresentationQueueStatistics Queue,
    ViewportPresentationExecutionStatistics Execution,
    ViewportContinuousFrameStatistics Frames,
    ViewportRenderFrameState? LastFrameState,
    ViewportRenderSurfaceSnapshot Surface,
    ViewportPresentationBufferSnapshot Buffer,
    bool IsGenerationStable,
    bool IsSurfaceStable,
    bool IsBufferStable,
    bool IsQueueStable)
{
    public bool IsPresentationStable =>
        IsGenerationStable &&
        IsSurfaceStable &&
        IsBufferStable &&
        IsQueueStable;
}

public sealed class ViewportPresentationLifecycleRuntime : IDisposable
{
    private readonly object _sync = new();
    private CancellationTokenSource? _runCancellation;
    private TaskCompletionSource<bool>? _runStopped;
    private int _disposed;
    private ViewportPresentationState _state = ViewportPresentationState.Created;

    public ViewportPresentationState State
    {
        get { lock (_sync) return _state; }
    }

    public ViewportPresentationSnapshot Capture(
        long generation,
        int pendingInput,
        ViewportDirtyFlags pendingDirtyFlags,
        ViewportInputBackpressureSnapshot backpressure,
        ViewportRenderSchedulerStatistics scheduler,
        ViewportRenderDeliveryStatistics delivery,
        ViewportPresentationQueueStatistics queue,
        ViewportPresentationExecutionStatistics execution,
        ViewportContinuousFrameStatistics frames,
        ViewportRenderFrameState? lastFrameState,
        ViewportRenderSurfaceSnapshot surface,
        ViewportPresentationBufferSnapshot buffer,
        bool isGenerationStable,
        bool isSurfaceStable,
        bool isBufferStable,
        bool isQueueStable) =>
        new(
            State,
            generation,
            pendingInput,
            pendingDirtyFlags,
            backpressure,
            scheduler,
            delivery,
            queue,
            execution,
            frames,
            lastFrameState,
            surface,
            buffer,
            isGenerationStable,
            isSurfaceStable,
            isBufferStable,
            isQueueStable);

    public bool TryStart(out CancellationToken token)
    {
        lock (_sync)
        {
            ThrowIfDisposed();

            if (_state == ViewportPresentationState.Running ||
                _state == ViewportPresentationState.Stopping)
            {
                token = default;
                return false;
            }

            _runCancellation?.Dispose();
            _runCancellation = new CancellationTokenSource();
            _runStopped = new TaskCompletionSource<bool>(
                TaskCreationOptions.RunContinuationsAsynchronously);
            _state = ViewportPresentationState.Running;
            token = _runCancellation.Token;
            return true;
        }
    }

    public void RequestStop()
    {
        lock (_sync)
        {
            if (_state == ViewportPresentationState.Disposed)
                return;

            if (_state == ViewportPresentationState.Running)
                _state = ViewportPresentationState.Stopping;

            _runCancellation?.Cancel();
        }
    }

    public void MarkStopped()
    {
        lock (_sync)
        {
            if (_state != ViewportPresentationState.Disposed)
                _state = ViewportPresentationState.Stopped;

            _runStopped?.TrySetResult(true);
        }
    }

    public async ValueTask WaitForStopAsync(
        CancellationToken cancellationToken = default)
    {
        Task? completion;

        lock (_sync)
        {
            ThrowIfDisposed();

            if (_state != ViewportPresentationState.Running &&
                _state != ViewportPresentationState.Stopping)
            {
                return;
            }

            completion = _runStopped?.Task;
        }

        if (completion is not null)
            await completion.WaitAsync(cancellationToken).ConfigureAwait(false);
    }

    public void Reset()
    {
        lock (_sync)
        {
            ThrowIfDisposed();

            if (_state == ViewportPresentationState.Running ||
                _state == ViewportPresentationState.Stopping)
                throw new InvalidOperationException(
                    "A running presentation must be stopped before reset.");

            _state = ViewportPresentationState.Created;
        }
    }

    public void Dispose()
    {
        lock (_sync)
        {
            if (_disposed != 0)
                return;

            _disposed = 1;
            _state = ViewportPresentationState.Disposed;
            _runCancellation?.Cancel();
            _runCancellation?.Dispose();
            _runCancellation = null;
            _runStopped?.TrySetResult(true);
            _runStopped = null;
        }
    }

    private void ThrowIfDisposed() =>
        ObjectDisposedException.ThrowIf(_disposed != 0, this);
}
