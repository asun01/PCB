namespace Asun.Domain.Quality;

public sealed record QualityInspectionEvidenceDiff(
    IReadOnlyList<QualityFindingEvidenceLink> AddedLinks,
    IReadOnlyList<QualityFindingEvidenceLink> RemovedLinks)
{
    public bool IsEmpty =>
        AddedLinks.Count == 0 &&
        RemovedLinks.Count == 0;
}
