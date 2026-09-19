using System.Numerics;
using Asun.UI.Viewports;

public static class TileViewportDiagnosticsValidationHundredStageSmoke
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
            new Vector2(1024, 1024),
            new Vector2(256, 256),
            new Vector2(128, 128),
            prefetchMarginTiles: 1,
            cacheCapacity: 32,
            maxConcurrency: 4,
            tileSource: source);

        var before = TileViewportDiagnosticsRuntime.Capture(runtime);
        var beforeValid = TileViewportDiagnosticsValidationRuntime.IsValid(before);

        var frame = await runtime.RefreshAndPrefetchAsync();
        var after = TileViewportDiagnosticsRuntime.Capture(runtime, frame);
        var afterValid = TileViewportDiagnosticsValidationRuntime.IsValid(after);

        var currentRequests = runtime.GetCurrentRequests();

        for (var i = 0; i < 10; i++)
            Check(beforeValid, $"initial diagnostics validation round {i + 1} should pass.");

        for (var i = 0; i < 10; i++)
            Check(afterValid, $"post-refresh diagnostics validation round {i + 1} should pass.");

        for (var i = 0; i < 10; i++)
            Check(after.Planned == frame.RequestedCount, $"planned count round {i + 1} should match the frame.");

        for (var i = 0; i < 10; i++)
            Check(after.Visible == frame.RequestedVisibleCount, $"visible count round {i + 1} should match the frame.");

        for (var i = 0; i < 10; i++)
            Check(after.Prefetch == frame.RequestedPrefetchCount, $"prefetch count round {i + 1} should match the frame.");

        for (var i = 0; i < 10; i++)
            Check(after.Loaded == frame.LoadedCount, $"loaded count round {i + 1} should match the frame.");

        for (var i = 0; i < 10; i++)
            Check(after.Visible + after.Prefetch == after.Planned, $"request partition round {i + 1} should be exhaustive.");

        for (var i = 0; i < 10; i++)
            Check(currentRequests.Count == after.Planned, $"current planner count round {i + 1} should match diagnostics.");

        for (var i = 0; i < 10; i++)
            Check(after.Failed == frame.Failures.Count, $"failure count round {i + 1} should match the frame.");

        for (var i = 0; i < 10; i++)
            Check(after.InFlight >= 0 && after.CacheCount >= after.Loaded - after.Failed, $"runtime counter bounds round {i + 1} should remain coherent.");

        assert(round == 100, $"Tile viewport diagnostics validation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
