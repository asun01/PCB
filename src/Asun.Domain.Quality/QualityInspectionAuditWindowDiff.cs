namespace Asun.Domain.Quality;

public sealed record QualityInspectionAuditWindowDiff(
    IReadOnlyList<Guid> AddedResultIds,
    IReadOnlyList<Guid> RemovedResultIds,
    IReadOnlyList<Guid> ChangedResultIds)
{
    public bool IsEmpty =>
        AddedResultIds.Count == 0 &&
        RemovedResultIds.Count == 0 &&
        ChangedResultIds.Count == 0;
}
