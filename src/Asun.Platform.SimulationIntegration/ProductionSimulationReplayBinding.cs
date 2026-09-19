namespace Asun.Platform.SimulationIntegration;

public sealed record ProductionSimulationReplayBinding(
    Guid ProductionSessionId,
    string ProductionFingerprint,
    IReadOnlyList<ProductionSimulationFrameLink> Frames,
    string Fingerprint);
