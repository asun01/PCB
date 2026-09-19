namespace Asun.Platform.Evidence;

public static class EvidenceCatalogQueryRuntime
{
    public static IReadOnlyList<EvidenceDescriptor> Find(
        EvidenceCatalogSnapshot snapshot,
        EvidenceDescriptorQuery query)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(query);

        if(!EvidenceCatalogSnapshotValidationRuntime.IsValid(snapshot))
            throw new ArgumentException(
                "Evidence catalog snapshot is invalid.",
                nameof(snapshot));

        if(!EvidenceDescriptorQueryValidationRuntime.IsValid(query))
            throw new ArgumentException(
                "Evidence descriptor query is invalid.",
                nameof(query));

        return snapshot.Descriptors
            .Where(descriptor=>
                (query.Kind is null || descriptor.Kind==query.Kind.Value) &&
                (query.MediaType is null ||
                 string.Equals(descriptor.MediaType,query.MediaType,StringComparison.Ordinal)))
            .OrderBy(descriptor=>descriptor.Handle.Value,StringComparer.Ordinal)
            .ToArray();
    }
}
