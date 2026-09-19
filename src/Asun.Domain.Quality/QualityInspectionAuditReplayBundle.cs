namespace Asun.Domain.Quality;

public sealed record QualityInspectionAuditReplayBundle(
    QualityInspectionAuditProjection? Previous,
    QualityInspectionAuditProjection Current,
    QualityInspectionAuditWindowDiff Diff);
