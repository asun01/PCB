namespace Asun.Domain.Quality;

public static class QualityInspectionRuleSummaryDiffValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        QualityInspectionRuleSummaryDiff diff)
    {
        ArgumentNullException.ThrowIfNull(diff);

        var errors = new List<string>();
        ValidateUnique(diff.AddedRuleCodes, "Added rule codes", errors);
        ValidateUnique(diff.RemovedRuleCodes, "Removed rule codes", errors);
        ValidateUnique(diff.ChangedRuleCodes, "Changed rule codes", errors);

        if (diff.AddedRuleCodes.Any(string.IsNullOrWhiteSpace) ||
            diff.RemovedRuleCodes.Any(string.IsNullOrWhiteSpace) ||
            diff.ChangedRuleCodes.Any(string.IsNullOrWhiteSpace))
        {
            errors.Add("Rule diff codes cannot be blank.");
        }

        if (diff.AddedRuleCodes.Intersect(
                diff.RemovedRuleCodes,
                StringComparer.Ordinal).Any() ||
            diff.AddedRuleCodes.Intersect(
                diff.ChangedRuleCodes,
                StringComparer.Ordinal).Any() ||
            diff.RemovedRuleCodes.Intersect(
                diff.ChangedRuleCodes,
                StringComparer.Ordinal).Any())
        {
            errors.Add("A rule code cannot appear in multiple diff categories.");
        }

        return errors;
    }

    public static bool IsValid(
        QualityInspectionRuleSummaryDiff diff) =>
        Validate(diff).Count == 0;

    private static void ValidateUnique(
        IReadOnlyList<string> values,
        string label,
        List<string> errors)
    {
        if (values.Count != values.Distinct(StringComparer.Ordinal).Count())
            errors.Add($"{label} must be unique.");
    }
}
