using Asun.Platform.PcbExecutionIntegration;
using Asun.Release.Core;

namespace Asun.Platform.PcbReleaseIntegration;

public static class PcbExecutionReleaseProjectionValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        PcbExecutionSnapshot executionSnapshot,
        ReleaseManifest manifest,
        PcbExecutionReleaseProjection projection)
    {
        ArgumentNullException.ThrowIfNull(executionSnapshot);
        ArgumentNullException.ThrowIfNull(manifest);
        ArgumentNullException.ThrowIfNull(projection);

        var errors=new List<string>();
        if(!ReleaseManifestValidationRuntime.IsValid(manifest))
            errors.Add("Release manifest is invalid.");

        if(projection.AssemblyFingerprint!=executionSnapshot.AssemblyFingerprint)
            errors.Add("Release projection assembly fingerprint must match.");
        if(projection.ProductionSessionId!=executionSnapshot.ProductionSessionId)
            errors.Add("Release projection production session id must match.");
        if(projection.ExecutionSnapshotFingerprint!=executionSnapshot.Fingerprint)
            errors.Add("Release projection execution snapshot fingerprint must match.");
        if(projection.ReleaseManifestFingerprint!=manifest.Fingerprint)
            errors.Add("Release projection manifest fingerprint must match.");
        if(projection.ArtifactCount!=manifest.Artifacts.Count)
            errors.Add("Release projection artifact count must match.");

        var readiness=ReleaseReadinessRuntime.Evaluate(manifest);
        if(projection.ReleaseReady!=readiness.Ready)
            errors.Add("Release projection readiness fact must match.");

        if(projection.Fingerprint.Length!=64 ||
           !projection.Fingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Release projection fingerprint must be 64 lowercase hexadecimal characters.");
        }

        if(errors.Count>0)
            return errors;

        var expected=PcbExecutionReleaseProjectionRuntime.CreateFingerprint(
            executionSnapshot,
            manifest,
            readiness.Ready);
        if(expected!=projection.Fingerprint)
            errors.Add("Release projection fingerprint does not match canonical content.");

        return errors;
    }

    public static bool IsValid(
        PcbExecutionSnapshot executionSnapshot,
        ReleaseManifest manifest,
        PcbExecutionReleaseProjection projection)=>
        Validate(executionSnapshot,manifest,projection).Count==0;
}
