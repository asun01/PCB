using Asun.Domain.Quality;

public static class QualityInspectionRuleFindingIndexHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        var a=new QualityFinding(QualityFindingId.Create("F-002"),"RULE.A",QualityOutcome.Pass,QualitySeverity.None,"a");
        var b=new QualityFinding(QualityFindingId.Create("F-001"),"RULE.A",QualityOutcome.Pass,QualitySeverity.None,"b");
        var c=new QualityFinding(QualityFindingId.Create("F-003"),"RULE.B",QualityOutcome.Fail,QualitySeverity.Warning,"c");
        var result=new QualityInspectionResult(
            Guid.NewGuid(),
            new QualityInspectionSnapshot(
                Guid.NewGuid(),
                106,
                new QualityFindingSet(new[]{a,b,c}),
                new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>())));

        var index=QualityInspectionRuleFindingIndexRuntime.Create(result);
        var invalid=new QualityInspectionRuleFindingIndex(new[]{
            new KeyValuePair<string,IReadOnlyList<QualityFindingId>>(
                "RULE.A",
                new[]{new QualityFindingId("F-002")})
        });

        for(var i=0;i<10;i++) Check(QualityInspectionRuleFindingIndexValidationRuntime.IsValid(result,index),$"rule finding index validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(index.RuleCount==2,$"rule count round {i+1} should be two.");
        for(var i=0;i<10;i++) Check(index.FindingsFor("RULE.A").SequenceEqual(new[]{new QualityFindingId("F-001"),new QualityFindingId("F-002")}),$"RULE.A finding ordering round {i+1} should be canonical.");
        for(var i=0;i<10;i++) Check(index.FindingsFor("RULE.B").Single()==c.Id,$"RULE.B finding round {i+1} should identify F-003.");
        for(var i=0;i<10;i++) Check(index.FindingsFor("RULE.NONE").Count==0,$"missing rule lookup round {i+1} should be empty.");
        for(var i=0;i<10;i++) Check(!QualityInspectionRuleFindingIndexValidationRuntime.IsValid(result,invalid),$"tampered rule index round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(index.FindingsFor("RULE.A").Distinct().Count()==2,$"RULE.A uniqueness round {i+1} should remain explicit.");
        for(var i=0;i<10;i++) Check(index.FindingsFor("RULE.A").SequenceEqual(index.FindingsFor("RULE.A").OrderBy(id=>id.Value,StringComparer.Ordinal)),$"rule index ordering round {i+1} should remain deterministic.");
        for(var i=0;i<10;i++) Check(QualityInspectionRuleFindingIndexRuntime.Create(result).FindingsFor("RULE.A").SequenceEqual(index.FindingsFor("RULE.A")),$"rule index determinism round {i+1} should remain stable.");
        for(var i=0;i<10;i++) Check(QualityInspectionRuleFindingIndexValidationRuntime.Validate(result,index).Count==0,$"rule index diagnostics round {i+1} should remain empty.");

        assert(round==100,$"Quality inspection rule finding index smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
