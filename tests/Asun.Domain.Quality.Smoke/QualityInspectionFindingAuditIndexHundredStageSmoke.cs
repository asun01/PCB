using Asun.Domain.Quality;

public static class QualityInspectionFindingAuditIndexHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        var f2=new QualityFinding(QualityFindingId.Create("F-002"),"RULE.B",QualityOutcome.Fail,QualitySeverity.Error,"two");
        var f1=new QualityFinding(QualityFindingId.Create("F-001"),"RULE.A",QualityOutcome.Pass,QualitySeverity.None,"one");
        var result=new QualityInspectionResult(
            Guid.NewGuid(),
            new QualityInspectionSnapshot(
                Guid.NewGuid(),
                111,
                new QualityFindingSet(new[]{f2,f1}),
                new QualityFindingEvidenceSet(new[]{
                    new QualityFindingEvidenceLink(f1.Id,QualityEvidenceKey.Create("a")),
                    new QualityFindingEvidenceLink(f2.Id,QualityEvidenceKey.Create("b"))
                })));

        var index=QualityInspectionFindingAuditIndexRuntime.Create(result);
        var tamperedRecord=QualityInspectionFindingAuditRecordRuntime.Create(result,f1.Id) with
        {
            Message="tampered"
        };
        var tampered=new QualityInspectionFindingAuditIndex(new[]{tamperedRecord});

        for(var i=0;i<10;i++) Check(QualityInspectionFindingAuditIndexValidationRuntime.IsValid(result,index),$"finding audit index validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(index.Count==2,$"finding audit index count round {i+1} should be two.");
        for(var i=0;i<10;i++) Check(index.FindingIds.SequenceEqual(new[]{new QualityFindingId("F-001"),new QualityFindingId("F-002")}),$"finding audit id ordering round {i+1} should be canonical.");
        for(var i=0;i<10;i++) Check(index.Find(f1.Id)?.RuleCode=="RULE.A",$"finding audit lookup round {i+1} should expose RULE.A.");
        for(var i=0;i<10;i++) Check(index.Find(f2.Id)?.RuleCode=="RULE.B",$"finding audit lookup round {i+1} should expose RULE.B.");
        for(var i=0;i<10;i++) Check(index.Find(QualityFindingId.Create("F-999")) is null,$"missing finding audit lookup round {i+1} should be empty.");
        for(var i=0;i<10;i++) Check(!QualityInspectionFindingAuditIndexValidationRuntime.IsValid(result,tampered),$"tampered finding audit index round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(index.FindingIds.Distinct().Count()==2,$"finding audit id uniqueness round {i+1} should remain explicit.");
        for(var i=0;i<10;i++) Check(QualityInspectionFindingAuditIndexRuntime.Create(result).FindingIds.SequenceEqual(index.FindingIds),$"finding audit index determinism round {i+1} should remain stable.");
        for(var i=0;i<10;i++) Check(QualityInspectionFindingAuditIndexValidationRuntime.Validate(result,index).Count==0,$"finding audit index diagnostics round {i+1} should remain empty.");

        assert(round==100,$"Quality inspection finding audit index smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
