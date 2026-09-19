namespace Asun.Domain.Quality;

public sealed record QualityInspectionFindingAuditRecord(
    QualityFindingId FindingId,
    string RuleCode,
    QualityOutcome Outcome,
    QualitySeverity Severity,
    string Message,
    IReadOnlyList<QualityEvidenceKey> EvidenceKeys);
