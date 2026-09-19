namespace Asun.Platform.Evidence;

public sealed record EvidenceReferenceResolution(
    string SnapshotFingerprint,
    IReadOnlyList<EvidenceHandle> FoundHandles,
    IReadOnlyList<EvidenceHandle> MissingHandles);
