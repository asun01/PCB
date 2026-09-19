using Asun.Domain.Quality;

public static class QualityInspectionFindingAuditIndexFingerprintHundredStageSmoke
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
            "fingerprint");
        var result=new QualityInspectionResult(
            Guid.NewGuid(),
            new QualityInspectionSnapshot(
                Guid.NewGuid(),
                113,
                new QualityFindingSet(new[]{finding}),
                new QualityFindingEvidenceSet(new[]{
                    new QualityFindingEvidenceLink(
                        finding.Id,
                        QualityEvidenceKey.Create("a"))
                })));
        var index=QualityInspectionFindingAuditIndexRuntime.Create(result);
        var fingerprint=QualityInspectionFindingAuditIndexFingerprintRuntime.CreateFingerprint(index);
        var tampered=new QualityInspectionFindingAuditIndex(new[]{
            QualityInspectionFindingAuditRecordRuntime.Create(result,finding.Id) with
            {
                Message="changed"
            }
        });

        for(var i=0;i<10;i++) Check(QualityInspectionFindingAuditIndexFingerprintValidationRuntime.IsValidIndex(result,index),$"finding audit fingerprint validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(fingerprint.Length==64,$"finding audit fingerprint width round {i+1} should be SHA-256 sized.");
        for(var i=0;i<10;i++) Check(QualityInspectionFindingAuditIndexFingerprintValidationRuntime.IsValidFingerprint(fingerprint),$"finding audit fingerprint syntax round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(QualityInspectionFindingAuditIndexFingerprintRuntime.CreateFingerprint(index)==fingerprint,$"finding audit fingerprint determinism round {i+1} should remain stable.");
        for(var i=0;i<10;i++) Check(!QualityInspectionFindingAuditIndexFingerprintValidationRuntime.IsValidIndex(result,tampered),$"tampered finding audit index round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(fingerprint.All(character=>Uri.IsHexDigit(character) && char.ToLowerInvariant(character)==character),$"finding audit fingerprint casing round {i+1} should remain normalized.");
        for(var i=0;i<10;i++) Check(index.Find(finding.Id)?.Message=="fingerprint",$"finding audit source message round {i+1} should remain stable.");
        for(var i=0;i<10;i++) Check(index.Find(finding.Id)?.EvidenceKeys.Single().Value=="a",$"finding audit source evidence round {i+1} should remain stable.");
        for(var i=0;i<10;i++) Check(index.FindingIds.Single()==finding.Id,$"finding audit source identity round {i+1} should remain stable.");
        for(var i=0;i<10;i++) Check(QualityInspectionFindingAuditIndexFingerprintValidationRuntime.ValidateIndex(result,index).Count==0,$"finding audit diagnostics round {i+1} should remain empty.");

        assert(round==100,$"Quality inspection finding audit index fingerprint smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
