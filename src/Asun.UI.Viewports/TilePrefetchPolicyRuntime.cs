namespace Asun.UI.Viewports;

public readonly record struct TilePrefetchPolicy(
    int MarginTiles,
    int MaximumTiles,
    bool PreferHorizontal,
    bool PreferVertical);

public static class TilePrefetchPolicyRuntime
{
    public static IReadOnlyList<TileRequest> Apply(
        IReadOnlyList<TileRequest> requests,
        TilePrefetchPolicy policy)
    {
        ArgumentNullException.ThrowIfNull(requests);
        if (policy.MarginTiles < 0 || policy.MaximumTiles <= 0)
            throw new ArgumentOutOfRangeException(nameof(policy));

        var visible = requests.Where(x => x.IsVisible).OrderBy(x => x.DistanceSquaredToViewportCenter);
        var prefetch = requests
            .Where(x => x.IsPrefetch)
            .OrderBy(x => policy.PreferHorizontal
                ? Math.Abs(x.Index.X)
                : policy.PreferVertical
                    ? Math.Abs(x.Index.Y)
                    : x.DistanceSquaredToViewportCenter);

        return visible.Concat(prefetch)
            .DistinctBy(x => x.Index)
            .Take(policy.MaximumTiles)
            .ToArray();
    }
}
