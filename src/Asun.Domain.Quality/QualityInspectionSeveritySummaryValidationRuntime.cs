namespace Asun.Domain.Quality;

public static class QualityInspectionSeveritySummaryValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        QualityInspectionResult result,
        QualityInspectionSeveritySummary summary)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(summary);

        var errors = new List<string>();

        if (!QualityInspectionResultValidationRuntime.IsValid(result))
        {
            errors.Add("Inspection result is invalid.");
            return errors;
        }

        if (summary.NoneCount < 0 ||
            summary.InformationCount < 0 ||
            summary.WarningCount < 0 ||
            summary.ErrorCount < 0 ||
            summary.CriticalCount < 0)
        {
            errors.Add("Inspection severity counts cannot be negative.");
            return errors;
        }

        if (summary.TotalCount != result.Findings.Count)
            errors.Add("Inspection severity total does not match finding count.");

        return errors;
    }

    public static bool IsValid(
        QualityInspectionResult result,
        QualityInspectionSeveritySummary summary) =>
        Validate(result, summary).Count == 0;
}
