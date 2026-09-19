namespace Asun.Platform.QualityReleaseIntegration;

public sealed record QualityReleaseFactProjection(
    Guid QualityRunId,
    int ResultCount,
    int FindingCount,
    int EvidenceLinkCount,
    int FailCount,
    int ReviewCount,
    int CriticalCount,
    string QualitySummaryFingerprint,
    string ReleaseManifestFingerprint,
    bool ReleaseReady,
    string Fingerprint);
