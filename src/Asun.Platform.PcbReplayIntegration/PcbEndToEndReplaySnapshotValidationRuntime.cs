using Asun.Platform.PcbExecutionIntegration;
using Asun.Platform.PcbReleaseIntegration;
using Asun.Platform.PcbSimulationIntegration;
using Asun.Platform.PcbEvidenceResolutionIntegration;
using Asun.Platform.RenderIntegration;

namespace Asun.Platform.PcbReplayIntegration;

public static class PcbEndToEndReplaySnapshotValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        PcbExecutionSnapshot executionSnapshot,
        IReadOnlyList<ProductionRenderReplayFrameIntegrity> renderReplayFrames,
        PcbExecutionSimulationReplayProjection simulationReplay,
        PcbExecutionEvidenceResolution evidenceResolution,
        PcbExecutionReleaseProjection releaseProjection,
        PcbEndToEndReplaySnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(executionSnapshot);
        ArgumentNullException.ThrowIfNull(renderReplayFrames);
        ArgumentNullException.ThrowIfNull(simulationReplay);
        ArgumentNullException.ThrowIfNull(evidenceResolution);
        ArgumentNullException.ThrowIfNull(releaseProjection);
        ArgumentNullException.ThrowIfNull(snapshot);

        var errors=new List<string>();
        if(snapshot.ExecutionSnapshotFingerprint!=executionSnapshot.Fingerprint)
            errors.Add("Replay snapshot execution fingerprint must match.");
        if(simulationReplay.ExecutionSnapshotFingerprint!=executionSnapshot.Fingerprint)
            errors.Add("Simulation replay must belong to execution snapshot.");
        if(evidenceResolution.ExecutionSnapshotFingerprint!=executionSnapshot.Fingerprint)
            errors.Add("Evidence resolution must belong to execution snapshot.");
        if(releaseProjection.ExecutionSnapshotFingerprint!=executionSnapshot.Fingerprint)
            errors.Add("Release projection must belong to execution snapshot.");
        if(renderReplayFrames.Count!=executionSnapshot.FrameCount)
            errors.Add("Render replay frame count must match execution snapshot.");

        if(renderReplayFrames.Select(frame=>frame.Sequence).Distinct().Count()!=renderReplayFrames.Count)
            errors.Add("Render replay sequences must be unique.");
        if(renderReplayFrames.Any(frame=>frame.RenderFingerprint.Length!=64))
            errors.Add("Render replay fingerprints must use 64-character SHA-256 shape.");

        var renderFingerprint=PcbEndToEndReplaySnapshotRuntime.CreateRenderFingerprint(renderReplayFrames);
        if(snapshot.RenderReplayFingerprint!=renderFingerprint)
            errors.Add("Replay snapshot render fingerprint must match.");
        if(snapshot.SimulationReplayFingerprint!=simulationReplay.Fingerprint)
            errors.Add("Replay snapshot simulation fingerprint must match.");
        if(snapshot.EvidenceResolutionFingerprint!=evidenceResolution.Fingerprint)
            errors.Add("Replay snapshot evidence resolution fingerprint must match.");
        if(snapshot.ReleaseProjectionFingerprint!=releaseProjection.Fingerprint)
            errors.Add("Replay snapshot release projection fingerprint must match.");

        if(snapshot.Fingerprint.Length!=64 ||
           !snapshot.Fingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Replay snapshot fingerprint must be 64 lowercase hexadecimal characters.");
        }

        if(errors.Count>0)
            return errors;

        var expected=PcbEndToEndReplaySnapshotRuntime.CreateFingerprint(
            executionSnapshot,
            renderFingerprint,
            simulationReplay,
            evidenceResolution,
            releaseProjection);
        if(expected!=snapshot.Fingerprint)
            errors.Add("Replay snapshot fingerprint does not match canonical content.");

        return errors;
    }

    public static bool IsValid(
        PcbExecutionSnapshot executionSnapshot,
        IReadOnlyList<ProductionRenderReplayFrameIntegrity> renderReplayFrames,
        PcbExecutionSimulationReplayProjection simulationReplay,
        PcbExecutionEvidenceResolution evidenceResolution,
        PcbExecutionReleaseProjection releaseProjection,
        PcbEndToEndReplaySnapshot snapshot)=>
        Validate(executionSnapshot,renderReplayFrames,simulationReplay,evidenceResolution,releaseProjection,snapshot).Count==0;
}
