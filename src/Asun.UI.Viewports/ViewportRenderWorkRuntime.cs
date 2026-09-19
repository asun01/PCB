using System.Drawing;

namespace Asun.UI.Viewports;

public enum ViewportRenderWorkKind
{
    Tile,
    Roi,
    Overlay,
    Selection,
    FullSurface
}

public readonly record struct ViewportRenderWorkItem(
    ViewportRenderWorkKind Kind,
    RectangleF Bounds,
    Guid RoiId,
    TileIndex? Tile,
    long Generation,
    bool IsInvalidation = false);

public sealed class ViewportRenderWorkPlan
{
    internal ViewportRenderWorkPlan(
        IReadOnlyList<ViewportRenderWorkItem> items,
        ViewportDirtyFlags consumedFlags,
        long generation)
    {
        Items = items.ToArray();
        ConsumedFlags = consumedFlags;
        Generation = generation;
    }

    public IReadOnlyList<ViewportRenderWorkItem> Items { get; }

    public ViewportDirtyFlags ConsumedFlags { get; }

    public long Generation { get; }

    public bool IsEmpty => Items.Count == 0;

    public int TileWorkCount =>
        Items.Count(item => item.Kind == ViewportRenderWorkKind.Tile);

    public int RoiWorkCount =>
        Items.Count(item => item.Kind == ViewportRenderWorkKind.Roi);
}

public static class ViewportRenderWorkRuntime
{
    public static ViewportRenderWorkPlan Plan<TTile>(
        ViewportCompositeFrame<TTile> frame,
        ViewportDirtyFlags dirtyFlags)
    {
        ArgumentNullException.ThrowIfNull(frame);

        var items = new List<ViewportRenderWorkItem>();

        var imageDirty =
            dirtyFlags.HasFlag(ViewportDirtyFlags.Image) ||
            dirtyFlags.HasFlag(ViewportDirtyFlags.Transform);

        var roiDirty =
            dirtyFlags.HasFlag(ViewportDirtyFlags.Roi) ||
            dirtyFlags.HasFlag(ViewportDirtyFlags.Selection);

        if (imageDirty)
        {
            foreach (var request in frame.Tiles.Requests)
            {
                if (!request.IsVisible)
                    continue;

                var imageBounds = TileRequestPlanner.GetRequestRectangle(
                    frame.Tiles.Transform.ImageSize,
                    frame.Tiles.TileSize,
                    request);

                var viewportBounds =
                    frame.Tiles.Transform.ImageToViewportRectangle(imageBounds);

                items.Add(
                    new ViewportRenderWorkItem(
                        ViewportRenderWorkKind.Tile,
                        viewportBounds,
                        Guid.Empty,
                        request.Index,
                        frame.Generation));
            }
        }

        if (roiDirty)
        {
            foreach (var command in frame.SceneCommands)
            {
                var bounds = ViewportSceneRuntime.GetCommandBounds(command);
                items.Add(
                    new ViewportRenderWorkItem(
                        ViewportRenderWorkKind.Roi,
                        bounds,
                        command.RoiId,
                        null,
                        frame.Generation));
            }
        }
        else if (frame.SceneDiff.Count > 0)
        {
            foreach (var diff in frame.SceneDiff)
            {
                if (diff.Current is { } current)
                {
                    items.Add(
                        new ViewportRenderWorkItem(
                            ViewportRenderWorkKind.Roi,
                            ViewportSceneRuntime.GetCommandBounds(current),
                            diff.RoiId,
                            null,
                            frame.Generation));
                }

                if (diff.Previous is { } previous)
                {
                    items.Add(
                        new ViewportRenderWorkItem(
                            ViewportRenderWorkKind.Roi,
                            ViewportSceneRuntime.GetCommandBounds(previous),
                            diff.RoiId,
                            null,
                            frame.Generation,
                            IsInvalidation: true));
                }
            }
        }

        if (dirtyFlags.HasFlag(ViewportDirtyFlags.Overlay))
        {
            items.Add(
                new ViewportRenderWorkItem(
                    ViewportRenderWorkKind.Overlay,
                    new RectangleF(
                        0,
                        0,
                        frame.Tiles.Transform.ViewportSize.X,
                        frame.Tiles.Transform.ViewportSize.Y),
                    Guid.Empty,
                    null,
                    frame.Generation));
        }

        if (dirtyFlags == ViewportDirtyFlags.All)
        {
            items.Add(
                new ViewportRenderWorkItem(
                    ViewportRenderWorkKind.FullSurface,
                    new RectangleF(
                        0,
                        0,
                        frame.Tiles.Transform.ViewportSize.X,
                        frame.Tiles.Transform.ViewportSize.Y),
                    Guid.Empty,
                    null,
                    frame.Generation));
        }

        return new ViewportRenderWorkPlan(
            Deduplicate(items),
            dirtyFlags,
            frame.Generation);
    }

    private static RectangleF Union(
        RectangleF a,
        RectangleF b)
    {
        if (a.IsEmpty)
            return b;

        if (b.IsEmpty)
            return a;

        return RectangleF.Union(a, b);
    }

    private static IReadOnlyList<ViewportRenderWorkItem> Deduplicate(
        IEnumerable<ViewportRenderWorkItem> items)
    {
        return items
            .GroupBy(item => new
            {
                item.Kind,
                item.RoiId,
                item.Tile,
                item.Bounds,
                item.Generation,
                item.IsInvalidation
            })
            .Select(group => group.First())
            .ToArray();
    }
}
