using Asun.Domain.Quality;

public static class QualityInspectionFindingAuditDiffHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        static QualityInspectionFindingAuditRecord Record(
            string id,
            string rule,
            string message) =>
            new(
                QualityFindingId.Create(id),
                rule,
                QualityOutcome.Review,
                QualitySeverity.Information,
                message,
                Array.Empty<QualityEvidenceKey>());

        var previous=new QualityInspectionFindingAuditIndex(new[]{
            Record("F-001","RULE.A","one"),
            Record("F-002","RULE.B","two")
        });
        var current=new QualityInspectionFindingAuditIndex(new[]{
            Record("F-002","RULE.B","changed"),
            Record("F-003","RULE.C","three")
        });

        var same=QualityInspectionFindingAuditDiffRuntime.Diff(previous,previous);
        var diff=QualityInspectionFindingAuditDiffRuntime.Diff(previous,current);
        var invalid=new QualityInspectionFindingAuditDiff(
            new[]{QualityFindingId.Create("F-001")},
            new[]{QualityFindingId.Create("F-001")},
            Array.Empty<QualityFindingId>());

        for(var i=0;i<10;i++) Check(same.IsEmpty,$"finding audit self diff round {i+1} should be empty.");
        for(var i=0;i<10;i++) Check(diff.AddedFindingIds.Single().Value=="F-003",$"finding audit added round {i+1} should identify F-003.");
        for(var i=0;i<10;i++) Check(diff.RemovedFindingIds.Single().Value=="F-001",$"finding audit removed round {i+1} should identify F-001.");
        for(var i=0;i<10;i++) Check(diff.ChangedFindingIds.Single().Value=="F-002",$"finding audit changed round {i+1} should identify F-002.");
        for(var i=0;i<10;i++) Check(QualityInspectionFindingAuditDiffValidationRuntime.IsValid(diff),$"finding audit diff validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(!QualityInspectionFindingAuditDiffValidationRuntime.IsValid(invalid),$"finding audit diff overlap round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(diff.AddedFindingIds.SequenceEqual(diff.AddedFindingIds.OrderBy(id=>id.Value,StringComparer.Ordinal)),$"finding audit added ordering round {i+1} should be deterministic.");
        for(var i=0;i<10;i++) Check(diff.RemovedFindingIds.SequenceEqual(diff.RemovedFindingIds.OrderBy(id=>id.Value,StringComparer.Ordinal)),$"finding audit removed ordering round {i+1} should be deterministic.");
        for(var i=0;i<10;i++) Check(diff.ChangedFindingIds.SequenceEqual(diff.ChangedFindingIds.OrderBy(id=>id.Value,StringComparer.Ordinal)),$"finding audit changed ordering round {i+1} should be deterministic.");
        for(var i=0;i<10;i++) Check(QualityInspectionFindingAuditDiffRuntime.Diff(previous,current).Equals(diff),$"finding audit diff determinism round {i+1} should remain stable.");

        assert(round==100,$"Quality inspection finding audit diff smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
