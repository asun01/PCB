namespace Asun.Platform.PcbSimulationIntegration;

public sealed record PcbExecutionSimulationReplayProjection(
    string ExecutionSnapshotFingerprint,
    string ProductionSessionId,
    string SimulationBindingFingerprint,
    int FrameCount,
    int ObservationCount,
    string Fingerprint);
