namespace Asun.Domain.Quality;

public sealed record QualityInspectionReplayProjectionDiff(
    IReadOnlyList<QualityFindingId> AddedFindingIds,
    IReadOnlyList<QualityFindingId> RemovedFindingIds,
    IReadOnlyList<QualityFindingEvidenceLink> AddedEvidenceLinks,
    IReadOnlyList<QualityFindingEvidenceLink> RemovedEvidenceLinks,
    bool ContentFingerprintChanged)
{
    public bool IsEmpty =>
        AddedFindingIds.Count == 0 &&
        RemovedFindingIds.Count == 0 &&
        AddedEvidenceLinks.Count == 0 &&
        RemovedEvidenceLinks.Count == 0 &&
        !ContentFingerprintChanged;
}
