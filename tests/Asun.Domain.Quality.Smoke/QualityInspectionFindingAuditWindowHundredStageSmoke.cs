using Asun.Domain.Quality;

public static class QualityInspectionFindingAuditWindowHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        static QualityInspectionResult Create(
            Guid resultId,
            long sequence,
            string findingId)
        {
            var finding=new QualityFinding(
                QualityFindingId.Create(findingId),
                "RULE",
                QualityOutcome.Review,
                QualitySeverity.Information,
                findingId);
            return new QualityInspectionResult(
                resultId,
                new QualityInspectionSnapshot(
                    Guid.NewGuid(),
                    sequence,
                    new QualityFindingSet(new[]{finding}),
                    new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>())));
        }

        var r1=Create(
            Guid.Parse("00000000-0000-0000-0000-000000000117"),
            1,
            "F-001");
        var r2=Create(
            Guid.Parse("00000000-0000-0000-0000-000000000118"),
            2,
            "F-002");
        var r3=Create(
            Guid.Parse("00000000-0000-0000-0000-000000000119"),
            3,
            "F-003");

        var window=QualityInspectionFindingAuditWindowRuntime.Create(
            new[]{r3,r1,r2});
        var partial=new QualityInspectionFindingAuditWindow(
            window.Envelopes.Take(2));

        for(var i=0;i<10;i++) Check(QualityInspectionFindingAuditWindowValidationRuntime.IsValid(new[]{r1,r2,r3},window),$"finding audit window validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(window.Count==3,$"finding audit window count round {i+1} should be three.");
        for(var i=0;i<10;i++) Check(window.Envelopes[0].Sequence==1,$"finding audit window ordering round {i+1} should start at sequence one.");
        for(var i=0;i<10;i++) Check(window.Envelopes[1].Sequence==2,$"finding audit window ordering round {i+1} should continue at sequence two.");
        for(var i=0;i<10;i++) Check(window.Envelopes[2].Sequence==3,$"finding audit window ordering round {i+1} should end at sequence three.");
        for(var i=0;i<10;i++) Check(!QualityInspectionFindingAuditWindowValidationRuntime.IsValid(new[]{r1,r2,r3},partial),$"partial finding audit window round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(window.Envelopes.Select(item=>item.ResultId).Distinct().Count()==3,$"finding audit window result uniqueness round {i+1} should remain explicit.");
        for(var i=0;i<10;i++) Check(window.Envelopes.All(item=>item.Projection.Index.Count==1),$"finding audit window member projection round {i+1} should remain valid.");
        for(var i=0;i<10;i++) Check(QualityInspectionFindingAuditWindowRuntime.Create(new[]{r1,r2,r3}).Envelopes.Select(item=>item.ResultId).SequenceEqual(window.Envelopes.Select(item=>item.ResultId)),$"finding audit window determinism round {i+1} should remain stable.");
        for(var i=0;i<10;i++) Check(QualityInspectionFindingAuditWindowValidationRuntime.Validate(new[]{r1,r2,r3},window).Count==0,$"finding audit window diagnostics round {i+1} should remain empty.");

        assert(round==100,$"Quality inspection finding audit window smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
