namespace Asun.Domain.Quality;

public static class QualityInspectionAuditProjectionRuntime
{
    public static QualityInspectionAuditProjection Create(
        QualityInspectionResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (!QualityInspectionResultValidationRuntime.IsValid(result))
            throw new ArgumentException(
                "Inspection result is invalid.",
                nameof(result));

        return new QualityInspectionAuditProjection(
            QualityInspectionAuditRuntime.Create(result),
            QualityInspectionSummaryRuntime.Create(result),
            QualityInspectionRuleAuditProjectionRuntime.Create(result),
            QualityInspectionFindingAuditProjectionRuntime.Create(result));
    }
}
