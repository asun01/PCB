using Asun.Domain.Quality;

public static class QualityInspectionAuditEnvelopeHundredStageSmoke
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
                123,
                new QualityFindingSet(new[]{finding}),
                new QualityFindingEvidenceSet(new[]{
                    new QualityFindingEvidenceLink(
                        finding.Id,
                        QualityEvidenceKey.Create("a"))
                })));

        var envelope=QualityInspectionAuditEnvelopeRuntime.Create(result);
        var badFingerprint=envelope with {ProjectionFingerprint=new string('c',64)};
        var badSequence=envelope with {Sequence=envelope.Sequence+1};

        for(var i=0;i<10;i++) Check(QualityInspectionAuditEnvelopeValidationRuntime.IsValid(result,envelope),$"top-level envelope validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(envelope.ResultId==result.ResultId,$"top-level envelope result id round {i+1} should match.");
        for(var i=0;i<10;i++) Check(envelope.SnapshotId==result.SnapshotId,$"top-level envelope snapshot id round {i+1} should match.");
        for(var i=0;i<10;i++) Check(envelope.Sequence==123,$"top-level envelope sequence round {i+1} should match.");
        for(var i=0;i<10;i++) Check(envelope.Projection.RuleAudit.Summary.RuleCount==1,$"top-level envelope rule audit round {i+1} should remain one.");
        for(var i=0;i<10;i++) Check(envelope.Projection.FindingAudit.Index.Count==1,$"top-level envelope finding audit round {i+1} should remain one.");
        for(var i=0;i<10;i++) Check(envelope.ProjectionFingerprint.Length==64,$"top-level envelope fingerprint width round {i+1} should be SHA-256 sized.");
        for(var i=0;i<10;i++) Check(!QualityInspectionAuditEnvelopeValidationRuntime.IsValid(result,badFingerprint),$"top-level envelope fingerprint tamper round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(!QualityInspectionAuditEnvelopeValidationRuntime.IsValid(result,badSequence),$"top-level envelope sequence tamper round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(QualityInspectionAuditEnvelopeRuntime.Create(result)==envelope,$"top-level envelope determinism round {i+1} should remain stable.");

        assert(round==100,$"Quality inspection audit envelope smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
