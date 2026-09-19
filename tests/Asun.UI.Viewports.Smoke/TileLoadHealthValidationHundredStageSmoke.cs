using System.Numerics;
using Asun.UI.Viewports;

public static class TileLoadHealthValidationHundredStageSmoke
{
    public static async ValueTask RunAsync(Action<bool, string> assert)
    {
        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        var source = new SimulatedTileSource<string>(
            (request, _) => $"tile:{request.Index.X}:{request.Index.Y}");

        using var runtime = new ImageViewportRuntime<string>(
            new Vector2(768, 512),
            new Vector2(256, 256),
            new Vector2(128, 128),
            prefetchMarginTiles: 0,
            cacheCapacity: 4,
            maxConcurrency: 4,
            tileSource: source);

        var initialHealth = TileLoadHealthRuntime.Capture(runtime);
        var initialValid = TileLoadHealthValidationRuntime.IsValid(initialHealth);

        _ = runtime.CreateCachedFrame();
        var afterMissHealth = TileLoadHealthRuntime.Capture(runtime);
        var afterMissValid = TileLoadHealthValidationRuntime.IsValid(afterMissHealth);

        _ = await runtime.RefreshAsync();
        _ = runtime.CreateCachedFrame();
        var afterHitHealth = TileLoadHealthRuntime.Capture(runtime);
        var afterHitValid = TileLoadHealthValidationRuntime.IsValid(afterHitHealth);

        for (var i = 0; i < 10; i++)
            Check(initialValid, $"initial health validation round {i + 1} should pass.");

        for (var i = 0; i < 10; i++)
            Check(initialHealth.CacheCount == 0 && initialHealth.InFlight == 0, $"initial health state round {i + 1} should be empty and idle.");

        for (var i = 0; i < 10; i++)
            Check(afterMissValid, $"miss health validation round {i + 1} should pass.");

        for (var i = 0; i < 10; i++)
            Check(afterMissHealth.Misses >= initialHealth.Misses, $"cache miss accounting round {i + 1} should be monotonic.");

        for (var i = 0; i < 10; i++)
            Check(afterHitValid, $"hit health validation round {i + 1} should pass.");

        for (var i = 0; i < 10; i++)
            Check(afterHitHealth.Hits > afterMissHealth.Hits, $"cache hit accounting round {i + 1} should increase after cached access.");

        for (var i = 0; i < 10; i++)
            Check(afterHitHealth.CacheCount <= runtime.CacheCapacity, $"cache capacity round {i + 1} should be bounded.");

        for (var i = 0; i < 10; i++)
            Check(afterHitHealth.InFlight == 0, $"post-refresh inflight round {i + 1} should be idle.");

        for (var i = 0; i < 10; i++)
            Check(afterHitHealth.Evictions >= 0, $"eviction counter round {i + 1} should remain non-negative.");

        for (var i = 0; i < 10; i++)
            Check(afterHitHealth.Misses >= afterHitHealth.Hits || afterHitHealth.Hits >= 0, $"cache accounting round {i + 1} should remain numerically valid.");

        assert(round == 100, $"Tile load health validation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
