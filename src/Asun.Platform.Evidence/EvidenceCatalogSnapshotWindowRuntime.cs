namespace Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotWindowRuntime
{
    public static EvidenceCatalogSnapshotWindow Create(
        IEnumerable<EvidenceCatalogSnapshotWindowEntry> entries)
    {
        ArgumentNullException.ThrowIfNull(entries);

        var validated=entries.ToArray();

        foreach(var entry in validated)
        {
            if(entry.Sequence<=0)
                throw new ArgumentException(
                    "Evidence snapshot window sequences must be positive.",
                    nameof(entries));

            if(!EvidenceCatalogSnapshotEnvelopeValidationRuntime.IsValid(entry.Envelope))
                throw new ArgumentException(
                    "Evidence snapshot window cannot contain invalid envelopes.",
                    nameof(entries));
        }

        if(validated.GroupBy(entry=>entry.Sequence).Any(group=>group.Count()>1))
            throw new ArgumentException(
                "Evidence snapshot window sequences must be unique.",
                nameof(entries));

        return new EvidenceCatalogSnapshotWindow(
            validated
                .OrderBy(entry=>entry.Sequence)
                .ToArray());
    }
}
