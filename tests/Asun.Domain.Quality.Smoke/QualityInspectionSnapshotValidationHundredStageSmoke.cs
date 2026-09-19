using Asun.Domain.Quality;

public static class QualityInspectionSnapshotValidationHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        var finding = new QualityFinding(
            QualityFindingId.Create("F-001"),
            "AOI.CLEARANCE",
            QualityOutcome.Fail,
            QualitySeverity.Warning,
            "Clearance finding.");
        var findings = new QualityFindingSet(new[]{finding});
        var evidence = new QualityFindingEvidenceSet(new[]{
            new QualityFindingEvidenceLink(finding.Id, QualityEvidenceKey.Create("frame://001"))
        });
        var snapshot = new QualityInspectionSnapshot(Guid.NewGuid(), 0, findings, evidence);
        var invalid = new QualityInspectionSnapshot(
            snapshot.SnapshotId,
            1,
            findings,
            new QualityFindingEvidenceSet(new[]{
                new QualityFindingEvidenceLink(QualityFindingId.Create("F-999"), QualityEvidenceKey.Create("frame://999"))
            }));

        for(var i=0;i<10;i++) Check(QualityInspectionSnapshotValidationRuntime.IsValid(snapshot),$"snapshot validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(snapshot.Sequence==0,$"snapshot sequence round {i+1} should remain deterministic.");
        for(var i=0;i<10;i++) Check(snapshot.SnapshotId!=Guid.Empty,$"snapshot identity round {i+1} should remain non-empty.");
        for(var i=0;i<10;i++) Check(snapshot.Findings.Count==1,$"snapshot finding count round {i+1} should remain one.");
        for(var i=0;i<10;i++) Check(snapshot.Evidence.Count==1,$"snapshot evidence count round {i+1} should remain one.");
        for(var i=0;i<10;i++) Check(snapshot.Findings.Find(finding.Id)==finding,$"snapshot finding lookup round {i+1} should remain stable.");
        for(var i=0;i<10;i++) Check(snapshot.Evidence.ForFinding(finding.Id).Single().Value=="frame://001",$"snapshot evidence lookup round {i+1} should remain stable.");
        for(var i=0;i<10;i++) Check(!QualityInspectionSnapshotValidationRuntime.IsValid(invalid),$"orphan snapshot round {i+1} should be invalid.");
        for(var i=0;i<10;i++) Check(QualityInspectionSnapshotValidationRuntime.Validate(invalid).Count>0,$"invalid snapshot diagnostics round {i+1} should be non-empty.");
        for(var i=0;i<10;i++) Check(QualityFindingEvidenceValidationRuntime.IsValid(snapshot.Findings,snapshot.Evidence),$"snapshot evidence cross-reference round {i+1} should pass.");

        assert(round==100,$"Quality inspection snapshot smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
