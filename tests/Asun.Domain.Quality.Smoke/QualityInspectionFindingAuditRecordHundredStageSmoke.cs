using Asun.Domain.Quality;

public static class QualityInspectionFindingAuditRecordHundredStageSmoke
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
            "finding audit");
        var result=new QualityInspectionResult(
            Guid.NewGuid(),
            new QualityInspectionSnapshot(
                Guid.NewGuid(),
                110,
                new QualityFindingSet(new[]{finding}),
                new QualityFindingEvidenceSet(new[]{
                    new QualityFindingEvidenceLink(
                        finding.Id,
                        QualityEvidenceKey.Create("z")),
                    new QualityFindingEvidenceLink(
                        finding.Id,
                        QualityEvidenceKey.Create("a"))
                })));

        var record=QualityInspectionFindingAuditRecordRuntime.Create(
            result,
            finding.Id);
        var bad=record with {RuleCode="RULE.BAD"};
        var missing=record with
        {
            FindingId=QualityFindingId.Create("F-999")
        };

        for(var i=0;i<10;i++) Check(QualityInspectionFindingAuditRecordValidationRuntime.IsValid(result,record),$"finding audit validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(record.FindingId==finding.Id,$"finding audit identity round {i+1} should match.");
        for(var i=0;i<10;i++) Check(record.RuleCode=="RULE.A",$"finding audit RuleCode round {i+1} should match.");
        for(var i=0;i<10;i++) Check(record.Outcome==QualityOutcome.Review && record.Severity==QualitySeverity.Warning,$"finding audit classification round {i+1} should match.");
        for(var i=0;i<10;i++) Check(record.EvidenceKeys.SequenceEqual(new[]{QualityEvidenceKey.Create("a"),QualityEvidenceKey.Create("z")}),$"finding audit evidence ordering round {i+1} should be canonical.");
        for(var i=0;i<10;i++) Check(!QualityInspectionFindingAuditRecordValidationRuntime.IsValid(result,bad),$"tampered finding RuleCode round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(!QualityInspectionFindingAuditRecordValidationRuntime.IsValid(result,missing),$"unknown finding audit id round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(record.EvidenceKeys.Distinct().Count()==2,$"finding audit evidence uniqueness round {i+1} should remain explicit.");
        for(var i=0;i<10;i++) Check(record.EvidenceKeys.SequenceEqual(record.EvidenceKeys.OrderBy(key=>key.Value,StringComparer.Ordinal)),$"finding audit evidence sort round {i+1} should remain deterministic.");
        for(var i=0;i<10;i++) Check(QualityInspectionFindingAuditRecordRuntime.Create(result,finding.Id)==record,$"finding audit determinism round {i+1} should remain stable.");

        assert(round==100,$"Quality inspection finding audit record smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
