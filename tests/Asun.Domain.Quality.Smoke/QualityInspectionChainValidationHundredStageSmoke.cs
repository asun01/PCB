using Asun.Domain.Quality;

public static class QualityInspectionChainValidationHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        QualityFinding Make(string id,string message) =>
            new(
                QualityFindingId.Create(id),
                "RULE",
                QualityOutcome.Review,
                QualitySeverity.Information,
                message);

        var f1=Make("F-001","first");
        var f2=Make("F-002","second");
        var previousSnapshot=new QualityInspectionSnapshot(
            Guid.NewGuid(),
            20,
            new QualityFindingSet(new[]{f1}),
            new QualityFindingEvidenceSet(new[]{
                new QualityFindingEvidenceLink(
                    f1.Id,
                    QualityEvidenceKey.Create("frame://001"))
            }));
        var currentSnapshot=new QualityInspectionSnapshot(
            Guid.NewGuid(),
            21,
            new QualityFindingSet(new[]{f1,f2}),
            new QualityFindingEvidenceSet(new[]{
                new QualityFindingEvidenceLink(
                    f1.Id,
                    QualityEvidenceKey.Create("frame://001")),
                new QualityFindingEvidenceLink(
                    f2.Id,
                    QualityEvidenceKey.Create("frame://002"))
            }));
        var previous=new QualityInspectionResult(Guid.NewGuid(),previousSnapshot);
        var current=new QualityInspectionResult(Guid.NewGuid(),currentSnapshot);
        var backward=new QualityInspectionResult(
            Guid.NewGuid(),
            new QualityInspectionSnapshot(
                Guid.NewGuid(),
                19,
                new QualityFindingSet(new[]{f1}),
                previousSnapshot.Evidence));
        var orphan=new QualityInspectionResult(
            Guid.NewGuid(),
            new QualityInspectionSnapshot(
                Guid.NewGuid(),
                22,
                new QualityFindingSet(new[]{f1}),
                new QualityFindingEvidenceSet(new[]{
                    new QualityFindingEvidenceLink(
                        QualityFindingId.Create("F-999"),
                        QualityEvidenceKey.Create("frame://999"))
                })));

        var relation=QualityInspectionSequenceRuntime.Observe(
            previousSnapshot,
            currentSnapshot);
        var findingDiff=QualityInspectionDiffRuntime.Diff(
            previousSnapshot,
            currentSnapshot);
        var evidenceDiff=QualityInspectionEvidenceDiffRuntime.Diff(
            previousSnapshot,
            currentSnapshot);
        var previousAudit=QualityInspectionAuditRuntime.Create(previous);
        var currentAudit=QualityInspectionAuditRuntime.Create(current);

        for(var i=0;i<10;i++) Check(QualityInspectionChainValidationRuntime.IsValid(previous,current),$"chain validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(relation.IsConsecutive,$"chain sequence round {i+1} should be consecutive.");
        for(var i=0;i<10;i++) Check(findingDiff.AddedFindingIds.Single()==f2.Id,$"chain finding delta round {i+1} should identify F-002.");
        for(var i=0;i<10;i++) Check(evidenceDiff.AddedLinks.Count==1,$"chain evidence delta round {i+1} should identify one added relationship.");
        for(var i=0;i<10;i++) Check(previous.ResultId!=current.ResultId,$"chain result identity round {i+1} should be distinct.");
        for(var i=0;i<10;i++) Check(previous.SnapshotId!=current.SnapshotId,$"chain snapshot identity round {i+1} should be distinct.");
        for(var i=0;i<10;i++) Check(QualityInspectionAuditValidationRuntime.IsValid(previousAudit) && QualityInspectionAuditValidationRuntime.IsValid(currentAudit),$"chain audit round {i+1} should validate both records.");
        for(var i=0;i<10;i++) Check(QualityInspectionChainValidationRuntime.IsValid(previous,backward),$"backward chain round {i+1} should remain policy-neutral.");
        for(var i=0;i<10;i++) Check(!QualityInspectionChainValidationRuntime.IsValid(previous,orphan),$"orphan chain round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(QualityInspectionChainValidationRuntime.Validate(previous,current).Count==0,$"clean chain diagnostics round {i+1} should remain empty.");

        assert(round==100,$"Quality inspection chain smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
