namespace Asun.Platform.Evidence;

public static class EvidenceReferenceResolutionFingerprintValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        EvidenceCatalogSnapshot snapshot,
        EvidenceReferenceSet referenceSet,
        EvidenceReferenceResolution resolution,
        string fingerprint)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(referenceSet);
        ArgumentNullException.ThrowIfNull(resolution);

        var errors=new List<string>(
            EvidenceReferenceResolutionValidationRuntime.Validate(
                snapshot,
                referenceSet,
                resolution));

        if(fingerprint is null ||
           fingerprint.Length!=64 ||
           !fingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Evidence reference resolution fingerprint must be 64 lowercase hexadecimal characters.");
        }
        else if(EvidenceReferenceResolutionFingerprintRuntime.CreateFingerprint(resolution)!=fingerprint)
        {
            errors.Add("Evidence reference resolution fingerprint does not match the resolution.");
        }

        return errors;
    }

    public static bool IsValid(
        EvidenceCatalogSnapshot snapshot,
        EvidenceReferenceSet referenceSet,
        EvidenceReferenceResolution resolution,
        string fingerprint)=>
        Validate(snapshot,referenceSet,resolution,fingerprint).Count==0;
}
