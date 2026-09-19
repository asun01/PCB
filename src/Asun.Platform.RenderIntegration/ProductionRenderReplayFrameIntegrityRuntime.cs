using Asun.Production.Runtime;
using Asun.UI.Viewports;

namespace Asun.Platform.RenderIntegration;

public static class ProductionRenderReplayFrameIntegrityRuntime
{
    public static IReadOnlyList<ProductionRenderReplayFrameIntegrity> CreateFrames(
        ProductionSessionReport productionReport,
        IReadOnlyList<ViewportRenderFrameSummary> renderSummaries)
    {
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(renderSummaries);

        if(productionReport.FrameCount!=renderSummaries.Count)
            throw new ArgumentException(
                "Render summary count must match production frame count.",
                nameof(renderSummaries));

        var productionFrames=productionReport.Frames
            .OrderBy(frame=>frame.Sequence.Value)
            .ToArray();
        var result=new List<ProductionRenderReplayFrameIntegrity>(renderSummaries.Count);

        for(var index=0;index<renderSummaries.Count;index++)
        {
            var summary=renderSummaries[index];
            if(!IsValidSummary(summary))
                throw new ArgumentException(
                    $"Render summary {index} is invalid.",
                    nameof(renderSummaries));

            result.Add(
                new ProductionRenderReplayFrameIntegrity(
                    productionFrames[index].Sequence.Value,
                    productionFrames[index].InputFingerprint,
                    summary.Generation,
                    ViewportRenderFrameFingerprintRuntime.CreateFingerprint(summary)));
        }

        return result;
    }

    public static IReadOnlyList<string> Validate(
        ProductionSessionReport productionReport,
        IReadOnlyList<ProductionRenderReplayFrameIntegrity> frames)
    {
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(frames);

        var errors=new List<string>();
        if(productionReport.FrameCount!=frames.Count)
            errors.Add("Render replay frame count must match production frame count.");

        var productionFrames=productionReport.Frames
            .OrderBy(frame=>frame.Sequence.Value)
            .ToArray();
        var orderedFrames=frames
            .OrderBy(frame=>frame.Sequence)
            .ToArray();
        var count=Math.Min(productionFrames.Length,orderedFrames.Length);

        for(var index=0;index<count;index++)
        {
            var production=productionFrames[index];
            var actual=orderedFrames[index];

            if(actual.Sequence!=production.Sequence.Value)
                errors.Add($"Render replay frame {index} sequence mismatch.");

            if(actual.ProductionInputFingerprint!=production.InputFingerprint)
                errors.Add($"Render replay frame {index} input fingerprint mismatch.");

            if(actual.RenderGeneration<0)
                errors.Add($"Render replay frame {index} render generation cannot be negative.");

            if(actual.RenderFingerprint.Length!=64 ||
               !actual.RenderFingerprint.All(character=>
                   Uri.IsHexDigit(character) &&
                   char.ToLowerInvariant(character)==character))
            {
                errors.Add($"Render replay frame {index} render fingerprint is invalid.");
            }
        }

        if(frames.Select(frame=>frame.Sequence).Distinct().Count()!=frames.Count)
            errors.Add("Render replay frame sequences must be unique.");

        return errors;
    }

    public static bool IsValid(
        ProductionSessionReport productionReport,
        IReadOnlyList<ProductionRenderReplayFrameIntegrity> frames)=>
        Validate(productionReport,frames).Count==0;

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
