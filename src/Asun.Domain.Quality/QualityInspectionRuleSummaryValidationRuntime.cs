namespace Asun.Domain.Quality;

public static class QualityInspectionRuleSummaryValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        QualityInspectionResult result,
        QualityInspectionRuleSummary summary)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(summary);

        var errors = new List<string>();

        if (!QualityInspectionResultValidationRuntime.IsValid(result))
        {
            errors.Add("Inspection result is invalid.");
            return errors;
        }

        var expected = QualityInspectionRuleSummaryRuntime.Create(result);

        if (!summary.Entries.SequenceEqual(expected.Entries))
            errors.Add("Rule summary entries do not match the inspection result.");

        if (summary.Entries.Any(entry =>
                string.IsNullOrWhiteSpace(entry.RuleCode) ||
                entry.FindingCount < 0 ||
                entry.DistinctEvidenceLinkCount < 0))
        {
            errors.Add("Rule summary entries contain invalid values.");
        }

        return errors;
    }

    public static bool IsValid(
        QualityInspectionResult result,
        QualityInspectionRuleSummary summary) =>
        Validate(result, summary).Count == 0;
}
