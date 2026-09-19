using Asun.Domain.Quality;

public static class QualityInspectionAuditReplayBundleHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        static QualityInspectionResult Create(Guid id,string findingId,string message)
        {
            var finding=new QualityFinding(
                QualityFindingId.Create(findingId),
                "RULE",
                QualityOutcome.Review,
                QualitySeverity.Information,
                message);
            return new QualityInspectionResult(
                id,
                new QualityInspectionSnapshot(
                    Guid.NewGuid(),
                    1,
                    new QualityFindingSet(new[]{finding}),
                    new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>())));
        }

        var previous=Create(
            Guid.Parse("00000000-0000-0000-0000-000000000601"),
            "F-001",
            "one");
        var current=Create(
            Guid.Parse("00000000-0000-0000-0000-000000000602"),
            "F-002",
            "two");

        var initial=QualityInspectionAuditReplayBundleRuntime.Create(current);
        var bundle=QualityInspectionAuditReplayBundleRuntime.Create(current,previous);
        var tampered=bundle with
        {
            Diff=QualityInspectionAuditWindowDiffRuntime.Diff(
                new QualityInspectionAuditWindow(new[]{
                    QualityInspectionAuditEnvelopeRuntime.Create(current)}),
                new QualityInspectionAuditWindow(new[]{
                    QualityInspectionAuditEnvelopeRuntime.Create(current)}))
        };

        for(var i=0;i<10;i++) Check(QualityInspectionAuditReplayBundleValidationRuntime.IsValid(current,initial),$"initial audit replay validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(initial.Previous is null,$"initial audit replay previous round {i+1} should be absent.");
        for(var i=0;i<10;i++) Check(initial.Diff.IsEmpty,$"initial audit replay diff round {i+1} should be empty.");
        for(var i=0;i<10;i++) Check(QualityInspectionAuditReplayBundleValidationRuntime.IsValid(current,bundle,previous),$"two-result audit replay validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(bundle.Previous is not null,$"two-result audit replay previous round {i+1} should exist.");
        for(var i=0;i<10;i++) Check(bundle.Diff.AddedResultIds.Single()==current.ResultId,$"audit replay added result round {i+1} should identify current result.");
        for(var i=0;i<10;i++) Check(bundle.Diff.RemovedResultIds.Single()==previous.ResultId,$"audit replay removed result round {i+1} should identify previous result.");
        for(var i=0;i<10;i++) Check(!QualityInspectionAuditReplayBundleValidationRuntime.IsValid(current,tampered,previous),$"tampered audit replay round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(QualityInspectionAuditReplayBundleValidationRuntime.Validate(current,bundle,previous).Count==0,$"audit replay diagnostics round {i+1} should remain empty.");

        assert(round==100,$"Quality inspection audit replay bundle smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
