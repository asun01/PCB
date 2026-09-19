namespace Asun.UI.Viewports;

public static class ViewportRenderPriorityRuntime
{
    public static ViewportRenderWorkPlan Prioritize<TTile>(
        ViewportRenderWorkPlan plan,
        ViewportCompositeFrame<TTile> frame)
    {
        ArgumentNullException.ThrowIfNull(plan);
        ArgumentNullException.ThrowIfNull(frame);

        var selectedIds = frame.Roi.Items
            .Where(item => item.IsSelected)
            .Select(item => item.Id)
            .ToHashSet();

        var tileDistances = frame.Tiles.Requests
            .Where(request => request.IsVisible)
            .ToDictionary(
                request => request.Index,
                request => request.DistanceSquaredToViewportCenter);

        var ordered = plan.Items
            .OrderBy(item => Priority(
                item.IsInvalidation,
                item.Kind,
                item.RoiId,
                item.Tile,
                selectedIds))
            .ThenBy(item => TileDistance(item.Tile, tileDistances))
            .ThenBy(item => item.RoiId)
            .ThenBy(item => item.Bounds.Top)
            .ThenBy(item => item.Bounds.Left)
            .ToArray();

        return new ViewportRenderWorkPlan(
            ordered,
            plan.ConsumedFlags,
            plan.Generation);
    }

    private static int Priority(
        bool isInvalidation,
        ViewportRenderWorkKind kind,
        Guid roiId,
        TileIndex? tile,
        IReadOnlySet<Guid> selectedIds)
    {
        if (isInvalidation)
            return -1;

        return kind switch
        {
            ViewportRenderWorkKind.Tile => 0,
            ViewportRenderWorkKind.FullSurface => 1,
            ViewportRenderWorkKind.Roi when selectedIds.Contains(roiId) => 2,
            ViewportRenderWorkKind.Roi => 3,
            ViewportRenderWorkKind.Selection => 4,
            ViewportRenderWorkKind.Overlay => 5,
            _ => 6
        };
    }

    private static double TileDistance(
        TileIndex? tile,
        IReadOnlyDictionary<TileIndex, double> distances) =>
        tile is TileIndex index &&
        distances.TryGetValue(index, out var distance)
            ? distance
            : double.MaxValue;
}
