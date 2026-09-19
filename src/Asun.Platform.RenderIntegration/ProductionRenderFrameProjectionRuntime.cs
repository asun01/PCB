using System.Security.Cryptography;
using System.Text;
using Asun.Production.Runtime;
using Asun.UI.Viewports;

namespace Asun.Platform.RenderIntegration;

public static class ProductionRenderFrameProjectionRuntime
{
    public static ProductionRenderFrameProjection Create(
        ProductionSessionReport productionReport,
        IReadOnlyList<ViewportRenderFrameSummary> renderSummaries)
    {
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(renderSummaries);

        if(productionReport.FrameCount!=renderSummaries.Count)
            throw new ArgumentException(
                "Render summary count must match the production frame count.",
                nameof(renderSummaries));

        var productionFrames=productionReport.Frames
            .OrderBy(frame=>frame.Sequence.Value)
            .ToArray();
        var entries=new List<ProductionRenderFrameEntry>(renderSummaries.Count);

        for(var index=0;index<productionFrames.Length;index++)
        {
            var productionFrame=productionFrames[index];
            var renderSummary=renderSummaries[index];

            if(!ViewportRenderFrameSummaryValidationRuntime
                .IsValidAgainstCounts(renderSummary))
            {
                throw new ArgumentException(
                    $"Render summary at index {index} is invalid.");
            }

            entries.Add(
                new ProductionRenderFrameEntry(
                    productionFrame.Sequence.Value,
                    productionFrame.InputFingerprint,
                    renderSummary));
        }

        var fingerprint=CreateFingerprint(
            productionReport.SessionId,
            productionReport.Fingerprint,
            entries);

        return new ProductionRenderFrameProjection(
            productionReport.SessionId,
            productionReport.Fingerprint,
            entries,
            fingerprint);
    }

    private static bool IsValidSummaryCounts(\n        ViewportRenderFrameSummary summary)=>\n        summary.Generation>=0 &&\n        summary.CommandCount>=0 &&\n        summary.RegionCount>=0 &&\n        summary.TileCount>=0 &&\n        summary.RoiCount>=0 &&\n        summary.OverlayCount>=0 &&\n        summary.InvalidationCount>=0 &&\n        summary.FullSurfaceCount>=0 &&\n        summary.TileCount+\n        summary.RoiCount+\n        summary.OverlayCount+\n        summary.InvalidationCount+\n        summary.FullSurfaceCount<=summary.CommandCount;\n\n    internal static string CreateFingerprint(
        Guid productionSessionId,
        string productionFingerprint,
        IReadOnlyList<ProductionRenderFrameEntry> frames)
    {
        var builder=new StringBuilder();
        builder.Append(productionSessionId).Append('|')
            .Append(productionFingerprint).Append('|');

        foreach(var frame in frames.OrderBy(item=>item.Sequence))
        {
            var summary=frame.RenderSummary;
            builder.Append(frame.Sequence).Append('|')
                .Append(frame.ProductionInputFingerprint).Append('|')
                .Append(summary.Generation).Append('|')
                .Append(summary.CommandCount).Append('|')
                .Append(summary.RegionCount).Append('|')
                .Append(summary.TileCount).Append('|')
                .Append(summary.RoiCount).Append('|')
                .Append(summary.OverlayCount).Append('|')
                .Append(summary.InvalidationCount).Append('|')
                .Append(summary.FullSurfaceCount).Append('|');
        }

        return Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(builder.ToString())))
            .ToLowerInvariant();
    }
}
