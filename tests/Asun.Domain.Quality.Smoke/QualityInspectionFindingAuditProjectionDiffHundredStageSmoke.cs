using Asun.Domain.Quality;

public static class QualityInspectionFindingAuditProjectionDiffHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        static QualityInspectionFindingAuditRecord Record(
            string id,
            string message) =>
            new(
                QualityFindingId.Create(id),
                "RULE",
                QualityOutcome.Review,
                QualitySeverity.Information,
                message,
                Array.Empty<QualityEvidenceKey>());

        var previous=new QualityInspectionFindingAuditProjection(
            new QualityInspectionFindingAuditIndex(new[]{
                Record("F-001","one"),
                Record("F-002","two")
            }),
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
        var current=new QualityInspectionFindingAuditProjection(
            new QualityInspectionFindingAuditIndex(new[]{
                Record("F-002","changed"),
                Record("F-003","three")
            }),
            "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb");

        var same=QualityInspectionFindingAuditProjectionDiffRuntime.Diff(previous,previous);
        var diff=QualityInspectionFindingAuditProjectionDiffRuntime.Diff(previous,current);
        var invalid=new QualityInspectionFindingAuditProjectionDiff(
            new[]{QualityFindingId.Create("F-001")},
            new[]{QualityFindingId.Create("F-001")},
            Array.Empty<QualityFindingId>());

        for(var i=0;i<10;i++) Check(same.IsEmpty,$"finding audit projection self diff round {i+1} should be empty.");
        for(var i=0;i<10;i++) Check(diff.AddedFindingIds.Single().Value=="F-003",$"projection added finding round {i+1} should identify F-003.");
        for(var i=0;i<10;i++) Check(diff.RemovedFindingIds.Single().Value=="F-001",$"projection removed finding round {i+1} should identify F-001.");
        for(var i=0;i<10;i++) Check(diff.ChangedFindingIds.Single().Value=="F-002",$"projection changed finding round {i+1} should identify F-002.");
        for(var i=0;i<10;i++) Check(QualityInspectionFindingAuditProjectionDiffValidationRuntime.IsValid(diff),$"projection diff validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(!QualityInspectionFindingAuditProjectionDiffValidationRuntime.IsValid(invalid),$"projection diff overlap round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(diff.AddedFindingIds.SequenceEqual(diff.AddedFindingIds.OrderBy(id=>id.Value,StringComparer.Ordinal)),$"projection added ordering round {i+1} should be deterministic.");
        for(var i=0;i<10;i++) Check(diff.RemovedFindingIds.SequenceEqual(diff.RemovedFindingIds.OrderBy(id=>id.Value,StringComparer.Ordinal)),$"projection removed ordering round {i+1} should be deterministic.");
        for(var i=0;i<10;i++) Check(diff.ChangedFindingIds.SequenceEqual(diff.ChangedFindingIds.OrderBy(id=>id.Value,StringComparer.Ordinal)),$"projection changed ordering round {i+1} should be deterministic.");
        for(var i=0;i<10;i++) Check(QualityInspectionFindingAuditProjectionDiffRuntime.Diff(previous,current).Equals(diff),$"projection diff determinism round {i+1} should remain stable.");

        assert(round==100,$"Quality inspection finding audit projection diff smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
