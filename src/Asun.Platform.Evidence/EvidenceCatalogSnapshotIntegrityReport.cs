namespace Asun.Platform.Evidence;

public sealed record EvidenceCatalogSnapshotIntegrityReport(
    int DescriptorCount,
    string SnapshotFingerprint);
