namespace Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotWindowReferenceResolutionFingerprintValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        EvidenceCatalogSnapshotWindow window,
        EvidenceCatalogSnapshotWindowReferenceResolution result,
        string fingerprint)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(result);

        var errors=new List<string>(
            EvidenceCatalogSnapshotWindowReferenceResolutionValidationRuntime.Validate(
                window,
                result));

        if(fingerprint is null ||
           fingerprint.Length!=64 ||
           !fingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Window reference resolution fingerprint must be 64 lowercase hexadecimal characters.");
        }
        else if(EvidenceCatalogSnapshotWindowReferenceResolutionFingerprintRuntime.CreateFingerprint(result)!=fingerprint)
        {
            errors.Add("Window reference resolution fingerprint does not match the result.");
        }

        return errors;
    }

    public static bool IsValid(
        EvidenceCatalogSnapshotWindow window,
        EvidenceCatalogSnapshotWindowReferenceResolution result,
        string fingerprint)=>
        Validate(window,result,fingerprint).Count==0;
}
