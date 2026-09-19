namespace Asun.Domain.Quality;

public sealed record QualityInspectionRuleAuditProjection(
    QualityInspectionRuleSummary Summary,
    QualityInspectionRuleFindingIndex FindingIndex,
    QualityInspectionRuleEvidenceIndex EvidenceIndex);
