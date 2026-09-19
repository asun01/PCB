namespace Asun.Domain.Quality;

public static class QualityInspectionSummaryDiffValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        QualityInspectionSummaryDiff diff)
    {
        ArgumentNullException.ThrowIfNull(diff);
        return Array.Empty<string>();
    }

    public static bool IsValid(
        QualityInspectionSummaryDiff diff) =>
        Validate(diff).Count == 0;
}
