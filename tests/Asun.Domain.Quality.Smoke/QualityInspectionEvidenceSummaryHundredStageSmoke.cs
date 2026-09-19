using Asun.Domain.Quality;

public static class QualityInspectionEvidenceSummaryHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        var f1=new QualityFinding(
            QualityFindingId.Create("F-001"),
            "RULE",
            QualityOutcome.Review,
            QualitySeverity.Information,
            "one");
        var f2=new QualityFinding(
            QualityFindingId.Create("F-002"),
            "RULE",
            QualityOutcome.Review,
            QualitySeverity.Warning,
            "two");
        var result=new QualityInspectionResult(
            Guid.NewGuid(),
            new QualityInspectionSnapshot(
                Guid.NewGuid(),
                102,
                new QualityFindingSet(new[]{f1,f2}),
                new QualityFindingEvidenceSet(new[]{
                    new QualityFindingEvidenceLink(f1.Id,QualityEvidenceKey.Create("shared")),
                    new QualityFindingEvidenceLink(f2.Id,QualityEvidenceKey.Create("shared")),
                    new QualityFindingEvidenceLink(f1.Id,QualityEvidenceKey.Create("unique"))
                })));

        var summary=QualityInspectionEvidenceSummaryRuntime.Create(result);
        var badLinks=summary with {LinkCount=summary.LinkCount+1};
        var badKeys=summary with {DistinctEvidenceKeyCount=summary.DistinctEvidenceKeyCount+1};
        var badFindings=summary with {LinkedFindingCount=summary.LinkedFindingCount+1};

        for(var i=0;i<10;i++) Check(QualityInspectionEvidenceSummaryValidationRuntime.IsValid(result,summary),$"evidence summary validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(summary.LinkCount==3,$"evidence link count round {i+1} should be three.");
        for(var i=0;i<10;i++) Check(summary.DistinctEvidenceKeyCount==2,$"distinct evidence-key count round {i+1} should be two.");
        for(var i=0;i<10;i++) Check(summary.LinkedFindingCount==2,$"linked finding count round {i+1} should be two.");
        for(var i=0;i<10;i++) Check(!QualityInspectionEvidenceSummaryValidationRuntime.IsValid(result,badLinks),$"link-count mismatch round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(!QualityInspectionEvidenceSummaryValidationRuntime.IsValid(result,badKeys),$"evidence-key mismatch round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(!QualityInspectionEvidenceSummaryValidationRuntime.IsValid(result,badFindings),$"linked-finding mismatch round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(summary.LinkCount==result.Evidence.Count,$"link source alignment round {i+1} should remain explicit.");
        for(var i=0;i<10;i++) Check(summary.DistinctEvidenceKeyCount==result.Evidence.Links.Select(link=>link.EvidenceKey).Distinct().Count(),$"key aggregation round {i+1} should remain deterministic.");
        for(var i=0;i<10;i++) Check(QualityInspectionEvidenceSummaryRuntime.Create(result)==summary,$"evidence summary determinism round {i+1} should remain stable.");

        assert(round==100,$"Quality inspection evidence summary smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
