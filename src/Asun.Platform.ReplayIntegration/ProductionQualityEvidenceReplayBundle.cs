namespace Asun.Platform.ReplayIntegration;

public sealed record ProductionQualityEvidenceReplayBundle(
    Guid ProductionSessionId,
    string ProductionFingerprint,
    Guid QualityRunId,
    string EvidenceProjectionFingerprint,
    int QualityResultCount,
    int EvidenceFrameCount,
    string Fingerprint);
