namespace Asun.Domain.Quality;

public static class QualityInspectionAuditProjectionValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        QualityInspectionResult result,
        QualityInspectionAuditProjection projection)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(projection);

        var errors=new List<string>();

        errors.AddRange(
            QualityInspectionAuditValidationRuntime.Validate(
                projection.Record));

        errors.AddRange(
            QualityInspectionSummaryValidationRuntime.Validate(
                result,
                projection.Summary));

        errors.AddRange(
            QualityInspectionRuleAuditProjectionValidationRuntime.Validate(
                result,
                projection.RuleAudit));

        errors.AddRange(
            QualityInspectionFindingAuditProjectionValidationRuntime.Validate(
                result,
                projection.FindingAudit));

        if(projection.Record.ResultId!=result.ResultId ||
           projection.Summary.ResultId!=result.ResultId)
        {
            errors.Add("Top-level audit projection result identity is inconsistent.");
        }

        return errors;
    }

    public static bool IsValid(
        QualityInspectionResult result,
        QualityInspectionAuditProjection projection) =>
        Validate(result,projection).Count==0;
}
