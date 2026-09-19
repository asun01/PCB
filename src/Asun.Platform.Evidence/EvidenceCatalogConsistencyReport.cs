namespace Asun.Platform.Evidence;

public sealed record EvidenceCatalogConsistencyReport(
    string SnapshotFingerprint,
    string StatisticsFingerprint,
    string? ReferenceResolutionFingerprint,
    int DescriptorCount,
    int ReferenceCount,
    int MissingReferenceCount);
