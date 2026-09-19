namespace Asun.Platform.Evidence;

public sealed record EvidenceCatalogSnapshotWindowIntegritySummary(
    int EntryCount,
    long? FirstSequence,
    long? LastSequence,
    string WindowFingerprint);
