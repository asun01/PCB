using System.Drawing;
using System.Numerics;

namespace Asun.UI.Viewports;

public readonly record struct ViewportVisibleRegion(
    RectangleF ImageRectangle,
    IReadOnlyList<TileIndex> Tiles);

public static class ViewportVisibleRegionRuntime
{
    public static ViewportVisibleRegion Capture(
        ViewportTransform transform,
        Vector2 tileSize)
    {
        var imageRectangle = transform.GetVisibleImageRectangle();
        var tiles = TileRangeRuntime.Enumerate(
            transform.GetVisibleTileRange(tileSize));
        return new ViewportVisibleRegion(imageRectangle, tiles);
    }
}
