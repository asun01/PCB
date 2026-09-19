namespace Asun.Platform.Evidence;

public static class EvidenceReferenceResolutionRuntime
{
    public static EvidenceReferenceResolution Resolve(
        EvidenceCatalogSnapshot snapshot,
        EvidenceReferenceSet referenceSet)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(referenceSet);

        if(!EvidenceCatalogSnapshotValidationRuntime.IsValid(snapshot))
            throw new ArgumentException(
                "Evidence catalog snapshot is invalid.",
                nameof(snapshot));

        if(!EvidenceReferenceSetValidationRuntime.IsValid(referenceSet))
            throw new ArgumentException(
                "Evidence reference set is invalid.",
                nameof(referenceSet));

        var catalogHandles=snapshot.Descriptors
            .Select(descriptor=>descriptor.Handle)
            .ToHashSet();

        var found=referenceSet.Handles
            .Where(catalogHandles.Contains)
            .OrderBy(handle=>handle.Value,StringComparer.Ordinal)
            .ToArray();
        var missing=referenceSet.Handles
            .Where(handle=>!catalogHandles.Contains(handle))
            .OrderBy(handle=>handle.Value,StringComparer.Ordinal)
            .ToArray();

        return new EvidenceReferenceResolution(
            EvidenceCatalogSnapshotFingerprintRuntime.CreateFingerprint(snapshot),
            found,
            missing);
    }
}
