using Asun.Domain.Quality;

public static class QualityInspectionReplayWindowDiffHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        static QualityInspectionReplayEnvelope Create(
            string resultId,
            long sequence,
            string message)
        {
            var finding=new QualityFinding(
                QualityFindingId.Create("F-001"),
                "RULE",
                QualityOutcome.Review,
                QualitySeverity.Information,
                message);
            var result=new QualityInspectionResult(
                Guid.Parse(resultId),
                new QualityInspectionSnapshot(
                    Guid.NewGuid(),
                    sequence,
                    new QualityFindingSet(new[]{finding}),
                    new QualityFindingEvidenceSet(new[]{
                        new QualityFindingEvidenceLink(
                            finding.Id,
                            QualityEvidenceKey.Create($"frame://{sequence}"))
                    })));
            return QualityInspectionReplayEnvelopeRuntime.Create(
                QualityInspectionReplayBundleRuntime.Create(result));
        }

        var one=Create(
            "00000000-0000-0000-0000-000000000001",
            1,
            "one");
        var two=Create(
            "00000000-0000-0000-0000-000000000002",
            2,
            "two");
        var twoChanged=Create(
            "00000000-0000-0000-0000-000000000002",
            2,
            "changed");
        var three=Create(
            "00000000-0000-0000-0000-000000000003",
            3,
            "three");

        var previous=QualityInspectionReplayWindowRuntime.Create(
            new[]{one,two});
        var current=QualityInspectionReplayWindowRuntime.Create(
            new[]{twoChanged,three});
        var unchanged=QualityInspectionReplayWindowDiffRuntime.Diff(previous,previous);
        var delta=QualityInspectionReplayWindowDiffRuntime.Diff(previous,current);
        var invalid=new QualityInspectionReplayWindowDiff(
            new[]{Guid.Empty},
            Array.Empty<Guid>(),
            Array.Empty<Guid>());

        for(var i=0;i<10;i++) Check(unchanged.IsEmpty,$"self window diff round {i+1} should be empty.");
        for(var i=0;i<10;i++) Check(delta.AddedResultIds.Single()==three.Bundle.Current.ResultId,$"window added result round {i+1} should identify result three.");
        for(var i=0;i<10;i++) Check(delta.RemovedResultIds.Single()==one.Bundle.Current.ResultId,$"window removed result round {i+1} should identify result one.");
        for(var i=0;i<10;i++) Check(delta.ChangedResultIds.Single()==two.Bundle.Current.ResultId,$"window changed result round {i+1} should identify result two.");
        for(var i=0;i<10;i++) Check(QualityInspectionReplayWindowDiffValidationRuntime.IsValid(delta),$"window diff validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(!QualityInspectionReplayWindowDiffValidationRuntime.IsValid(invalid),$"invalid window diff round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(QualityInspectionReplayWindowDiffRuntime.Diff(previous,current).Equals(delta),$"window diff determinism round {i+1} should be stable.");
        for(var i=0;i<10;i++) Check(delta.AddedResultIds.SequenceEqual(delta.AddedResultIds.OrderBy(id=>id)),$"window added ordering round {i+1} should be deterministic.");
        for(var i=0;i<10;i++) Check(delta.RemovedResultIds.SequenceEqual(delta.RemovedResultIds.OrderBy(id=>id)),$"window removed ordering round {i+1} should be deterministic.");
        for(var i=0;i<10;i++) Check(delta.ChangedResultIds.SequenceEqual(delta.ChangedResultIds.OrderBy(id=>id)),$"window changed ordering round {i+1} should be deterministic.");

        assert(round==100,$"Quality inspection replay window diff smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
