namespace Asun.Domain.Quality;

public static class QualityInspectionEvidenceSummaryRuntime
{
    public static QualityInspectionEvidenceSummary Create(
        QualityInspectionResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (!QualityInspectionResultValidationRuntime.IsValid(result))
            throw new ArgumentException(
                "Inspection result is invalid.",
                nameof(result));

        return new QualityInspectionEvidenceSummary(
            result.Evidence.Count,
            result.Evidence.Links
                .Select(link => link.EvidenceKey)
                .Distinct()
                .Count(),
            result.Evidence.Links
                .Select(link => link.FindingId)
                .Distinct()
                .Count());
    }
}
