namespace Asun.Domain.Quality;

public sealed record QualityInspectionAuditEnvelope(
    Guid ResultId,
    Guid SnapshotId,
    long Sequence,
    QualityInspectionAuditProjection Projection,
    string ProjectionFingerprint);
