namespace Asun.Platform.PcbProductionIntegration;

public sealed record PcbProductionQualityProvenanceBundle(
    string AssemblyFingerprint,
    Guid ProductionSessionId,
    string ProductionFingerprint,
    Guid QualityRunId,
    int FrameCount,
    int ComponentCount,
    string Fingerprint);
