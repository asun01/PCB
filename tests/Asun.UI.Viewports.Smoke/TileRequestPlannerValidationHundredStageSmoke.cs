using System.Numerics;
using Asun.UI.Viewports;

public static class TileRequestPlannerValidationHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        var imageSize = new Vector2(256, 256);
        var tileSize = new Vector2(64, 64);
        var transform = ViewportTransform.Create(
            imageSize,
            new Vector2(128, 128),
            1,
            Vector2.Zero);

        var visibleRange = transform.GetVisibleTileRange(tileSize);
        var prefetchRange = ImageTileGeometry.ExpandTileRange(
            imageSize,
            tileSize,
            visibleRange,
            1);

        var requests = TileRequestPlanner.Plan(
            imageSize,
            tileSize,
            visibleRange,
            prefetchRange,
            transform.ImagePointAtViewportCenter);

        var valid =
            TileRequestPlannerValidationRuntime.IsValid(
                requests,
                imageSize,
                tileSize,
                visibleRange,
                transform.ImagePointAtViewportCenter);

        var expectedCount = requests.Count == 16;
        var visibleCount =
            requests.Count(request => request.IsVisible) ==
            visibleRange.Count;
        var visibleFirst =
            requests.Take(visibleRange.Count).All(
                request => request.IsVisible);
        var prefetchSuffix =
            requests.Skip(visibleRange.Count).All(
                request => request.IsPrefetch);
        var unique =
            requests.Select(request => request.Index).Distinct().Count() ==
            requests.Count;
        var finiteDistances =
            requests.All(
                request =>
                    double.IsFinite(
                        request.DistanceSquaredToViewportCenter));

        var rectangles = requests
            .Select(request =>
                TileRequestPlanner.GetRequestRectangle(
                    imageSize,
                    tileSize,
                    request))
            .ToArray();

        var finiteRectangles =
            rectangles.All(
                rectangle =>
                    rectangle.Width >= 0 &&
                    rectangle.Height >= 0 &&
                    float.IsFinite(rectangle.X) &&
                    float.IsFinite(rectangle.Y));

        var repeat = TileRequestPlanner.Plan(
            imageSize,
            tileSize,
            visibleRange,
            prefetchRange,
            transform.ImagePointAtViewportCenter);

        var deterministic = requests.SequenceEqual(repeat);

        var invalidRangeRejected = false;
        try
        {
            TileRequestPlanner.Plan(
                imageSize,
                tileSize,
                new VisibleTileRange(
                    new TileIndex(-1, 0),
                    new TileIndex(1, 1)),
                prefetchRange,
                transform.ImagePointAtViewportCenter);
        }
        catch (ArgumentOutOfRangeException)
        {
            invalidRangeRejected = true;
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                valid,
                $"planner validation round {i + 1} should pass.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                expectedCount,
                $"planned request count round {i + 1} should be sixteen.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                visibleCount,
                $"visible request count round {i + 1} should match the range.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                visibleFirst,
                $"visible-first ordering round {i + 1} should hold.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                prefetchSuffix,
                $"prefetch suffix round {i + 1} should contain only prefetch work.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                unique,
                $"planned tile index round {i + 1} should be unique.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                finiteDistances,
                $"distance values round {i + 1} should remain finite.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                finiteRectangles,
                $"request geometry round {i + 1} should remain finite.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                deterministic,
                $"planner determinism round {i + 1} should hold.");
        }

        assert(
            invalidRangeRejected,
            "Out-of-grid visible range should be rejected.");

        for (var i = 0; i < 10; i++)
        {
            Check(
                invalidRangeRejected,
                $"out-of-grid visible range round {i + 1} should be rejected.");
        }

        assert(
            round == 100,
            $"Tile request planner smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
