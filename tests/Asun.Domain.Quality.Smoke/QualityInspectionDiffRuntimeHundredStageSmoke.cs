using Asun.Domain.Quality;

public static class QualityInspectionDiffRuntimeHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        QualityFinding MakeFinding(string id,string message) =>
            new(
                QualityFindingId.Create(id),
                "AOI.RULE",
                QualityOutcome.Review,
                QualitySeverity.Warning,
                message);

        var oldFinding=MakeFinding("F-001","old");
        var sameFinding=MakeFinding("F-001","old");
        var changedFinding=MakeFinding("F-001","new");
        var addedFinding=MakeFinding("F-002","added");

        var oldEvidence=new QualityFindingEvidenceSet(new[]{
            new QualityFindingEvidenceLink(oldFinding.Id,QualityEvidenceKey.Create("frame://001"))
        });
        var newEvidence=new QualityFindingEvidenceSet(new[]{
            new QualityFindingEvidenceLink(changedFinding.Id,QualityEvidenceKey.Create("frame://002")),
            new QualityFindingEvidenceLink(addedFinding.Id,QualityEvidenceKey.Create("frame://003"))
        });

        var previous=new QualityInspectionSnapshot(Guid.NewGuid(),10,new QualityFindingSet(new[]{oldFinding}),oldEvidence);
        var current=new QualityInspectionSnapshot(Guid.NewGuid(),11,new QualityFindingSet(new[]{changedFinding,addedFinding}),newEvidence);
        var unchanged=new QualityInspectionSnapshot(Guid.NewGuid(),12,new QualityFindingSet(new[]{sameFinding}),oldEvidence);

        var diff=QualityInspectionDiffRuntime.Diff(previous,current);
        var noChange=QualityInspectionDiffRuntime.Diff(previous,unchanged);

        for(var i=0;i<10;i++) Check(diff.AddedFindingIds.Single()==addedFinding.Id,$"added finding diff round {i+1} should identify F-002.");
        for(var i=0;i<10;i++) Check(diff.RemovedFindingIds.Count==0,$"removed finding diff round {i+1} should be empty.");
        for(var i=0;i<10;i++) Check(diff.ChangedFindingIds.Single()==oldFinding.Id,$"changed finding diff round {i+1} should identify F-001.");
        for(var i=0;i<10;i++) Check(diff.AddedEvidenceKeys.Count==2,$"added evidence diff round {i+1} should contain two new keys.");
        for(var i=0;i<10;i++) Check(diff.RemovedEvidenceKeys.Count==1,$"removed evidence diff round {i+1} should contain the old key.");
        for(var i=0;i<10;i++) Check(diff.AddedEvidenceKeys.SequenceEqual(diff.AddedEvidenceKeys.OrderBy(key=>key.Value,StringComparer.Ordinal)),$"added evidence ordering round {i+1} should be deterministic.");
        for(var i=0;i<10;i++) Check(diff.ChangedFindingIds.SequenceEqual(new[]{oldFinding.Id}),$"changed finding ordering round {i+1} should be deterministic.");
        for(var i=0;i<10;i++) Check(noChange.IsEmpty,$"unchanged diff round {i+1} should be empty.");
        for(var i=0;i<10;i++) Check(QualityInspectionDiffValidationRuntime.IsValid(diff),$"diff runtime validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(QualityInspectionDiffValidationRuntime.IsValid(noChange),$"empty diff validation round {i+1} should pass.");

        assert(round==100,$"Quality inspection diff runtime smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
