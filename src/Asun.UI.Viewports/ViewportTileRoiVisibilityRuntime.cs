using System.Drawing;

namespace Asun.UI.Viewports;

public readonly record struct ViewportTileVisibility(
    TileIndex Index,
    RectangleF ViewportBounds,
    IReadOnlyList<Guid> IntersectingRoiIds,
    bool IsLoaded);

public sealed class ViewportTileRoiVisibilitySnapshot
{
    internal ViewportTileRoiVisibilitySnapshot(
        IReadOnlyList<ViewportTileVisibility> tiles,
        IReadOnlySet<Guid> visibleRoiIds)
    {
        Tiles = tiles.ToArray();
        VisibleRoiIds = visibleRoiIds.ToHashSet();
    }

    public IReadOnlyList<ViewportTileVisibility> Tiles { get; }

    public IReadOnlySet<Guid> VisibleRoiIds { get; }

    public int TileCount => Tiles.Count;

    public int LoadedTileCount =>
        Tiles.Count(tile => tile.IsLoaded);
}

public static class ViewportTileRoiVisibilityRuntime
{
    public static ViewportTileRoiVisibilitySnapshot Build<TTile>(
        ViewportCompositeFrame<TTile> frame)
    {
        ArgumentNullException.ThrowIfNull(frame);

        var items = frame.Roi.Items
            .Select(item =>
            (
                item.Id,
                Bounds: item.Bounds,
                IsVisible: item.Bounds.IntersectsWith(
                    new RectangleF(
                        0,
                        0,
                        frame.Roi.Transform.ViewportSize.X,
                        frame.Roi.Transform.ViewportSize.Y))))
            .Where(item => item.IsVisible)
            .ToArray();

        var visibleRoiIds = items
            .Select(item => item.Id)
            .ToHashSet();

        var tiles = new List<ViewportTileVisibility>();

        foreach (var request in frame.Tiles.Requests.Where(
            request => request.IsVisible))
        {
            var imageRectangle = TileRequestPlanner.GetRequestRectangle(
                frame.Tiles.Transform.ImageSize,
                frame.Tiles.TileSize,
                request);

            var viewportBounds =
                frame.Tiles.Transform.ImageToViewportRectangle(
                    imageRectangle);

            var roiIds = items
                .Where(item => item.Bounds.IntersectsWith(viewportBounds))
                .Select(item => item.Id)
                .Distinct()
                .ToArray();

            tiles.Add(
                new ViewportTileVisibility(
                    request.Index,
                    viewportBounds,
                    roiIds,
                    frame.Tiles.LoadedTiles.ContainsKey(request.Index)));
        }

        return new ViewportTileRoiVisibilitySnapshot(
            tiles,
            visibleRoiIds);
    }
}
