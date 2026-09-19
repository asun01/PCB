namespace Asun.Platform.Evidence;

public sealed record EvidenceCatalogSnapshotEnvelope(
    EvidenceCatalogSnapshot Snapshot,
    string Fingerprint);
