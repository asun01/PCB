namespace Asun.UI.Viewports;

public readonly record struct TileViewportDiagnostics(
    int Planned,
    int Visible,
    int Prefetch,
    int Loaded,
    int Failed,
    int CacheCount,
    long CacheHits,
    long CacheMisses,
    long Evictions,
    int InFlight);

public static class TileViewportDiagnosticsRuntime
{
    public static TileViewportDiagnostics Capture<TTile>(
        ImageViewportRuntime<TTile> runtime,
        ViewportTileFrame<TTile>? frame = null)
    {
        ArgumentNullException.ThrowIfNull(runtime);
        var stats = runtime.GetStatistics();
        var current = frame?.RequestedCount ?? stats.PlannedRequestCount;
        var visible = frame?.RequestedVisibleCount ?? runtime.GetCurrentRequests().Count(x => x.IsVisible);
        var prefetch = frame?.RequestedPrefetchCount ?? runtime.GetCurrentRequests().Count(x => x.IsPrefetch);
        var loaded = frame?.LoadedCount ?? 0;
        var failed = frame?.Failures.Count ?? 0;
        return new TileViewportDiagnostics(
            current,
            visible,
            prefetch,
            loaded,
            failed,
            stats.Cache.Count,
            stats.Cache.Hits,
            stats.Cache.Misses,
            stats.Cache.Evictions,
            stats.InFlightCount);
    }
}
