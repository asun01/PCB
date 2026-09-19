namespace Asun.Domain.Quality;

public static class QualityInspectionRunSummaryValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        QualityInspectionRun run,
        QualityInspectionRunSummary summary)
    {
        ArgumentNullException.ThrowIfNull(run);
        ArgumentNullException.ThrowIfNull(summary);

        var errors=new List<string>(
            QualityInspectionRunValidationRuntime.Validate(run));

        if(summary.RunId!=run.RunId)
            errors.Add("Run summary run id must match the source run.");

        var expected=QualityInspectionRunSummaryRuntime.Create(run);

        if(summary.ResultCount!=expected.ResultCount)
            errors.Add("Run summary result count does not match.");

        if(summary.FindingCount!=expected.FindingCount)
            errors.Add("Run summary finding count does not match.");

        if(summary.EvidenceLinkCount!=expected.EvidenceLinkCount)
            errors.Add("Run summary evidence-link count does not match.");

        if(summary.FailCount!=expected.FailCount)
            errors.Add("Run summary fail count does not match.");

        if(summary.ReviewCount!=expected.ReviewCount)
            errors.Add("Run summary review count does not match.");

        if(summary.CriticalCount!=expected.CriticalCount)
            errors.Add("Run summary critical count does not match.");

        if(summary.ContentFingerprint!=expected.ContentFingerprint)
            errors.Add("Run summary content fingerprint does not match.");

        return errors;
    }

    public static bool IsValid(
        QualityInspectionRun run,
        QualityInspectionRunSummary summary)=>
        Validate(run,summary).Count==0;
}
