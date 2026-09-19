namespace Asun.Domain.Quality;

public sealed class QualityInspectionRuleEvidenceIndex
{
    private readonly IReadOnlyDictionary<
        string,
        IReadOnlyList<QualityEvidenceKey>> _byRule;

    public QualityInspectionRuleEvidenceIndex(
        IEnumerable<KeyValuePair<
            string,
            IReadOnlyList<QualityEvidenceKey>>> entries)
    {
        ArgumentNullException.ThrowIfNull(entries);

        _byRule = entries.ToDictionary(
            entry => entry.Key,
            entry => entry.Value,
            StringComparer.Ordinal);
    }

    public IReadOnlyList<QualityEvidenceKey> EvidenceFor(
        string ruleCode) =>
        _byRule.TryGetValue(ruleCode, out var evidence)
            ? evidence
            : Array.Empty<QualityEvidenceKey>();

    public int RuleCount => _byRule.Count;
}
