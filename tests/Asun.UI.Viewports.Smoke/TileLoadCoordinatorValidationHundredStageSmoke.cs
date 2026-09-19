using System.Drawing;
using Asun.UI.Viewports;

public static class TileLoadCoordinatorValidationHundredStageSmoke
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
            var source = new GateTileSource();
            using var loader = new TileLoadCoordinator<int>(
                source,
                new TileCache<int>(4));

            var request =
                new TileRequest(
                    new TileIndex(i, 0),
                    true,
                    0);

            var rectangle = new RectangleF(0, 0, 32, 32);

            Check(
                TileLoadCoordinatorValidationRuntime.IsValid(
                    TileLoadCoordinatorValidationRuntime.Capture(loader)),
                $"initial coordinator state {i + 1} should be valid.");

            var first = loader.LoadAsync(request, rectangle).AsTask();
            var second = loader.LoadAsync(request, rectangle).AsTask();

            await source.Started.Task;

            Check(
                source.CallCount == 1,
                $"deduplicated source call {i + 1} should execute once.");

            Check(
                loader.InFlightCount == 1,
                $"shared in-flight load {i + 1} should expose one task.");

            using var canceledWaiter = new CancellationTokenSource();
            canceledWaiter.Cancel();
            var canceled = false;

            try
            {
                await loader.LoadAsync(
                    request,
                    rectangle,
                    canceledWaiter.Token);
            }
            catch (OperationCanceledException)
            {
                canceled = true;
            }

            Check(
                canceled && loader.InFlightCount == 1,
                $"caller cancellation {i + 1} should cancel only its wait.");

            source.Release();

            var values = await Task.WhenAll(first, second);

            Check(
                values.SequenceEqual(new[] { 42, 42 }),
                $"shared load results {i + 1} should be identical.");

            Check(
                source.CallCount == 1,
                $"shared load completion {i + 1} should not duplicate the source call.");

            Check(
                loader.InFlightCount == 0,
                $"completed load {i + 1} should clear in-flight state.");

            Check(
                loader.Cache.TryGet(request.Index, out var cached) &&
                cached == 42,
                $"completed load {i + 1} should populate the cache.");

            Check(
                TileLoadCoordinatorValidationRuntime.IsValid(
                    TileLoadCoordinatorValidationRuntime.Capture(loader)),
                $"completed coordinator state {i + 1} should remain valid.");

            Check(
                loader.Cache.Statistics.Hits >= 1 &&
                loader.Cache.Statistics.Misses >= 1,
                $"cache accounting {i + 1} should record both lookup directions.");
        }

        assert(
            round == 100,
            $"Tile load coordinator smoke should execute exactly 100 numbered rounds; actual {round}.");
    }

    private sealed class GateTileSource : ITileSource<int>
    {
        private int _callCount;

        public TaskCompletionSource<bool> Started { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public TaskCompletionSource<bool> Gate { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public int CallCount => Volatile.Read(ref _callCount);

        public ValueTask<int> LoadAsync(
            TileRequest request,
            RectangleF imageRectangle,
            CancellationToken cancellationToken = default)
        {
            Interlocked.Increment(ref _callCount);
            Started.TrySetResult(true);
            return AwaitGateAsync();
        }

        public async ValueTask<int> AwaitGateAsync()
        {
            await Gate.Task.ConfigureAwait(false);
            return 42;
        }

        public void Release() => Gate.TrySetResult(true);
    }
}
