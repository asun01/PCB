using Asun.Domain.Quality;

public static class QualityInspectionReplayProjectionDiffHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        var f1=new QualityFinding(
            QualityFindingId.Create("F-001"),
            "RULE",
            QualityOutcome.Review,
            QualitySeverity.Information,
            "first");
        var f2=new QualityFinding(
            QualityFindingId.Create("F-002"),
            "RULE",
            QualityOutcome.Review,
            QualitySeverity.Warning,
            "second");

        var previousSnapshot=new QualityInspectionSnapshot(
            Guid.NewGuid(),
            50,
            new QualityFindingSet(new[]{f1}),
            new QualityFindingEvidenceSet(new[]{
                new QualityFindingEvidenceLink(f1.Id,QualityEvidenceKey.Create("a"))
            }));
        var currentSnapshot=new QualityInspectionSnapshot(
            Guid.NewGuid(),
            51,
            new QualityFindingSet(new[]{f1,f2}),
            new QualityFindingEvidenceSet(new[]{
                new QualityFindingEvidenceLink(f1.Id,QualityEvidenceKey.Create("a")),
                new QualityFindingEvidenceLink(f2.Id,QualityEvidenceKey.Create("b"))
            }));
        var previous=QualityInspectionReplayProjectionRuntime.Create(
            new QualityInspectionResult(Guid.NewGuid(),previousSnapshot));
        var current=QualityInspectionReplayProjectionRuntime.Create(
            new QualityInspectionResult(Guid.NewGuid(),currentSnapshot));

        var same=QualityInspectionReplayProjectionDiffRuntime.Diff(previous,previous);
        var diff=QualityInspectionReplayProjectionDiffRuntime.Diff(previous,current);

        for(var i=0;i<10;i++) Check(same.IsEmpty,$"self projection diff round {i+1} should be empty.");
        for(var i=0;i<10;i++) Check(diff.AddedFindingIds.Single()==f2.Id,$"projection added finding round {i+1} should identify F-002.");
        for(var i=0;i<10;i++) Check(diff.RemovedFindingIds.Count==0,$"projection removed finding round {i+1} should be empty.");
        for(var i=0;i<10;i++) Check(diff.AddedEvidenceLinks.Single().FindingId==f2.Id,$"projection added evidence relationship round {i+1} should identify F-002.");
        for(var i=0;i<10;i++) Check(diff.RemovedEvidenceLinks.Count==0,$"projection removed evidence round {i+1} should be empty.");
        for(var i=0;i<10;i++) Check(diff.ContentFingerprintChanged,$"projection content fingerprint round {i+1} should detect content change.");
        for(var i=0;i<10;i++) Check(QualityInspectionReplayProjectionDiffRuntime.Diff(previous,current).Equals(diff),$"projection diff determinism round {i+1} should be stable.");
        for(var i=0;i<10;i++) Check(diff.AddedFindingIds.SequenceEqual(diff.AddedFindingIds.OrderBy(id=>id.Value,StringComparer.Ordinal)),$"projection finding ordering round {i+1} should be deterministic.");
        for(var i=0;i<10;i++) Check(diff.AddedEvidenceLinks.SequenceEqual(diff.AddedEvidenceLinks.OrderBy(link=>link.FindingId.Value,StringComparer.Ordinal).ThenBy(link=>link.EvidenceKey.Value,StringComparer.Ordinal)),$"projection evidence ordering round {i+1} should be deterministic.");
        for(var i=0;i<10;i++) Check(!diff.IsEmpty,$"non-empty projection diff round {i+1} should remain explicit.");

        assert(round==100,$"Quality inspection replay projection diff smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
