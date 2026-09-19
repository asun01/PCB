namespace Asun.Domain.Quality;

public static class QualityInspectionSummaryRuntime
{
    public static QualityInspectionSummary Create(
        QualityInspectionResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (!QualityInspectionResultValidationRuntime.IsValid(result))
            throw new ArgumentException(
                "Inspection result is invalid.",
                nameof(result));

        return new QualityInspectionSummary(
            result.ResultId,
            result.SnapshotId,
            result.Sequence,
            QualityInspectionOutcomeSummaryRuntime.Create(result),
            QualityInspectionSeveritySummaryRuntime.Create(result),
            QualityInspectionEvidenceSummaryRuntime.Create(result),
            QualityInspectionResultDeterminismRuntime
                .CreateContentFingerprint(result));
    }
}
