using Asun.Domain.Quality;

public static class QualityInspectionResultValidationHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        var finding=new QualityFinding(
            QualityFindingId.Create("F-001"),
            "RULE",
            QualityOutcome.Review,
            QualitySeverity.Information,
            "result finding");
        var findings=new QualityFindingSet(new[]{finding});
        var evidence=new QualityFindingEvidenceSet(new[]{
            new QualityFindingEvidenceLink(
                finding.Id,
                QualityEvidenceKey.Create("frame://001"))
        });
        var snapshot=new QualityInspectionSnapshot(
            Guid.NewGuid(),
            7,
            findings,
            evidence);
        var result=new QualityInspectionResult(
            Guid.NewGuid(),
            snapshot);

        var orphanSnapshot=new QualityInspectionSnapshot(
            Guid.NewGuid(),
            8,
            findings,
            new QualityFindingEvidenceSet(new[]{
                new QualityFindingEvidenceLink(
                    QualityFindingId.Create("F-999"),
                    QualityEvidenceKey.Create("frame://999"))
            }));
        var invalidResult=new QualityInspectionResult(Guid.NewGuid(),orphanSnapshot);
        var resultFingerprint=QualityInspectionResultDeterminismRuntime
            .CreateContentFingerprint(result);
        var snapshotFingerprint=QualityInspectionDeterminismRuntime
            .CreateContentFingerprint(snapshot);

        for(var i=0;i<10;i++) Check(QualityInspectionResultValidationRuntime.IsValid(result),$"result validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(result.ResultId!=Guid.Empty,$"result identity round {i+1} should remain non-empty.");
        for(var i=0;i<10;i++) Check(result.SnapshotId==snapshot.SnapshotId,$"snapshot identity projection round {i+1} should remain stable.");
        for(var i=0;i<10;i++) Check(result.Sequence==7,$"result sequence round {i+1} should mirror the snapshot.");
        for(var i=0;i<10;i++) Check(result.Findings.Find(finding.Id)==finding,$"result finding access round {i+1} should remain stable.");
        for(var i=0;i<10;i++) Check(result.Evidence.ForFinding(finding.Id).Single().Value=="frame://001",$"result evidence access round {i+1} should remain stable.");
        for(var i=0;i<10;i++) Check(resultFingerprint==snapshotFingerprint,$"result fingerprint round {i+1} should delegate to canonical snapshot content.");
        for(var i=0;i<10;i++) Check(QualityInspectionResultDeterminismRuntime.AreContentEquivalent(result,result),$"result content equivalence round {i+1} should be deterministic.");
        for(var i=0;i<10;i++) Check(!QualityInspectionResultValidationRuntime.IsValid(invalidResult),$"invalid result snapshot round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(QualityInspectionResultDeterminismRuntime.CreateContentFingerprint(result).Length==64,$"result fingerprint format round {i+1} should remain SHA-256 sized.");

        assert(round==100,$"Quality inspection result smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
