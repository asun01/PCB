namespace Asun.Domain.Quality;

public static class QualityInspectionRunRuntime
{
    public static QualityInspectionRun Create(
        Guid runId,
        IEnumerable<QualityInspectionResult> results)
    {
        ArgumentNullException.ThrowIfNull(results);

        var materialized=results.ToArray();

        foreach(var result in materialized)
        {
            if(!QualityInspectionResultValidationRuntime.IsValid(result))
                throw new ArgumentException(
                    "Inspection run contains an invalid result.",
                    nameof(results));
        }

        if(materialized.GroupBy(result=>result.ResultId).Any(group=>group.Count()>1))
            throw new ArgumentException(
                "Inspection result ids must be unique within a run.",
                nameof(results));

        if(materialized.GroupBy(result=>result.SnapshotId).Any(group=>group.Count()>1))
            throw new ArgumentException(
                "Inspection snapshot ids must be unique within a run.",
                nameof(results));

        return new QualityInspectionRun(
            runId,
            materialized
                .OrderBy(result=>result.Sequence)
                .ThenBy(result=>result.SnapshotId)
                .ThenBy(result=>result.ResultId)
                .ToArray());
    }
}
