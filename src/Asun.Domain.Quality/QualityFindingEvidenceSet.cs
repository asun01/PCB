namespace Asun.Domain.Quality;

public sealed class QualityFindingEvidenceSet
{
    private readonly QualityFindingEvidenceLink[] _links;

    public QualityFindingEvidenceSet(
        IEnumerable<QualityFindingEvidenceLink> links)
    {
        ArgumentNullException.ThrowIfNull(links);

        _links = links
            .Select(link => link ??
                throw new ArgumentException(
                    "Evidence-link collection cannot contain null entries.",
                    nameof(links)))
            .ToArray();
    }

    public int Count => _links.Length;

    public IReadOnlyList<QualityFindingEvidenceLink> Links =>
        Array.AsReadOnly(_links);

    public IReadOnlyList<QualityEvidenceKey> ForFinding(
        QualityFindingId findingId) =>
        _links
            .Where(link => link.FindingId == findingId)
            .Select(link => link.EvidenceKey)
            .Distinct()
            .ToArray();
}
