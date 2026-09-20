using System.Security.Cryptography;
using System.Text;
using Asun.Platform.PcbExecutionIntegration;
using Asun.Platform.PcbEvidenceResolutionIntegration;
using Asun.Release.Core;

namespace Asun.Platform.PcbEvidenceReleaseIntegration;

public static class PcbEvidenceReleaseFactProjectionRuntime
{
    public static PcbEvidenceReleaseFactProjection Create(
        PcbExecutionSnapshot executionSnapshot,
        PcbExecutionEvidenceResolution evidenceResolution,
        ReleaseManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(executionSnapshot);
        ArgumentNullException.ThrowIfNull(evidenceResolution);
        ArgumentNullException.ThrowIfNull(manifest);

        if(!ReleaseManifestValidationRuntime.IsValid(manifest))
            throw new ArgumentException("Release manifest is invalid.",nameof(manifest));
        if(evidenceResolution.ExecutionSnapshotFingerprint!=executionSnapshot.Fingerprint)
            throw new ArgumentException("Evidence resolution must belong to the execution snapshot.",nameof(evidenceResolution));

        var fingerprint=CreateFingerprint(
            executionSnapshot,
            evidenceResolution,
            manifest);
        var requested=evidenceResolution.RequestedHandles.Count;
        var found=evidenceResolution.FoundHandles.Count;
        var missing=evidenceResolution.MissingHandles.Count;

        return new PcbEvidenceReleaseFactProjection(
            executionSnapshot.Fingerprint,
            evidenceResolution.EvidenceProjectionFingerprint,
            evidenceResolution.CatalogSnapshotFingerprint,
            manifest.Fingerprint,
            requested,
            found,
            missing,
            missing==0,
            fingerprint);
    }

    internal static string CreateFingerprint(
        PcbExecutionSnapshot executionSnapshot,
        PcbExecutionEvidenceResolution evidenceResolution,
        ReleaseManifest manifest)
    {
        var canonical=string.Join(
            "|",
            executionSnapshot.Fingerprint,
            evidenceResolution.EvidenceProjectionFingerprint,
            evidenceResolution.CatalogSnapshotFingerprint,
            manifest.Fingerprint,
            evidenceResolution.RequestedHandles.Count,
            evidenceResolution.FoundHandles.Count,
            evidenceResolution.MissingHandles.Count,
            evidenceResolution.MissingHandles.Count==0);

        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical)))
            .ToLowerInvariant();
    }
}
