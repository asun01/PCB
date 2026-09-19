namespace Asun.Domain.Quality;

public sealed record QualityInspectionFindingAuditReplayBundle(
    QualityInspectionFindingAuditProjection? Previous,
    QualityInspectionFindingAuditProjection Current,
    QualityInspectionFindingAuditProjectionDiff Diff);
