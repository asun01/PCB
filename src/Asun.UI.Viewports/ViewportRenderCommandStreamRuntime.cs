using System.Drawing;

namespace Asun.UI.Viewports;

public enum ViewportRenderCommandKind
{
    ClearInvalidatedRegion,
    FullSurfaceClear,
    DrawTile,
    DrawRoi,
    DrawOverlay
}

public readonly record struct ViewportRenderCommand(
    int Sequence,
    ViewportRenderCommandKind Kind,
    ViewportRenderWorkItem WorkItem,
    RectangleF Bounds);

public sealed class ViewportRenderCommandStream
{
    internal ViewportRenderCommandStream(
        long generation,
        IReadOnlyList<ViewportRenderCommand> commands,
        IReadOnlyList<RectangleF> regions)
    {
        Generation = generation;
        Commands = commands.ToArray();
        Regions = regions.ToArray();
    }

    public long Generation { get; }

    public IReadOnlyList<ViewportRenderCommand> Commands { get; }

    public IReadOnlyList<RectangleF> Regions { get; }

    public int CommandCount => Commands.Count;

    public int RegionCount => Regions.Count;

    public int TileCount =>
        Commands.Count(command =>
            command.Kind == ViewportRenderCommandKind.DrawTile &&
            !command.WorkItem.IsInvalidation);

    public int RoiCount =>
        Commands.Count(command =>
            command.Kind == ViewportRenderCommandKind.DrawRoi &&
            !command.WorkItem.IsInvalidation);

    public int OverlayCount =>
        Commands.Count(command =>
            command.Kind == ViewportRenderCommandKind.DrawOverlay);

    public int InvalidationCount =>
        Commands.Count(command =>
            command.Kind == ViewportRenderCommandKind.ClearInvalidatedRegion);

    public int FullSurfaceCount =>
        Commands.Count(command =>
            command.Kind == ViewportRenderCommandKind.FullSurfaceClear);
}

public static class ViewportRenderCommandStreamRuntime
{
    public static ViewportRenderCommandStream Build(
        ViewportRenderBatch batch)
    {
        ArgumentNullException.ThrowIfNull(batch);

        var commands = new List<ViewportRenderCommand>(batch.ItemCount);
        var sequence = 0;

        foreach (var item in batch.Items)
        {
            var kind = item.Kind switch
            {
                ViewportRenderWorkKind.Tile =>
                    ViewportRenderCommandKind.DrawTile,
                ViewportRenderWorkKind.Roi =>
                    item.IsInvalidation
                        ? ViewportRenderCommandKind.ClearInvalidatedRegion
                        : ViewportRenderCommandKind.DrawRoi,
                ViewportRenderWorkKind.Overlay =>
                    ViewportRenderCommandKind.DrawOverlay,
                ViewportRenderWorkKind.FullSurface =>
                    ViewportRenderCommandKind.FullSurfaceClear,
                _ => throw new ArgumentOutOfRangeException()
            };

            commands.Add(
                new ViewportRenderCommand(
                    ++sequence,
                    kind,
                    item,
                    item.Bounds));
        }

        return new ViewportRenderCommandStream(
            batch.Generation,
            commands,
            batch.Regions);
    }
}
