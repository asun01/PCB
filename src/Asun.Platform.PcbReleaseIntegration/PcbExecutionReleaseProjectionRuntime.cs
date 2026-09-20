using System.Security.Cryptography;
using System.Text;
using Asun.Platform.PcbExecutionIntegration;
using Asun.Release.Core;

namespace Asun.Platform.PcbReleaseIntegration;

public static class PcbExecutionReleaseProjectionRuntime
{
    public static PcbExecutionReleaseProjection Create(
        PcbExecutionSnapshot executionSnapshot,
        ReleaseManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(executionSnapshot);
        ArgumentNullException.ThrowIfNull(manifest);

        if(!ReleaseManifestValidationRuntime.IsValid(manifest))
            throw new ArgumentException("Release manifest is invalid.",nameof(manifest));

        var readiness=ReleaseReadinessRuntime.Evaluate(manifest);
        var fingerprint=CreateFingerprint(executionSnapshot,manifest,readiness.Ready);

        return new PcbExecutionReleaseProjection(
            executionSnapshot.AssemblyFingerprint,
            executionSnapshot.ProductionSessionId,
            executionSnapshot.Fingerprint,
            manifest.Fingerprint,
            manifest.Artifacts.Count,
            readiness.Ready,
            fingerprint);
    }

    internal static string CreateFingerprint(
        PcbExecutionSnapshot executionSnapshot,
        ReleaseManifest manifest,
        bool releaseReady)
    {
        var canonical=string.Join(
            "|",
            executionSnapshot.AssemblyFingerprint,
            executionSnapshot.ProductionSessionId,
            executionSnapshot.Fingerprint,
            manifest.Fingerprint,
            manifest.Artifacts.Count,
            releaseReady);

        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical)))
            .ToLowerInvariant();
    }
}
