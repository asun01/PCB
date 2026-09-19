using System.Security.Cryptography;
using System.Text;
using Asun.Domain.Quality;

namespace Asun.Production.Runtime;

public static class ProductionQualityInspectionProjectionRuntime
{
    public static ProductionQualityInspectionProjection Create(
        ProductionSessionReport productionReport,
        QualityInspectionRun qualityRun)
    {
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(qualityRun);

        if(!productionReport.Frames.Any() ||
           !QualityInspectionRunValidationRuntime.IsValid(qualityRun))
        {
            throw new ArgumentException(
                "Production report and quality run must both be valid.");
        }

        if(productionReport.FrameCount!=qualityRun.ResultCount)
            throw new ArgumentException(
                "Production frame count must equal quality result count.");

        var frames=productionReport.Frames
            .OrderBy(frame=>frame.Sequence.Value)
            .ToArray();
        var results=qualityRun.Results
            .OrderBy(result=>result.Sequence)
            .ThenBy(result=>result.SnapshotId)
            .ThenBy(result=>result.ResultId)
            .ToArray();

        var links=new List<ProductionQualityFrameLink>(frames.Length);

        for(var index=0;index<frames.Length;index++)
        {
            var frame=frames[index];
            var result=results[index];

            if(frame.Sequence.Value!=result.Sequence)
                throw new ArgumentException(
                    $"Production sequence {frame.Sequence.Value} does not align with quality sequence {result.Sequence}.");

            links.Add(
                new ProductionQualityFrameLink(
                    frame.Sequence.Value,
                    frame.InputFingerprint,
                    result.ResultId,
                    result.SnapshotId,
                    result.Findings.Count,
                    result.Evidence.Count));
        }

        var fingerprint=CreateFingerprint(
            productionReport.SessionId,
            productionReport.Fingerprint,
            qualityRun.RunId,
            links);

        return new ProductionQualityInspectionProjection(
            productionReport.SessionId,
            productionReport.Fingerprint,
            qualityRun.RunId,
            links,
            fingerprint);
    }

    internal static string CreateFingerprint(
        Guid productionSessionId,
        string productionFingerprint,
        Guid qualityRunId,
        IReadOnlyList<ProductionQualityFrameLink> links)
    {
        var builder=new StringBuilder();
        builder.Append(productionSessionId).Append('|')
            .Append(productionFingerprint).Append('|')
            .Append(qualityRunId).Append('|');

        foreach(var link in links)
        {
            builder.Append(link.Sequence).Append('|')
                .Append(link.ProductionInputFingerprint).Append('|')
                .Append(link.QualityResultId).Append('|')
                .Append(link.QualitySnapshotId).Append('|')
                .Append(link.FindingCount).Append('|')
                .Append(link.EvidenceLinkCount).Append('|');
        }

        return Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(builder.ToString())))
            .ToLowerInvariant();
    }
}
