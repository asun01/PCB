using System.Numerics;

namespace Asun.UI.Viewports;

public static class TileRangeValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        VisibleTileRange range,
        Vector2 imageSize,
        Vector2 tileSize)
    {
        var errors = new List<string>();

        if (!range.IsEmpty &&
            !ImageTileGeometry.IsRangeWithinGrid(imageSize, tileSize, range))
        {
            errors.Add("Tile range must remain inside the image grid.");
        }

        if (range.IsEmpty)
        {
            if (range.Count != 0)
                errors.Add("An empty tile range must report zero count.");
        }
        else
        {
            if (range.Width <= 0 || range.Height <= 0 || range.Count <= 0)
                errors.Add("A non-empty tile range must have positive dimensions and count.");

            if (range.Enumerate().Count() != range.Count)
                errors.Add("Tile range enumeration count must match the range count.");
        }

        return errors;
    }

    public static bool IsValid(
        VisibleTileRange range,
        Vector2 imageSize,
        Vector2 tileSize) =>
        Validate(range, imageSize, tileSize).Count == 0;
}
