using System.Numerics;
using Asun.UI.Viewports;

public static class TileRangeValidationHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        var imageSize = new Vector2(1000, 800);
        var tileSize = new Vector2(256, 256);

        var visible = ImageTileGeometry.CalculateVisibleTiles(
            imageSize,
            tileSize,
            new System.Drawing.RectangleF(100, 100, 300, 300));

        var edge = ImageTileGeometry.CalculateVisibleTiles(
            imageSize,
            tileSize,
            new System.Drawing.RectangleF(768, 512, 232, 288));

        var empty = ImageTileGeometry.CalculateVisibleTiles(
            imageSize,
            tileSize,
            new System.Drawing.RectangleF(1200, 900, 100, 100));

        var expanded = ImageTileGeometry.ExpandTileRange(
            imageSize,
            tileSize,
            visible,
            1);

        var intersection = ImageTileGeometry.IntersectRanges(
            visible,
            new VisibleTileRange(
                new TileIndex(1, 1),
                new TileIndex(3, 3)));

        var union = ImageTileGeometry.UnionRanges(
            visible,
            new VisibleTileRange(
                new TileIndex(2, 0),
                new TileIndex(3, 1)));

        var enumerated = TileRangeRuntime.Enumerate(visible);
        var unionRectangle = TileRangeRuntime.UnionRectangle(
            imageSize,
            tileSize,
            visible);

        var visibleValid =
            TileRangeValidationRuntime.IsValid(
                visible,
                imageSize,
                tileSize);

        var edgeValid =
            TileRangeValidationRuntime.IsValid(
                edge,
                imageSize,
                tileSize);

        var emptyValid =
            TileRangeValidationRuntime.IsValid(
                empty,
                imageSize,
                tileSize);

        for (var i = 0; i < 10; i++)
            Check(visible.Width == 2 && visible.Height == 2, $"visible range dimensions round {i + 1} should be 2x2.");

        for (var i = 0; i < 10; i++)
            Check(visible.Count == 4, $"visible range count round {i + 1} should be four.");

        for (var i = 0; i < 10; i++)
            Check(enumerated.Count == visible.Count, $"range enumeration round {i + 1} should cover every tile.");

        for (var i = 0; i < 10; i++)
            Check(expanded.Minimum == new TileIndex(0, 0) && expanded.Maximum == new TileIndex(2, 2), $"expanded range round {i + 1} should clamp inside the grid.");

        for (var i = 0; i < 10; i++)
            Check(intersection.Minimum == new TileIndex(1, 1) && intersection.Maximum == new TileIndex(1, 1), $"range intersection round {i + 1} should isolate the overlap.");

        for (var i = 0; i < 10; i++)
            Check(union.Minimum == new TileIndex(0, 0) && union.Maximum == new TileIndex(3, 1), $"range union round {i + 1} should contain both ranges.");

        for (var i = 0; i < 10; i++)
            Check(unionRectangle.Width > 0 && unionRectangle.Height > 0, $"range rectangle round {i + 1} should be non-empty.");

        for (var i = 0; i < 10; i++)
            Check(edge.Minimum == new TileIndex(3, 2) && edge.Maximum == new TileIndex(3, 3), $"edge range round {i + 1} should stay inside the final tiles.");

        for (var i = 0; i < 10; i++)
            Check(empty.IsEmpty && empty.Count == 0, $"empty range round {i + 1} should report zero coverage.");

        for (var i = 0; i < 10; i++)
            Check(visibleValid && edgeValid && emptyValid, $"range validation round {i + 1} should pass.");

        assert(round == 100, $"Tile range validation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
