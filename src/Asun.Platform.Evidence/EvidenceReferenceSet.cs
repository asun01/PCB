namespace Asun.Platform.Evidence;

public sealed record EvidenceReferenceSet(
    IReadOnlyList<EvidenceHandle> Handles);
