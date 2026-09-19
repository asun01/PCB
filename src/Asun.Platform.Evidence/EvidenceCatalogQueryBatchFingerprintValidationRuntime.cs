namespace Asun.Platform.Evidence;

public static class EvidenceCatalogQueryBatchFingerprintValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        EvidenceCatalogSnapshot snapshot,
        EvidenceCatalogQueryBatch batch,
        string fingerprint)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(batch);

        var errors=new List<string>(
            EvidenceCatalogQueryBatchValidationRuntime.Validate(snapshot,batch));

        if(fingerprint is null ||
           fingerprint.Length!=64 ||
           !fingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Evidence query batch fingerprint must be 64 lowercase hexadecimal characters.");
        }
        else if(EvidenceCatalogQueryBatchFingerprintRuntime.CreateFingerprint(batch)!=fingerprint)
        {
            errors.Add("Evidence query batch fingerprint does not match the batch.");
        }

        return errors;
    }

    public static bool IsValid(
        EvidenceCatalogSnapshot snapshot,
        EvidenceCatalogQueryBatch batch,
        string fingerprint)=>
        Validate(snapshot,batch,fingerprint).Count==0;
}
