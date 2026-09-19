using Asun.Domain.Quality;

public static class QualityInspectionRuleSummaryHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        static QualityFinding Make(
            string id,
            string rule,
            string message) =>
            new(
                QualityFindingId.Create(id),
                rule,
                QualityOutcome.Review,
                QualitySeverity.Information,
                message);

        var f1=Make("F-001","RULE.A","one");
        var f2=Make("F-002","RULE.A","two");
        var f3=Make("F-003","RULE.B","three");
        var result=new QualityInspectionResult(
            Guid.NewGuid(),
            new QualityInspectionSnapshot(
                Guid.NewGuid(),
                105,
                new QualityFindingSet(new[]{f2,f1,f3}),
                new QualityFindingEvidenceSet(new[]{
                    new QualityFindingEvidenceLink(f1.Id,QualityEvidenceKey.Create("a")),
                    new QualityFindingEvidenceLink(f3.Id,QualityEvidenceKey.Create("b"))
                })));

        var summary=QualityInspectionRuleSummaryRuntime.Create(result);
        var bad= new QualityInspectionRuleSummary(new[]{
            new QualityInspectionRuleSummaryEntry("RULE.A",1,1),
            new QualityInspectionRuleSummaryEntry("RULE.B",1,1)
        });

        for(var i=0;i<10;i++) Check(QualityInspectionRuleSummaryValidationRuntime.IsValid(result,summary),$"rule summary validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(summary.RuleCount==2,$"rule count round {i+1} should be two.");
        for(var i=0;i<10;i++) Check(summary.Entries[0].RuleCode=="RULE.A",$"rule ordering round {i+1} should start with RULE.A.");
        for(var i=0;i<10;i++) Check(summary.Entries[0].FindingCount==2,$"RULE.A finding count round {i+1} should be two.");
        for(var i=0;i<10;i++) Check(summary.Entries[0].DistinctEvidenceLinkCount==1,$"RULE.A evidence link count round {i+1} should be one.");
        for(var i=0;i<10;i++) Check(summary.Entries[1].RuleCode=="RULE.B",$"rule ordering round {i+1} should end with RULE.B.");
        for(var i=0;i<10;i++) Check(summary.Entries[1].FindingCount==1,$"RULE.B finding count round {i+1} should be one.");
        for(var i=0;i<10;i++) Check(summary.Entries[1].DistinctEvidenceLinkCount==1,$"RULE.B evidence link count round {i+1} should be one.");
        for(var i=0;i<10;i++) Check(!QualityInspectionRuleSummaryValidationRuntime.IsValid(result,bad),$"tampered rule summary round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(QualityInspectionRuleSummaryRuntime.Create(result).Entries.SequenceEqual(summary.Entries),$"rule summary determinism round {i+1} should remain stable.");

        assert(round==100,$"Quality inspection rule summary smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
