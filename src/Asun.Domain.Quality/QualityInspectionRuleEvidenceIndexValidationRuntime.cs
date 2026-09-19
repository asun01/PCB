namespace Asun.Domain.Quality;

public static class QualityInspectionRuleEvidenceIndexValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        QualityInspectionResult result,
        QualityInspectionRuleEvidenceIndex index)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(index);

        var errors = new List<string>();

        if (!QualityInspectionResultValidationRuntime.IsValid(result))
        {
            errors.Add("Inspection result is invalid.");
            return errors;
        }

        var expected=QualityInspectionRuleEvidenceIndexRuntime.Create(result);

        foreach (var finding in result.Findings.Findings)
        {
            if (!expected.EvidenceFor(finding.RuleCode)
                .SequenceEqual(index.EvidenceFor(finding.RuleCode)))
            {
                errors.Add(
                    $"Rule evidence index does not match rule {finding.RuleCode}.");
            }
        }

        return errors;
    }

    public static bool IsValid(
        QualityInspectionResult result,
        QualityInspectionRuleEvidenceIndex index) =>
        Validate(result,index).Count==0;
}
