namespace Asun.Domain.Quality;

public sealed record QualityInspectionFindingAuditEnvelope(
    Guid ResultId,
    Guid SnapshotId,
    long Sequence,
    QualityInspectionFindingAuditProjection Projection,
    string ProjectionFingerprint);
