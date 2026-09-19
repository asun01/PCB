namespace Asun.UI.Viewports;

public enum ViewportRenderSurfaceState
{
    Idle,
    Rendering,
    Presented,
    Discarded,
    Disposed
}

public readonly record struct ViewportRenderSurfaceSnapshot(
    ViewportRenderSurfaceState State,
    long? RenderingGeneration,
    long? PresentedGeneration,
    long PresentationSequence,
    int LastRenderedUnits,
    int LastPlannedUnits);

public sealed class ViewportRenderSurfaceRuntime : IDisposable
{
    private readonly object _sync = new();
    private int _disposed;
    private ViewportRenderSurfaceState _state = ViewportRenderSurfaceState.Idle;
    private long? _renderingGeneration;
    private long? _presentedGeneration;
    private long _presentationSequence;
    private int _lastRenderedUnits;
    private int _lastPlannedUnits;

    public ViewportRenderSurfaceState State
    {
        get
        {
            lock (_sync)
                return _state;
        }
    }

    public ViewportRenderSurfaceSnapshot Snapshot
    {
        get
        {
            lock (_sync)
            {
                return new(
                    _state,
                    _renderingGeneration,
                    _presentedGeneration,
                    _presentationSequence,
                    _lastRenderedUnits,
                    _lastPlannedUnits);
            }
        }
    }

    public void Begin(long generation)
    {
        lock (_sync)
        {
            ThrowIfDisposed();

            if (_state == ViewportRenderSurfaceState.Rendering)
            {
                if (_renderingGeneration == generation)
                    return;

                throw new InvalidOperationException(
                    "A different render generation is already in progress.");
            }

            _state = ViewportRenderSurfaceState.Rendering;
            _renderingGeneration = generation;
        }
    }

    public void Commit(
        long generation,
        int plannedUnits,
        int renderedUnits)
    {
        if (plannedUnits < 0)
            throw new ArgumentOutOfRangeException(nameof(plannedUnits));

        if (renderedUnits < 0)
            throw new ArgumentOutOfRangeException(nameof(renderedUnits));

        lock (_sync)
        {
            ThrowIfDisposed();

            if (_state != ViewportRenderSurfaceState.Rendering ||
                _renderingGeneration != generation)
                throw new InvalidOperationException(
                    "Only the active rendering generation can be committed.");

            _lastPlannedUnits = plannedUnits;
            _lastRenderedUnits = renderedUnits;
            _presentedGeneration = generation;
            _presentationSequence++;
            _renderingGeneration = null;
            _state = ViewportRenderSurfaceState.Presented;
        }
    }

    public void Discard(long generation)
    {
        lock (_sync)
        {
            ThrowIfDisposed();

            if (_state != ViewportRenderSurfaceState.Rendering ||
                _renderingGeneration != generation)
                return;

            _renderingGeneration = null;
            _state = ViewportRenderSurfaceState.Discarded;
        }
    }

    public void Reset()
    {
        lock (_sync)
        {
            ThrowIfDisposed();

            _renderingGeneration = null;
            _state = ViewportRenderSurfaceState.Idle;
        }
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
            return;

        lock (_sync)
        {
            _state = ViewportRenderSurfaceState.Disposed;
            _renderingGeneration = null;
        }
    }

    private void ThrowIfDisposed() =>
        ObjectDisposedException.ThrowIf(_disposed != 0, this);
}
