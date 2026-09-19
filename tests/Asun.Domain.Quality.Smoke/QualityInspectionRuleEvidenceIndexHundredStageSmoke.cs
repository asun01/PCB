using Asun.Domain.Quality;

public static class QualityInspectionRuleEvidenceIndexHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        var f1=new QualityFinding(QualityFindingId.Create("F-001"),"RULE.A",QualityOutcome.Pass,QualitySeverity.None,"a");
        var f2=new QualityFinding(QualityFindingId.Create("F-002"),"RULE.A",QualityOutcome.Pass,QualitySeverity.None,"b");
        var f3=new QualityFinding(QualityFindingId.Create("F-003"),"RULE.B",QualityOutcome.Fail,QualitySeverity.Warning,"c");
        var result=new QualityInspectionResult(
            Guid.NewGuid(),
            new QualityInspectionSnapshot(
                Guid.NewGuid(),
                107,
                new QualityFindingSet(new[]{f1,f2,f3}),
                new QualityFindingEvidenceSet(new[]{
                    new QualityFindingEvidenceLink(f1.Id,QualityEvidenceKey.Create("z")),
                    new QualityFindingEvidenceLink(f2.Id,QualityEvidenceKey.Create("a")),
                    new QualityFindingEvidenceLink(f3.Id,QualityEvidenceKey.Create("b"))
                })));

        var index=QualityInspectionRuleEvidenceIndexRuntime.Create(result);
        var invalid=new QualityInspectionRuleEvidenceIndex(new[]{
            new KeyValuePair<string,IReadOnlyList<QualityEvidenceKey>>(
                "RULE.A",
                new[]{QualityEvidenceKey.Create("z")})
        });

        for(var i=0;i<10;i++) Check(QualityInspectionRuleEvidenceIndexValidationRuntime.IsValid(result,index),$"rule evidence index validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(index.RuleCount==2,$"rule evidence count round {i+1} should be two.");
        for(var i=0;i<10;i++) Check(index.EvidenceFor("RULE.A").SequenceEqual(new[]{QualityEvidenceKey.Create("a"),QualityEvidenceKey.Create("z")}),$"RULE.A evidence ordering round {i+1} should be canonical.");
        for(var i=0;i<10;i++) Check(index.EvidenceFor("RULE.B").Single()==QualityEvidenceKey.Create("b"),$"RULE.B evidence round {i+1} should identify b.");
        for(var i=0;i<10;i++) Check(index.EvidenceFor("RULE.NONE").Count==0,$"missing rule evidence round {i+1} should be empty.");
        for(var i=0;i<10;i++) Check(!QualityInspectionRuleEvidenceIndexValidationRuntime.IsValid(result,invalid),$"tampered rule evidence index round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(index.EvidenceFor("RULE.A").Distinct().Count()==2,$"RULE.A evidence uniqueness round {i+1} should remain explicit.");
        for(var i=0;i<10;i++) Check(index.EvidenceFor("RULE.A").SequenceEqual(index.EvidenceFor("RULE.A").OrderBy(key=>key.Value,StringComparer.Ordinal)),$"rule evidence ordering round {i+1} should remain deterministic.");
        for(var i=0;i<10;i++) Check(QualityInspectionRuleEvidenceIndexRuntime.Create(result).EvidenceFor("RULE.A").SequenceEqual(index.EvidenceFor("RULE.A")),$"rule evidence determinism round {i+1} should remain stable.");
        for(var i=0;i<10;i++) Check(QualityInspectionRuleEvidenceIndexValidationRuntime.Validate(result,index).Count==0,$"rule evidence diagnostics round {i+1} should remain empty.");

        assert(round==100,$"Quality inspection rule evidence index smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
