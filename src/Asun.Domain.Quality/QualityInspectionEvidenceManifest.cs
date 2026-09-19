namespace Asun.Domain.Quality;

public sealed class QualityInspectionEvidenceManifest
{
    private readonly QualityFindingEvidenceLink[] _links;

    public QualityInspectionEvidenceManifest(
        IEnumerable<QualityFindingEvidenceLink> links)
    {
        ArgumentNullException.ThrowIfNull(links);

        _links = links
            .Select(link => link ??
                throw new ArgumentException(
                    "Evidence manifest cannot contain null links.",
                    nameof(links)))
            .ToArray();
    }

    public int Count => _links.Length;

    public IReadOnlyList<QualityFindingEvidenceLink> Links =>
        Array.AsReadOnly(_links);
}
