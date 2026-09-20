using Asun.Platform.PcbExecutionIntegration;
using Asun.Platform.PcbEvidenceResolutionIntegration;
using Asun.Release.Core;

namespace Asun.Platform.PcbEvidenceReleaseIntegration;

public static class PcbEvidenceReleaseFactProjectionValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        PcbExecutionSnapshot executionSnapshot,
        PcbExecutionEvidenceResolution evidenceResolution,
        ReleaseManifest manifest,
        PcbEvidenceReleaseFactProjection projection)
    {
        ArgumentNullException.ThrowIfNull(executionSnapshot);
        ArgumentNullException.ThrowIfNull(evidenceResolution);
        ArgumentNullException.ThrowIfNull(manifest);
        ArgumentNullException.ThrowIfNull(projection);

        var errors=new List<string>();
        if(!ReleaseManifestValidationRuntime.IsValid(manifest))
            errors.Add("Release manifest is invalid.");
        if(evidenceResolution.ExecutionSnapshotFingerprint!=executionSnapshot.Fingerprint)
            errors.Add("Evidence resolution execution fingerprint must match.");
        if(projection.ExecutionSnapshotFingerprint!=executionSnapshot.Fingerprint)
            errors.Add("Projection execution snapshot fingerprint must match.");
        if(projection.EvidenceProjectionFingerprint!=evidenceResolution.EvidenceProjectionFingerprint)
            errors.Add("Projection evidence fingerprint must match.");
        if(projection.CatalogSnapshotFingerprint!=evidenceResolution.CatalogSnapshotFingerprint)
            errors.Add("Projection catalog fingerprint must match.");
        if(projection.ReleaseManifestFingerprint!=manifest.Fingerprint)
            errors.Add("Projection manifest fingerprint must match.");

        var requested=evidenceResolution.RequestedHandles.Count;
        var found=evidenceResolution.FoundHandles.Count;
        var missing=evidenceResolution.MissingHandles.Count;
        if(projection.RequestedCount!=requested)
            errors.Add("Projection requested count must match.");
        if(projection.FoundCount!=found)
            errors.Add("Projection found count must match.");
        if(projection.MissingCount!=missing)
            errors.Add("Projection missing count must match.");
        if(projection.AllRequestedResolved!=(missing==0))
            errors.Add("Projection all-resolved fact must match the missing-handle count.");

        if(projection.Fingerprint.Length!=64 ||
           !projection.Fingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Projection fingerprint must be 64 lowercase hexadecimal characters.");
        }

        if(errors.Count>0)
            return errors;

        var expected=PcbEvidenceReleaseFactProjectionRuntime.CreateFingerprint(
            executionSnapshot,
            evidenceResolution,
            manifest);
        if(expected!=projection.Fingerprint)
            errors.Add("Projection fingerprint does not match canonical content.");

        return errors;
    }

    public static bool IsValid(
        PcbExecutionSnapshot executionSnapshot,
        PcbExecutionEvidenceResolution evidenceResolution,
        ReleaseManifest manifest,
        PcbEvidenceReleaseFactProjection projection)=>
        Validate(executionSnapshot,evidenceResolution,manifest,projection).Count==0;
}
