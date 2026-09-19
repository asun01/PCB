using Asun.Domain.Quality;

public static class QualityInspectionSummaryDiffHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        var resultBase=new QualityInspectionResult(
            Guid.Parse("00000000-0000-0000-0000-000000000101"),
            new QualityInspectionSnapshot(
                Guid.Parse("10000000-0000-0000-0000-000000000101"),
                104,
                new QualityFindingSet(new[]{
                    new QualityFinding(
                        QualityFindingId.Create("F-001"),
                        "RULE",
                        QualityOutcome.Review,
                        QualitySeverity.Warning,
                        "base")
                }),
                new QualityFindingEvidenceSet(new[]{
                    new QualityFindingEvidenceLink(
                        QualityFindingId.Create("F-001"),
                        QualityEvidenceKey.Create("a"))
                })));
        var resultChanged=new QualityInspectionResult(
            Guid.Parse("00000000-0000-0000-0000-000000000102"),
            new QualityInspectionSnapshot(
                Guid.Parse("10000000-0000-0000-0000-000000000102"),
                105,
                new QualityFindingSet(new[]{
                    new QualityFinding(
                        QualityFindingId.Create("F-001"),
                        "RULE",
                        QualityOutcome.Fail,
                        QualitySeverity.Error,
                        "changed")
                }),
                new QualityFindingEvidenceSet(new[]{
                    new QualityFindingEvidenceLink(
                        QualityFindingId.Create("F-001"),
                        QualityEvidenceKey.Create("b"))
                })));

        var first=QualityInspectionSummaryRuntime.Create(resultBase);
        var second=QualityInspectionSummaryRuntime.Create(resultChanged);
        var same=QualityInspectionSummaryDiffRuntime.Diff(first,first);
        var delta=QualityInspectionSummaryDiffRuntime.Diff(first,second);

        for(var i=0;i<10;i++) Check(same.IsEmpty,$"summary self diff round {i+1} should be empty.");
        for(var i=0;i<10;i++) Check(delta.OutcomeCountsChanged,$"summary outcome diff round {i+1} should detect changed outcomes.");
        for(var i=0;i<10;i++) Check(delta.SeverityCountsChanged,$"summary severity diff round {i+1} should detect changed severities.");
        for(var i=0;i<10;i++) Check(delta.EvidenceCountsChanged,$"summary evidence diff round {i+1} should detect evidence change.");
        for(var i=0;i<10;i++) Check(delta.ContentFingerprintChanged,$"summary fingerprint diff round {i+1} should detect content change.");
        for(var i=0;i<10;i++) Check(QualityInspectionSummaryDiffValidationRuntime.IsValid(delta),$"summary diff validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(!delta.IsEmpty,$"summary non-empty diff round {i+1} should remain explicit.");
        for(var i=0;i<10;i++) Check(QualityInspectionSummaryDiffRuntime.Diff(first,second).Equals(delta),$"summary diff determinism round {i+1} should remain stable.");
        for(var i=0;i<10;i++) Check(first.ContentFingerprint!=second.ContentFingerprint,$"summary content mutation round {i+1} should remain visible.");
        for(var i=0;i<10;i++) Check(first.Evidence.DistinctEvidenceKeyCount!=second.Evidence.DistinctEvidenceKeyCount,$"summary evidence cardinality change round {i+1} should remain observable.");

        assert(round==100,$"Quality inspection summary diff smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
