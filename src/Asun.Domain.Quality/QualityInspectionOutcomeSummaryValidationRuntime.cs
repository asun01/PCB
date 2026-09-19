namespace Asun.Domain.Quality;

public static class QualityInspectionOutcomeSummaryValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        QualityInspectionResult result,
        QualityInspectionOutcomeSummary summary)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(summary);

        var errors = new List<string>();

        if (!QualityInspectionResultValidationRuntime.IsValid(result))
        {
            errors.Add("Inspection result is invalid.");
            return errors;
        }

        if (summary.UnknownCount < 0 ||
            summary.PassCount < 0 ||
            summary.FailCount < 0 ||
            summary.ReviewCount < 0)
        {
            errors.Add("Inspection outcome counts cannot be negative.");
            return errors;
        }

        if (summary.TotalCount != result.Findings.Count)
            errors.Add("Inspection outcome total does not match finding count.");

        return errors;
    }

    public static bool IsValid(
        QualityInspectionResult result,
        QualityInspectionOutcomeSummary summary) =>
        Validate(result, summary).Count == 0;
}
