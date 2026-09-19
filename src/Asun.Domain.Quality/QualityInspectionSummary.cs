namespace Asun.Domain.Quality;

public sealed record QualityInspectionSummary(
    Guid ResultId,
    Guid SnapshotId,
    long Sequence,
    QualityInspectionOutcomeSummary Outcomes,
    QualityInspectionSeveritySummary Severities,
    QualityInspectionEvidenceSummary Evidence,
    string ContentFingerprint);
