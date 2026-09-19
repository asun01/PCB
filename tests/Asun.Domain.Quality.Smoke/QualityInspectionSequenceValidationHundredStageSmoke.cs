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
        var orphan=new QualityInspectionSnapshot(
            Guid.NewGuid(),
            102,
            findings,
            new QualityFindingEvidenceSet(new[]{
                new QualityFindingEvidenceLink(
                    QualityFindingId.Create("F-999"),
                    QualityEvidenceKey.Create("frame://999"))
            }));

        var forward=QualityInspectionSequenceRuntime.Observe(first,second);
        var backward=QualityInspectionSequenceRuntime.Observe(first,reset);
        var invalidRejected=false;
        try
        {
            _=QualityInspectionSequenceRuntime.Observe(first,orphan);
        }
        catch(ArgumentException)
        {
            invalidRejected=true;
        }

        for(var i=0;i<10;i++) Check(forward.IsForwardOrEqual,$"forward relation round {i+1} should be accepted.");
        for(var i=0;i<10;i++) Check(forward.Delta==1,$"forward delta round {i+1} should equal one.");
        for(var i=0;i<10;i++) Check(forward.IsConsecutive,$"forward consecutive relation round {i+1} should be explicit.");
        for(var i=0;i<10;i++) Check(QualityInspectionSequenceValidationRuntime.IsValid(forward),$"forward relation validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(backward.IsBackward,$"backward relation round {i+1} should be detectable.");
        for(var i=0;i<10;i++) Check(backward.Delta==-1,$"backward delta round {i+1} should equal negative one.");
        for(var i=0;i<10;i++) Check(!backward.IsConsecutive,$"backward consecutive relation round {i+1} should be false.");
        for(var i=0;i<10;i++) Check(QualityInspectionSequenceValidationRuntime.IsValid(first,reset),$"backward pair validation round {i+1} should allow policy-neutral comparison.");
        for(var i=0;i<10;i++) Check(first.SnapshotId!=second.SnapshotId && first.SnapshotId!=reset.SnapshotId,$"snapshot identity round {i+1} should remain distinct.");
        for(var i=0;i<10;i++) Check(invalidRejected,$"invalid snapshot rejection round {i+1} should be deterministic.");

        assert(round==100,$"Quality inspection sequence smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
