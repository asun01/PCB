namespace Asun.Platform.Evidence;

public static class EvidenceCatalogStatisticsFingerprintValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        EvidenceCatalogSnapshot snapshot,
        EvidenceCatalogStatistics statistics,
        string fingerprint)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(statistics);

        var errors=new List<string>(
            EvidenceCatalogStatisticsValidationRuntime.Validate(
                snapshot,
                statistics));

        if(fingerprint is null ||
           fingerprint.Length!=64 ||
           !fingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Evidence catalog statistics fingerprint must be 64 lowercase hexadecimal characters.");
        }
        else if(EvidenceCatalogStatisticsFingerprintRuntime.CreateFingerprint(statistics)!=fingerprint)
        {
            errors.Add("Evidence catalog statistics fingerprint does not match the statistics.");
        }

        return errors;
    }

    public static bool IsValid(
        EvidenceCatalogSnapshot snapshot,
        EvidenceCatalogStatistics statistics,
        string fingerprint)=>
        Validate(snapshot,statistics,fingerprint).Count==0;
}
