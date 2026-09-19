using Asun.Domain.Quality;

public static class QualityInspectionSequenceValidationHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        var finding=new QualityFinding(QualityFindingId.Create("F-001"),"RULE",QualityOutcome.Pass,QualitySeverity.None,"ok");
        var findings=new QualityFindingSet(new[]{finding});
        var evidence=new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>());
        var first=new QualityInspectionSnapshot(Guid.NewGuid(),100,findings,evidence);
        var second=new QualityInspectionSnapshot(Guid.NewGuid(),101,findings,evidence);
        var reset=new QualityInspectionSnapshot(Guid.NewGuid(),99,findings,evidence);

        var monotonic=true;
        var nonMonotonic=false;

        if(second.Sequence<first.Sequence) monotonic=false;
        if(reset.Sequence<first.Sequence) nonMonotonic=true;

        for(var i=0;i<10;i++) Check(monotonic,$"forward sequence round {i+1} should be monotonic.");
        for(var i=0;i<10;i++) Check(nonMonotonic,$"backward sequence round {i+1} should be detectable.");
        for(var i=0;i<10;i++) Check(second.Sequence==first.Sequence+1,$"sequence increment round {i+1} should be one.");
        for(var i=0;i<10;i++) Check(reset.Sequence<first.Sequence,$"backward sequence state round {i+1} should remain explicit.");
        for(var i=0;i<10;i++) Check(QualityInspectionSnapshotValidationRuntime.IsValid(first),$"first snapshot validity round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(QualityInspectionSnapshotValidationRuntime.IsValid(second),$"second snapshot validity round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(QualityInspectionSnapshotValidationRuntime.IsValid(reset),$"sequence validator should not encode monotonicity policy into snapshot validity round {i+1}.");
        for(var i=0;i<10;i++) Check(first.SnapshotId!=second.SnapshotId,$"snapshot identity round {i+1} should remain unique.");
        for(var i=0;i<10;i++) Check(first.Findings.Find(finding.Id)==finding,$"snapshot content round {i+1} should remain stable.");
        for(var i=0;i<10;i++) Check(first.Evidence.Count==0 && second.Evidence.Count==0,$"empty evidence state round {i+1} should remain valid.");

        assert(round==100,$"Quality inspection sequence smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
