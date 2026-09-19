namespace Asun.Platform.Evidence;

public static class EvidenceCatalogQueryResultFingerprintValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        EvidenceCatalogSnapshot snapshot,
        EvidenceCatalogQueryResult result,
        string fingerprint)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(result);

        var errors=new List<string>(
            EvidenceCatalogQueryResultValidationRuntime.Validate(snapshot,result));

        if(fingerprint is null ||
           fingerprint.Length!=64 ||
           !fingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Evidence catalog query result fingerprint must be 64 lowercase hexadecimal characters.");
        }
        else if(EvidenceCatalogQueryResultFingerprintRuntime.CreateFingerprint(result)!=fingerprint)
        {
            errors.Add("Evidence catalog query result fingerprint does not match the result.");
        }

        return errors;
    }

    public static bool IsValid(
        EvidenceCatalogSnapshot snapshot,
        EvidenceCatalogQueryResult result,
        string fingerprint)=>
        Validate(snapshot,result,fingerprint).Count==0;
}
