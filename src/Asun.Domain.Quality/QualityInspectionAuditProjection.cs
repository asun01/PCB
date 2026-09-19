namespace Asun.Domain.Quality;

public sealed record QualityInspectionAuditProjection(
    QualityInspectionAuditRecord Record,
    QualityInspectionSummary Summary,
    QualityInspectionRuleAuditProjection RuleAudit,
    QualityInspectionFindingAuditProjection FindingAudit);
