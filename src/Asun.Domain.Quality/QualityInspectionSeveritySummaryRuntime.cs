namespace Asun.Domain.Quality;

public static class QualityInspectionSeveritySummaryRuntime
{
    public static QualityInspectionSeveritySummary Create(
        QualityInspectionResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (!QualityInspectionResultValidationRuntime.IsValid(result))
            throw new ArgumentException(
                "Inspection result is invalid.",
                nameof(result));

        var findings = result.Findings.Findings;

        return new QualityInspectionSeveritySummary(
            findings.Count(finding => finding.Severity == QualitySeverity.None),
            findings.Count(finding => finding.Severity == QualitySeverity.Information),
            findings.Count(finding => finding.Severity == QualitySeverity.Warning),
            findings.Count(finding => finding.Severity == QualitySeverity.Error),
            findings.Count(finding => finding.Severity == QualitySeverity.Critical));
    }
}
