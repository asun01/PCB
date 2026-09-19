namespace Asun.Platform.SimulationIntegration;

public sealed record ProductionSimulationFrameLink(
    long ProductionSequence,
    string ProductionInputFingerprint,
    long SimulationSequence,
    string SimulationObservationFingerprint);
