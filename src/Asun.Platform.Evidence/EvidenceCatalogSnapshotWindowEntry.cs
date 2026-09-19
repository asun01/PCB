namespace Asun.Platform.Evidence;

public sealed record EvidenceCatalogSnapshotWindowEntry(
    long Sequence,
    EvidenceCatalogSnapshotEnvelope Envelope);
