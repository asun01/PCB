using System.Drawing;

namespace Asun.UI.Viewports;

public sealed class ViewportRenderBatch
{
    internal ViewportRenderBatch(
        IReadOnlyList<ViewportRenderWorkItem> items,
        IReadOnlyList<RectangleF> regions,
        long generation)
    {
        Items = items.ToArray();
        Regions = regions.ToArray();
        Generation = generation;
    }

    public IReadOnlyList<ViewportRenderWorkItem> Items { get; }

    public IReadOnlyList<RectangleF> Regions { get; }

    public long Generation { get; }

    public int ItemCount => Items.Count;

    public int RegionCount => Regions.Count;

    public int TileCount =>
        Items.Count(item =>
            item.Kind == ViewportRenderWorkKind.Tile &&
            !item.IsInvalidation);

    public int RoiCount =>
        Items.Count(item =>
            item.Kind == ViewportRenderWorkKind.Roi &&
            !item.IsInvalidation);

    public int OverlayCount =>
        Items.Count(item =>
            item.Kind == ViewportRenderWorkKind.Overlay);

    public int InvalidationCount =>
        Items.Count(item => item.IsInvalidation);

    public int FullSurfaceCount =>
        Items.Count(item =>
            item.Kind == ViewportRenderWorkKind.FullSurface);

    public bool HasFullSurface => FullSurfaceCount != 0;

    public bool IsEmpty => Items.Count == 0;
}

public static class ViewportRenderBatchRuntime
{
    public static ViewportRenderBatch Create(
        ViewportRenderWorkPlan plan,
        ViewportTransform transform)
    {
        ArgumentNullException.ThrowIfNull(plan);

        var regions = ViewportRenderRegionRuntime.ClipAndMerge(
            plan.Items.Select(item => item.Bounds),
            transform);

        return new ViewportRenderBatch(
            plan.Items,
            regions,
            plan.Generation);
    }

    public static ViewportRenderBatch Empty(long generation) =>
        new(
            Array.Empty<ViewportRenderWorkItem>(),
            Array.Empty<RectangleF>(),
            generation);
}
