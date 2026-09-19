using Asun.Domain.Quality;

public static class QualityInspectionReplayBundleFingerprintHundredStageSmoke
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
            "one");
        var f2=new QualityFinding(
            QualityFindingId.Create("F-002"),
            "RULE",
            QualityOutcome.Warning,
            QualitySeverity.Warning,
            "two");

        var previousResult=new QualityInspectionResult(
            Guid.NewGuid(),
            new QualityInspectionSnapshot(
                Guid.NewGuid(),
                70,
                new QualityFindingSet(new[]{f1}),
                new QualityFindingEvidenceSet(new[]{
                    new QualityFindingEvidenceLink(
                        f1.Id,
                        QualityEvidenceKey.Create("a"))
                })));
        var currentResult=new QualityInspectionResult(
            Guid.NewGuid(),
            new QualityInspectionSnapshot(
                Guid.NewGuid(),
                71,
                new QualityFindingSet(new[]{f1,f2}),
                new QualityFindingEvidenceSet(new[]{
                    new QualityFindingEvidenceLink(
                        f1.Id,
                        QualityEvidenceKey.Create("a")),
                    new QualityFindingEvidenceLink(
                        f2.Id,
                        QualityEvidenceKey.Create("b"))
                })));

        var first=QualityInspectionReplayBundleRuntime.Create(
            currentResult,
            previousResult);
        var second=QualityInspectionReplayBundleRuntime.Create(
            currentResult,
            previousResult);
        var fingerprint=QualityInspectionReplayBundleFingerprintRuntime
            .CreateFingerprint(first);
        var tampered=first with
        {
            Diff=first.Diff with { ContentFingerprintChanged=!first.Diff.ContentFingerprintChanged }
        };

        for(var i=0;i<10;i++) Check(QualityInspectionReplayBundleFingerprintValidationRuntime.IsValidBundle(first),$"bundle fingerprint validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(fingerprint.Length==64,$"bundle fingerprint width round {i+1} should be SHA-256 sized.");
        for(var i=0;i<10;i++) Check(fingerprint==QualityInspectionReplayBundleFingerprintRuntime.CreateFingerprint(second),$"bundle fingerprint determinism round {i+1} should be stable.");
        for(var i=0;i<10;i++) Check(QualityInspectionReplayBundleFingerprintRuntime.AreEquivalent(first,second),$"bundle fingerprint equivalence round {i+1} should remain true.");
        for(var i=0;i<10;i++) Check(!QualityInspectionReplayBundleFingerprintRuntime.AreEquivalent(first,tampered),$"tampered bundle fingerprint round {i+1} should be detected.");
        for(var i=0;i<10;i++) Check(QualityInspectionReplayBundleFingerprintValidationRuntime.IsValidFingerprint(fingerprint),$"bundle fingerprint syntax round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(fingerprint.All(character=>Uri.IsHexDigit(character) && char.ToLowerInvariant(character)==character),$"bundle fingerprint casing round {i+1} should remain normalized.");
        for(var i=0;i<10;i++) Check(first.Previous is not null && first.Current is not null,$"bundle projection presence round {i+1} should remain explicit.");
        for(var i=0;i<10;i++) Check(first.Diff.AddedFindingIds.Single()==f2.Id,$"bundle fingerprint source diff round {i+1} should remain stable.");
        for(var i=0;i<10;i++) Check(QualityInspectionReplayBundleValidationRuntime.IsValid(first),$"bundle source validation round {i+1} should remain valid.");

        assert(round==100,$"Quality inspection replay bundle fingerprint smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
