namespace Asun.UI.Viewports;

public static class TileCacheValidationRuntime
{
    public static IReadOnlyList<string> Validate<TTile>(
        TileCache<TTile> cache)
    {
        ArgumentNullException.ThrowIfNull(cache);

        var errors = new List<string>();
        var stats = cache.Statistics;
        var recent = cache.GetMostRecentFirst();

        if (cache.Count < 0 || cache.Count > cache.Capacity)
            errors.Add("Tile cache count must remain within capacity.");

        if (stats.Count != cache.Count)
            errors.Add("Tile cache statistics count must match cache count.");

        if (stats.Hits < 0 ||
            stats.Misses < 0 ||
            stats.Evictions < 0)
        {
            errors.Add("Tile cache statistics must be non-negative.");
        }

        if (recent.Count != cache.Count)
            errors.Add("Tile cache recency view must match cache count.");

        if (recent.Distinct().Count() != recent.Count)
            errors.Add("Tile cache recency view must contain unique tile indices.");

        return errors;
    }

    public static bool IsValid<TTile>(TileCache<TTile> cache) =>
        Validate(cache).Count == 0;
}
