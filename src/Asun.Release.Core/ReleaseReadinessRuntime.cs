namespace Asun.Release.Core;

public sealed record ReleaseReadinessReport(
    bool Ready,
    int ArtifactCount,
    string ManifestFingerprint);

public static class ReleaseReadinessRuntime
{
    public static ReleaseReadinessReport Evaluate(
        ReleaseManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(manifest);

        var valid=ReleaseManifestValidationRuntime.IsValid(manifest);

        return new ReleaseReadinessReport(
            valid && manifest.Artifacts.Count>0,
            manifest.Artifacts.Count,
            manifest.Fingerprint);
    }
}
