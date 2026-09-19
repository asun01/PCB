namespace Asun.Domain.Quality;

public sealed record QualityInspectionRuleSummaryEntry(
    string RuleCode,
    int FindingCount,
    int DistinctEvidenceLinkCount);
