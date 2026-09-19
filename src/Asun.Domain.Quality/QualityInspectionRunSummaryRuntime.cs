namespace Asun.Domain.Quality;

public static class QualityInspectionRunSummaryRuntime
{
    public static QualityInspectionRunSummary Create(
        QualityInspectionRun run)
    {
        ArgumentNullException.ThrowIfNull(run);

        if(!QualityInspectionRunValidationRuntime.IsValid(run))
            throw new ArgumentException(
                "Inspection run is invalid.",
                nameof(run));

        var findingCount=run.Results.Sum(result=>result.Findings.Count);
        var evidenceLinkCount=run.Results.Sum(result=>result.Evidence.Count);
        var failCount=run.Results.Sum(result=>
            result.Findings.Findings.Count(
                finding=>finding.Outcome==QualityOutcome.Fail));
        var reviewCount=run.Results.Sum(result=>
            result.Findings.Findings.Count(
                finding=>finding.Outcome==QualityOutcome.Review));
        var criticalCount=run.Results.Sum(result=>
            result.Findings.Findings.Count(
                finding=>finding.Severity==QualitySeverity.Critical));

        var canonical=string.Join(
            "|",
            run.Results.Select(result=>
                QualityInspectionSummaryRuntime.Create(result).ContentFingerprint));

        return new QualityInspectionRunSummary(
            run.RunId,
            run.ResultCount,
            findingCount,
            evidenceLinkCount,
            failCount,
            reviewCount,
            criticalCount,
            QualityInspectionResultDeterminismRuntime.CreateHash(canonical));
    }
}
