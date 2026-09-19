namespace Asun.Domain.Quality;

public static class QualityInspectionFindingAuditProjectionDiffValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        QualityInspectionFindingAuditProjectionDiff diff)
    {
        ArgumentNullException.ThrowIfNull(diff);

        var errors=new List<string>();

        ValidateUnique(diff.AddedFindingIds,"Added finding ids",errors);
        ValidateUnique(diff.RemovedFindingIds,"Removed finding ids",errors);
        ValidateUnique(diff.ChangedFindingIds,"Changed finding ids",errors);

        if(diff.AddedFindingIds.Any(id=>!id.IsValid) ||
           diff.RemovedFindingIds.Any(id=>!id.IsValid) ||
           diff.ChangedFindingIds.Any(id=>!id.IsValid))
        {
            errors.Add("Finding audit projection diff ids must be valid.");
        }

        if(diff.AddedFindingIds.Intersect(diff.RemovedFindingIds).Any() ||
           diff.AddedFindingIds.Intersect(diff.ChangedFindingIds).Any() ||
           diff.RemovedFindingIds.Intersect(diff.ChangedFindingIds).Any())
        {
            errors.Add("A finding id cannot appear in multiple projection diff categories.");
        }

        return errors;
    }

    public static bool IsValid(
        QualityInspectionFindingAuditProjectionDiff diff) =>
        Validate(diff).Count==0;

    private static void ValidateUnique(
        IReadOnlyList<QualityFindingId> values,
        string label,
        List<string> errors)
    {
        if(values.Count!=values.Distinct().Count())
            errors.Add($"{label} must be unique.");
    }
}
