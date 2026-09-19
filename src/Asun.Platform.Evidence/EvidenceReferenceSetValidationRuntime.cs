namespace Asun.Platform.Evidence;

public static class EvidenceReferenceSetValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        EvidenceReferenceSet referenceSet)
    {
        ArgumentNullException.ThrowIfNull(referenceSet);

        var errors=new List<string>();

        if(referenceSet.Handles.Any(handle=>!handle.IsValid))
            errors.Add("Evidence reference handles must be valid.");

        if(referenceSet.Handles.Count!=referenceSet.Handles.Distinct().Count())
            errors.Add("Evidence reference handles must be unique.");

        if(!referenceSet.Handles.SequenceEqual(
            referenceSet.Handles.OrderBy(handle=>handle.Value,StringComparer.Ordinal)))
        {
            errors.Add("Evidence reference handles must use canonical opaque ordering.");
        }

        return errors;
    }

    public static bool IsValid(
        EvidenceReferenceSet referenceSet)=>
        Validate(referenceSet).Count==0;
}
