using System.Drawing;

namespace Asun.UI.Viewports;

public enum ViewportRenderReplayOperationKind
{
    Begin,
    DrawTile,
    DrawRoi,
    DrawOverlay,
    ClearInvalidatedRegion,
    End,
    Commit,
    Discard
}

public readonly record struct ViewportRenderReplayOperation(
    int Sequence,
    ViewportRenderReplayOperationKind Kind,
    long Generation,
    TileIndex? Tile,
    Guid RoiId,
    RectangleF Bounds,
    int RenderedUnits,
    ViewportRenderDeliveryStatus? DeliveryStatus);

public readonly record struct ViewportRenderReplaySnapshot(
    int OperationCount,
    long? LastGeneration,
    int BeginCount,
    int EndCount,
    int TileCount,
    int RoiCount,
    int OverlayCount,
    int InvalidationCount,
    int CommitCount,
    int DiscardCount,
    int RenderedUnits,
    IReadOnlyList<ViewportRenderReplayOperation> Operations);

/// <summary>
/// Deterministic, vendor-neutral render sink used for replay/golden evidence.
/// It records the logical rendering contract without depending on WPF, Skia,
/// DevExpress, HALCON, or a hardware SDK.
/// </summary>
public sealed class ViewportRenderReplaySink<TTile> : IViewportRenderSink<TTile>
{
    private readonly object _sync = new();
    private readonly List<ViewportRenderReplayOperation> _operations = new();
    private long? _activeGeneration;
    private long? _lastEndedGeneration;
    private bool _frameOpen;
    private int _sequence;
    private int _renderedUnits;

    public ViewportRenderReplaySnapshot Snapshot
    {
        get
        {
            lock (_sync)
            {
                return new(
                    _operations.Count,
                    _operations.Count == 0
                        ? null
                        : _operations[^1].Generation,
                    _operations.Count(operation =>
                        operation.Kind == ViewportRenderReplayOperationKind.Begin),
                    _operations.Count(operation =>
                        operation.Kind == ViewportRenderReplayOperationKind.End),
                    _operations.Count(operation =>
                        operation.Kind == ViewportRenderReplayOperationKind.DrawTile),
                    _operations.Count(operation =>
                        operation.Kind == ViewportRenderReplayOperationKind.DrawRoi),
                    _operations.Count(operation =>
                        operation.Kind == ViewportRenderReplayOperationKind.DrawOverlay),
                    _operations.Count(operation =>
                        operation.Kind == ViewportRenderReplayOperationKind.ClearInvalidatedRegion),
                    _operations.Count(operation =>
                        operation.Kind == ViewportRenderReplayOperationKind.Commit),
                    _operations.Count(operation =>
                        operation.Kind == ViewportRenderReplayOperationKind.Discard),
                    _renderedUnits,
                    _operations.ToArray());
            }
        }
    }

    public void Reset()
    {
        lock (_sync)
        {
            _operations.Clear();
            _activeGeneration = null;
            _lastEndedGeneration = null;
            _frameOpen = false;
            _sequence = 0;
            _renderedUnits = 0;
        }
    }

    public ValueTask BeginFrameAsync(
        ViewportRenderFrameContext context,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            if (_frameOpen)
                throw new InvalidOperationException(
                    "Replay sink cannot begin a second frame before the active frame ends.");

            _frameOpen = true;
            _activeGeneration = context.Generation;
            _lastEndedGeneration = null;
            Add(
                ViewportRenderReplayOperationKind.Begin,
                context.Generation,
                default,
                Guid.Empty,
                context.ViewportBounds,
                0,
                null);
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask DrawTileAsync(
        ViewportRenderTileContext<TTile> tile,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            EnsureFrame(tile.Generation);

            Add(
                ViewportRenderReplayOperationKind.DrawTile,
                tile.Generation,
                tile.Index,
                Guid.Empty,
                tile.ViewportBounds,
                1,
                null);

            _renderedUnits++;
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask DrawRoiAsync(
        ViewportRenderRoiContext roi,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            EnsureFrame(roi.Generation);

            Add(
                ViewportRenderReplayOperationKind.DrawRoi,
                roi.Generation,
                default,
                roi.Command.RoiId,
                roi.ClipBounds,
                1,
                null);

            _renderedUnits++;
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask DrawOverlayAsync(
        ViewportRenderOverlayContext overlay,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            EnsureFrame(overlay.Generation);

            Add(
                ViewportRenderReplayOperationKind.DrawOverlay,
                overlay.Generation,
                default,
                Guid.Empty,
                overlay.ViewportBounds,
                1,
                null);

            _renderedUnits++;
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask ClearInvalidatedRegionAsync(
        ViewportRenderInvalidationContext invalidation,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            EnsureFrame(invalidation.Generation);

            Add(
                ViewportRenderReplayOperationKind.ClearInvalidatedRegion,
                invalidation.Generation,
                default,
                Guid.Empty,
                invalidation.Bounds,
                1,
                null);

            _renderedUnits++;
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask EndFrameAsync(
        ViewportRenderFrameContext context,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            EnsureFrame(context.Generation);

            Add(
                ViewportRenderReplayOperationKind.End,
                context.Generation,
                default,
                Guid.Empty,
                context.ViewportBounds,
                0,
                null);

            _frameOpen = false;
            _activeGeneration = null;
            _lastEndedGeneration = context.Generation;
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask CommitFrameAsync(
        ViewportRenderCommitContext context,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            EnsureGeneration(context.Generation);

            Add(
                ViewportRenderReplayOperationKind.Commit,
                context.Generation,
                default,
                Guid.Empty,
                context.Regions.Count == 0
                    ? RectangleF.Empty
                    : ViewportRenderRegionRuntime.Merge(context.Regions)
                        .Aggregate(
                            RectangleF.Empty,
                            Union),
                context.RenderedUnits,
                ViewportRenderDeliveryStatus.Succeeded);
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask DiscardFrameAsync(
        ViewportRenderDiscardContext context,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            EnsureGeneration(context.Generation);

            Add(
                ViewportRenderReplayOperationKind.Discard,
                context.Generation,
                default,
                Guid.Empty,
                RectangleF.Empty,
                context.RenderedUnits,
                context.Status);
        }

        return ValueTask.CompletedTask;
    }

    private void EnsureFrame(long generation)
    {
        if (!_frameOpen ||
            _activeGeneration != generation)
        {
            throw new InvalidOperationException(
                "Replay sink received a draw operation outside the active frame.");
        }
    }

    private void EnsureGeneration(long generation)
    {
        var expected = _activeGeneration ?? _lastEndedGeneration;

        if (expected is null || expected.Value != generation)
        {
            throw new InvalidOperationException(
                "Replay sink generation does not match the most recently delivered frame.");
        }
    }

    private void Add(
        ViewportRenderReplayOperationKind kind,
        long generation,
        TileIndex? tile,
        Guid roiId,
        RectangleF bounds,
        int renderedUnits,
        ViewportRenderDeliveryStatus? deliveryStatus)
    {
        _operations.Add(
            new ViewportRenderReplayOperation(
                ++_sequence,
                kind,
                generation,
                tile,
                roiId,
                bounds,
                renderedUnits,
                deliveryStatus));
    }

    private static RectangleF Union(
        RectangleF left,
        RectangleF right)
    {
        if (left.IsEmpty)
            return right;

        return RectangleF.Union(left, right);
    }
}
