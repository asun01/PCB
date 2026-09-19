using System.Drawing;
using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportTileFrameValidationHundredStageSmoke
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
            new Vector2(512, 512),
            new Vector2(256, 256),
            new Vector2(128, 128),
            prefetchMarginTiles: 0,
            cacheCapacity: 16,
            maxConcurrency: 4,
            tileSource: source);

        var frame = await runtime.RefreshAsync();
        var validation = ViewportTileFrameValidationRuntime.Validate(frame);
        var cached = runtime.CreateCachedFrame();
        var cachedValidation = ViewportTileFrameValidationRuntime.Validate(cached);

        var failingSource = new SimulatedTileSource<string>(
            (request, _) => $"tile:{request.Index.X}:{request.Index.Y}",
            shouldFail: request => request.Index == new TileIndex(0, 0));

        using var failingRuntime = new ImageViewportRuntime<string>(
            new Vector2(512, 512),
            new Vector2(256, 256),
            new Vector2(128, 128),
            prefetchMarginTiles: 0,
            cacheCapacity: 16,
            maxConcurrency: 4,
            tileSource: failingSource);

        var failedFrame = await failingRuntime.RefreshAsync();
        var failedValidation =
            ViewportTileFrameValidationRuntime.Validate(failedFrame);

        for (var i = 0; i < 10; i++)
            Check(validation.Count == 0, $"complete frame validation round {i + 1} should pass.");

        for (var i = 0; i < 10; i++)
            Check(frame.RequestedCount == 4 && frame.RequestedVisibleCount == 4, $"requested tile accounting round {i + 1} should expose four visible requests.");

        for (var i = 0; i < 10; i++)
            Check(frame.LoadedCount == frame.RequestedCount && frame.Failures.Count == 0, $"complete frame load state round {i + 1} should contain every tile.");

        for (var i = 0; i < 10; i++)
            Check(frame.IsComplete && frame.IsCompleteForVisible && frame.IsCompleteForAllRequests, $"completion state round {i + 1} should be complete.");

        for (var i = 0; i < 10; i++)
            Check(frame.MissingVisibleRequests().Count == 0, $"missing-visible state round {i + 1} should be empty.");

        for (var i = 0; i < 10; i++)
            Check(frame.LoadedTiles.Keys.SequenceEqual(frame.Requests.Select(request => request.Index).OrderBy(index => index.Y).ThenBy(index => index.X)), $"loaded tile identity round {i + 1} should cover requested indices.");

        for (var i = 0; i < 10; i++)
            Check(cached.IsCompleteForVisible && cachedValidation.Count == 0, $"cached frame round {i + 1} should remain structurally valid.");

        for (var i = 0; i < 10; i++)
            Check(frame.LoadedTiles is not System.Collections.IDictionary, $"frame exposure round {i + 1} should not expose a mutable dictionary surface.");

        for (var i = 0; i < 10; i++)
            Check(failedFrame.Failures.Count > 0 && !failedFrame.IsComplete, $"failed frame round {i + 1} should remain incomplete.");

        for (var i = 0; i < 10; i++)
            Check(failedValidation.Count == 0, $"failed frame validation round {i + 1} should accept explicit failure state.");

        assert(round == 100, $"Viewport tile frame validation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
