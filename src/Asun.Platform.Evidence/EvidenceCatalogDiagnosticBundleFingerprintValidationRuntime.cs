namespace Asun.Platform.Evidence;

public static class EvidenceCatalogDiagnosticBundleFingerprintValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        EvidenceCatalogSnapshot snapshot,
        EvidenceCatalogDiagnosticBundle bundle,
        string fingerprint)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(bundle);

        var errors=new List<string>(
            EvidenceCatalogDiagnosticBundleValidationRuntime.Validate(
                snapshot,
                bundle));

        if(fingerprint is null ||
           fingerprint.Length!=64 ||
           !fingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Evidence diagnostic bundle fingerprint must be 64 lowercase hexadecimal characters.");
        }
        else if(EvidenceCatalogDiagnosticBundleFingerprintRuntime.CreateFingerprint(bundle)!=fingerprint)
        {
            errors.Add("Evidence diagnostic bundle fingerprint does not match the bundle.");
        }

        return errors;
    }

    public static bool IsValid(
        EvidenceCatalogSnapshot snapshot,
        EvidenceCatalogDiagnosticBundle bundle,
        string fingerprint)=>
        Validate(snapshot,bundle,fingerprint).Count==0;
}
