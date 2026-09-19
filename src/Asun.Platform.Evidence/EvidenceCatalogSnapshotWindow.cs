namespace Asun.Platform.Evidence;

public sealed class EvidenceCatalogSnapshotWindow
{
    private readonly EvidenceCatalogSnapshotWindowEntry[] _entries;

    public EvidenceCatalogSnapshotWindow(
        IEnumerable<EvidenceCatalogSnapshotWindowEntry> entries)
    {
        ArgumentNullException.ThrowIfNull(entries);

        _entries=entries.ToArray();
    }

    public int Count=>_entries.Length;

    public IReadOnlyList<EvidenceCatalogSnapshotWindowEntry> Entries=>
        Array.AsReadOnly(_entries);
}
