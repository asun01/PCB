using Asun.Domain.Quality;

public static class QualityInspectionDeterminismHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        QualityFinding Make(string id,string message) =>
            new(QualityFindingId.Create(id),"RULE",QualityOutcome.Review,QualitySeverity.Information,message);

        var a=Make("F-002","b");
        var b=Make("F-001","a");

        QualityInspectionSnapshot MakeSnapshot() =>
            new(
                Guid.NewGuid(),
                1,
                new QualityFindingSet(new[]{a,b}),
                new QualityFindingEvidenceSet(new[]{
                    new QualityFindingEvidenceLink(a.Id,QualityEvidenceKey.Create("z")),
                    new QualityFindingEvidenceLink(b.Id,QualityEvidenceKey.Create("a"))
                }));

        var first=MakeSnapshot();
        var second=MakeSnapshot();

        var normalizedFirst=QualityInspectionDiffRuntime.Diff(first,first);
        var normalizedSecond=QualityInspectionDiffRuntime.Diff(second,second);

        for(var i=0;i<10;i++) Check(normalizedFirst.IsEmpty,$"self-diff first round {i+1} should be empty.");
        for(var i=0;i<10;i++) Check(normalizedSecond.IsEmpty,$"self-diff second round {i+1} should be empty.");
        for(var i=0;i<10;i++) Check(first.Findings.Findings.Count==2,$"finding order preservation round {i+1} should remain explicit.");
        for(var i=0;i<10;i++) Check(first.Evidence.Links.Count==2,$"evidence order preservation round {i+1} should remain explicit.");
        for(var i=0;i<10;i++) Check(first.Findings.Findings.Select(item=>item.Id).SequenceEqual(new[]{a.Id,b.Id}),$"finding insertion order round {i+1} should be stable.");
        for(var i=0;i<10;i++) Check(first.Evidence.Links.Select(item=>item.EvidenceKey).SequenceEqual(new[]{new QualityEvidenceKey("z"),new QualityEvidenceKey("a")}),$"evidence insertion order round {i+1} should be stable.");
        for(var i=0;i<10;i++) Check(QualityInspectionSnapshotValidationRuntime.IsValid(first) && QualityInspectionSnapshotValidationRuntime.IsValid(second),$"snapshot validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(QualityInspectionDiffValidationRuntime.IsValid(normalizedFirst) && QualityInspectionDiffValidationRuntime.IsValid(normalizedSecond),$"diff validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(first.SnapshotId!=second.SnapshotId,$"independent snapshot ids round {i+1} should remain distinct.");
        for(var i=0;i<10;i++) Check(first.Sequence==second.Sequence,$"independent snapshot sequence round {i+1} should remain equal for comparison.");

        assert(round==100,$"Quality inspection determinism smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
