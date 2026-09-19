namespace Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotChangeRuntime
{
    public static IReadOnlyList<EvidenceCatalogSnapshotChange> Create(
        EvidenceCatalogSnapshot previous,
        EvidenceCatalogSnapshot current)
    {
        ArgumentNullException.ThrowIfNull(previous);
        ArgumentNullException.ThrowIfNull(current);

        if(!EvidenceCatalogSnapshotValidationRuntime.IsValid(previous) ||
           !EvidenceCatalogSnapshotValidationRuntime.IsValid(current))
        {
            throw new ArgumentException(
                "Both evidence catalog snapshots must be valid.");
        }

        var previousByHandle=previous.Descriptors.ToDictionary(descriptor=>descriptor.Handle);
        var currentByHandle=current.Descriptors.ToDictionary(descriptor=>descriptor.Handle);

        return currentByHandle.Keys
            .Intersect(previousByHandle.Keys)
            .Where(handle=>previousByHandle[handle]!=currentByHandle[handle])
            .OrderBy(handle=>handle.Value,StringComparer.Ordinal)
            .Select(handle=>new EvidenceCatalogSnapshotChange(
                handle,
                EvidenceDescriptorFingerprintRuntime.CreateFingerprint(previousByHandle[handle]),
                EvidenceDescriptorFingerprintRuntime.CreateFingerprint(currentByHandle[handle])))
            .ToArray();
    }
}
