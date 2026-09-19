namespace Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotDiffRuntime
{
    public static EvidenceCatalogSnapshotDiff Diff(
        EvidenceCatalogSnapshot previous,
        EvidenceCatalogSnapshot current)
    {
        ArgumentNullException.ThrowIfNull(previous);
        ArgumentNullException.ThrowIfNull(current);

        var previousByHandle=previous.Descriptors
            .ToDictionary(descriptor=>descriptor.Handle);
        var currentByHandle=current.Descriptors
            .ToDictionary(descriptor=>descriptor.Handle);

        var added=currentByHandle.Keys
            .Except(previousByHandle.Keys)
            .OrderBy(handle=>handle.Value,StringComparer.Ordinal)
            .ToArray();
        var removed=previousByHandle.Keys
            .Except(currentByHandle.Keys)
            .OrderBy(handle=>handle.Value,StringComparer.Ordinal)
            .ToArray();
        var changed=currentByHandle.Keys
            .Intersect(previousByHandle.Keys)
            .Where(handle =>
                previousByHandle[handle]!=currentByHandle[handle])
            .OrderBy(handle=>handle.Value,StringComparer.Ordinal)
            .ToArray();

        return new EvidenceCatalogSnapshotDiff(
            added,
            removed,
            changed);
    }
}
