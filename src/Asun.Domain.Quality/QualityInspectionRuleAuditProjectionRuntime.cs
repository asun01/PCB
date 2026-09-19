namespace Asun.Domain.Quality;

public static class QualityInspectionRuleAuditProjectionRuntime
{
    public static QualityInspectionRuleAuditProjection Create(
        QualityInspectionResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (!QualityInspectionResultValidationRuntime.IsValid(result))
            throw new ArgumentException(
                "Inspection result is invalid.",
                nameof(result));

        return new QualityInspectionRuleAuditProjection(
            QualityInspectionRuleSummaryRuntime.Create(result),
            QualityInspectionRuleFindingIndexRuntime.Create(result),
            QualityInspectionRuleEvidenceIndexRuntime.Create(result));
    }
}
