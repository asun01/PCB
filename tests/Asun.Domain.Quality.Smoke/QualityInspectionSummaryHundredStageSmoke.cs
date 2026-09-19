using Asun.Domain.Quality;

public static class QualityInspectionSummaryHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        var finding=new QualityFinding(
            QualityFindingId.Create("F-001"),
            "RULE",
            QualityOutcome.Fail,
            QualitySeverity.Error,
            "summary finding");
        var second=new QualityFinding(
            QualityFindingId.Create("F-002"),
            "RULE",
            QualityOutcome.Review,
            QualitySeverity.Warning,
            "summary review");
        var result=new QualityInspectionResult(
            Guid.NewGuid(),
            new QualityInspectionSnapshot(
                Guid.NewGuid(),
                103,
                new QualityFindingSet(new[]{finding,second}),
                new QualityFindingEvidenceSet(new[]{
                    new QualityFindingEvidenceLink(finding.Id,QualityEvidenceKey.Create("a")),
                    new QualityFindingEvidenceLink(second.Id,QualityEvidenceKey.Create("b"))
                })));

        var summary=QualityInspectionSummaryRuntime.Create(result);
        var wrongResultId=summary with
        {
            ResultId=Guid.NewGuid()
        };
        var wrongFingerprint=summary with
        {
            ContentFingerprint=new string('a',64)
        };

        for(var i=0;i<10;i++) Check(QualityInspectionSummaryValidationRuntime.IsValid(result,summary),$"inspection summary validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(summary.ResultId==result.ResultId,$"summary result identity round {i+1} should match.");
        for(var i=0;i<10;i++) Check(summary.SnapshotId==result.SnapshotId,$"summary snapshot identity round {i+1} should match.");
        for(var i=0;i<10;i++) Check(summary.Sequence==103,$"summary sequence round {i+1} should match.");
        for(var i=0;i<10;i++) Check(summary.Outcomes.FailCount==1 && summary.Outcomes.ReviewCount==1,$"summary outcome counts round {i+1} should be explicit.");
        for(var i=0;i<10;i++) Check(summary.Severities.ErrorCount==1 && summary.Severities.WarningCount==1,$"summary severity counts round {i+1} should be explicit.");
        for(var i=0;i<10;i++) Check(summary.Evidence.LinkCount==2 && summary.Evidence.DistinctEvidenceKeyCount==2,$"summary evidence counts round {i+1} should be explicit.");
        for(var i=0;i<10;i++) Check(summary.ContentFingerprint==QualityInspectionResultDeterminismRuntime.CreateContentFingerprint(result),$"summary fingerprint round {i+1} should match canonical result content.");
        for(var i=0;i<10;i++) Check(!QualityInspectionSummaryValidationRuntime.IsValid(result,wrongResultId),$"summary identity mismatch round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(!QualityInspectionSummaryValidationRuntime.IsValid(result,wrongFingerprint),$"summary fingerprint mismatch round {i+1} should be rejected.");

        assert(round==100,$"Quality inspection summary smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
