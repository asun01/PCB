namespace Asun.UI.Viewports;

public readonly record struct TileLoadCoordinatorSnapshot(
    int InFlight,
    int CacheCount,
    int CacheCapacity,
    long Hits,
    long Misses,
    long Evictions);

public static class TileLoadCoordinatorValidationRuntime
{
    public static TileLoadCoordinatorSnapshot Capture<TTile>(
        TileLoadCoordinator<TTile> loader)
    {
        ArgumentNullException.ThrowIfNull(loader);

        var statistics = loader.Cache.Statistics;
        return new TileLoadCoordinatorSnapshot(
            loader.InFlightCount,
            statistics.Count,
            loader.Cache.Capacity,
            statistics.Hits,
            statistics.Misses,
            statistics.Evictions);
    }

    public static bool IsValid(
        TileLoadCoordinatorSnapshot snapshot)
    {
        return snapshot.InFlight >= 0 &&
               snapshot.CacheCapacity > 0 &&
               snapshot.CacheCount >= 0 &&
               snapshot.CacheCount <= snapshot.CacheCapacity &&
               snapshot.Hits >= 0 &&
               snapshot.Misses >= 0 &&
               snapshot.Evictions >= 0;
    }
}
