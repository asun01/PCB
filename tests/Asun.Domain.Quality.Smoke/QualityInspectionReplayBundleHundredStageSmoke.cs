using Asun.Domain.Quality;

public static class QualityInspectionReplayBundleHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        var finding=new QualityFinding(
            QualityFindingId.Create("F-001"),
            "RULE",
            QualityOutcome.Review,
            QualitySeverity.Information,
            "bundle finding");
        var nextFinding=new QualityFinding(
            QualityFindingId.Create("F-002"),
            "RULE",
            QualityOutcome.Warning,
            QualitySeverity.Warning,
            "next finding");
        var previousResult=new QualityInspectionResult(
            Guid.NewGuid(),
            new QualityInspectionSnapshot(
                Guid.NewGuid(),
                60,
                new QualityFindingSet(new[]{finding}),
                new QualityFindingEvidenceSet(new[]{
                    new QualityFindingEvidenceLink(
                        finding.Id,
                        QualityEvidenceKey.Create("frame://001"))
                })));
        var currentResult=new QualityInspectionResult(
            Guid.NewGuid(),
            new QualityInspectionSnapshot(
                Guid.NewGuid(),
                61,
                new QualityFindingSet(new[]{finding,nextFinding}),
                new QualityFindingEvidenceSet(new[]{
                    new QualityFindingEvidenceLink(
                        finding.Id,
                        QualityEvidenceKey.Create("frame://001")),
                    new QualityFindingEvidenceLink(
                        nextFinding.Id,
                        QualityEvidenceKey.Create("frame://002"))
                })));

        var initial=QualityInspectionReplayBundleRuntime.Create(currentResult);
        var bundle=QualityInspectionReplayBundleRuntime.Create(
            currentResult,
            previousResult);
        var invalid=new QualityInspectionReplayBundle(
            bundle.Previous,
            bundle.Current,
            QualityInspectionReplayProjectionDiffRuntime.Diff(
                bundle.Current,
                bundle.Current));
        var nullCurrent=new QualityInspectionReplayBundle(
            bundle.Previous,
            null!,
            bundle.Diff);
        var nullDiff=new QualityInspectionReplayBundle(
            bundle.Previous,
            bundle.Current,
            null!);

        for(var i=0;i<10;i++) Check(QualityInspectionReplayBundleValidationRuntime.IsValid(initial),$"initial bundle validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(initial.Previous is null,$"initial bundle previous projection round {i+1} should remain absent.");
        for(var i=0;i<10;i++) Check(initial.Diff.IsEmpty,$"initial bundle diff round {i+1} should be empty.");
        for(var i=0;i<10;i++) Check(QualityInspectionReplayBundleValidationRuntime.IsValid(bundle),$"two-result bundle validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(bundle.Previous is not null,$"two-result previous projection round {i+1} should exist.");
        for(var i=0;i<10;i++) Check(bundle.Diff.AddedFindingIds.Single()==nextFinding.Id,$"bundle finding diff round {i+1} should identify F-002.");
        for(var i=0;i<10;i++) Check(bundle.Diff.AddedEvidenceLinks.Single().FindingId==nextFinding.Id,$"bundle evidence diff round {i+1} should identify F-002.");
        for(var i=0;i<10;i++) Check(QualityInspectionReplayBundleValidationRuntime.Validate(bundle).Count==0,$"bundle diagnostics round {i+1} should remain empty.");
        for(var i=0;i<10;i++) Check(!QualityInspectionReplayBundleValidationRuntime.IsValid(invalid),$"tampered bundle round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(!QualityInspectionReplayBundleValidationRuntime.IsValid(nullCurrent) && !QualityInspectionReplayBundleValidationRuntime.IsValid(nullDiff),$"null bundle boundaries round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(QualityInspectionReplayProjectionDiffRuntime.Diff(bundle.Previous!,bundle.Current).Equals(bundle.Diff),$"bundle diff determinism round {i+1} should remain stable.");

        assert(round==100,$"Quality inspection replay bundle smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
