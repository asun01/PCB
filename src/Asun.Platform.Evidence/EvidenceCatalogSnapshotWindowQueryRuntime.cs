namespace Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotWindowQueryRuntime
{
    public static EvidenceCatalogSnapshotWindowQueryResultSet Execute(
        EvidenceCatalogSnapshotWindow window,
        EvidenceDescriptorQuery query)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(query);

        if(!EvidenceCatalogSnapshotWindowValidationRuntime.IsValid(window))
            throw new ArgumentException(
                "Evidence catalog snapshot window is invalid.",
                nameof(window));

        if(!EvidenceDescriptorQueryValidationRuntime.IsValid(query))
            throw new ArgumentException(
                "Evidence descriptor query is invalid.",
                nameof(query));

        var results=window.Entries
            .Select(entry=>new EvidenceCatalogSnapshotWindowQueryResult(
                entry.Sequence,
                EvidenceCatalogQueryResultRuntime.Execute(
                    entry.Envelope.Snapshot,
                    query)))
            .ToArray();

        return new EvidenceCatalogSnapshotWindowQueryResultSet(
            EvidenceCatalogSnapshotWindowIntegritySummaryRuntime.Create(window).WindowFingerprint,
            query,
            results,
            results.Length);
    }
}
