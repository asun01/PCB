namespace Asun.Platform.Evidence;

public static class EvidenceCatalogQueryBatchValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        EvidenceCatalogSnapshot snapshot,
        EvidenceCatalogQueryBatch batch)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(batch);

        var errors=new List<string>();

        var snapshotFingerprint=
            EvidenceCatalogSnapshotFingerprintRuntime.CreateFingerprint(snapshot);

        if(batch.SnapshotFingerprint!=snapshotFingerprint)
            errors.Add("Evidence query batch snapshot fingerprint does not match the source snapshot.");

        if(batch.QueryCount!=batch.Results.Count)
            errors.Add("Evidence query batch query count must match the result count.");

        var resultFingerprints=new List<string>();

        foreach(var result in batch.Results)
        {
            errors.AddRange(
                EvidenceCatalogQueryResultValidationRuntime.Validate(
                    snapshot,
                    result));

            resultFingerprints.Add(
                EvidenceCatalogQueryResultFingerprintRuntime.CreateFingerprint(result));
        }

        if(resultFingerprints.Count!=resultFingerprints.Distinct(StringComparer.Ordinal).Count())
            errors.Add("Evidence query batch result fingerprints must be unique.");

        return errors;
    }

    public static bool IsValid(
        EvidenceCatalogSnapshot snapshot,
        EvidenceCatalogQueryBatch batch)=>
        Validate(snapshot,batch).Count==0;
}
