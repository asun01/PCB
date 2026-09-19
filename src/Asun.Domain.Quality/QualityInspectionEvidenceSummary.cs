namespace Asun.Domain.Quality;

public sealed record QualityInspectionEvidenceSummary(
    int LinkCount,
    int DistinctEvidenceKeyCount,
    int LinkedFindingCount);
