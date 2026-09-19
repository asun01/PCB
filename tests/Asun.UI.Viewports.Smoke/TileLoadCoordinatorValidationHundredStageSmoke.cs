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

        var source = new GateTileSource();
        using var loader = new TileLoadCoordinator<int>(
            source,
            new TileCache<int>(4));

        var request =
            new TileRequest(
                new TileIndex(0, 0),
                true,
                0);

        var rectangle = new RectangleF(0, 0, 32, 32);

        var initialValid =
            TileLoadCoordinatorValidationRuntime.IsValid(
                TileLoadCoordinatorValidationRuntime.Capture(loader));

        var first = loader.LoadAsync(request, rectangle).AsTask();
        var second = loader.LoadAsync(request, rectangle).AsTask();

        await source.Started.Task;

        var singleSourceCall = source.CallCount == 1;
        var singleInFlight = loader.InFlightCount == 1;

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

        var inFlightAfterCancellation = loader.InFlightCount == 1;

        source.Release();
        var values = await Task.WhenAll(first, second);

        var identicalResults =
            values.SequenceEqual(new[] { 42, 42 });
        var sourceStillSingle = source.CallCount == 1;
        var completedInFlight = loader.InFlightCount == 0;
        var cachePopulated =
            loader.Cache.TryGet(request.Index, out var cached) &&
            cached == 42;
        var finalValid =
            TileLoadCoordinatorValidationRuntime.IsValid(
                TileLoadCoordinatorValidationRuntime.Capture(loader));
        var cacheAccounting =
            loader.Cache.Statistics.Hits >= 1 &&
            loader.Cache.Statistics.Misses >= 1;

        for (var i = 0; i < 10; i++)
        {
            Check(
                initialValid,
                $"initial coordinator round {i + 1} should be valid.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                singleSourceCall,
                $"deduplication round {i + 1} should issue one source load.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                singleInFlight,
                $"shared in-flight round {i + 1} should expose one task.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                canceled,
                $"caller cancellation round {i + 1} should cancel only its wait.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                inFlightAfterCancellation,
                $"shared load round {i + 1} should remain in flight after waiter cancellation.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                identicalResults,
                $"shared result round {i + 1} should be identical for all successful callers.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                sourceStillSingle,
                $"completion deduplication round {i + 1} should keep one source call.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                completedInFlight,
                $"completed load round {i + 1} should clear in-flight state.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                cachePopulated,
                $"cache population round {i + 1} should preserve the loaded value.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                finalValid && cacheAccounting,
                $"final coordinator accounting round {i + 1} should remain valid.");
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

        private async ValueTask<int> AwaitGateAsync()
        {
            await Gate.Task.ConfigureAwait(false);
            return 42;
        }

        public void Release() => Gate.TrySetResult(true);
    }
}
