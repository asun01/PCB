namespace Asun.Platform.Evidence;

public sealed record EvidenceCatalogQueryResult(
    EvidenceDescriptorQuery Query,
    string SnapshotFingerprint,
    IReadOnlyList<EvidenceHandle> Handles,
    int MatchCount);
