namespace Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotRuntime
{
    public static EvidenceCatalogSnapshot Create(
        IEnumerable<EvidenceDescriptor> descriptors)
    {
        ArgumentNullException.ThrowIfNull(descriptors);

        var validated=descriptors.ToArray();

        foreach(var descriptor in validated)
        {
            if(!EvidenceDescriptorValidationRuntime.IsValid(descriptor))
                throw new ArgumentException(
                    "Evidence catalog snapshot cannot contain invalid descriptors.",
                    nameof(descriptors));
        }

        return new EvidenceCatalogSnapshot(
            validated
                .OrderBy(descriptor=>descriptor.Handle.Value,StringComparer.Ordinal)
                .ToArray());
    }
}
