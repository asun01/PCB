namespace Asun.UI.Viewports;

public static class ViewportRenderFrameSummaryValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        ViewportRenderCommandStream stream,
        ViewportRenderFrameSummary summary)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(summary);

        var errors=new List<string>();

        if(summary.Generation!=stream.Generation)
            errors.Add("Render frame summary generation must match the command stream.");

        if(summary.CommandCount!=stream.CommandCount)
            errors.Add("Render frame summary command count must match the command stream.");

        if(summary.RegionCount!=stream.RegionCount)
            errors.Add("Render frame summary region count must match the command stream.");

        if(summary.TileCount!=stream.TileCount)
            errors.Add("Render frame summary tile count must match the command stream.");

        if(summary.RoiCount!=stream.RoiCount)
            errors.Add("Render frame summary ROI count must match the command stream.");

        if(summary.OverlayCount!=stream.OverlayCount)
            errors.Add("Render frame summary overlay count must match the command stream.");

        if(summary.InvalidationCount!=stream.InvalidationCount)
            errors.Add("Render frame summary invalidation count must match the command stream.");

        if(summary.FullSurfaceCount!=stream.FullSurfaceCount)
            errors.Add("Render frame summary full-surface count must match the command stream.");

        if(summary.CommandCount<0 ||
           summary.RegionCount<0 ||
           summary.TileCount<0 ||
           summary.RoiCount<0 ||
           summary.OverlayCount<0 ||
           summary.InvalidationCount<0 ||
           summary.FullSurfaceCount<0)
        {
            errors.Add("Render frame summary counts cannot be negative.");
        }

        if(summary.TileCount+
           summary.RoiCount+
           summary.OverlayCount+
           summary.InvalidationCount+
           summary.FullSurfaceCount>
           summary.CommandCount)
        {
            errors.Add("Render frame summary category counts cannot exceed command count.");
        }

        return errors;
    }

    public static bool IsValid(
        ViewportRenderCommandStream stream,
        ViewportRenderFrameSummary summary)=>
        Validate(stream,summary).Count==0;
}
