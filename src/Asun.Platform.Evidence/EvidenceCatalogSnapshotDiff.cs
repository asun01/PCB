namespace Asun.Platform.Evidence;

public sealed record EvidenceCatalogSnapshotDiff(
    IReadOnlyList<EvidenceHandle> AddedHandles,
    IReadOnlyList<EvidenceHandle> RemovedHandles,
    IReadOnlyList<EvidenceHandle> ChangedHandles)
{
    public bool IsEmpty =>
        AddedHandles.Count == 0 &&
        RemovedHandles.Count == 0 &&
        ChangedHandles.Count == 0;
}
