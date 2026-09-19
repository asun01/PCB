namespace Asun.Platform.Evidence;

public sealed record EvidenceCatalogSnapshotChange(
    EvidenceHandle Handle,
    string PreviousDescriptorFingerprint,
    string CurrentDescriptorFingerprint);
