namespace Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotDiffValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        EvidenceCatalogSnapshotDiff diff)
    {
        ArgumentNullException.ThrowIfNull(diff);

        var errors=new List<string>();

        ValidateUnique(diff.AddedHandles,"Added handles",errors);
        ValidateUnique(diff.RemovedHandles,"Removed handles",errors);
        ValidateUnique(diff.ChangedHandles,"Changed handles",errors);

        if(diff.AddedHandles.Any(handle=>!handle.IsValid) ||
           diff.RemovedHandles.Any(handle=>!handle.IsValid) ||
           diff.ChangedHandles.Any(handle=>!handle.IsValid))
        {
            errors.Add("Evidence snapshot diff handles must be valid.");
        }

        if(diff.AddedHandles.Intersect(diff.RemovedHandles).Any() ||
           diff.AddedHandles.Intersect(diff.ChangedHandles).Any() ||
           diff.RemovedHandles.Intersect(diff.ChangedHandles).Any())
        {
            errors.Add("An evidence handle cannot appear in multiple diff categories.");
        }

        return errors;
    }

    public static bool IsValid(
        EvidenceCatalogSnapshotDiff diff)=>
        Validate(diff).Count==0;

    private static void ValidateUnique(
        IReadOnlyList<EvidenceHandle> values,
        string label,
        List<string> errors)
    {
        if(values.Count!=values.Distinct().Count())
            errors.Add($"{label} must be unique.");
    }
}
