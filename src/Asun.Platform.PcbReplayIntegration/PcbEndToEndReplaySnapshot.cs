namespace Asun.Platform.PcbReplayIntegration;

public sealed record PcbEndToEndReplaySnapshot(
    string ExecutionSnapshotFingerprint,
    string RenderReplayFingerprint,
    string SimulationReplayFingerprint,
    string EvidenceResolutionFingerprint,
    string ReleaseProjectionFingerprint,
    string Fingerprint);
