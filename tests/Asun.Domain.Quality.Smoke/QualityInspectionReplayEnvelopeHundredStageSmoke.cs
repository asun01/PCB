using Asun.Domain.Quality;

public static class QualityInspectionReplayEnvelopeHundredStageSmoke
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
            "envelope");
        var result=new QualityInspectionResult(
            Guid.NewGuid(),
            new QualityInspectionSnapshot(
                Guid.NewGuid(),
                80,
                new QualityFindingSet(new[]{finding}),
                new QualityFindingEvidenceSet(new[]{
                    new QualityFindingEvidenceLink(
                        finding.Id,
                        QualityEvidenceKey.Create("frame://001"))
                })));
        var bundle=QualityInspectionReplayBundleRuntime.Create(result);
        var envelope=QualityInspectionReplayEnvelopeRuntime.Create(bundle);
        var tampered=envelope with
        {
            BundleFingerprint =
                envelope.BundleFingerprint.Replace(
                    '0',
                    '1')
        };
        var nullBundle=new QualityInspectionReplayEnvelope(
            null!,
            envelope.BundleFingerprint);

        for(var i=0;i<10;i++) Check(QualityInspectionReplayEnvelopeValidationRuntime.IsValid(envelope),$"envelope validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(envelope.BundleFingerprint.Length==64,$"envelope fingerprint width round {i+1} should be SHA-256 sized.");
        for(var i=0;i<10;i++) Check(envelope.BundleFingerprint==QualityInspectionReplayBundleFingerprintRuntime.CreateFingerprint(bundle),$"envelope fingerprint round {i+1} should match bundle content.");
        for(var i=0;i<10;i++) Check(!QualityInspectionReplayEnvelopeValidationRuntime.IsValid(tampered),$"tampered envelope round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(!QualityInspectionReplayEnvelopeValidationRuntime.IsValid(nullBundle),$"null bundle envelope round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(QualityInspectionReplayBundleValidationRuntime.IsValid(envelope.Bundle),$"envelope bundle round {i+1} should remain valid.");
        for(var i=0;i<10;i++) Check(QualityInspectionReplayBundleFingerprintValidationRuntime.IsValidFingerprint(envelope.BundleFingerprint),$"envelope fingerprint syntax round {i+1} should remain valid.");
        for(var i=0;i<10;i++) Check(envelope.Bundle.Current.Sequence==80,$"envelope sequence round {i+1} should remain stable.");
        for(var i=0;i<10;i++) Check(envelope.Bundle.Current.FindingIds.Single()==finding.Id,$"envelope finding identity round {i+1} should remain stable.");
        for(var i=0;i<10;i++) Check(envelope.Bundle.Current.EvidenceManifest.Count==1,$"envelope evidence count round {i+1} should remain stable.");

        assert(round==100,$"Quality inspection replay envelope smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
