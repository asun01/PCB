using Asun.Domain.Quality;

public static class QualityInspectionRuleAuditProjectionFingerprintHundredStageSmoke
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
                109,
                new QualityFindingSet(new[]{finding}),
                new QualityFindingEvidenceSet(new[]{
                    new QualityFindingEvidenceLink(
                        finding.Id,
                        QualityEvidenceKey.Create("a"))
                })));

        var projection=QualityInspectionRuleAuditProjectionRuntime.Create(result);
        var fingerprint=QualityInspectionRuleAuditProjectionFingerprintRuntime.CreateFingerprint(projection);
        var secondProjection=QualityInspectionRuleAuditProjectionRuntime.Create(result);
        var tampered=new QualityInspectionRuleAuditProjection(
            new QualityInspectionRuleSummary(new[]{
                new QualityInspectionRuleSummaryEntry("RULE.A",2,1)
            }),
            projection.FindingIndex,
            projection.EvidenceIndex);

        for(var i=0;i<10;i++) Check(QualityInspectionRuleAuditProjectionFingerprintValidationRuntime.IsValidProjection(result,projection),$"rule audit fingerprint validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(fingerprint.Length==64,$"rule audit fingerprint width round {i+1} should be SHA-256 sized.");
        for(var i=0;i<10;i++) Check(QualityInspectionRuleAuditProjectionFingerprintRuntime.CreateFingerprint(secondProjection)==fingerprint,$"rule audit fingerprint determinism round {i+1} should remain stable.");
        for(var i=0;i<10;i++) Check(QualityInspectionRuleAuditProjectionFingerprintValidationRuntime.IsValidFingerprint(fingerprint),$"rule audit fingerprint syntax round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(!QualityInspectionRuleAuditProjectionFingerprintValidationRuntime.IsValidProjection(result,tampered),$"tampered rule audit projection round {i+1} should be rejected by projection validation.");
        for(var i=0;i<10;i++) Check(fingerprint.All(character=>Uri.IsHexDigit(character) && char.ToLowerInvariant(character)==character),$"rule audit fingerprint casing round {i+1} should remain normalized.");
        for(var i=0;i<10;i++) Check(projection.Summary.RuleCount==1,$"rule audit source summary round {i+1} should remain one.");
        for(var i=0;i<10;i++) Check(projection.FindingIndex.FindingsFor("RULE.A").Single()==finding.Id,$"rule audit source finding round {i+1} should remain stable.");
        for(var i=0;i<10;i++) Check(projection.EvidenceIndex.EvidenceFor("RULE.A").Single().Value=="a",$"rule audit source evidence round {i+1} should remain stable.");
        for(var i=0;i<10;i++) Check(QualityInspectionRuleAuditProjectionFingerprintRuntime.CreateFingerprint(projection)==fingerprint,$"rule audit repeated fingerprint round {i+1} should remain deterministic.");

        assert(round==100,$"Quality inspection rule audit projection fingerprint smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
