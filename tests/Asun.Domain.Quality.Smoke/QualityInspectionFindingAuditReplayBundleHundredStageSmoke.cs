using Asun.Domain.Quality;

public static class QualityInspectionFindingAuditReplayBundleHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        static QualityInspectionResult Create(
            string resultId,
            string findingId,
            string message)
        {
            var finding=new QualityFinding(
                QualityFindingId.Create(findingId),
                "RULE",
                QualityOutcome.Review,
                QualitySeverity.Information,
                message);
            return new QualityInspectionResult(
                Guid.Parse(resultId),
                new QualityInspectionSnapshot(
                    Guid.NewGuid(),
                    1,
                    new QualityFindingSet(new[]{finding}),
                    new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>())));
        }

        var previous=Create(
            "00000000-0000-0000-0000-000000000301",
            "F-001",
            "one");
        var current=Create(
            "00000000-0000-0000-0000-000000000302",
            "F-002",
            "two");

        var initial=QualityInspectionFindingAuditReplayBundleRuntime.Create(current);
        var bundle=QualityInspectionFindingAuditReplayBundleRuntime.Create(
            current,
            previous);
        var tampered=bundle with
        {
            Diff=QualityInspectionFindingAuditProjectionDiffRuntime.Diff(
                bundle.Current,
                bundle.Current)
        };

        for(var i=0;i<10;i++) Check(QualityInspectionFindingAuditReplayBundleValidationRuntime.IsValid(current,initial),$"initial finding audit replay bundle validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(initial.Previous is null,$"initial finding audit replay bundle previous round {i+1} should be absent.");
        for(var i=0;i<10;i++) Check(initial.Diff.IsEmpty,$"initial finding audit replay bundle diff round {i+1} should be empty.");
        for(var i=0;i<10;i++) Check(QualityInspectionFindingAuditReplayBundleValidationRuntime.IsValid(current,bundle,previous),$"two-result finding audit replay bundle round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(bundle.Previous is not null,$"two-result finding audit replay previous round {i+1} should exist.");
        for(var i=0;i<10;i++) Check(bundle.Diff.AddedFindingIds.Single().Value=="F-002",$"finding audit replay added round {i+1} should identify F-002.");
        for(var i=0;i<10;i++) Check(bundle.Diff.RemovedFindingIds.Single().Value=="F-001",$"finding audit replay removed round {i+1} should identify F-001.");
        for(var i=0;i<10;i++) Check(!QualityInspectionFindingAuditReplayBundleValidationRuntime.IsValid(current,tampered,previous),$"tampered finding audit replay bundle round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(QualityInspectionFindingAuditReplayBundleValidationRuntime.Validate(current,bundle,previous).Count==0,$"finding audit replay diagnostics round {i+1} should remain empty.");

        assert(round==100,$"Quality inspection finding audit replay bundle smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
