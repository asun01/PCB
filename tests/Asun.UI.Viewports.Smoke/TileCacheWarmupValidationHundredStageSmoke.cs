using System.Numerics;
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

        for (var i = 0; i < 10; i++)
        {
            var source = new CountingTileSource();
            using var loader = new TileLoadCoordinator<int>(
                source,
                new TileCache<int>(4));

            var requests = new[]
            {
                new TileRequest(new TileIndex(0, i), true, 0),
                new TileRequest(new TileIndex(0, i), false, 1),
                new TileRequest(new TileIndex(1, i), false, 4)
            };

            var loaded = await TileCacheWarmupRuntime.WarmAsync(
                loader,
                requests,
                request => new System.Drawing.RectangleF(
                    request.Index.X * 32,
                    request.Index.Y * 32,
                    32,
                    32));

            var statistics = loader.Cache.Statistics;
            var health = new TileLoadHealth(
                statistics.Count,
                loader.InFlightCount,
                statistics.Hits,
                statistics.Misses,
                statistics.Evictions);

            Check(
                TileCacheWarmupValidationRuntime.IsValid(
                    requests,
                    loaded,
                    health),
                $"first warmup state {i + 1} should satisfy the validator.");

            Check(
                loaded == 2,
                $"first warmup {i + 1} should load only unique missing tiles.");

            Check(
                source.CallCount == 2,
                $"duplicate warmup request {i + 1} should not load twice.");

            Check(
                loader.InFlightCount == 0,
                $"warmup completion {i + 1} should leave no in-flight requests.");

            Check(
                loader.Cache.Count == 2,
                $"warmup cache count {i + 1} should contain two unique tiles.");

            var cachedRepeat = await TileCacheWarmupRuntime.WarmAsync(
                loader,
                requests,
                request => new System.Drawing.RectangleF(
                    request.Index.X * 32,
                    request.Index.Y * 32,
                    32,
                    32));

            Check(
                cachedRepeat == 0,
                $"second warmup {i + 1} should report no newly loaded tiles.");

            Check(
                source.CallCount == 2,
                $"second warmup {i + 1} should remain cache-only.");

            Check(
                loader.Cache.Statistics.Hits >= 3,
                $"second warmup {i + 1} should record cached requests.");

            Check(
                TileCacheWarmupValidationRuntime.IsValid(
                    requests,
                    cachedRepeat,
                    new TileLoadHealth(
                        loader.Cache.Count,
                        loader.InFlightCount,
                        loader.Cache.Statistics.Hits,
                        loader.Cache.Statistics.Misses,
                        loader.Cache.Statistics.Evictions)),
                $"final warmup state {i + 1} should remain valid.");
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
            return ValueTask.FromResult(request.Index.X + request.Index.Y);
        }
    }
}
