namespace Asun.UI.Viewports;

public static class ViewportRenderFrameSummaryRuntime
{
    public static ViewportRenderFrameSummary Create(
        ViewportRenderCommandStream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        return new ViewportRenderFrameSummary(
            stream.Generation,
            stream.CommandCount,
            stream.RegionCount,
            stream.TileCount,
            stream.RoiCount,
            stream.OverlayCount,
            stream.InvalidationCount,
            stream.FullSurfaceCount);
    }
}
