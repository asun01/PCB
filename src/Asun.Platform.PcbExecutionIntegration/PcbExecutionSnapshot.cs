namespace Asun.Platform.PcbExecutionIntegration;

public sealed record PcbExecutionSnapshot(
    string AssemblyFingerprint,
    Guid ProductionSessionId,
    string ProductionFingerprint,
    string PipelineAuditFingerprint,
    int FrameCount,
    int MeasurementFactCount,
    Guid QualityRunId,
    string EvidenceProjectionFingerprint,
    string Fingerprint);
