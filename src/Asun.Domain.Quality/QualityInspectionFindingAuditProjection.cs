namespace Asun.Domain.Quality;

public sealed record QualityInspectionFindingAuditProjection(
    QualityInspectionFindingAuditIndex Index,
    string ContentFingerprint);
