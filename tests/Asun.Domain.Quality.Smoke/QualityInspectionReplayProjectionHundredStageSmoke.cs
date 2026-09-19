using Asun.Domain.Quality;

public static class QualityInspectionReplayProjectionHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        var f2=new QualityFinding(
            QualityFindingId.Create("F-002"),
            "RULE",
            QualityOutcome.Review,
            QualitySeverity.Warning,
            "second");
        var f1=new QualityFinding(
            QualityFindingId.Create("F-001"),
            "RULE",
            QualityOutcome.Review,
            QualitySeverity.Information,
            "first");
        var snapshot=new QualityInspectionSnapshot(
            Guid.NewGuid(),
            40,
            new QualityFindingSet(new[]{f2,f1}),
            new QualityFindingEvidenceSet(new[]{
                new QualityFindingEvidenceLink(f2.Id,QualityEvidenceKey.Create("z")),
                new QualityFindingEvidenceLink(f1.Id,QualityEvidenceKey.Create("a"))
            }));
        var result=new QualityInspectionResult(Guid.NewGuid(),snapshot);
        var projection=QualityInspectionReplayProjectionRuntime.Create(result);
        var invalid=new QualityInspectionReplayProjection(
            projection.ResultId,
            projection.SnapshotId,
            projection.Sequence,
            new[]{QualityFindingId.Create("F-999")},
            projection.EvidenceManifest,
            projection.ContentFingerprint);

        for(var i=0;i<10;i++) Check(QualityInspectionReplayProjectionValidationRuntime.IsValid(projection),$"projection validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(projection.ResultId==result.ResultId,$"projection result identity round {i+1} should match.");
        for(var i=0;i<10;i++) Check(projection.SnapshotId==snapshot.SnapshotId,$"projection snapshot identity round {i+1} should match.");
        for(var i=0;i<10;i++) Check(projection.Sequence==40,$"projection sequence round {i+1} should match.");
        for(var i=0;i<10;i++) Check(projection.FindingIds.SequenceEqual(new[]{new QualityFindingId("F-001"),new QualityFindingId("F-002")}),$"projection finding ordering round {i+1} should be canonical.");
        for(var i=0;i<10;i++) Check(projection.EvidenceManifest.Count==2,$"projection evidence manifest round {i+1} should preserve link count.");
        for(var i=0;i<10;i++) Check(projection.ContentFingerprint==QualityInspectionResultDeterminismRuntime.CreateContentFingerprint(result),$"projection fingerprint round {i+1} should match canonical result content.");
        for(var i=0;i<10;i++) Check(QualityInspectionReplayProjectionValidationRuntime.Validate(projection).Count==0,$"projection diagnostics round {i+1} should remain empty.");
        for(var i=0;i<10;i++) Check(!QualityInspectionReplayProjectionValidationRuntime.IsValid(invalid),$"projection orphan finding round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(projection.FindingIds.Distinct().Count()==2,$"projection finding uniqueness round {i+1} should remain stable.");

        assert(round==100,$"Quality inspection replay projection smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
