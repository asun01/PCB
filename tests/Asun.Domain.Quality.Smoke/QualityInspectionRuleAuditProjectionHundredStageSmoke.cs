using Asun.Domain.Quality;

public static class QualityInspectionRuleAuditProjectionHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        var f1=new QualityFinding(QualityFindingId.Create("F-001"),"RULE.A",QualityOutcome.Pass,QualitySeverity.None,"a");
        var f2=new QualityFinding(QualityFindingId.Create("F-002"),"RULE.A",QualityOutcome.Review,QualitySeverity.Warning,"b");
        var f3=new QualityFinding(QualityFindingId.Create("F-003"),"RULE.B",QualityOutcome.Fail,QualitySeverity.Error,"c");
        var result=new QualityInspectionResult(
            Guid.NewGuid(),
            new QualityInspectionSnapshot(
                Guid.NewGuid(),
                108,
                new QualityFindingSet(new[]{f1,f2,f3}),
                new QualityFindingEvidenceSet(new[]{
                    new QualityFindingEvidenceLink(f1.Id,QualityEvidenceKey.Create("a")),
                    new QualityFindingEvidenceLink(f3.Id,QualityEvidenceKey.Create("b"))
                })));

        var projection=QualityInspectionRuleAuditProjectionRuntime.Create(result);
        var invalidFindingIndex=new QualityInspectionRuleFindingIndex(new[]{
            new KeyValuePair<string,IReadOnlyList<QualityFindingId>>(
                "RULE.A",
                new[]{f1.Id})
        });
        var invalid=projection with {FindingIndex=invalidFindingIndex};

        for(var i=0;i<10;i++) Check(QualityInspectionRuleAuditProjectionValidationRuntime.IsValid(result,projection),$"rule audit projection validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(projection.Summary.RuleCount==2,$"rule audit summary count round {i+1} should be two.");
        for(var i=0;i<10;i++) Check(projection.FindingIndex.FindingsFor("RULE.A").Count==2,$"rule audit finding index round {i+1} should contain two findings.");
        for(var i=0;i<10;i++) Check(projection.EvidenceIndex.EvidenceFor("RULE.A").Single()==QualityEvidenceKey.Create("a"),$"rule audit evidence index round {i+1} should expose a.");
        for(var i=0;i<10;i++) Check(projection.EvidenceIndex.EvidenceFor("RULE.B").Single()==QualityEvidenceKey.Create("b"),$"rule audit evidence index round {i+1} should expose b.");
        for(var i=0;i<10;i++) Check(!QualityInspectionRuleAuditProjectionValidationRuntime.IsValid(result,invalid),$"tampered rule audit projection round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(QualityInspectionRuleAuditProjectionValidationRuntime.Validate(result,projection).Count==0,$"rule audit diagnostics round {i+1} should remain empty.");
        for(var i=0;i<10;i++) Check(QualityInspectionRuleAuditProjectionRuntime.Create(result).Summary.Entries.SequenceEqual(projection.Summary.Entries),$"rule audit summary determinism round {i+1} should remain stable.");
        for(var i=0;i<10;i++) Check(QualityInspectionRuleAuditProjectionRuntime.Create(result).FindingIndex.FindingsFor("RULE.A").SequenceEqual(projection.FindingIndex.FindingsFor("RULE.A")),$"rule audit finding determinism round {i+1} should remain stable.");
        for(var i=0;i<10;i++) Check(QualityInspectionRuleAuditProjectionRuntime.Create(result).EvidenceIndex.EvidenceFor("RULE.A").SequenceEqual(projection.EvidenceIndex.EvidenceFor("RULE.A")),$"rule audit evidence determinism round {i+1} should remain stable.");

        assert(round==100,$"Quality inspection rule audit projection smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
