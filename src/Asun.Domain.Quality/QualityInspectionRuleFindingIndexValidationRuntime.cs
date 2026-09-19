namespace Asun.Domain.Quality;

public static class QualityInspectionRuleFindingIndexValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        QualityInspectionResult result,
        QualityInspectionRuleFindingIndex index)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(index);

        var errors = new List<string>();

        if (!QualityInspectionResultValidationRuntime.IsValid(result))
        {
            errors.Add("Inspection result is invalid.");
            return errors;
        }

        var expected=QualityInspectionRuleFindingIndexRuntime.Create(result);

        foreach (var entry in result.Findings.Findings)
        {
            if (!expected.FindingsFor(entry.RuleCode)
                .SequenceEqual(index.FindingsFor(entry.RuleCode)))
            {
                errors.Add(
                    $"Rule finding index does not match rule {entry.RuleCode}.");
            }
        }

        return errors;
    }

    public static bool IsValid(
        QualityInspectionResult result,
        QualityInspectionRuleFindingIndex index) =>
        Validate(result,index).Count==0;
}
