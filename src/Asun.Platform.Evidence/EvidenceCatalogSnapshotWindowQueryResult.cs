namespace Asun.Platform.Evidence;

public sealed record EvidenceCatalogSnapshotWindowQueryResult(
    long Sequence,
    EvidenceCatalogQueryResult Result);
