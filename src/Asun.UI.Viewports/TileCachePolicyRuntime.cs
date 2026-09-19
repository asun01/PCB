namespace Asun.UI.Viewports;

public readonly record struct TileCachePolicy(
    int Capacity,
    bool EnablePrefetch,
    int PrefetchMarginTiles);

public static class TileCachePolicyRuntime
{
    public static TileCachePolicy Normalize(TileCachePolicy value)
    {
        if (value.Capacity <= 0) throw new ArgumentOutOfRangeException(nameof(value.Capacity));
        if (value.PrefetchMarginTiles < 0) throw new ArgumentOutOfRangeException(nameof(value.PrefetchMarginTiles));
        return value;
    }
}
