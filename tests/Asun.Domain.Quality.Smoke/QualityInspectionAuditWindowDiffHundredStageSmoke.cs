using Asun.Domain.Quality;

public static class QualityInspectionAuditWindowDiffHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        static QualityInspectionResult Create(Guid id,long sequence,string message)
        {
            var finding=new QualityFinding(
                QualityFindingId.Create("F-001"),
                "RULE",
                QualityOutcome.Review,
                QualitySeverity.Information,
                message);
            return new QualityInspectionResult(
                id,
                new QualityInspectionSnapshot(
                    Guid.NewGuid(),
                    sequence,
                    new QualityFindingSet(new[]{finding}),
                    new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>())));
        }

        var r1=Create(Guid.Parse("00000000-0000-0000-0000-000000000501"),1,"one");
        var r2=Create(Guid.Parse("00000000-0000-0000-0000-000000000502"),2,"two");
        var r2Changed=Create(Guid.Parse("00000000-0000-0000-0000-000000000502"),2,"changed");
        var r3=Create(Guid.Parse("00000000-0000-0000-0000-000000000503"),3,"three");

        var previous=QualityInspectionAuditWindowRuntime.Create(new[]{r1,r2});
        var current=QualityInspectionAuditWindowRuntime.Create(new[]{r2Changed,r3});
        var same=QualityInspectionAuditWindowDiffRuntime.Diff(previous,previous);
        var diff=QualityInspectionAuditWindowDiffRuntime.Diff(previous,current);
        var invalid=new QualityInspectionAuditWindowDiff(new[]{Guid.Empty},Array.Empty<Guid>(),Array.Empty<Guid>());

        for(var i=0;i<10;i++) Check(same.IsEmpty,$"audit window self diff round {i+1} should be empty.");
        for(var i=0;i<10;i++) Check(diff.AddedResultIds.Single()==r3.ResultId,$"audit window added round {i+1} should identify result three.");
        for(var i=0;i<10;i++) Check(diff.RemovedResultIds.Single()==r1.ResultId,$"audit window removed round {i+1} should identify result one.");
        for(var i=0;i<10;i++) Check(diff.ChangedResultIds.Single()==r2.ResultId,$"audit window changed round {i+1} should identify result two.");
        for(var i=0;i<10;i++) Check(QualityInspectionAuditWindowDiffValidationRuntime.IsValid(diff),$"audit window diff validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(!QualityInspectionAuditWindowDiffValidationRuntime.IsValid(invalid),$"audit window invalid diff round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(diff.AddedResultIds.SequenceEqual(diff.AddedResultIds.OrderBy(id=>id)),$"audit window added ordering round {i+1} should be deterministic.");
        for(var i=0;i<10;i++) Check(diff.RemovedResultIds.SequenceEqual(diff.RemovedResultIds.OrderBy(id=>id)),$"audit window removed ordering round {i+1} should be deterministic.");
        for(var i=0;i<10;i++) Check(diff.ChangedResultIds.SequenceEqual(diff.ChangedResultIds.OrderBy(id=>id)),$"audit window changed ordering round {i+1} should be deterministic.");
        for(var i=0;i<10;i++) Check(QualityInspectionAuditWindowDiffRuntime.Diff(previous,current).Equals(diff),$"audit window diff determinism round {i+1} should remain stable.");

        assert(round==100,$"Quality inspection audit window diff smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
