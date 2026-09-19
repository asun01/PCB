namespace Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        EvidenceCatalogSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var errors=new List<string>();
        var handles=new HashSet<EvidenceHandle>();

        foreach(var descriptor in snapshot.Descriptors)
        {
            errors.AddRange(
                EvidenceDescriptorValidationRuntime.Validate(descriptor));

            if(!handles.Add(descriptor.Handle))
                errors.Add("Evidence catalog snapshot handles must be unique.");
        }

        return errors;
    }

    public static bool IsValid(
        EvidenceCatalogSnapshot snapshot)=>
        Validate(snapshot).Count==0;
}
