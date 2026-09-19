using System.Drawing;
using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportTileRoiVisibilityValidationHundredStageSmoke
{
    private sealed class Source : ITileSource<string>
    {
        public ValueTask<string> LoadAsync(
            TileRequest request,
            RectangleF imageRectangle,
            CancellationToken cancellationToken = default) =>
            ValueTask.FromResult($"tile:{request.Index.X}:{request.Index.Y}");
    }

    public static void Run(Action<bool, string> assert)
    {
        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        using var composite = new ViewportCompositeRuntime<string>(
            new Vector2(512, 512),
            new Vector2(256, 256),
            new Vector2(64, 64),
            prefetchMarginTiles: 1,
            cacheCapacity: 32,
            maxConcurrency: 2,
            tileSource: new Source());

        var visibleId = composite.AddRoi(
            RoiGeometry.CreateRectangle(
                new Vector2(256, 256),
                new Vector2(80, 80)));

        var offViewportId = composite.AddRoi(
            RoiGeometry.CreateRectangle(
                new Vector2(5000, 5000),
                new Vector2(20, 20)));

        var frame = composite.CreateCachedFrame();
        var snapshot = ViewportTileRoiVisibilityRuntime.Build(frame);
        var valid =
            ViewportTileRoiVisibilityValidationRuntime.IsValid<string>(
                snapshot);

        var visibleTileIds = snapshot.Tiles
            .SelectMany(tile => tile.IntersectingRoiIds)
            .ToHashSet();

        for (var i = 0; i < 10; i++)
            Check(
                snapshot.VisibleRoiIds.Contains(visibleId),
                $"visible ROI membership round {i + 1} should include the on-screen ROI.");

        for (var i = 0; i < 10; i++)
            Check(
                !snapshot.VisibleRoiIds.Contains(offViewportId),
                $"off-viewport ROI membership round {i + 1} should exclude the distant ROI.");

        for (var i = 0; i < 10; i++)
            Check(
                snapshot.TileCount > 0,
                $"visible tile coverage round {i + 1} should be non-empty.");

        for (var i = 0; i < 10; i++)
            Check(
                visibleTileIds.Contains(visibleId),
                $"tile ROI union round {i + 1} should contain the visible ROI.");

        for (var i = 0; i < 10; i++)
            Check(
                !visibleTileIds.Contains(offViewportId),
                $"tile ROI union round {i + 1} should exclude the distant ROI.");

        for (var i = 0; i < 10; i++)
            Check(
                snapshot.Tiles.Select(tile => tile.Index).Distinct().Count() ==
                snapshot.TileCount,
                $"tile identity round {i + 1} should be unique.");

        for (var i = 0; i < 10; i++)
            Check(
                snapshot.Tiles.All(tile =>
                    tile.IntersectingRoiIds.Distinct().Count() ==
                    tile.IntersectingRoiIds.Count),
                $"tile ROI uniqueness round {i + 1} should hold.");

        for (var i = 0; i < 10; i++)
            Check(
                valid,
                $"tile ROI visibility validation round {i + 1} should pass.");

        for (var i = 0; i < 10; i++)
            Check(
                snapshot.LoadedTileCount <= snapshot.TileCount,
                $"loaded tile accounting round {i + 1} should be bounded.");

        for (var i = 0; i < 10; i++)
            Check(
                snapshot.Tiles.All(tile =>
                    tile.ViewportBounds.Width >= 0 &&
                    tile.ViewportBounds.Height >= 0 &&
                    float.IsFinite(tile.ViewportBounds.X) &&
                    float.IsFinite(tile.ViewportBounds.Y)),
                $"tile viewport geometry round {i + 1} should remain finite.");

        assert(
            round == 100,
            $"Tile ROI visibility validation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
