namespace Asun.UI.Viewports;

public readonly record struct TileLoadHealth(
    int CacheCount,
    int InFlight,
    long Hits,
    long Misses,
    long Evictions);

public static class TileLoadHealthRuntime
{
    public static TileLoadHealth Capture<TTile>(ImageViewportRuntime<TTile> runtime)
    {
        ArgumentNullException.ThrowIfNull(runtime);
        var stats = runtime.GetStatistics();
        return new TileLoadHealth(
            stats.Cache.Count,
            stats.InFlightCount,
            stats.Cache.Hits,
            stats.Cache.Misses,
            stats.Cache.Evictions);
    }
}
