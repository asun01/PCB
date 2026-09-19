namespace Asun.UI.Viewports;

public static class TileCacheWarmupValidationRuntime
{
    public static bool IsValid(
        IReadOnlyList<TileRequest> requests,
        int loadedCount,
        TileLoadHealth health)
    {
        ArgumentNullException.ThrowIfNull(requests);

        if (loadedCount < 0)
            return false;

        var uniqueRequestCount = requests
            .Select(request => request.Index)
            .Distinct()
            .Count();

        return loadedCount <= uniqueRequestCount &&
               health.CacheCount >= 0 &&
               health.InFlight >= 0 &&
               health.Hits >= 0 &&
               health.Misses >= 0 &&
               health.Evictions >= 0;
    }
}
