namespace Asun.UI.Viewports;

public static class TileRequestPriorityRuntime
{
    public static IReadOnlyList<TileRequest> Sort(IEnumerable<TileRequest> requests)
    {
        ArgumentNullException.ThrowIfNull(requests);
        return requests
            .OrderByDescending(x => x.IsVisible)
            .ThenBy(x => x.DistanceSquaredToViewportCenter)
            .ThenBy(x => x.Index.Y)
            .ThenBy(x => x.Index.X)
            .ToArray();
    }
}
