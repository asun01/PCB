using Asun.Domain.Quality;

public static class QualityInspectionFindingAuditWindowDiffHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        static QualityInspectionFindingAuditEnvelope Envelope(
            string resultId,
            string message)
        {
            var result=new QualityInspectionResult(
                Guid.Parse(resultId),
                new QualityInspectionSnapshot(
                    Guid.NewGuid(),
                    1,
                    new QualityFindingSet(new[]{
                        new QualityFinding(
                            QualityFindingId.Create("F-001"),
                            "RULE",
                            QualityOutcome.Review,
                            QualitySeverity.Information,
                            message)
                    }),
                    new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>())));
            return QualityInspectionFindingAuditEnvelopeRuntime.Create(result);
        }

        var one=Envelope(
            "00000000-0000-0000-0000-000000000201",
            "one");
        var two=Envelope(
            "00000000-0000-0000-0000-000000000202",
            "two");
        var twoChanged=Envelope(
            "00000000-0000-0000-0000-000000000202",
            "changed");
        var three=Envelope(
            "00000000-0000-0000-0000-000000000203",
            "three");

        var previous=new QualityInspectionFindingAuditWindow(new[]{one,two});
        var current=new QualityInspectionFindingAuditWindow(new[]{twoChanged,three});
        var same=QualityInspectionFindingAuditWindowDiffRuntime.Diff(previous,previous);
        var diff=QualityInspectionFindingAuditWindowDiffRuntime.Diff(previous,current);
        var invalid=new QualityInspectionFindingAuditWindowDiff(
            new[]{Guid.Empty},
            Array.Empty<Guid>(),
            Array.Empty<Guid>());

        for(var i=0;i<10;i++) Check(same.IsEmpty,$"finding audit window self diff round {i+1} should be empty.");
        for(var i=0;i<10;i++) Check(diff.AddedResultIds.Single()==three.ResultId,$"finding audit window added round {i+1} should identify result three.");
        for(var i=0;i<10;i++) Check(diff.RemovedResultIds.Single()==one.ResultId,$"finding audit window removed round {i+1} should identify result one.");
        for(var i=0;i<10;i++) Check(diff.ChangedResultIds.Single()==two.ResultId,$"finding audit window changed round {i+1} should identify result two.");
        for(var i=0;i<10;i++) Check(QualityInspectionFindingAuditWindowDiffValidationRuntime.IsValid(diff),$"finding audit window diff validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(!QualityInspectionFindingAuditWindowDiffValidationRuntime.IsValid(invalid),$"invalid finding audit window diff round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(diff.AddedResultIds.SequenceEqual(diff.AddedResultIds.OrderBy(id=>id)),$"finding audit window added ordering round {i+1} should be deterministic.");
        for(var i=0;i<10;i++) Check(diff.RemovedResultIds.SequenceEqual(diff.RemovedResultIds.OrderBy(id=>id)),$"finding audit window removed ordering round {i+1} should be deterministic.");
        for(var i=0;i<10;i++) Check(diff.ChangedResultIds.SequenceEqual(diff.ChangedResultIds.OrderBy(id=>id)),$"finding audit window changed ordering round {i+1} should be deterministic.");
        for(var i=0;i<10;i++) Check(QualityInspectionFindingAuditWindowDiffRuntime.Diff(previous,current).Equals(diff),$"finding audit window diff determinism round {i+1} should remain stable.");

        assert(round==100,$"Quality inspection finding audit window diff smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
