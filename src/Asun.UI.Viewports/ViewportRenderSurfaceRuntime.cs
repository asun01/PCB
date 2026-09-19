namespace Asun.UI.Viewports;

public enum ViewportRenderSurfaceState
{
    Idle,
    Rendering,
    Presented,
    Discarded,
    Disposed
}

public readonly record struct ViewportRenderSurfaceTransaction(
    long Generation,
    long Sequence);

public readonly record struct ViewportRenderSurfaceSnapshot(
    ViewportRenderSurfaceState State,
    long? RenderingGeneration,
    long? RenderingSequence,
    long? PresentedGeneration,
    long? DiscardedGeneration,
    ViewportRenderDeliveryStatus? LastDiscardStatus,
    long PresentationSequence,
    int LastRenderedUnits,
    int LastPlannedUnits,
    IReadOnlyList<System.Drawing.RectangleF> PresentedRegions)
{
    public int PresentedRegionCount => PresentedRegions.Count;
}

public sealed class ViewportRenderSurfaceRuntime : IDisposable
{
    private readonly object _sync = new();
    private int _disposed;
    private ViewportRenderSurfaceState _state = ViewportRenderSurfaceState.Idle;
    private long? _renderingGeneration;
    private long? _renderingSequence;
    private long? _presentedGeneration;
    private long? _discardedGeneration;
    private ViewportRenderDeliveryStatus? _lastDiscardStatus;
    private long _transactionSequence;
    private long _presentationSequence;
    private int _lastRenderedUnits;
    private int _lastPlannedUnits;
    private IReadOnlyList<System.Drawing.RectangleF> _presentedRegions =
        Array.Empty<System.Drawing.RectangleF>();

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
                    _renderingSequence,
                    _presentedGeneration,
                    _discardedGeneration,
                    _lastDiscardStatus,
                    _presentationSequence,
                    _lastRenderedUnits,
                    _lastPlannedUnits,
                    _presentedRegions);
            }
        }
    }

    public ViewportRenderSurfaceTransaction Begin(long generation)
    {
        lock (_sync)
        {
            ThrowIfDisposed();

            if (_presentedGeneration is long presented &&
                generation < presented)
            {
                throw new InvalidOperationException(
                    "A stale render generation cannot replace a newer presented generation.");
            }

            if (_state == ViewportRenderSurfaceState.Rendering)
                throw new InvalidOperationException(
                    "A render surface transaction is already in progress.");

            var transaction = new ViewportRenderSurfaceTransaction(
                generation,
                ++_transactionSequence);

            _state = ViewportRenderSurfaceState.Rendering;
            _renderingGeneration = generation;
            _renderingSequence = transaction.Sequence;

            return transaction;
        }
    }

    public void Commit(
        ViewportRenderSurfaceTransaction transaction,
        int plannedUnits,
        int renderedUnits,
        IReadOnlyList<System.Drawing.RectangleF>? regions = null)
    {
        if (plannedUnits < 0)
            throw new ArgumentOutOfRangeException(nameof(plannedUnits));

        if (renderedUnits < 0)
            throw new ArgumentOutOfRangeException(nameof(renderedUnits));

        lock (_sync)
        {
            ThrowIfDisposed();

            if (_state != ViewportRenderSurfaceState.Rendering ||
                _renderingGeneration != transaction.Generation ||
                _renderingSequence != transaction.Sequence)
                throw new InvalidOperationException(
                    "Only the active surface transaction can be committed.");

            _lastPlannedUnits = plannedUnits;
            _lastRenderedUnits = renderedUnits;
            _presentedRegions = regions is null
                ? Array.Empty<System.Drawing.RectangleF>()
                : regions.ToArray();
            _presentedGeneration = generation;
            _presentationSequence++;
            _renderingGeneration = null;
            _state = ViewportRenderSurfaceState.Presented;
        }
    }

    public void Discard(
        ViewportRenderSurfaceTransaction transaction,
        ViewportRenderDeliveryStatus status = ViewportRenderDeliveryStatus.Failed)
    {
        lock (_sync)
        {
            ThrowIfDisposed();

            if (_state != ViewportRenderSurfaceState.Rendering ||
                _renderingGeneration != transaction.Generation ||
                _renderingSequence != transaction.Sequence)
                return;

            _renderingGeneration = null;
            _renderingSequence = null;
            _discardedGeneration = transaction.Generation;
            _lastDiscardStatus = status;
            _state = ViewportRenderSurfaceState.Discarded;
        }
    }

    public void Reset()
    {
        lock (_sync)
        {
            ThrowIfDisposed();

            _renderingGeneration = null;
            _renderingSequence = null;
            _presentedGeneration = null;
            _discardedGeneration = null;
            _lastDiscardStatus = null;
            _transactionSequence = 0;
            _presentationSequence = 0;
            _lastRenderedUnits = 0;
            _lastPlannedUnits = 0;
            _presentedRegions = Array.Empty<System.Drawing.RectangleF>();
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
            _renderingSequence = null;
        }
    }

    private void ThrowIfDisposed() =>
        ObjectDisposedException.ThrowIf(_disposed != 0, this);
}
