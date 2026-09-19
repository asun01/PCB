using Asun.Domain.Quality;

public static class QualityInspectionAuditDiffHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        var baseRecord=new QualityInspectionAuditRecord(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Guid.Parse("22222222-2222-2222-2222-222222222222"),
            10,
            2,
            3,
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
        var sequenceChanged=baseRecord with {Sequence=11};
        var resultChanged=baseRecord with
        {
            ResultId=Guid.Parse("33333333-3333-3333-3333-333333333333")
        };
        var snapshotChanged=baseRecord with
        {
            SnapshotId=Guid.Parse("44444444-4444-4444-4444-444444444444")
        };
        var findingCountChanged=baseRecord with {FindingCount=3};
        var evidenceCountChanged=baseRecord with {EvidenceLinkCount=4};
        var fingerprintChanged=baseRecord with
        {
            ContentFingerprint="bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb"
        };

        var unchanged=QualityInspectionAuditDiffRuntime.Diff(baseRecord,baseRecord);
        var sequenceDiff=QualityInspectionAuditDiffRuntime.Diff(baseRecord,sequenceChanged);
        var resultDiff=QualityInspectionAuditDiffRuntime.Diff(baseRecord,resultChanged);
        var snapshotDiff=QualityInspectionAuditDiffRuntime.Diff(baseRecord,snapshotChanged);
        var findingDiff=QualityInspectionAuditDiffRuntime.Diff(baseRecord,findingCountChanged);
        var evidenceDiff=QualityInspectionAuditDiffRuntime.Diff(baseRecord,evidenceCountChanged);
        var fingerprintDiff=QualityInspectionAuditDiffRuntime.Diff(baseRecord,fingerprintChanged);

        for(var i=0;i<10;i++) Check(unchanged.IsEmpty,$"self audit diff round {i+1} should be empty.");
        for(var i=0;i<10;i++) Check(sequenceDiff.SequenceChanged && !sequenceDiff.ResultIdentityChanged,$"sequence audit diff round {i+1} should isolate sequence change.");
        for(var i=0;i<10;i++) Check(resultDiff.ResultIdentityChanged && resultDiff.IsEmpty==false,$"result identity diff round {i+1} should be explicit.");
        for(var i=0;i<10;i++) Check(snapshotDiff.SnapshotIdentityChanged,$"snapshot identity diff round {i+1} should be explicit.");
        for(var i=0;i<10;i++) Check(findingDiff.FindingCountChanged,$"finding count diff round {i+1} should be explicit.");
        for(var i=0;i<10;i++) Check(evidenceDiff.EvidenceLinkCountChanged,$"evidence count diff round {i+1} should be explicit.");
        for(var i=0;i<10;i++) Check(fingerprintDiff.ContentFingerprintChanged,$"content fingerprint diff round {i+1} should be explicit.");
        for(var i=0;i<10;i++) Check(QualityInspectionAuditValidationRuntime.IsValid(baseRecord) && QualityInspectionAuditValidationRuntime.IsValid(sequenceChanged),$"audit source validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(QualityInspectionAuditDiffRuntime.Diff(baseRecord,fingerprintChanged).Equals(fingerprintDiff),$"audit diff determinism round {i+1} should remain stable.");
        for(var i=0;i<10;i++) Check(!QualityInspectionAuditDiffRuntime.Diff(baseRecord,findingCountChanged).IsEmpty,$"non-empty audit diff round {i+1} should remain explicit.");

        assert(round==100,$"Quality inspection audit diff smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
