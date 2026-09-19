namespace Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotFingerprintValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        EvidenceCatalogSnapshot snapshot,
        string fingerprint)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var errors=new List<string>(
            EvidenceCatalogSnapshotValidationRuntime.Validate(snapshot));

        if(!IsValidFingerprint(fingerprint))
            errors.Add("Evidence catalog snapshot fingerprint must be 64 lowercase hexadecimal characters.");
        else if(EvidenceCatalogSnapshotFingerprintRuntime.CreateFingerprint(snapshot)!=fingerprint)
            errors.Add("Evidence catalog snapshot fingerprint does not match the snapshot.");

        return errors;
    }

    public static bool IsValid(
        EvidenceCatalogSnapshot snapshot,
        string fingerprint)=>
        Validate(snapshot,fingerprint).Count==0;

    public static bool IsValidFingerprint(string? fingerprint)=>
        fingerprint is not null &&
        fingerprint.Length==64 &&
        fingerprint.All(character=>
            Uri.IsHexDigit(character) &&
            char.ToLowerInvariant(character)==character);
}
