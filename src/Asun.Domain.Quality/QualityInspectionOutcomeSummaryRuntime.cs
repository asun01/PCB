namespace Asun.Domain.Quality;

public static class QualityInspectionOutcomeSummaryRuntime
{
    public static QualityInspectionOutcomeSummary Create(
        QualityInspectionResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (!QualityInspectionResultValidationRuntime.IsValid(result))
            throw new ArgumentException(
                "Inspection result is invalid.",
                nameof(result));

        var findings = result.Findings.Findings;

        return new QualityInspectionOutcomeSummary(
            findings.Count(finding => finding.Outcome == QualityOutcome.Unknown),
            findings.Count(finding => finding.Outcome == QualityOutcome.Pass),
            findings.Count(finding => finding.Outcome == QualityOutcome.Fail),
            findings.Count(finding => finding.Outcome == QualityOutcome.Review));
    }
}
