namespace Asun.Platform.PcbAuditIntegration;

public sealed record PcbExecutionAuditWindowProjection(
    string EvidenceEnvelopeFingerprint,
    Guid QualityRunId,
    int AuditEnvelopeCount,
    string AuditWindowFingerprint,
    string Fingerprint);
