using Asun.Production.Runtime;
using Asun.UI.Viewports;

namespace Asun.Platform.RenderIntegration;

public static class ProductionRenderFrameProjectionValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        ProductionSessionReport productionReport,
        ProductionRenderFrameProjection projection)
    {
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(projection);

        var errors=new List<string>();

        if(projection.ProductionSessionId!=productionReport.SessionId)
            errors.Add("Render projection production session id must match the report.");

        if(projection.ProductionFingerprint!=productionReport.Fingerprint)
            errors.Add("Render projection production fingerprint must match the report.");

        if(projection.Frames.Count!=productionReport.FrameCount)
            errors.Add("Render projection frame count must match the production report.");

        var productionFrames=productionReport.Frames
            .OrderBy(frame=>frame.Sequence.Value)
            .ToArray();
        var projectionFrames=projection.Frames
            .OrderBy(frame=>frame.Sequence)
            .ToArray();

        var count=Math.Min(productionFrames.Length,projectionFrames.Length);
        for(var index=0;index<count;index++)
        {
            var productionFrame=productionFrames[index];
            var projectionFrame=projectionFrames[index];

            if(projectionFrame.Sequence!=productionFrame.Sequence.Value)
                errors.Add($"Render projection frame {index} sequence mismatch.");

            if(projectionFrame.ProductionInputFingerprint!=productionFrame.InputFingerprint)
                errors.Add($"Render projection frame {index} input fingerprint mismatch.");

            if(!IsValidSummary(projectionFrame.RenderSummary))
                errors.Add($"Render projection frame {index} summary is invalid.");
        }

        if(projection.Fingerprint.Length!=64 ||
           !projection.Fingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Render projection fingerprint must be 64 lowercase hexadecimal characters.");
        }

        if(errors.Count==0)
        {
            var expected=ProductionRenderFrameProjectionRuntime.CreateFingerprint(
                productionReport.SessionId,
                productionReport.Fingerprint,
                projection.Frames);

            if(expected!=projection.Fingerprint)
                errors.Add("Render projection fingerprint does not match its canonical content.");
        }

        return errors;
    }

    public static bool IsValid(
        ProductionSessionReport productionReport,
        ProductionRenderFrameProjection projection)=>
        Validate(productionReport,projection).Count==0;

    private static bool IsValidSummary(
        ViewportRenderFrameSummary summary)=>
        summary.Generation>=0 &&
        summary.CommandCount>=0 &&
        summary.RegionCount>=0 &&
        summary.TileCount>=0 &&
        summary.RoiCount>=0 &&
        summary.OverlayCount>=0 &&
        summary.InvalidationCount>=0 &&
        summary.FullSurfaceCount>=0 &&
        summary.TileCount+
        summary.RoiCount+
        summary.OverlayCount+
        summary.InvalidationCount+
        summary.FullSurfaceCount<=summary.CommandCount;
}
