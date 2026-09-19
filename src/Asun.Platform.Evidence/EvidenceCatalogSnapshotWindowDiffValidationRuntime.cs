namespace Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotWindowDiffValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        EvidenceCatalogSnapshotWindowDiff diff)
    {
        ArgumentNullException.ThrowIfNull(diff);

        var errors=new List<string>();
        ValidateUniquePositive(diff.AddedSequences,"Added sequences",errors);
        ValidateUniquePositive(diff.RemovedSequences,"Removed sequences",errors);
        ValidateUniquePositive(diff.ChangedSequences,"Changed sequences",errors);

        if(diff.AddedSequences.Intersect(diff.RemovedSequences).Any() ||
           diff.AddedSequences.Intersect(diff.ChangedSequences).Any() ||
           diff.RemovedSequences.Intersect(diff.ChangedSequences).Any())
        {
            errors.Add("A snapshot window sequence cannot appear in multiple diff categories.");
        }

        return errors;
    }

    public static bool IsValid(
        EvidenceCatalogSnapshotWindowDiff diff)=>
        Validate(diff).Count==0;

    private static void ValidateUniquePositive(
        IReadOnlyList<long> values,
        string label,
        List<string> errors)
    {
        if(values.Any(value=>value<=0))
            errors.Add($"{label} must contain only positive sequences.");

        if(values.Count!=values.Distinct().Count())
            errors.Add($"{label} must be unique.");
    }
}
