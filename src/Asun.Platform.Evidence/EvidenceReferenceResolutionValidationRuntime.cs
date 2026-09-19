namespace Asun.Platform.Evidence;

public static class EvidenceReferenceResolutionValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        EvidenceCatalogSnapshot snapshot,
        EvidenceReferenceSet referenceSet,
        EvidenceReferenceResolution resolution)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(referenceSet);
        ArgumentNullException.ThrowIfNull(resolution);

        var errors=new List<string>();

        var snapshotErrors=
            EvidenceCatalogSnapshotValidationRuntime.Validate(snapshot);
        var referenceSetErrors=
            EvidenceReferenceSetValidationRuntime.Validate(referenceSet);

        errors.AddRange(snapshotErrors);
        errors.AddRange(referenceSetErrors);

        if(errors.Count>0)
            return errors;

        var snapshotFingerprint=
            EvidenceCatalogSnapshotFingerprintRuntime.CreateFingerprint(snapshot);

        if(resolution.SnapshotFingerprint!=snapshotFingerprint)
            errors.Add("Evidence reference resolution snapshot fingerprint must match the source snapshot.");

        if(resolution.FoundHandles.Intersect(resolution.MissingHandles).Any())
            errors.Add("Evidence reference resolution found and missing sets must be disjoint.");

        var sourceSet=referenceSet.Handles.ToHashSet();
        if(resolution.FoundHandles.Any(handle=>!sourceSet.Contains(handle)) ||
           resolution.MissingHandles.Any(handle=>!sourceSet.Contains(handle)))
        {
            errors.Add("Evidence reference resolution must contain only source reference handles.");
        }

        if(!resolution.FoundHandles.SequenceEqual(
            resolution.FoundHandles.OrderBy(handle=>handle.Value,StringComparer.Ordinal)) ||
           !resolution.MissingHandles.SequenceEqual(
            resolution.MissingHandles.OrderBy(handle=>handle.Value,StringComparer.Ordinal)))
        {
            errors.Add("Evidence reference resolution handle ordering must be canonical.");
        }

        if(resolution.FoundHandles.Count+resolution.MissingHandles.Count!=referenceSet.Handles.Count)
            errors.Add("Evidence reference resolution must account for every source handle.");

        var expected=EvidenceReferenceResolutionRuntime.Resolve(snapshot,referenceSet);

        if(!expected.FoundHandles.SequenceEqual(resolution.FoundHandles) ||
           !expected.MissingHandles.SequenceEqual(resolution.MissingHandles))
        {
            errors.Add("Evidence reference resolution does not match the source snapshot.");
        }

        return errors;
    }

    public static bool IsValid(
        EvidenceCatalogSnapshot snapshot,
        EvidenceReferenceSet referenceSet,
        EvidenceReferenceResolution resolution)=>
        Validate(snapshot,referenceSet,resolution).Count==0;
}
