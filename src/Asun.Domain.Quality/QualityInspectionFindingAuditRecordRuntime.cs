namespace Asun.Domain.Quality;

public static class QualityInspectionFindingAuditRecordRuntime
{
    public static QualityInspectionFindingAuditRecord Create(
        QualityInspectionResult result,
        QualityFindingId findingId)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (!QualityInspectionResultValidationRuntime.IsValid(result))
            throw new ArgumentException(
                "Inspection result is invalid.",
                nameof(result));

        var finding=result.Findings.Find(findingId)
            ?? throw new ArgumentException(
                "Finding id is not present in the inspection result.",
                nameof(findingId));

        var evidence=result.Evidence
            .ForFinding(finding.Id)
            .OrderBy(key=>key.Value,StringComparer.Ordinal)
            .ToArray();

        return new QualityInspectionFindingAuditRecord(
            finding.Id,
            finding.RuleCode,
            finding.Outcome,
            finding.Severity,
            finding.Message,
            evidence);
    }
}
