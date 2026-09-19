namespace Asun.Domain.Quality;

/// <summary>
/// Immutable collection boundary for quality findings.
/// </summary>
public sealed class QualityFindingSet
{
    private readonly QualityFinding[] _findings;

    public QualityFindingSet(IEnumerable<QualityFinding> findings)
    {
        ArgumentNullException.ThrowIfNull(findings);

        _findings = findings
            .Select(finding => finding ??
                throw new ArgumentException(
                    "Finding collection cannot contain null entries.",
                    nameof(findings)))
            .ToArray();
    }

    public int Count => _findings.Length;

    public IReadOnlyList<QualityFinding> Findings =>
        Array.AsReadOnly(_findings);

    public QualityFinding? Find(QualityFindingId id) =>
        _findings.FirstOrDefault(finding => finding.Id == id);
}
