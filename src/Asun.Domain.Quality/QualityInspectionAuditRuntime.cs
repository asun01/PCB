namespace Asun.Domain.Quality;

public static class QualityInspectionAuditRuntime
{
    public static QualityInspectionAuditRecord Create(
        QualityInspectionResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (!QualityInspectionResultValidationRuntime.IsValid(result))
            throw new ArgumentException(
                "Inspection result is invalid.",
                nameof(result));

        return new QualityInspectionAuditRecord(
            result.ResultId,
            result.SnapshotId,
            result.Sequence,
            result.Findings.Count,
            result.Evidence.Count,
            QualityInspectionResultDeterminismRuntime
                .CreateContentFingerprint(result));
    }
}
