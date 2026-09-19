namespace Asun.Domain.Quality;

public static class QualityInspectionRuleEvidenceIndexRuntime
{
    public static QualityInspectionRuleEvidenceIndex Create(
        QualityInspectionResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (!QualityInspectionResultValidationRuntime.IsValid(result))
            throw new ArgumentException(
                "Inspection result is invalid.",
                nameof(result));

        var findingToRule = result.Findings.Findings
            .ToDictionary(finding => finding.Id, finding => finding.RuleCode);

        var entries = result.Evidence.Links
            .GroupBy(
                link => findingToRule[link.FindingId],
                StringComparer.Ordinal)
            .Select(group =>
                new KeyValuePair<string, IReadOnlyList<QualityEvidenceKey>>(
                    group.Key,
                    group
                        .Select(link => link.EvidenceKey)
                        .Distinct()
                        .OrderBy(key => key.Value, StringComparer.Ordinal)
                        .ToArray()))
            .OrderBy(entry => entry.Key, StringComparer.Ordinal)
            .ToArray();

        return new QualityInspectionRuleEvidenceIndex(entries);
    }
}
