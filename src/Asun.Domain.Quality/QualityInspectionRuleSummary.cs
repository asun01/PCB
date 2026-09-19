namespace Asun.Domain.Quality;

public sealed class QualityInspectionRuleSummary
{
    private readonly QualityInspectionRuleSummaryEntry[] _entries;

    public QualityInspectionRuleSummary(
        IEnumerable<QualityInspectionRuleSummaryEntry> entries)
    {
        ArgumentNullException.ThrowIfNull(entries);

        _entries = entries.ToArray();
    }

    public int RuleCount => _entries.Length;

    public IReadOnlyList<QualityInspectionRuleSummaryEntry> Entries =>
        Array.AsReadOnly(_entries);
}
