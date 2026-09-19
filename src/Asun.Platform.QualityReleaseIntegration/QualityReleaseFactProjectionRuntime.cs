using System.Security.Cryptography;
using System.Text;
using Asun.Domain.Quality;
using Asun.Release.Core;

namespace Asun.Platform.QualityReleaseIntegration;

public static class QualityReleaseFactProjectionRuntime
{
    public static QualityReleaseFactProjection Create(
        QualityInspectionRun run,
        ReleaseManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(run);
        ArgumentNullException.ThrowIfNull(manifest);

        if(!QualityInspectionRunValidationRuntime.IsValid(run))
            throw new ArgumentException("Quality inspection run is invalid.",nameof(run));

        if(!ReleaseManifestValidationRuntime.IsValid(manifest))
            throw new ArgumentException("Release manifest is invalid.",nameof(manifest));

        var summary=QualityInspectionRunSummaryRuntime.Create(run);
        var readiness=ReleaseReadinessRuntime.Evaluate(manifest);
        var fingerprint=CreateFingerprint(summary,manifest,readiness.Ready);

        return new QualityReleaseFactProjection(
            summary.RunId,
            summary.ResultCount,
            summary.FindingCount,
            summary.EvidenceLinkCount,
            summary.FailCount,
            summary.ReviewCount,
            summary.CriticalCount,
            summary.ContentFingerprint,
            manifest.Fingerprint,
            readiness.Ready,
            fingerprint);
    }

    internal static string CreateFingerprint(
        QualityInspectionRunSummary summary,
        ReleaseManifest manifest,
        bool releaseReady)
    {
        var canonical=string.Join(
            "|",
            summary.RunId,
            summary.ResultCount,
            summary.FindingCount,
            summary.EvidenceLinkCount,
            summary.FailCount,
            summary.ReviewCount,
            summary.CriticalCount,
            summary.ContentFingerprint,
            manifest.Fingerprint,
            releaseReady);

        return Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(canonical)))
            .ToLowerInvariant();
    }
}
