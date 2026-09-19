namespace Asun.UI.Viewports;

using System.Numerics;

/// <summary>
/// Deterministically orders image-tile requests for a viewport.
/// Visible tiles are always prioritized before prefetch tiles.
/// </summary>
public readonly record struct TileRequest(
    TileIndex Index,
    bool IsVisible,
    double DistanceSquaredToViewportCenter)
{
    public bool IsPrefetch => !IsVisible;
}

public static class TileRequestPlanner
{
    public static IReadOnlyList<TileRequest> Plan(
        Vector2 imageSize,
        Vector2 tileSize,
        VisibleTileRange visibleRange,
        VisibleTileRange prefetchRange,
        Vector2 viewportCenterImagePoint)
    {
        ValidatePositiveFinite(imageSize, nameof(imageSize));
        ValidatePositiveFinite(tileSize, nameof(tileSize));

        if (!float.IsFinite(viewportCenterImagePoint.X) ||
            !float.IsFinite(viewportCenterImagePoint.Y))
        {
            throw new ArgumentOutOfRangeException(nameof(viewportCenterImagePoint));
        }

        ValidateRange(imageSize, tileSize, visibleRange, nameof(visibleRange));
        ValidateRange(imageSize, tileSize, prefetchRange, nameof(prefetchRange));

        var requests = new List<TileRequest>();
        var visibleTiles = visibleRange.Enumerate().ToHashSet();

        foreach (var tile in visibleTiles)
        {
            requests.Add(CreateRequest(
                imageSize,
                tileSize,
                tile,
                isVisible: true,
                viewportCenterImagePoint));
        }

        foreach (var tile in prefetchRange.Enumerate())
        {
            if (!visibleTiles.Add(tile))
                continue;

            requests.Add(CreateRequest(
                imageSize,
                tileSize,
                tile,
                isVisible: false,
                viewportCenterImagePoint));
        }

        return requests
            .OrderByDescending(request => request.IsVisible)
            .ThenBy(request => request.DistanceSquaredToViewportCenter)
            .ThenBy(request => request.Index.Y)
            .ThenBy(request => request.Index.X)
            .ToArray();
    }

    private static TileRequest CreateRequest(
        Vector2 imageSize,
        Vector2 tileSize,
        TileIndex index,
        bool isVisible,
        Vector2 viewportCenterImagePoint)
    {
        var center = ImageTileGeometry.GetTileCenter(imageSize, tileSize, index);
        var dx = center.X - viewportCenterImagePoint.X;
        var dy = center.Y - viewportCenterImagePoint.Y;

        return new TileRequest(
            index,
            isVisible,
            dx * dx + dy * dy);
    }

    private static void ValidateRange(
        Vector2 imageSize,
        Vector2 tileSize,
        VisibleTileRange range,
        string parameterName)
    {
        if (range.IsEmpty)
            return;

        if (!ImageTileGeometry.ContainsTile(imageSize, tileSize, range.Minimum) ||
            !ImageTileGeometry.ContainsTile(imageSize, tileSize, range.Maximum))
        {
            throw new ArgumentOutOfRangeException(parameterName);
        }
    }

    private static void ValidatePositiveFinite(Vector2 value, string parameterName)
    {
        if (!float.IsFinite(value.X) ||
            !float.IsFinite(value.Y) ||
            value.X <= 0 ||
            value.Y <= 0)
        {
            throw new ArgumentOutOfRangeException(parameterName);
        }
    }
}
