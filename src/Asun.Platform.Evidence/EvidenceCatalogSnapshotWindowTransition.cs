namespace Asun.Platform.Evidence;

public sealed record EvidenceCatalogSnapshotWindowTransition(
    string PreviousWindowFingerprint,
    string CurrentWindowFingerprint,
    EvidenceCatalogSnapshotWindowDiff Diff);
