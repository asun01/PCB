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

        for (var i = 0; i < 10; i++)
        {
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

            Check(
                TileRequestPlannerValidationRuntime.IsValid(
                    requests,
                    imageSize,
                    tileSize,
                    visibleRange,
                    transform.ImagePointAtViewportCenter),
                $"planned request set {i + 1} should satisfy geometry invariants.");

            Check(
                requests.Count == 16,
                $"full one-tile margin window {i + 1} should produce sixteen requests.");

            Check(
                requests.Count(request => request.IsVisible) ==
                    visibleRange.Count,
                $"visible request count {i + 1} should match the visible tile range.");

            Check(
                requests.Take(visibleRange.Count).All(
                    request => request.IsVisible),
                $"visible-first ordering {i + 1} should be preserved.");

            Check(
                requests.Skip(visibleRange.Count).All(
                    request => request.IsPrefetch),
                $"prefetch suffix {i + 1} should contain only prefetch requests.");

            Check(
                requests.Select(request => request.Index).Distinct().Count() ==
                    requests.Count,
                $"planned indices {i + 1} should be unique.");

            Check(
                requests.All(
                    request =>
                        double.IsFinite(
                            request.DistanceSquaredToViewportCenter)),
                $"planned distances {i + 1} should remain finite.");

            var rectangles = requests
                .Select(request =>
                    TileRequestPlanner.GetRequestRectangle(
                        imageSize,
                        tileSize,
                        request))
                .ToArray();

            Check(
                rectangles.All(
                    rectangle =>
                        rectangle.Width >= 0 &&
                        rectangle.Height >= 0 &&
                        float.IsFinite(rectangle.X) &&
                        float.IsFinite(rectangle.Y)),
                $"request rectangles {i + 1} should remain finite.");

            var repeat = TileRequestPlanner.Plan(
                imageSize,
                tileSize,
                visibleRange,
                prefetchRange,
                transform.ImagePointAtViewportCenter);

            Check(
                requests.SequenceEqual(repeat),
                $"planner determinism {i + 1} should hold for identical geometry.");

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

            Check(
                invalidRangeRejected,
                $"out-of-grid visible range {i + 1} should be rejected.");
        }

        assert(
            round == 100,
            $"Tile request planner smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
