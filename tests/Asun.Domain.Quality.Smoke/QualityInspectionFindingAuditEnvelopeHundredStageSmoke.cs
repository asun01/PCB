using Asun.Domain.Quality;

public static class QualityInspectionFindingAuditEnvelopeHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        var finding=new QualityFinding(
            QualityFindingId.Create("F-001"),
            "RULE.A",
            QualityOutcome.Review,
            QualitySeverity.Warning,
            "envelope");
        var result=new QualityInspectionResult(
            Guid.NewGuid(),
            new QualityInspectionSnapshot(
                Guid.NewGuid(),
                116,
                new QualityFindingSet(new[]{finding}),
                new QualityFindingEvidenceSet(new[]{
                    new QualityFindingEvidenceLink(
                        finding.Id,
                        QualityEvidenceKey.Create("a"))
                })));

        var envelope=QualityInspectionFindingAuditEnvelopeRuntime.Create(result);
        var badFingerprint=envelope with
        {
            ProjectionFingerprint=new string('b',64)
        };
        var badSequence=envelope with
        {
            Sequence=envelope.Sequence+1
        };

        for(var i=0;i<10;i++) Check(QualityInspectionFindingAuditEnvelopeValidationRuntime.IsValid(result,envelope),$"finding audit envelope validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(envelope.ResultId==result.ResultId,$"finding audit envelope result identity round {i+1} should match.");
        for(var i=0;i<10;i++) Check(envelope.SnapshotId==result.SnapshotId,$"finding audit envelope snapshot identity round {i+1} should match.");
        for(var i=0;i<10;i++) Check(envelope.Sequence==116,$"finding audit envelope sequence round {i+1} should match.");
        for(var i=0;i<10;i++) Check(envelope.Projection.Index.Count==1,$"finding audit envelope index count round {i+1} should be one.");
        for(var i=0;i<10;i++) Check(envelope.ProjectionFingerprint.Length==64,$"finding audit envelope fingerprint width round {i+1} should be SHA-256 sized.");
        for(var i=0;i<10;i++) Check(!QualityInspectionFindingAuditEnvelopeValidationRuntime.IsValid(result,badFingerprint),$"finding audit envelope fingerprint tamper round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(!QualityInspectionFindingAuditEnvelopeValidationRuntime.IsValid(result,badSequence),$"finding audit envelope sequence tamper round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(envelope.ProjectionFingerprint==envelope.Projection.ContentFingerprint,$"finding audit envelope fingerprint linkage round {i+1} should remain exact.");
        for(var i=0;i<10;i++) Check(QualityInspectionFindingAuditEnvelopeRuntime.Create(result)==envelope,$"finding audit envelope determinism round {i+1} should remain stable.");

        assert(round==100,$"Quality inspection finding audit envelope smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
