namespace Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotWindowReferenceResolutionRuntime
{
    public static EvidenceCatalogSnapshotWindowReferenceResolution Resolve(
        EvidenceCatalogSnapshotWindow window,
        EvidenceReferenceSet referenceSet)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(referenceSet);

        if(!EvidenceCatalogSnapshotWindowValidationRuntime.IsValid(window))
            throw new ArgumentException(
                "Evidence catalog snapshot window is invalid.",
                nameof(window));

        if(!EvidenceReferenceSetValidationRuntime.IsValid(referenceSet))
            throw new ArgumentException(
                "Evidence reference set is invalid.",
                nameof(referenceSet));

        var entries=window.Entries
            .Select(entry=>new EvidenceCatalogSnapshotWindowReferenceResolutionEntry(
                entry.Sequence,
                EvidenceReferenceResolutionRuntime.Resolve(
                    entry.Envelope.Snapshot,
                    referenceSet)))
            .ToArray();

        return new EvidenceCatalogSnapshotWindowReferenceResolution(
            EvidenceCatalogSnapshotWindowIntegritySummaryRuntime.Create(window).WindowFingerprint,
            referenceSet,
            entries);
    }
}
