using Asun.Domain.Quality;

public static class QualityInspectionOutcomeSummaryHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        static QualityFinding Make(
            string id,
            QualityOutcome outcome) =>
            new(
                QualityFindingId.Create(id),
                "RULE",
                outcome,
                QualitySeverity.Information,
                id);

        var result=new QualityInspectionResult(
            Guid.NewGuid(),
            new QualityInspectionSnapshot(
                Guid.NewGuid(),
                100,
                new QualityFindingSet(new[]{
                    Make("F-001",QualityOutcome.Unknown),
                    Make("F-002",QualityOutcome.Pass),
                    Make("F-003",QualityOutcome.Fail),
                    Make("F-004",QualityOutcome.Review),
                    Make("F-005",QualityOutcome.Pass)
                }),
                new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>())));

        var summary=QualityInspectionOutcomeSummaryRuntime.Create(result);
        var invalid=summary with {FailCount=-1};
        var wrong=summary with {PassCount=summary.PassCount+1};

        for(var i=0;i<10;i++) Check(QualityInspectionOutcomeSummaryValidationRuntime.IsValid(result,summary),$"outcome summary validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(summary.UnknownCount==1,$"unknown count round {i+1} should be one.");
        for(var i=0;i<10;i++) Check(summary.PassCount==2,$"pass count round {i+1} should be two.");
        for(var i=0;i<10;i++) Check(summary.FailCount==1,$"fail count round {i+1} should be one.");
        for(var i=0;i<10;i++) Check(summary.ReviewCount==1,$"review count round {i+1} should be one.");
        for(var i=0;i<10;i++) Check(summary.TotalCount==5,$"total count round {i+1} should match five findings.");
        for(var i=0;i<10;i++) Check(!QualityInspectionOutcomeSummaryValidationRuntime.IsValid(result,invalid),$"negative outcome count round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(summary.TotalCount==result.Findings.Count,$"summary/finding alignment round {i+1} should remain explicit.");
        for(var i=0;i<10;i++) Check(QualityInspectionOutcomeSummaryRuntime.Create(result)==summary,$"summary determinism round {i+1} should remain stable.");
        for(var i=0;i<10;i++) Check(!QualityInspectionOutcomeSummaryValidationRuntime.IsValid(result,wrong),$"summary total mismatch round {i+1} should be rejected.");

        assert(round==100,$"Quality inspection outcome summary smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
