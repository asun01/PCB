using Asun.Domain.Quality;

public static class QualityInspectionFindingAuditProjectionHundredStageSmoke
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
            "projection");
        var result=new QualityInspectionResult(
            Guid.NewGuid(),
            new QualityInspectionSnapshot(
                Guid.NewGuid(),
                114,
                new QualityFindingSet(new[]{finding}),
                new QualityFindingEvidenceSet(new[]{
                    new QualityFindingEvidenceLink(
                        finding.Id,
                        QualityEvidenceKey.Create("a"))
                })));

        var projection=QualityInspectionFindingAuditProjectionRuntime.Create(result);
        var bad=projection with
        {
            ContentFingerprint=new string('a',64)
        };
        var badIndex=new QualityInspectionFindingAuditIndex(Array.Empty<QualityInspectionFindingAuditRecord>());
        var badProjection=projection with {Index=badIndex};

        for(var i=0;i<10;i++) Check(QualityInspectionFindingAuditProjectionValidationRuntime.IsValid(result,projection),$"finding audit projection validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(projection.Index.Count==1,$"finding audit projection count round {i+1} should be one.");
        for(var i=0;i<10;i++) Check(projection.Index.Find(finding.Id)?.RuleCode=="RULE.A",$"finding audit projection RuleCode round {i+1} should remain stable.");
        for(var i=0;i<10;i++) Check(projection.Index.Find(finding.Id)?.EvidenceKeys.Single().Value=="a",$"finding audit projection evidence round {i+1} should remain stable.");
        for(var i=0;i<10;i++) Check(projection.ContentFingerprint.Length==64,$"finding audit projection fingerprint width round {i+1} should be SHA-256 sized.");
        for(var i=0;i<10;i++) Check(!QualityInspectionFindingAuditProjectionValidationRuntime.IsValid(result,bad),$"finding audit projection fingerprint tamper round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(!QualityInspectionFindingAuditProjectionValidationRuntime.IsValid(result,badProjection),$"finding audit projection index tamper round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(QualityInspectionFindingAuditProjectionRuntime.Create(result).ContentFingerprint==projection.ContentFingerprint,$"finding audit projection determinism round {i+1} should remain stable.");
        for(var i=0;i<10;i++) Check(projection.ContentFingerprint.All(character=>Uri.IsHexDigit(character) && char.ToLowerInvariant(character)==character),$"finding audit projection fingerprint casing round {i+1} should remain normalized.");
        for(var i=0;i<10;i++) Check(QualityInspectionFindingAuditProjectionValidationRuntime.Validate(result,projection).Count==0,$"finding audit projection diagnostics round {i+1} should remain empty.");

        assert(round==100,$"Quality inspection finding audit projection smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
