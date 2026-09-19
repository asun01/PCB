namespace Asun.Domain.Quality;

public sealed class QualityInspectionRuleFindingIndex
{
    private readonly IReadOnlyDictionary<
        string,
        IReadOnlyList<QualityFindingId>> _byRule;

    public QualityInspectionRuleFindingIndex(
        IEnumerable<KeyValuePair<
            string,
            IReadOnlyList<QualityFindingId>>> entries)
    {
        ArgumentNullException.ThrowIfNull(entries);

        _byRule = entries.ToDictionary(
            entry => entry.Key,
            entry => entry.Value,
            StringComparer.Ordinal);
    }

    public IReadOnlyList<QualityFindingId> FindingsFor(
        string ruleCode) =>
        _byRule.TryGetValue(ruleCode, out var findings)
            ? findings
            : Array.Empty<QualityFindingId>();

    public int RuleCount => _byRule.Count;
}
