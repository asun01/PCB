using System.Drawing;

namespace Asun.UI.Viewports;

public static class ViewportTileRoiVisibilityValidationRuntime
{
    public static IReadOnlyList<string> Validate<TTile>(
        ViewportTileRoiVisibilitySnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var errors = new List<string>();

        if (snapshot.Tiles.Select(tile => tile.Index).Distinct().Count() !=
            snapshot.Tiles.Count)
        {
            errors.Add("Tile visibility snapshot contains duplicate tile indices.");
        }

        var knownVisibleIds = snapshot.VisibleRoiIds;

        foreach (var tile in snapshot.Tiles)
        {
            if (!IsFinite(tile.ViewportBounds))
                errors.Add("Tile ROI visibility contains non-finite viewport bounds.");

            if (tile.IntersectingRoiIds.Any(id =>
                id == Guid.Empty || !knownVisibleIds.Contains(id)))
            {
                errors.Add("A tile references a ROI that is absent from visible ROI ids.");
            }

            if (tile.IntersectingRoiIds.Count !=
                tile.IntersectingRoiIds.Distinct().Count())
            {
                errors.Add("A tile contains duplicate ROI ids.");
            }
        }

        if (snapshot.LoadedTileCount > snapshot.TileCount)
            errors.Add("Loaded tile count cannot exceed the tile count.");

        if (snapshot.Tiles.Count > 0)
        {
            var tileVisibleIds = snapshot.Tiles
                .SelectMany(tile => tile.IntersectingRoiIds)
                .ToHashSet();

            foreach (var id in knownVisibleIds)
            {
                if (!tileVisibleIds.Contains(id))
                    errors.Add("A visible ROI is missing from every visible tile.");
            }
        }

        return errors;
    }

    public static bool IsValid<TTile>(
        ViewportTileRoiVisibilitySnapshot snapshot) =>
        Validate<TTile>(snapshot).Count == 0;

    private static bool IsFinite(RectangleF rectangle) =>
        float.IsFinite(rectangle.X) &&
        float.IsFinite(rectangle.Y) &&
        float.IsFinite(rectangle.Width) &&
        float.IsFinite(rectangle.Height);
}
