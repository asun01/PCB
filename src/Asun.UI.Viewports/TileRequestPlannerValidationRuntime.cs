using System.Numerics;

namespace Asun.UI.Viewports;

public static class TileRequestPlannerValidationRuntime
{
    public static bool IsValid(
        IReadOnlyList<TileRequest> requests,
        Vector2 imageSize,
        Vector2 tileSize,
        VisibleTileRange visibleRange,
        Vector2 viewportCenterImagePoint)
    {
        ArgumentNullException.ThrowIfNull(requests);

        if (!IsPositiveFinite(imageSize) ||
            !IsPositiveFinite(tileSize) ||
            !IsFinite(viewportCenterImagePoint))
            return false;

        var visibleCount = visibleRange.Count;
        if (requests.Count < visibleCount)
            return false;

        if (requests
            .Select(request => request.Index)
            .Distinct()
            .Count() != requests.Count)
            return false;

        if (requests.Take(visibleCount).Any(request => !request.IsVisible))
            return false;

        if (requests
            .Where(request => request.IsVisible)
            .Any(request => !visibleRange.Contains(request.Index)))
            return false;

        for (var index = 0; index < requests.Count; index++)
        {
            var request = requests[index];

            if (!ImageTileGeometry.ContainsTile(
                    imageSize,
                    tileSize,
                    request.Index))
                return false;

            if (!double.IsFinite(request.DistanceSquaredToViewportCenter) ||
                request.DistanceSquaredToViewportCenter < 0)
                return false;

            var center = ImageTileGeometry.GetTileCenter(
                imageSize,
                tileSize,
                request.Index);

            var dx = center.X - viewportCenterImagePoint.X;
            var dy = center.Y - viewportCenterImagePoint.Y;
            var expectedDistance = dx * dx + dy * dy;

            if (Math.Abs(
                    expectedDistance -
                    request.DistanceSquaredToViewportCenter) >
                1e-4)
                return false;

            if (index >= visibleCount && request.IsVisible)
                return false;
        }

        var visibleIndexes = requests
            .Where(request => request.IsVisible)
            .Select(request => request.Index)
            .ToHashSet();

        return visibleIndexes.Count == visibleRange.Count &&
               visibleRange.Enumerate().All(visibleIndexes.Contains);
    }

    private static bool IsPositiveFinite(Vector2 value) =>
        float.IsFinite(value.X) &&
        float.IsFinite(value.Y) &&
        value.X > 0 &&
        value.Y > 0;

    private static bool IsFinite(Vector2 value) =>
        float.IsFinite(value.X) &&
        float.IsFinite(value.Y);
}
