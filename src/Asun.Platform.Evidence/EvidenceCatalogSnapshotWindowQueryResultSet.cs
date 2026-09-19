namespace Asun.Platform.Evidence;

public sealed record EvidenceCatalogSnapshotWindowQueryResultSet(
    string WindowFingerprint,
    EvidenceDescriptorQuery Query,
    IReadOnlyList<EvidenceCatalogSnapshotWindowQueryResult> Results,
    int EntryCount);
