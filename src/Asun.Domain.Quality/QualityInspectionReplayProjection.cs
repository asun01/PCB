namespace Asun.Domain.Quality;

public sealed record QualityInspectionReplayProjection(
    Guid ResultId,
    Guid SnapshotId,
    long Sequence,
    IReadOnlyList<QualityFindingId> FindingIds,
    QualityInspectionEvidenceManifest EvidenceManifest,
    string ContentFingerprint);
