namespace Asun.UI.Viewports;

public static class TileRangeRuntime
{
    public static IReadOnlyList<TileIndex> Enumerate(VisibleTileRange range)
    {
        if (range.IsEmpty) return Array.Empty<TileIndex>();
        var result = new List<TileIndex>();
        for (var y = range.MinY; y <= range.MaxY; y++)
        for (var x = range.MinX; x <= range.MaxX; x++)
            result.Add(new TileIndex(x, y));
        return result;
    }

    public static RectangleF UnionRectangle(
        Vector2 imageSize,
        Vector2 tileSize,
        VisibleTileRange range)
    {
        return ImageTileGeometry.GetTileRangeRectangle(imageSize, tileSize, range);
    }
}
