namespace Asun.Platform.Evidence;

public sealed record EvidenceCatalogQueryBatch(
    string SnapshotFingerprint,
    IReadOnlyList<EvidenceCatalogQueryResult> Results,
    int QueryCount);
