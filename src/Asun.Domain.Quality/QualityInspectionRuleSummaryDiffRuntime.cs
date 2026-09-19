namespace Asun.Domain.Quality;

public static class QualityInspectionRuleSummaryDiffRuntime
{
    public static QualityInspectionRuleSummaryDiff Diff(
        QualityInspectionRuleSummary previous,
        QualityInspectionRuleSummary current)
    {
        ArgumentNullException.ThrowIfNull(previous);
        ArgumentNullException.ThrowIfNull(current);

        var previousByRule = previous.Entries
            .ToDictionary(entry => entry.RuleCode, StringComparer.Ordinal);
        var currentByRule = current.Entries
            .ToDictionary(entry => entry.RuleCode, StringComparer.Ordinal);

        var added = currentByRule.Keys
            .Except(previousByRule.Keys, StringComparer.Ordinal)
            .OrderBy(code => code, StringComparer.Ordinal)
            .ToArray();

        var removed = previousByRule.Keys
            .Except(currentByRule.Keys, StringComparer.Ordinal)
            .OrderBy(code => code, StringComparer.Ordinal)
            .ToArray();

        var changed = currentByRule.Keys
            .Intersect(previousByRule.Keys, StringComparer.Ordinal)
            .Where(code => previousByRule[code] != currentByRule[code])
            .OrderBy(code => code, StringComparer.Ordinal)
            .ToArray();

        return new QualityInspectionRuleSummaryDiff(
            added,
            removed,
            changed);
    }
}
