namespace Asun.Platform.Evidence;

public static class EvidenceCatalogQueryBatchRuntime
{
    public static EvidenceCatalogQueryBatch Execute(
        EvidenceCatalogSnapshot snapshot,
        IEnumerable<EvidenceDescriptorQuery> queries)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(queries);

        var materialized=queries.ToArray();

        foreach(var query in materialized)
        {
            if(!EvidenceDescriptorQueryValidationRuntime.IsValid(query))
                throw new ArgumentException(
                    "Evidence query batch cannot contain invalid queries.",
                    nameof(queries));
        }

        var results=materialized
            .Select(query=>EvidenceCatalogQueryResultRuntime.Execute(snapshot,query))
            .ToArray();

        return new EvidenceCatalogQueryBatch(
            EvidenceCatalogSnapshotFingerprintRuntime.CreateFingerprint(snapshot),
            results,
            results.Length);
    }
}
