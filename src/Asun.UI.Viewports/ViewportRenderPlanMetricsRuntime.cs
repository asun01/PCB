namespace Asun.UI.Viewports;

public readonly record struct ViewportRenderPlanMetrics(
    int Total,
    int Tile,
    int SelectedRoi,
    int Roi,
    int Overlay,
    int FullSurface,
    int Regions);

public static class ViewportRenderPlanMetricsRuntime
{
    public static ViewportRenderPlanMetrics Capture(
        ViewportRenderWorkPlan plan,
        ViewportRenderBatch batch)
    {
        ArgumentNullException.ThrowIfNull(plan);
        ArgumentNullException.ThrowIfNull(batch);

        return new ViewportRenderPlanMetrics(
            plan.Items.Count,
            plan.Items.Count(item => item.Kind == ViewportRenderWorkKind.Tile),
            plan.Items.Count(item =>
                item.Kind == ViewportRenderWorkKind.Roi &&
                item.RoiId != Guid.Empty &&
                item.Generation == plan.Generation),
            plan.Items.Count(item => item.Kind == ViewportRenderWorkKind.Roi),
            plan.Items.Count(item => item.Kind == ViewportRenderWorkKind.Overlay),
            plan.Items.Count(item => item.Kind == ViewportRenderWorkKind.FullSurface),
            batch.RegionCount);
    }
}
