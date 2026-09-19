namespace Asun.Domain.Quality;

public sealed record QualityInspectionRunSummary(
    Guid RunId,
    int ResultCount,
    int FindingCount,
    int EvidenceLinkCount,
    int FailCount,
    int ReviewCount,
    int CriticalCount,
    string ContentFingerprint);
