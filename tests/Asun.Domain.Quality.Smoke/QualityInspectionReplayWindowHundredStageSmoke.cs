using Asun.Domain.Quality;

public static class QualityInspectionReplayWindowHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        static QualityInspectionReplayEnvelope CreateEnvelope(long sequence,string resultSuffix)
        {
            var finding=new QualityFinding(
                QualityFindingId.Create("F-001"),
                "RULE",
                QualityOutcome.Review,
                QualitySeverity.Information,
                resultSuffix);
            var result=new QualityInspectionResult(
                Guid.Parse($"00000000-0000-0000-0000-0000000000{resultSuffix[^1]}1"),
                new QualityInspectionSnapshot(
                    Guid.Parse($"10000000-0000-0000-0000-0000000000{resultSuffix[^1]}1"),
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

        var third=CreateEnvelope(3,"3");
        var first=CreateEnvelope(1,"1");
        var second=CreateEnvelope(2,"2");

        var window=QualityInspectionReplayWindowRuntime.Create(
            new[]{third,first,second});
        var duplicate=QualityInspectionReplayWindowRuntime.Create(
            new[]{first,first with {BundleFingerprint=first.BundleFingerprint}});

        for(var i=0;i<10;i++) Check(QualityInspectionReplayWindowValidationRuntime.IsValid(window),$"window validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(window.Count==3,$"window count round {i+1} should remain three.");
        for(var i=0;i<10;i++) Check(window.Envelopes[0].Bundle.Current.Sequence==1,$"window first sequence round {i+1} should be one.");
        for(var i=0;i<10;i++) Check(window.Envelopes[1].Bundle.Current.Sequence==2,$"window second sequence round {i+1} should be two.");
        for(var i=0;i<10;i++) Check(window.Envelopes[2].Bundle.Current.Sequence==3,$"window third sequence round {i+1} should be three.");
        for(var i=0;i<10;i++) Check(QualityInspectionReplayWindowValidationRuntime.Validate(window).Count==0,$"window diagnostics round {i+1} should remain empty.");
        for(var i=0;i<10;i++) Check(window.Envelopes.Select(item=>item.BundleFingerprint).Distinct(StringComparer.Ordinal).Count()==3,$"window fingerprint uniqueness round {i+1} should remain explicit.");
        for(var i=0;i<10;i++) Check(!QualityInspectionReplayWindowValidationRuntime.IsValid(duplicate),$"duplicate window round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(window.Envelopes.SequenceEqual(window.Envelopes.OrderBy(item=>item.Bundle.Current.Sequence).ThenBy(item=>item.Bundle.Current.SnapshotId).ThenBy(item=>item.Bundle.Current.ResultId)),$"window ordering round {i+1} should be canonical.");
        for(var i=0;i<10;i++) Check(window.Envelopes.All(item=>QualityInspectionReplayEnvelopeValidationRuntime.IsValid(item)),$"window member validation round {i+1} should remain valid.");

        assert(round==100,$"Quality inspection replay window smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
