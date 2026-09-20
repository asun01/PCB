using System.Security.Cryptography;
using System.Text;
using Asun.Platform.PcbExecutionIntegration;
using Asun.Platform.PcbReleaseIntegration;
using Asun.Platform.PcbSimulationIntegration;
using Asun.Platform.PcbEvidenceResolutionIntegration;
using Asun.Platform.RenderIntegration;

namespace Asun.Platform.PcbReplayIntegration;

public static class PcbEndToEndReplaySnapshotRuntime
{
    public static PcbEndToEndReplaySnapshot Create(
        PcbExecutionSnapshot executionSnapshot,
        IReadOnlyList<ProductionRenderReplayFrameIntegrity> renderReplayFrames,
        PcbExecutionSimulationReplayProjection simulationReplay,
        PcbExecutionEvidenceResolution evidenceResolution,
        PcbExecutionReleaseProjection releaseProjection)
    {
        ArgumentNullException.ThrowIfNull(executionSnapshot);
        ArgumentNullException.ThrowIfNull(renderReplayFrames);
        ArgumentNullException.ThrowIfNull(simulationReplay);
        ArgumentNullException.ThrowIfNull(evidenceResolution);
        ArgumentNullException.ThrowIfNull(releaseProjection);

        if(simulationReplay.ExecutionSnapshotFingerprint!=executionSnapshot.Fingerprint)
            throw new ArgumentException("Simulation replay must belong to the execution snapshot.",nameof(simulationReplay));
        if(evidenceResolution.ExecutionSnapshotFingerprint!=executionSnapshot.Fingerprint)
            throw new ArgumentException("Evidence resolution must belong to the execution snapshot.",nameof(evidenceResolution));
        if(releaseProjection.ExecutionSnapshotFingerprint!=executionSnapshot.Fingerprint)
            throw new ArgumentException("Release projection must belong to the execution snapshot.",nameof(releaseProjection));
        if(renderReplayFrames.Count!=executionSnapshot.FrameCount)
            throw new ArgumentException("Render replay frame count must match the execution snapshot.",nameof(renderReplayFrames));

        var renderFingerprint=CreateRenderFingerprint(renderReplayFrames);
        var fingerprint=CreateFingerprint(
            executionSnapshot,
            renderFingerprint,
            simulationReplay,
            evidenceResolution,
            releaseProjection);

        return new PcbEndToEndReplaySnapshot(
            executionSnapshot.Fingerprint,
            renderFingerprint,
            simulationReplay.Fingerprint,
            evidenceResolution.Fingerprint,
            releaseProjection.Fingerprint,
            fingerprint);
    }

    internal static string CreateRenderFingerprint(
        IReadOnlyList<ProductionRenderReplayFrameIntegrity> frames)
    {
        var canonical=new StringBuilder();
        foreach(var frame in frames.OrderBy(frame=>frame.Sequence))
            canonical.Append(frame.Sequence).Append('|')
                .Append(frame.RenderFingerprint).Append('|');
        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical.ToString())))
            .ToLowerInvariant();
    }

    internal static string CreateFingerprint(
        PcbExecutionSnapshot executionSnapshot,
        string renderFingerprint,
        PcbExecutionSimulationReplayProjection simulationReplay,
        PcbExecutionEvidenceResolution evidenceResolution,
        PcbExecutionReleaseProjection releaseProjection)
    {
        var canonical=string.Join(
            "|",
            executionSnapshot.Fingerprint,
            renderFingerprint,
            simulationReplay.Fingerprint,
            evidenceResolution.Fingerprint,
            releaseProjection.Fingerprint);
        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical)))
            .ToLowerInvariant();
    }
}
