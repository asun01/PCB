namespace Asun.Domain.Quality;

public sealed record QualityInspectionDiff(
    IReadOnlyList<QualityFindingId> AddedFindingIds,
    IReadOnlyList<QualityFindingId> RemovedFindingIds,
    IReadOnlyList<QualityFindingId> ChangedFindingIds,
    IReadOnlyList<QualityEvidenceKey> AddedEvidenceKeys,
    IReadOnlyList<QualityEvidenceKey> RemovedEvidenceKeys)
{
    public IReadOnlyList<QualityEvidenceKey> RelinkedEvidenceKeys { get; init; } =
        Array.Empty<QualityEvidenceKey>();

    public bool IsEmpty =>
        AddedFindingIds.Count == 0 &&
        RemovedFindingIds.Count == 0 &&
        ChangedFindingIds.Count == 0 &&
        AddedEvidenceKeys.Count == 0 &&
        RemovedEvidenceKeys.Count == 0 &&
        RelinkedEvidenceKeys.Count == 0;
}
