namespace Asun.Domain.Quality;

public static class QualityInspectionRuleFindingIndexRuntime
{
    public static QualityInspectionRuleFindingIndex Create(
        QualityInspectionResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (!QualityInspectionResultValidationRuntime.IsValid(result))
            throw new ArgumentException(
                "Inspection result is invalid.",
                nameof(result));

        var entries = result.Findings.Findings
            .GroupBy(finding => finding.RuleCode, StringComparer.Ordinal)
            .Select(group =>
                new KeyValuePair<string, IReadOnlyList<QualityFindingId>>(
                    group.Key,
                    group
                        .Select(finding => finding.Id)
                        .OrderBy(id => id.Value, StringComparer.Ordinal)
                        .ToArray()))
            .OrderBy(entry => entry.Key, StringComparer.Ordinal)
            .ToArray();

        return new QualityInspectionRuleFindingIndex(entries);
    }
}
