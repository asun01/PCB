namespace Asun.Platform.Evidence;

public sealed record EvidenceCatalogSnapshotWindowDiff(
    IReadOnlyList<long> AddedSequences,
    IReadOnlyList<long> RemovedSequences,
    IReadOnlyList<long> ChangedSequences)
{
    public bool IsEmpty=>
        AddedSequences.Count==0 &&
        RemovedSequences.Count==0 &&
        ChangedSequences.Count==0;
}
