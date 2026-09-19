using Asun.Domain.Quality;

public static class QualityInspectionAuditProjectionFingerprintHundredStageSmoke
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
                122,
                new QualityFindingSet(new[]{finding}),
                new QualityFindingEvidenceSet(new[]{
                    new QualityFindingEvidenceLink(
                        finding.Id,
                        QualityEvidenceKey.Create("a"))
                })));
        var projection=QualityInspectionAuditProjectionRuntime.Create(result);
        var fingerprint=QualityInspectionAuditProjectionFingerprintRuntime.CreateFingerprint(projection);
        var bad=new string('a',64);

        for(var i=0;i<10;i++) Check(QualityInspectionAuditProjectionFingerprintValidationRuntime.IsValidProjection(result,projection,fingerprint),$"top-level fingerprint validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(fingerprint.Length==64,$"top-level fingerprint width round {i+1} should be SHA-256 sized.");
        for(var i=0;i<10;i++) Check(QualityInspectionAuditProjectionFingerprintRuntime.CreateFingerprint(projection)==fingerprint,$"top-level fingerprint determinism round {i+1} should remain stable.");
        for(var i=0;i<10;i++) Check(QualityInspectionAuditProjectionFingerprintValidationRuntime.ValidateFingerprint(fingerprint).Count==0,$"top-level fingerprint syntax round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(!QualityInspectionAuditProjectionFingerprintValidationRuntime.IsValidProjection(result,projection,bad),$"top-level fingerprint tamper round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(fingerprint.All(character=>Uri.IsHexDigit(character) && char.ToLowerInvariant(character)==character),$"top-level fingerprint casing round {i+1} should remain normalized.");
        for(var i=0;i<10;i++) Check(projection.Record.ContentFingerprint.Length==64,$"top-level source record fingerprint round {i+1} should remain valid.");
        for(var i=0;i<10;i++) Check(projection.Summary.ContentFingerprint.Length==64,$"top-level source summary fingerprint round {i+1} should remain valid.");
        for(var i=0;i<10;i++) Check(projection.RuleAudit.Summary.RuleCount==1,$"top-level source rule audit round {i+1} should remain stable.");
        for(var i=0;i<10;i++) Check(projection.FindingAudit.Index.Count==1,$"top-level source finding audit round {i+1} should remain stable.");

        assert(round==100,$"Quality inspection audit projection fingerprint smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
