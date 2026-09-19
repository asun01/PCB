namespace Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotWindowDiffRuntime
{
    public static EvidenceCatalogSnapshotWindowDiff Diff(
        EvidenceCatalogSnapshotWindow previous,
        EvidenceCatalogSnapshotWindow current)
    {
        ArgumentNullException.ThrowIfNull(previous);
        ArgumentNullException.ThrowIfNull(current);

        var previousBySequence=previous.Entries
            .ToDictionary(entry=>entry.Sequence);
        var currentBySequence=current.Entries
            .ToDictionary(entry=>entry.Sequence);

        var added=currentBySequence.Keys
            .Except(previousBySequence.Keys)
            .OrderBy(sequence=>sequence)
            .ToArray();
        var removed=previousBySequence.Keys
            .Except(currentBySequence.Keys)
            .OrderBy(sequence=>sequence)
            .ToArray();
        var changed=currentBySequence.Keys
            .Intersect(previousBySequence.Keys)
            .Where(sequence=>
                currentBySequence[sequence].Envelope.Fingerprint!=
                previousBySequence[sequence].Envelope.Fingerprint)
            .OrderBy(sequence=>sequence)
            .ToArray();

        return new EvidenceCatalogSnapshotWindowDiff(
            added,
            removed,
            changed);
    }
}
