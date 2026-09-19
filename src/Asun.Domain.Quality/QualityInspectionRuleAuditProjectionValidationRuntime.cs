namespace Asun.Domain.Quality;

public static class QualityInspectionRuleAuditProjectionValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        QualityInspectionResult result,
        QualityInspectionRuleAuditProjection projection)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(projection);

        var errors = new List<string>();

        errors.AddRange(
            QualityInspectionRuleSummaryValidationRuntime.Validate(
                result,
                projection.Summary));

        errors.AddRange(
            QualityInspectionRuleFindingIndexValidationRuntime.Validate(
                result,
                projection.FindingIndex));

        errors.AddRange(
            QualityInspectionRuleEvidenceIndexValidationRuntime.Validate(
                result,
                projection.EvidenceIndex));

        return errors;
    }

    public static bool IsValid(
        QualityInspectionResult result,
        QualityInspectionRuleAuditProjection projection) =>
        Validate(result,projection).Count==0;
}
