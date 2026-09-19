namespace Asun.Platform.Evidence;

public sealed record EvidenceCatalogSnapshotWindowReferenceResolution(
    string WindowFingerprint,
    EvidenceReferenceSet ReferenceSet,
    IReadOnlyList<EvidenceCatalogSnapshotWindowReferenceResolutionEntry> Entries);
