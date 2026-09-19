namespace Asun.Domain.Quality;

public sealed record QualityInspectionAuditRecord(
    Guid ResultId,
    Guid SnapshotId,
    long Sequence,
    int FindingCount,
    int EvidenceLinkCount,
    string ContentFingerprint);
