using Asun.Domain.Quality;

namespace Asun.Production.Runtime;

public static class ProductionQualityInspectionProjectionValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        ProductionSessionReport productionReport,
        QualityInspectionRun qualityRun,
        ProductionQualityInspectionProjection projection)
    {
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(qualityRun);
        ArgumentNullException.ThrowIfNull(projection);

        var errors=new List<string>();

        if(!ProductionSessionValidationShapeRuntime.HasValidIdentity(
            productionReport))
        {
            errors.Add("Production report identity is invalid.");
        }

        if(!QualityInspectionRunValidationRuntime.IsValid(qualityRun))
            errors.Add("Quality inspection run is invalid.");

        if(projection.ProductionSessionId!=productionReport.SessionId)
            errors.Add("Projection production session id must match the report.");

        if(projection.ProductionFingerprint!=productionReport.Fingerprint)
            errors.Add("Projection production fingerprint must match the report.");

        if(projection.QualityRunId!=qualityRun.RunId)
            errors.Add("Projection quality run id must match the run.");

        if(projection.Links.Count!=productionReport.FrameCount ||
           projection.Links.Count!=qualityRun.ResultCount)
        {
            errors.Add("Projection link count must match production frames and quality results.");
        }

        var frames=productionReport.Frames
            .OrderBy(frame=>frame.Sequence.Value)
            .ToArray();
        var results=qualityRun.Results
            .OrderBy(result=>result.Sequence)
            .ThenBy(result=>result.SnapshotId)
            .ThenBy(result=>result.ResultId)
            .ToArray();

        var count=Math.Min(
            projection.Links.Count,
            Math.Min(frames.Length,results.Length));

        for(var index=0;index<count;index++)
        {
            var link=projection.Links[index];
            var frame=frames[index];
            var result=results[index];

            if(link.Sequence!=frame.Sequence.Value ||
               link.Sequence!=result.Sequence)
            {
                errors.Add($"Projection link {index} sequence mismatch.");
            }

            if(link.ProductionInputFingerprint!=frame.InputFingerprint)
                errors.Add($"Projection link {index} input fingerprint mismatch.");

            if(link.QualityResultId!=result.ResultId)
                errors.Add($"Projection link {index} quality result id mismatch.");

            if(link.QualitySnapshotId!=result.SnapshotId)
                errors.Add($"Projection link {index} quality snapshot id mismatch.");

            if(link.FindingCount!=result.Findings.Count)
                errors.Add($"Projection link {index} finding count mismatch.");

            if(link.EvidenceLinkCount!=result.Evidence.Count)
                errors.Add($"Projection link {index} evidence-link count mismatch.");
        }

        if(projection.Fingerprint.Length!=64 ||
           !projection.Fingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Production-quality projection fingerprint must be 64 lowercase hexadecimal characters.");
        }

        if(errors.Count==0)
        {
            var expected=ProductionQualityInspectionProjectionRuntime.CreateFingerprint(
                productionReport.SessionId,
                productionReport.Fingerprint,
                qualityRun.RunId,
                projection.Links);

            if(expected!=projection.Fingerprint)
                errors.Add("Production-quality projection fingerprint does not match its canonical content.");
        }

        return errors;
    }

    public static bool IsValid(
        ProductionSessionReport productionReport,
        QualityInspectionRun qualityRun,
        ProductionQualityInspectionProjection projection)=>
        Validate(productionReport,qualityRun,projection).Count==0;
}

internal static class ProductionSessionValidationShapeRuntime
{
    public static bool HasValidIdentity(ProductionSessionReport report)=>
        report.SessionId!=Guid.Empty &&
        report.Fingerprint.Length==64 &&
        report.Fingerprint.All(character=>
            Uri.IsHexDigit(character) &&
            char.ToLowerInvariant(character)==character);
}
