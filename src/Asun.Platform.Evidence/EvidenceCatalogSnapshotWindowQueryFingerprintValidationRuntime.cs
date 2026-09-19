namespace Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotWindowQueryFingerprintValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        EvidenceCatalogSnapshotWindow window,
        EvidenceCatalogSnapshotWindowQueryResultSet resultSet,
        string fingerprint)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(resultSet);

        var errors=new List<string>(
            EvidenceCatalogSnapshotWindowQueryValidationRuntime.Validate(
                window,
                resultSet));

        if(fingerprint is null ||
           fingerprint.Length!=64 ||
           !fingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Window query fingerprint must be 64 lowercase hexadecimal characters.");
        }
        else if(EvidenceCatalogSnapshotWindowQueryFingerprintRuntime.CreateFingerprint(resultSet)!=fingerprint)
        {
            errors.Add("Window query fingerprint does not match the result set.");
        }

        return errors;
    }

    public static bool IsValid(
        EvidenceCatalogSnapshotWindow window,
        EvidenceCatalogSnapshotWindowQueryResultSet resultSet,
        string fingerprint)=>
        Validate(window,resultSet,fingerprint).Count==0;
}
