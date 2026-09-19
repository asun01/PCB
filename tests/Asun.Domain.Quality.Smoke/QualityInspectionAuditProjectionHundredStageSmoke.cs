using Asun.Domain.Quality;

public static class QualityInspectionAuditProjectionHundredStageSmoke
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
            "audit projection");
        var result=new QualityInspectionResult(
            Guid.NewGuid(),
            new QualityInspectionSnapshot(
                Guid.NewGuid(),
                121,
                new QualityFindingSet(new[]{finding}),
                new QualityFindingEvidenceSet(new[]{
                    new QualityFindingEvidenceLink(
                        finding.Id,
                        QualityEvidenceKey.Create("a"))
                })));

        var projection=QualityInspectionAuditProjectionRuntime.Create(result);
        var badRecord=projection with
        {
            Record=projection.Record with {Sequence=projection.Record.Sequence+1}
        };
        var badSummary=projection with
        {
            Summary=projection.Summary with {ResultId=Guid.NewGuid()}
        };

        for(var i=0;i<10;i++) Check(QualityInspectionAuditProjectionValidationRuntime.IsValid(result,projection),$"top-level audit projection validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(projection.Record.ResultId==result.ResultId,$"top-level audit record identity round {i+1} should match.");
        for(var i=0;i<10;i++) Check(projection.Summary.ResultId==result.ResultId,$"top-level summary identity round {i+1} should match.");
        for(var i=0;i<10;i++) Check(projection.RuleAudit.Summary.RuleCount==1,$"top-level rule audit round {i+1} should expose one rule.");
        for(var i=0;i<10;i++) Check(projection.FindingAudit.Index.Count==1,$"top-level finding audit round {i+1} should expose one finding.");
        for(var i=0;i<10;i++) Check(projection.Record.ContentFingerprint==projection.Summary.ContentFingerprint,$"top-level content fingerprint round {i+1} should align.");
        for(var i=0;i<10;i++) Check(!QualityInspectionAuditProjectionValidationRuntime.IsValid(result,badRecord),$"top-level record tamper round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(!QualityInspectionAuditProjectionValidationRuntime.IsValid(result,badSummary),$"top-level summary tamper round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(QualityInspectionAuditProjectionRuntime.Create(result).FindingAudit.ContentFingerprint==projection.FindingAudit.ContentFingerprint,$"top-level finding audit determinism round {i+1} should remain stable.");
        for(var i=0;i<10;i++) Check(QualityInspectionAuditProjectionValidationRuntime.Validate(result,projection).Count==0,$"top-level audit diagnostics round {i+1} should remain empty.");

        assert(round==100,$"Quality inspection audit projection smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
