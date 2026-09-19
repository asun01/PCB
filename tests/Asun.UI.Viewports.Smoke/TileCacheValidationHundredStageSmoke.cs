using Asun.UI.Viewports;

public static class TileCacheValidationHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        for (var i = 0; i < 10; i++)
        {
            var cache = new TileCache<string>(3);
            Check(
                TileCacheValidationRuntime.IsValid(cache),
                $"empty cache {i + 1} should validate.");
        }

        for (var i = 0; i < 10; i++)
        {
            var cache = new TileCache<string>(2);
            cache.Set(new TileIndex(0, 0), "a");
            cache.Set(new TileIndex(1, 0), "b");

            Check(
                TileCacheValidationRuntime.IsValid(cache) &&
                cache.Count == 2,
                $"cache fill {i + 1} should remain bounded.");
        }

        for (var i = 0; i < 10; i++)
        {
            var cache = new TileCache<string>(2);
            cache.Set(new TileIndex(0, 0), "a");
            cache.Set(new TileIndex(1, 0), "b");
            cache.TryGet(new TileIndex(0, 0), out _);
            cache.Set(new TileIndex(2, 0), "c");

            var recent = cache.GetMostRecentFirst();

            Check(
                recent.SequenceEqual(
                    new[]
                    {
                        new TileIndex(2, 0),
                        new TileIndex(0, 0)
                    }) &&
                !recent.Contains(new TileIndex(1, 0)),
                $"LRU eviction {i + 1} should remove the oldest entry.");
        }

        for (var i = 0; i < 10; i++)
        {
            var cache = new TileCache<string>(2);
            cache.Set(new TileIndex(0, 0), "a");
            cache.Set(new TileIndex(1, 0), "b");
            cache.TryGet(new TileIndex(1, 0), out _);
            Check(
                cache.Statistics.Hits == 1 &&
                cache.Statistics.Misses == 0,
                $"cache hit accounting {i + 1} should be correct.");
        }

        for (var i = 0; i < 10; i++)
        {
            var cache = new TileCache<string>(2);
            cache.TryGet(new TileIndex(9, 9), out _);
            Check(
                cache.Statistics.Misses == 1 &&
                cache.Statistics.Hits == 0,
                $"cache miss accounting {i + 1} should be correct.");
        }

        for (var i = 0; i < 10; i++)
        {
            var cache = new TileCache<string>(1);
            cache.Set(new TileIndex(0, 0), "a");
            cache.Set(new TileIndex(1, 0), "b");
            Check(
                cache.Statistics.Evictions == 1 &&
                TileCacheValidationRuntime.IsValid(cache),
                $"cache eviction statistics {i + 1} should be correct.");
        }

        for (var i = 0; i < 10; i++)
        {
            var cache = new TileCache<string>(2);
            cache.Set(new TileIndex(0, 0), "a");
            cache.Set(new TileIndex(0, 0), "b");
            var found = cache.TryGet(new TileIndex(0, 0), out var value);

            Check(
                found &&
                value == "b" &&
                cache.Count == 1 &&
                cache.Statistics.Evictions == 0,
                $"cache replacement {i + 1} should update in place.");
        }

        for (var i = 0; i < 10; i++)
        {
            var cache = new TileCache<string>(2);
            cache.Set(new TileIndex(0, 0), "a");
            cache.Set(new TileIndex(1, 0), "b");
            cache.Remove(new TileIndex(0, 0));

            Check(
                cache.Count == 1 &&
                !cache.GetMostRecentFirst().Contains(new TileIndex(0, 0)) &&
                TileCacheValidationRuntime.IsValid(cache),
                $"cache removal {i + 1} should stay coherent.");
        }

        for (var i = 0; i < 10; i++)
        {
            var cache = new TileCache<string>(3);
            cache.Set(new TileIndex(0, 0), "a");
            cache.Set(new TileIndex(1, 0), "b");
            cache.Clear();

            Check(
                cache.Count == 0 &&
                cache.GetMostRecentFirst().Count == 0 &&
                TileCacheValidationRuntime.IsValid(cache),
                $"cache clear {i + 1} should preserve valid statistics.");
        }

        for (var i = 0; i < 10; i++)
        {
            var cache = new TileCache<string>(3);
            cache.Set(new TileIndex(0, 0), "a");
            cache.Set(new TileIndex(1, 0), "b");
            var before = cache.GetMostRecentFirst();
            cache.TryGet(new TileIndex(0, 0), out _);
            var after = cache.GetMostRecentFirst();

            Check(
                before.First() != after.First() &&
                after.First() == new TileIndex(0, 0) &&
                TileCacheValidationRuntime.IsValid(cache),
                $"LRU touch {i + 1} should move the hit to the front.");
        }

        assert(
            round == 100,
            $"Tile cache validation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
