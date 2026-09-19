namespace Asun.Domain.Quality;

public sealed record QualityInspectionFindingAuditDiff(
    IReadOnlyList<QualityFindingId> AddedFindingIds,
    IReadOnlyList<QualityFindingId> RemovedFindingIds,
    IReadOnlyList<QualityFindingId> ChangedFindingIds)
{
    public bool IsEmpty =>
        AddedFindingIds.Count == 0 &&
        RemovedFindingIds.Count == 0 &&
        ChangedFindingIds.Count == 0;
}
