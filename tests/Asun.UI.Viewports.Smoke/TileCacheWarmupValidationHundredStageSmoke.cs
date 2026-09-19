using Asun.UI.Viewports;

public static class TileCacheWarmupValidationHundredStageSmoke
{
    public static async ValueTask RunAsync(Action<bool, string> assert)
    {
        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        var source = new CountingTileSource();
        using var loader = new TileLoadCoordinator<int>(
            source,
            new TileCache<int>(4));

        var requests = new[]
        {
            new TileRequest(new TileIndex(0, 0), true, 0),
            new TileRequest(new TileIndex(0, 0), false, 1),
            new TileRequest(new TileIndex(1, 0), false, 4)
        };

        var loaded = await TileCacheWarmupRuntime.WarmAsync(
            loader,
            requests,
            request => new System.Drawing.RectangleF(
                request.Index.X * 32,
                request.Index.Y * 32,
                32,
                32));

        var firstStatistics = loader.Cache.Statistics;
        var firstHealth = new TileLoadHealth(
            firstStatistics.Count,
            loader.InFlightCount,
            firstStatistics.Hits,
            firstStatistics.Misses,
            firstStatistics.Evictions);

        var firstValid =
            TileCacheWarmupValidationRuntime.IsValid(
                requests,
                loaded,
                firstHealth);

        var firstLoadedExactlyTwo = loaded == 2;
        var firstSourceCalls = source.CallCount == 2;
        var firstInFlightClear = loader.InFlightCount == 0;
        var firstCacheCount = loader.Cache.Count == 2;

        var cachedRepeat = await TileCacheWarmupRuntime.WarmAsync(
            loader,
            requests,
            request => new System.Drawing.RectangleF(
                request.Index.X * 32,
                request.Index.Y * 32,
                32,
                32));

        var secondStatistics = loader.Cache.Statistics;
        var secondHealth = new TileLoadHealth(
            secondStatistics.Count,
            loader.InFlightCount,
            secondStatistics.Hits,
            secondStatistics.Misses,
            secondStatistics.Evictions);

        var secondLoadedZero = cachedRepeat == 0;
        var secondSourceCalls = source.CallCount == 2;
        var cacheHitAccounting = secondStatistics.Hits >= 3;
        var secondValid =
            TileCacheWarmupValidationRuntime.IsValid(
                requests,
                cachedRepeat,
                secondHealth);

        for (var i = 0; i < 10; i++)
        {
            Check(
                firstValid,
                $"first warmup validation round {i + 1} should pass.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                firstLoadedExactlyTwo,
                $"unique load count round {i + 1} should be two.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                firstSourceCalls,
                $"duplicate request round {i + 1} should not duplicate source work.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                firstInFlightClear,
                $"first warmup completion round {i + 1} should clear in-flight work.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                firstCacheCount,
                $"first cache population round {i + 1} should contain two tiles.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                secondLoadedZero,
                $"cached warmup round {i + 1} should load nothing new.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                secondSourceCalls,
                $"cached warmup round {i + 1} should not call the source again.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                cacheHitAccounting,
                $"cache hit accounting round {i + 1} should record repeated lookups.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                secondValid,
                $"final warmup validation round {i + 1} should pass.");
        }

        assert(
            round == 100,
            $"Tile cache warmup smoke should execute exactly 100 numbered rounds; actual {round}.");
    }

    private sealed class CountingTileSource : ITileSource<int>
    {
        private int _callCount;

        public int CallCount => Volatile.Read(ref _callCount);

        public ValueTask<int> LoadAsync(
            TileRequest request,
            System.Drawing.RectangleF imageRectangle,
            CancellationToken cancellationToken = default)
        {
            Interlocked.Increment(ref _callCount);
            return ValueTask.FromResult(
                request.Index.X + request.Index.Y);
        }
    }
}
