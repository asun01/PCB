namespace Asun.Platform.PcbAuditReleaseIntegration;

public sealed record PcbAuditReleaseTransitionProjection(
    string EnvelopeFingerprint,
    Guid QualityRunId,
    string AuditWindowFingerprint,
    string ReleaseManifestFingerprint,
    int AuditCount,
    bool ReleaseReady,
    string Fingerprint);
