using Asun.Domain.Quality;

public static class QualityInspectionEvidenceDiffHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        QualityFinding Make(string id) =>
            new(
                QualityFindingId.Create(id),
                "RULE",
                QualityOutcome.Review,
                QualitySeverity.Information,
                id);

        var f1=Make("F-001");
        var f2=Make("F-002");
        var f3=Make("F-003");
        var keyA=QualityEvidenceKey.Create("frame://A");
        var keyB=QualityEvidenceKey.Create("frame://B");
        var keyC=QualityEvidenceKey.Create("frame://C");

        var previous=new QualityInspectionSnapshot(
            Guid.NewGuid(),
            1,
            new QualityFindingSet(new[]{f1,f2,f3}),
            new QualityFindingEvidenceSet(new[]{
                new QualityFindingEvidenceLink(f1.Id,keyA),
                new QualityFindingEvidenceLink(f2.Id,keyB)
            }));
        var current=new QualityInspectionSnapshot(
            Guid.NewGuid(),
            2,
            new QualityFindingSet(new[]{f1,f2,f3}),
            new QualityFindingEvidenceSet(new[]{
                new QualityFindingEvidenceLink(f1.Id,keyA),
                new QualityFindingEvidenceLink(f2.Id,keyC),
                new QualityFindingEvidenceLink(f3.Id,keyB)
            }));
        var unchanged=QualityInspectionEvidenceDiffRuntime.Diff(previous,previous);
        var delta=QualityInspectionEvidenceDiffRuntime.Diff(previous,current);
        var overlap=new QualityInspectionEvidenceDiff(
            new[]{new QualityFindingEvidenceLink(f1.Id,keyA)},
            new[]{new QualityFindingEvidenceLink(f1.Id,keyA)});
        var duplicate=new QualityInspectionEvidenceDiff(
            new[]{new QualityFindingEvidenceLink(f1.Id,keyA),new QualityFindingEvidenceLink(f1.Id,keyA)},
            Array.Empty<QualityFindingEvidenceLink>());

        for(var i=0;i<10;i++) Check(unchanged.IsEmpty,$"self evidence diff round {i+1} should be empty.");
        for(var i=0;i<10;i++) Check(delta.AddedLinks.Count==2,$"added link count round {i+1} should include C and the moved B link.");
        for(var i=0;i<10;i++) Check(delta.RemovedLinks.Count==1,$"removed link count round {i+1} should include the old B link.");
        for(var i=0;i<10;i++) Check(delta.AddedLinks.SequenceEqual(delta.AddedLinks.OrderBy(link=>link.FindingId.Value,StringComparer.Ordinal).ThenBy(link=>link.EvidenceKey.Value,StringComparer.Ordinal)),$"added link ordering round {i+1} should be deterministic.");
        for(var i=0;i<10;i++) Check(delta.RemovedLinks.Single().FindingId==f2.Id && delta.RemovedLinks.Single().EvidenceKey==keyB,$"removed relationship round {i+1} should identify the old binding.");
        for(var i=0;i<10;i++) Check(delta.AddedLinks.Any(link=>link.FindingId==f3.Id && link.EvidenceKey==keyB),$"moved relationship round {i+1} should identify the new binding.");
        for(var i=0;i<10;i++) Check(QualityInspectionEvidenceDiffValidationRuntime.IsValid(delta),$"evidence diff validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(!QualityInspectionEvidenceDiffValidationRuntime.IsValid(overlap),$"overlap evidence diff round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(!QualityInspectionEvidenceDiffValidationRuntime.IsValid(duplicate),$"duplicate evidence diff round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(!delta.IsEmpty && delta.AddedLinks.Select(link=>link.EvidenceKey).Contains(keyB),$"opaque-key relink coverage round {i+1} should remain observable at link level.");

        assert(round==100,$"Quality inspection evidence diff smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
