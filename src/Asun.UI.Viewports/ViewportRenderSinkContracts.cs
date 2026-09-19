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

    ValueTask EndFrameAsync(
        ViewportRenderFrameContext context,
        CancellationToken cancellationToken = default);
}

public static class ViewportRenderSinkExtensions
{
    public static RectangleF GetViewportBounds(
        ViewportRenderPipelineFrame<object> frame)
    {
        var size = frame.Composite.Tiles.Transform.ViewportSize;
        return new RectangleF(0, 0, size.X, size.Y);
    }
}
