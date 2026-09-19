namespace Asun.UI.Viewports;

public sealed record ViewportRenderFrameSummary(
    long Generation,
    int CommandCount,
    int RegionCount,
    int TileCount,
    int RoiCount,
    int OverlayCount,
    int InvalidationCount,
    int FullSurfaceCount);
