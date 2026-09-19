namespace Asun.Domain.Quality;

public static class QualityInspectionAuditWindowDiffValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        QualityInspectionAuditWindowDiff diff)
    {
        ArgumentNullException.ThrowIfNull(diff);

        var errors=new List<string>();

        ValidateUnique(diff.AddedResultIds,"Added result ids",errors);
        ValidateUnique(diff.RemovedResultIds,"Removed result ids",errors);
        ValidateUnique(diff.ChangedResultIds,"Changed result ids",errors);

        if(diff.AddedResultIds.Any(id=>id==Guid.Empty) ||
           diff.RemovedResultIds.Any(id=>id==Guid.Empty) ||
           diff.ChangedResultIds.Any(id=>id==Guid.Empty))
        {
            errors.Add("Audit window diff result ids cannot be empty.");
        }

        if(diff.AddedResultIds.Intersect(diff.RemovedResultIds).Any() ||
           diff.AddedResultIds.Intersect(diff.ChangedResultIds).Any() ||
           diff.RemovedResultIds.Intersect(diff.ChangedResultIds).Any())
        {
            errors.Add("A result id cannot appear in multiple audit window diff categories.");
        }

        return errors;
    }

    public static bool IsValid(
        QualityInspectionAuditWindowDiff diff) =>
        Validate(diff).Count==0;

    private static void ValidateUnique(
        IReadOnlyList<Guid> values,
        string label,
        List<string> errors)
    {
        if(values.Count!=values.Distinct().Count())
            errors.Add($"{label} must be unique.");
    }
}
