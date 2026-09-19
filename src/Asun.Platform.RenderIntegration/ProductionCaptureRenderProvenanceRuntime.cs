using Asun.Production.Runtime;
using Asun.UI.Viewports;

namespace Asun.Platform.RenderIntegration;

public static class ProductionCaptureRenderProvenanceRuntime
{
    public static IReadOnlyList<ProductionCaptureRenderProvenance> Create(
        ProductionSessionReport productionReport,
        IReadOnlyList<ProductionFrameProvenance> provenance,
        IReadOnlyList<ViewportRenderFrameSummary> renderSummaries)
    {
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(provenance);
        ArgumentNullException.ThrowIfNull(renderSummaries);

        if(productionReport.FrameCount!=provenance.Count ||
           productionReport.FrameCount!=renderSummaries.Count)
        {
            throw new ArgumentException("Production, capture provenance, and render summaries must have equal counts.");
        }

        if(!ProductionFrameProvenanceRuntime.IsValid(productionReport,provenance))
            throw new ArgumentException("Production frame provenance is invalid.",nameof(provenance));

        var orderedProvenance=provenance.OrderBy(item=>item.Sequence.Value).ToArray();
        var productionFrames=productionReport.Frames.OrderBy(item=>item.Sequence.Value).ToArray();
        var result=new List<ProductionCaptureRenderProvenance>(renderSummaries.Count);

        for(var index=0;index<renderSummaries.Count;index++)
        {
            var capture=orderedProvenance[index];
            var production=productionFrames[index];
            var summary=renderSummaries[index];

            if(capture.Sequence.Value!=production.Sequence.Value)
                throw new ArgumentException("Capture provenance and production sequence mismatch.");

            if(capture.PayloadFingerprint!=production.InputFingerprint)
                throw new ArgumentException("Capture provenance and production fingerprint mismatch.");

            if(!IsValidSummary(summary))
                throw new ArgumentException("Render summary contains invalid factual counts.",nameof(renderSummaries));

            result.Add(
                new ProductionCaptureRenderProvenance(
                    capture.Sequence.Value,
                    capture.PayloadFingerprint,
                    capture.Width,
                    capture.Height,
                    capture.PixelFormat,
                    summary.Generation,
                    summary,
                    ViewportRenderFrameFingerprintRuntime.CreateFingerprint(summary)));
        }

        return result;
    }

    public static IReadOnlyList<string> Validate(
        ProductionSessionReport productionReport,
        IReadOnlyList<ProductionCaptureRenderProvenance> provenance)
    {
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(provenance);

        var errors=new List<string>();
        if(productionReport.FrameCount!=provenance.Count)
            errors.Add("Capture/render provenance count must match production frame count.");

        var productionFrames=productionReport.Frames.OrderBy(item=>item.Sequence.Value).ToArray();
        var ordered=provenance.OrderBy(item=>item.Sequence).ToArray();
        var count=Math.Min(productionFrames.Length,ordered.Length);

        for(var index=0;index<count;index++)
        {
            var production=productionFrames[index];
            var actual=ordered[index];

            if(actual.Sequence!=production.Sequence.Value)
                errors.Add($"Capture/render provenance {index} sequence mismatch.");
            if(actual.PayloadFingerprint!=production.InputFingerprint)
                errors.Add($"Capture/render provenance {index} payload fingerprint mismatch.");
            if(actual.Width<=0 || actual.Height<=0)
                errors.Add($"Capture/render provenance {index} dimensions must be positive.");
            if(string.IsNullOrWhiteSpace(actual.PixelFormat))
                errors.Add($"Capture/render provenance {index} pixel format cannot be blank.");
            if(!IsValidSummary(actual.RenderSummary))
                errors.Add($"Capture/render provenance {index} render summary is invalid.");
            if(actual.RenderGeneration!=actual.RenderSummary.Generation)
                errors.Add($"Capture/render provenance {index} render generation mismatch.");
            if(actual.RenderFingerprint.Length!=64 ||
               !actual.RenderFingerprint.All(character=>
                   Uri.IsHexDigit(character) &&
                   char.ToLowerInvariant(character)==character))
            {
                errors.Add($"Capture/render provenance {index} render fingerprint is invalid.");
            }
            else if(ViewportRenderFrameFingerprintRuntime.CreateFingerprint(actual.RenderSummary)!=actual.RenderFingerprint)
            {
                errors.Add($"Capture/render provenance {index} render fingerprint does not match the summary.");
            }
        }

        if(provenance.Select(item=>item.Sequence).Distinct().Count()!=provenance.Count)
            errors.Add("Capture/render provenance sequences must be unique.");

        return errors;
    }

    public static bool IsValid(
        ProductionSessionReport productionReport,
        IReadOnlyList<ProductionCaptureRenderProvenance> provenance)=>
        Validate(productionReport,provenance).Count==0;

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
