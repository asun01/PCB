using Asun.Domain.Quality;

public static class QualityInspectionDiffValidationHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        var added = new QualityFindingId("F-001");
        var removed = new QualityFindingId("F-002");
        var changed = new QualityFindingId("F-003");
        var keyAdded = new QualityEvidenceKey("frame://new");
        var keyRemoved = new QualityEvidenceKey("frame://old");

        var diff = new QualityInspectionDiff(
            new[]{added},
            new[]{removed},
            new[]{changed},
            new[]{keyAdded},
            new[]{keyRemoved});

        var invalidOverlap = new QualityInspectionDiff(
            new[]{added},
            new[]{added},
            Array.Empty<QualityFindingId>(),
            Array.Empty<QualityEvidenceKey>(),
            Array.Empty<QualityEvidenceKey>())
        {
            RelinkedEvidenceKeys = new[]{keyAdded}
        };
        var invalidKey = new QualityInspectionDiff(
            Array.Empty<QualityFindingId>(),
            Array.Empty<QualityFindingId>(),
            Array.Empty<QualityFindingId>(),
            new[]{new QualityEvidenceKey("")},
            Array.Empty<QualityEvidenceKey>());

        for(var i=0;i<10;i++) Check(!diff.IsEmpty,$"non-empty diff round {i+1} should remain explicit.");
        for(var i=0;i<10;i++) Check(QualityInspectionDiffValidationRuntime.IsValid(diff),$"diff validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(diff.AddedFindingIds.Single()==added,$"added finding round {i+1} should be stable.");
        for(var i=0;i<10;i++) Check(diff.RemovedFindingIds.Single()==removed,$"removed finding round {i+1} should be stable.");
        for(var i=0;i<10;i++) Check(diff.ChangedFindingIds.Single()==changed,$"changed finding round {i+1} should be stable.");
        for(var i=0;i<10;i++) Check(diff.AddedEvidenceKeys.Single()==keyAdded,$"added evidence round {i+1} should be stable.");
        for(var i=0;i<10;i++) Check(diff.RemovedEvidenceKeys.Single()==keyRemoved,$"removed evidence round {i+1} should be stable.");
        for(var i=0;i<10;i++) Check(!QualityInspectionDiffValidationRuntime.IsValid(invalidOverlap),$"overlap diff round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(QualityInspectionDiffValidationRuntime.Validate(invalidOverlap).Count>0,$"invalid diff diagnostics round {i+1} should be non-empty.");
        for(var i=0;i<10;i++) Check(diff.AddedFindingIds.Distinct().Count()==diff.AddedFindingIds.Count && diff.RelinkedEvidenceKeys.Distinct().Count()==diff.RelinkedEvidenceKeys.Count && !QualityInspectionDiffValidationRuntime.IsValid(invalidKey),$"diff uniqueness and identifier validation round {i+1} should remain stable.");

        assert(round==100,$"Quality inspection diff smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
