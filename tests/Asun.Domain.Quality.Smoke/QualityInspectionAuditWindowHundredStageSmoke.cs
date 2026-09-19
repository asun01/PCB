using Asun.Domain.Quality;

public static class QualityInspectionAuditWindowHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        static QualityInspectionResult Create(Guid id,long sequence)
        {
            var finding=new QualityFinding(
                QualityFindingId.Create($"F-{sequence}"),
                "RULE",
                QualityOutcome.Review,
                QualitySeverity.Information,
                sequence.ToString());
            return new QualityInspectionResult(
                id,
                new QualityInspectionSnapshot(
                    Guid.NewGuid(),
                    sequence,
                    new QualityFindingSet(new[]{finding}),
                    new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>())));
        }

        var r1=Create(Guid.Parse("00000000-0000-0000-0000-000000000401"),1);
        var r2=Create(Guid.Parse("00000000-0000-0000-0000-000000000402"),2);
        var r3=Create(Guid.Parse("00000000-0000-0000-0000-000000000403"),3);
        var window=QualityInspectionAuditWindowRuntime.Create(new[]{r3,r1,r2});
        var partial=new QualityInspectionAuditWindow(window.Envelopes.Take(2));

        for(var i=0;i<10;i++) Check(QualityInspectionAuditWindowValidationRuntime.IsValid(new[]{r1,r2,r3},window),$"audit window validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(window.Count==3,$"audit window count round {i+1} should be three.");
        for(var i=0;i<10;i++) Check(window.Envelopes[0].Sequence==1,$"audit window first sequence round {i+1} should be one.");
        for(var i=0;i<10;i++) Check(window.Envelopes[1].Sequence==2,$"audit window second sequence round {i+1} should be two.");
        for(var i=0;i<10;i++) Check(window.Envelopes[2].Sequence==3,$"audit window third sequence round {i+1} should be three.");
        for(var i=0;i<10;i++) Check(!QualityInspectionAuditWindowValidationRuntime.IsValid(new[]{r1,r2,r3},partial),$"partial audit window round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(window.Envelopes.Select(item=>item.ResultId).Distinct().Count()==3,$"audit window result uniqueness round {i+1} should remain explicit.");
        for(var i=0;i<10;i++) Check(window.Envelopes.All(item=>item.Projection.FindingAudit.Index.Count==1),$"audit window finding coverage round {i+1} should remain explicit.");
        for(var i=0;i<10;i++) Check(QualityInspectionAuditWindowRuntime.Create(new[]{r1,r2,r3}).Envelopes.Select(item=>item.ResultId).SequenceEqual(window.Envelopes.Select(item=>item.ResultId)),$"audit window determinism round {i+1} should remain stable.");
        for(var i=0;i<10;i++) Check(QualityInspectionAuditWindowValidationRuntime.Validate(new[]{r1,r2,r3},window).Count==0,$"audit window diagnostics round {i+1} should remain empty.");

        assert(round==100,$"Quality inspection audit window smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
