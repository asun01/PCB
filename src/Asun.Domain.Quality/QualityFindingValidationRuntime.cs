namespace Asun.Domain.Quality;

public static class QualityFindingValidationRuntime
{
    public static IReadOnlyList<string> Validate(QualityFinding finding)
    {
        ArgumentNullException.ThrowIfNull(finding);

        var errors = new List<string>();

        if (!finding.Id.IsValid)
            errors.Add("Finding id must be valid.");

        if (string.IsNullOrWhiteSpace(finding.RuleCode))
            errors.Add("Finding rule code cannot be blank.");

        if (!QualityOutcomeValidationRuntime.IsValid(finding.Outcome))
            errors.Add("Finding outcome must be a supported concrete state.");

        if (!QualitySeverityValidationRuntime.IsValid(finding.Severity))
            errors.Add("Finding severity must be a supported value.");

        if (string.IsNullOrWhiteSpace(finding.Message))
            errors.Add("Finding message cannot be blank.");

        return errors;
    }

    public static bool IsValid(QualityFinding finding) =>
        Validate(finding).Count == 0;
}
