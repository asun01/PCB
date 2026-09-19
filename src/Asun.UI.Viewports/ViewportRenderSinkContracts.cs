using System.Drawing;
using System.Numerics;

namespace Asun.UI.Viewports;

public readonly record struct ViewportRenderTileContext<TTile>(
    TileIndex Index,
    TileRequest Request,
    TTile Tile,
    RectangleF ViewportBounds,
    long Generation);

public readonly record struct ViewportRenderRoiContext(
    RoiRenderCommand Command,
    RectangleF ClipBounds,
    long Generation);

public readonly record struct ViewportRenderFrameContext(
    long Generation,
    RectangleF ViewportBounds,
    ViewportDirtyFlags DirtyFlags,
    int WorkItemCount);

public readonly record struct ViewportRenderOverlayContext(
    RectangleF ViewportBounds,
    ViewportDirtyFlags DirtyFlags,
    long Generation);

public readonly record struct ViewportRenderInvalidationContext(
    RectangleF Bounds,
    long Generation);

public readonly record struct ViewportRenderCommitContext(
    long Generation,
    int PlannedUnits,
    int RenderedUnits,
    int RegionCount,
    IReadOnlyList<RectangleF> Regions);

public readonly record struct ViewportRenderDiscardContext(
    long Generation,
    ViewportRenderDeliveryStatus Status,
    int PlannedUnits,
    int RenderedUnits,
    int DeferredUnits,
    Exception? Error);

/// <summary>
/// Rendering target contract intentionally contains no Skia, WPF, DevExpress,
/// HALCON, or hardware types. A Skia adapter can implement this interface later.
/// </summary>
public interface IViewportRenderSink<TTile>
{
    ValueTask BeginFrameAsync(
        ViewportRenderFrameContext context,
        CancellationToken cancellationToken = default);

    ValueTask DrawTileAsync(
        ViewportRenderTileContext<TTile> tile,
        CancellationToken cancellationToken = default);

    ValueTask DrawRoiAsync(
        ViewportRenderRoiContext roi,
        CancellationToken cancellationToken = default);

    ValueTask DrawOverlayAsync(
        ViewportRenderOverlayContext overlay,
        CancellationToken cancellationToken = default) =>
        ValueTask.CompletedTask;

    ValueTask ClearInvalidatedRegionAsync(
        ViewportRenderInvalidationContext invalidation,
        CancellationToken cancellationToken = default) =>
        ValueTask.CompletedTask;

    ValueTask EndFrameAsync(
        ViewportRenderFrameContext context,
        CancellationToken cancellationToken = default);

    ValueTask CommitFrameAsync(
        ViewportRenderCommitContext context,
        CancellationToken cancellationToken = default) =>
        ValueTask.CompletedTask;

    ValueTask DiscardFrameAsync(
        ViewportRenderDiscardContext context,
        CancellationToken cancellationToken = default) =>
        ValueTask.CompletedTask;
}
