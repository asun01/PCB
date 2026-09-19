namespace Asun.Domain.Quality;

public static class QualityInspectionFindingAuditIndexRuntime
{
    public static QualityInspectionFindingAuditIndex Create(
        QualityInspectionResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (!QualityInspectionResultValidationRuntime.IsValid(result))
            throw new ArgumentException(
                "Inspection result is invalid.",
                nameof(result));

        var records=result.Findings.Findings
            .Select(finding =>
                QualityInspectionFindingAuditRecordRuntime.Create(
                    result,
                    finding.Id))
            .OrderBy(record => record.FindingId.Value,StringComparer.Ordinal)
            .ToArray();

        return new QualityInspectionFindingAuditIndex(records);
    }
}
