using Asun.Domain.Quality;

public static class QualityInspectionRunHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var finding1=new QualityFinding(
            QualityFindingId.Create("F-001"),
            "PAD_WIDTH",
            QualityOutcome.Fail,
            QualitySeverity.Critical,
            "Pad width outside observation range.");
        var finding2=new QualityFinding(
            QualityFindingId.Create("F-002"),
            "COPPER",
            QualityOutcome.Review,
            QualitySeverity.Warning,
            "Copper observation requires review.");
        var emptyEvidence=new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>());
        var snapshot1=new QualityInspectionSnapshot(
            Guid.NewGuid(),
            1,
            new QualityFindingSet(new[]{finding1}),
            emptyEvidence);
        var snapshot2=new QualityInspectionSnapshot(
            Guid.NewGuid(),
            2,
            new QualityFindingSet(new[]{finding2}),
            emptyEvidence);
        var result1=new QualityInspectionResult(Guid.NewGuid(),snapshot1);
        var result2=new QualityInspectionResult(Guid.NewGuid(),snapshot2);
        var run=QualityInspectionRunRuntime.Create(
            Guid.NewGuid(),
            new[]{result2,result1});
        var summary=QualityInspectionRunSummaryRuntime.Create(run);
        var fingerprint=QualityInspectionRunFingerprintRuntime.CreateFingerprint(run);
        var invalid=summary with {FailCount=0};

        for(var i=0;i<10;i++) Check(run.ResultCount==2,"Inspection run should preserve two results.");
        for(var i=0;i<10;i++) Check(run.Results[0].Sequence==1 && run.Results[1].Sequence==2,"Inspection run should canonicalize sequence order.");
        for(var i=0;i<10;i++) Check(QualityInspectionRunValidationRuntime.IsValid(run),"Inspection run should validate.");
        for(var i=0;i<10;i++) Check(summary.ResultCount==2,"Run summary should preserve result count.");
        for(var i=0;i<10;i++) Check(summary.FindingCount==2,"Run summary should preserve finding count.");
        for(var i=0;i<10;i++) Check(summary.FailCount==1,"Run summary should count one fail.");
        for(var i=0;i<10;i++) Check(summary.ReviewCount==1,"Run summary should count one review.");
        for(var i=0;i<10;i++) Check(summary.CriticalCount==1,"Run summary should count one critical finding.");
        for(var i=0;i<10;i++) Check(QualityInspectionRunSummaryValidationRuntime.IsValid(run,summary),"Run summary should validate.");
        for(var i=0;i<10;i++) Check(!QualityInspectionRunSummaryValidationRuntime.IsValid(run,invalid),"Tampered run summary should be rejected.");

        assert(round==100,$"Quality inspection run smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
