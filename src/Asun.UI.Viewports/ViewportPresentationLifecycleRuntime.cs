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
    ViewportRenderDeliveryStatistics Delivery,
    ViewportContinuousFrameStatistics Frames);

public sealed class ViewportPresentationLifecycleRuntime : IDisposable
{
    private readonly object _sync = new();
    private CancellationTokenSource? _runCancellation;
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
        ViewportRenderDeliveryStatistics delivery,
        ViewportContinuousFrameStatistics frames) =>
        new(State, generation, pendingInput, pendingDirtyFlags, delivery, frames);

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
        }
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
        }
    }

    private void ThrowIfDisposed() =>
        ObjectDisposedException.ThrowIf(_disposed != 0, this);
}
