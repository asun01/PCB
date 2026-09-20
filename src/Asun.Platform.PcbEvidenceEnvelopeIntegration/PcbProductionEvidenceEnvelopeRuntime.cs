using System.Security.Cryptography;
using System.Text;
using Asun.Platform.PcbExecutionIntegration;
using Asun.Platform.PcbEvidenceResolutionIntegration;
using Asun.Platform.PcbReleaseIntegration;
using Asun.Platform.PcbReplayIntegration;

namespace Asun.Platform.PcbEvidenceEnvelopeIntegration;

public static class PcbProductionEvidenceEnvelopeRuntime
{
    public static PcbProductionEvidenceEnvelope Create(
        PcbExecutionSnapshot executionSnapshot,
        PcbEndToEndReplaySnapshot replaySnapshot,
        PcbExecutionEvidenceResolution evidenceResolution,
        PcbExecutionReleaseProjection releaseProjection)
    {
        ArgumentNullException.ThrowIfNull(executionSnapshot);
        ArgumentNullException.ThrowIfNull(replaySnapshot);
        ArgumentNullException.ThrowIfNull(evidenceResolution);
        ArgumentNullException.ThrowIfNull(releaseProjection);

        if(replaySnapshot.ExecutionSnapshotFingerprint!=executionSnapshot.Fingerprint)
            throw new ArgumentException("Replay snapshot must belong to execution snapshot.",nameof(replaySnapshot));
        if(evidenceResolution.ExecutionSnapshotFingerprint!=executionSnapshot.Fingerprint)
            throw new ArgumentException("Evidence resolution must belong to execution snapshot.",nameof(evidenceResolution));
        if(releaseProjection.ExecutionSnapshotFingerprint!=executionSnapshot.Fingerprint)
            throw new ArgumentException("Release projection must belong to execution snapshot.",nameof(releaseProjection));

        var fingerprint=CreateFingerprint(
            executionSnapshot,
            replaySnapshot,
            evidenceResolution,
            releaseProjection);

        return new PcbProductionEvidenceEnvelope(
            executionSnapshot.AssemblyFingerprint,
            executionSnapshot.ProductionSessionId,
            executionSnapshot.Fingerprint,
            replaySnapshot.Fingerprint,
            evidenceResolution.Fingerprint,
            releaseProjection.Fingerprint,
            fingerprint);
    }

    internal static string CreateFingerprint(
        PcbExecutionSnapshot executionSnapshot,
        PcbEndToEndReplaySnapshot replaySnapshot,
        PcbExecutionEvidenceResolution evidenceResolution,
        PcbExecutionReleaseProjection releaseProjection)
    {
        var canonical=string.Join(
            "|",
            executionSnapshot.AssemblyFingerprint,
            executionSnapshot.ProductionSessionId,
            executionSnapshot.Fingerprint,
            replaySnapshot.Fingerprint,
            evidenceResolution.Fingerprint,
            releaseProjection.Fingerprint);

        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical)))
            .ToLowerInvariant();
    }
}
