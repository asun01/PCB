namespace Asun.Platform.Evidence;

public static class EvidenceCatalogWindowDiagnosticBundleFingerprintValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        EvidenceCatalogSnapshotWindow window,
        EvidenceCatalogWindowDiagnosticBundle bundle,
        string fingerprint)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(bundle);

        var errors=new List<string>(
            EvidenceCatalogWindowDiagnosticBundleValidationRuntime.Validate(
                window,
                bundle));

        if(fingerprint is null ||
           fingerprint.Length!=64 ||
           !fingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Window diagnostic bundle fingerprint must be 64 lowercase hexadecimal characters.");
        }
        else if(EvidenceCatalogWindowDiagnosticBundleFingerprintRuntime.CreateFingerprint(bundle)!=fingerprint)
        {
            errors.Add("Window diagnostic bundle fingerprint does not match the bundle.");
        }

        return errors;
    }

    public static bool IsValid(
        EvidenceCatalogSnapshotWindow window,
        EvidenceCatalogWindowDiagnosticBundle bundle,
        string fingerprint)=>
        Validate(window,bundle,fingerprint).Count==0;
}
