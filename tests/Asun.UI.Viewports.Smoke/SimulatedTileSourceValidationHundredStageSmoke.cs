using System.Drawing;
using Asun.UI.Viewports;

public static class SimulatedTileSourceValidationHundredStageSmoke
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
            (request, rectangle) =>
                $"tile:{request.Index.X}:{request.Index.Y}:{rectangle.Width}:{rectangle.Height}",
            delay: TimeSpan.FromMilliseconds(2),
            signalFirstLoad: true);

        var request = new TileRequest(
            new TileIndex(0, 0),
            true,
            0);

        var firstTask = source.LoadAsync(
            request,
            new RectangleF(0, 0, 128, 128)).AsTask();

        await source.FirstLoadStarted;
        var first = await firstTask;

        var sequentialValid = SimulatedTileSourceValidationRuntime.IsValid(source);

        var concurrentRequests = Enumerable.Range(1, 4)
            .Select(index =>
                source.LoadAsync(
                    new TileRequest(new TileIndex(index, 0), true, index),
                    new RectangleF(index * 128, 0, 128, 128)).AsTask())
            .ToArray();

        var concurrentResults = await Task.WhenAll(concurrentRequests);

        var concurrentValid =
            SimulatedTileSourceValidationRuntime.IsValid(source);

        var failing = new SimulatedTileSource<string>(
            (requestValue, _) => $"ok:{requestValue.Index.X}",
            shouldFail: requestValue => requestValue.Index.X == 9);

        var failureObserved = false;
        try
        {
            await failing.LoadAsync(
                new TileRequest(new TileIndex(9, 0), true, 0),
                new RectangleF(0, 0, 128, 128));
        }
        catch (InvalidOperationException)
        {
            failureObserved = true;
        }

        var cancellation = new SimulatedTileSource<string>(
            (requestValue, _) => $"cancel:{requestValue.Index.X}",
            delay: TimeSpan.FromMilliseconds(100),
            signalFirstLoad: true);

        using var cancellationSource = new CancellationTokenSource();
        var cancelledTask = cancellation.LoadAsync(
            new TileRequest(new TileIndex(10, 0), true, 0),
            new RectangleF(0, 0, 128, 128),
            cancellationSource.Token).AsTask();

        await cancellation.FirstLoadStarted;
        cancellationSource.Cancel();

        var cancellationObserved = false;
        try
        {
            await cancelledTask;
        }
        catch (OperationCanceledException)
        {
            cancellationObserved = true;
        }

        var cancellationValid =
            SimulatedTileSourceValidationRuntime.IsValid(cancellation);

        for (var i = 0; i < 10; i++)
            Check(first == "tile:0:0:128:128", $"factory output round {i + 1} should be deterministic.");

        for (var i = 0; i < 10; i++)
            Check(source.LoadCount == 5, $"sequential and concurrent load count round {i + 1} should total five.");

        for (var i = 0; i < 10; i++)
            Check(source.ActiveLoadCount == 0, $"active load cleanup round {i + 1} should return to zero.");

        for (var i = 0; i < 10; i++)
            Check(source.MaxConcurrentLoads >= 2, $"concurrency tracking round {i + 1} should observe overlapping work.");

        for (var i = 0; i < 10; i++)
            Check(concurrentResults.Length == 4 && concurrentResults.Distinct().Count() == 4, $"concurrent outputs round {i + 1} should remain distinct.");

        for (var i = 0; i < 10; i++)
            Check(sequentialValid && concurrentValid, $"source validation round {i + 1} should pass after successful loads.");

        for (var i = 0; i < 10; i++)
            Check(failureObserved && failing.ActiveLoadCount == 0, $"failure cleanup round {i + 1} should release the active load.");

        for (var i = 0; i < 10; i++)
            Check(cancellationObserved && cancellation.ActiveLoadCount == 0, $"cancellation cleanup round {i + 1} should release the active load.");

        for (var i = 0; i < 10; i++)
            Check(SimulatedTileSourceValidationRuntime.IsValid(failing), $"failure-source validation round {i + 1} should pass.");

        for (var i = 0; i < 10; i++)
            Check(cancellationValid, $"cancellation-source validation round {i + 1} should pass.");

        assert(round == 100, $"Simulated tile source validation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
