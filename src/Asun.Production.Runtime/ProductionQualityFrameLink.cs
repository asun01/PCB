namespace Asun.Production.Runtime;

public sealed record ProductionQualityFrameLink(
    long Sequence,
    string ProductionInputFingerprint,
    Guid QualityResultId,
    Guid QualitySnapshotId,
    int FindingCount,
    int EvidenceLinkCount);
