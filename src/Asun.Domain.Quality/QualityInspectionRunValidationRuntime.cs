namespace Asun.Domain.Quality;

public static class QualityInspectionRunValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        QualityInspectionRun run)
    {
        ArgumentNullException.ThrowIfNull(run);

        var errors=new List<string>();

        if(run.RunId==Guid.Empty)
            errors.Add("Inspection run id cannot be empty.");

        var resultIds=new HashSet<Guid>();
        var snapshotIds=new HashSet<Guid>();
        long? previousSequence=null;

        foreach(var result in run.Results)
        {
            errors.AddRange(
                QualityInspectionResultValidationRuntime.Validate(result));

            if(!resultIds.Add(result.ResultId))
                errors.Add("Inspection result ids must be unique within a run.");

            if(!snapshotIds.Add(result.SnapshotId))
                errors.Add("Inspection snapshot ids must be unique within a run.");

            if(previousSequence is not null &&
               result.Sequence<previousSequence.Value)
            {
                errors.Add("Inspection run results must be ordered by non-decreasing sequence.");
            }

            previousSequence=result.Sequence;
        }

        if(run.ResultCount!=run.Results.Count)
            errors.Add("Inspection run result count must match the result collection.");

        if(!run.Results.SequenceEqual(
            run.Results
                .OrderBy(result=>result.Sequence)
                .ThenBy(result=>result.SnapshotId)
                .ThenBy(result=>result.ResultId)))
        {
            errors.Add("Inspection run results must use canonical ordering.");
        }

        return errors;
    }

    public static bool IsValid(
        QualityInspectionRun run)=>
        Validate(run).Count==0;
}
