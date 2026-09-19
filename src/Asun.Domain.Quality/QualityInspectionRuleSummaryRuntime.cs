namespace Asun.Domain.Quality;

public static class QualityInspectionRuleSummaryRuntime
{
    public static QualityInspectionRuleSummary Create(
        QualityInspectionResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (!QualityInspectionResultValidationRuntime.IsValid(result))
            throw new ArgumentException(
                "Inspection result is invalid.",
                nameof(result));

        var entries = result.Findings.Findings
            .GroupBy(finding => finding.RuleCode, StringComparer.Ordinal)
            .Select(group => new QualityInspectionRuleSummaryEntry(
                group.Key,
                group.Count(),
                result.Evidence.Links
                    .Where(link =>
                    {
                        var ids = group
                            .Select(finding => finding.Id)
                            .ToHashSet();

                        return ids.Contains(link.FindingId);
                    })
                    .Distinct()
                    .Count()))
            .OrderBy(entry => entry.RuleCode, StringComparer.Ordinal)
            .ToArray();

        return new QualityInspectionRuleSummary(entries);
    }
}
