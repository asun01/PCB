using Asun.Domain.Quality;
using Asun.Release.Core;

namespace Asun.Platform.QualityReleaseIntegration;

public static class QualityReleaseFactProjectionValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        QualityInspectionRun run,
        ReleaseManifest manifest,
        QualityReleaseFactProjection projection)
    {
        ArgumentNullException.ThrowIfNull(run);
        ArgumentNullException.ThrowIfNull(manifest);
        ArgumentNullException.ThrowIfNull(projection);

        var errors=new List<string>();

        if(!QualityInspectionRunValidationRuntime.IsValid(run))
            errors.Add("Quality inspection run is invalid.");

        if(!ReleaseManifestValidationRuntime.IsValid(manifest))
            errors.Add("Release manifest is invalid.");

        if(errors.Count>0)
            return errors;

        var summary=QualityInspectionRunSummaryRuntime.Create(run);
        var readiness=ReleaseReadinessRuntime.Evaluate(manifest);

        if(projection.QualityRunId!=summary.RunId)
            errors.Add("Quality release projection run id must match.");

        if(projection.ResultCount!=summary.ResultCount)
            errors.Add("Quality release projection result count must match.");

        if(projection.FindingCount!=summary.FindingCount)
            errors.Add("Quality release projection finding count must match.");

        if(projection.EvidenceLinkCount!=summary.EvidenceLinkCount)
            errors.Add("Quality release projection evidence-link count must match.");

        if(projection.FailCount!=summary.FailCount)
            errors.Add("Quality release projection fail count must match.");

        if(projection.ReviewCount!=summary.ReviewCount)
            errors.Add("Quality release projection review count must match.");

        if(projection.CriticalCount!=summary.CriticalCount)
            errors.Add("Quality release projection critical count must match.");

        if(projection.QualitySummaryFingerprint!=summary.ContentFingerprint)
            errors.Add("Quality release projection summary fingerprint must match.");

        if(projection.ReleaseManifestFingerprint!=manifest.Fingerprint)
            errors.Add("Quality release projection manifest fingerprint must match.");

        if(projection.ReleaseReady!=readiness.Ready)
            errors.Add("Quality release projection readiness fact must match.");

        if(projection.Fingerprint.Length!=64 ||
           !projection.Fingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Quality release projection fingerprint must be 64 lowercase hexadecimal characters.");
        }

        if(errors.Count==0)
        {
            var expected=QualityReleaseFactProjectionRuntime.CreateFingerprint(
                summary,
                manifest,
                readiness.Ready);

            if(expected!=projection.Fingerprint)
                errors.Add("Quality release projection fingerprint does not match canonical content.");
        }

        return errors;
    }

    public static bool IsValid(
        QualityInspectionRun run,
        ReleaseManifest manifest,
        QualityReleaseFactProjection projection)=>
        Validate(run,manifest,projection).Count==0;
}
