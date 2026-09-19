namespace Asun.Domain.Quality;

/// <summary>
/// A backend-neutral inspection finding. Domain-specific defect taxonomy and
/// acceptance thresholds remain outside this primitive until authoritative
/// contracts are available.
/// </summary>
public sealed record InspectionFinding(
    string FindingId,
    string Source,
    double Confidence,
    double X,
    double Y)
{
    public bool HasValidGeometry =>
        double.IsFinite(X) && double.IsFinite(Y);

    public bool HasValidConfidence =>
        double.IsFinite(Confidence) &&
        Confidence >= 0 &&
        Confidence <= 1;
}

public sealed class InspectionFindingSet
{
    private readonly IReadOnlyList<InspectionFinding> _items;

    public InspectionFindingSet(IEnumerable<InspectionFinding> findings)
    {
        ArgumentNullException.ThrowIfNull(findings);

        _items = findings
            .OrderBy(item => item.FindingId, StringComparer.Ordinal)
            .ToArray();

        if (_items.Any(item => string.IsNullOrWhiteSpace(item.FindingId)))
            throw new ArgumentException("Every finding requires an identifier.", nameof(findings));

        if (_items.GroupBy(item => item.FindingId, StringComparer.Ordinal).Any(g => g.Count() > 1))
            throw new ArgumentException("Finding identifiers must be unique.", nameof(findings));
    }

    public int Count => _items.Count;
    public IReadOnlyList<InspectionFinding> Items => _items;
    public int ValidGeometryCount => _items.Count(item => item.HasValidGeometry);
    public int ValidConfidenceCount => _items.Count(item => item.HasValidConfidence);
}
