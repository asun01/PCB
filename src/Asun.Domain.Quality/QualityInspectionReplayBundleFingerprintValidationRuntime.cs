namespace Asun.Domain.Quality;

public static class QualityInspectionReplayBundleFingerprintValidationRuntime
{
    public static IReadOnlyList<string> ValidateFingerprint(
        string fingerprint)
    {
        ArgumentNullException.ThrowIfNull(fingerprint);

        var errors = new List<string>();

        if (fingerprint.Length != 64)
            errors.Add("Replay bundle fingerprint must be 64 hexadecimal characters.");

        if (fingerprint.Any(character => !Uri.IsHexDigit(character)))
            errors.Add("Replay bundle fingerprint must contain only hexadecimal characters.");

        return errors;
    }

    public static bool IsValidFingerprint(
        string fingerprint) =>
        ValidateFingerprint(fingerprint).Count == 0;

    public static IReadOnlyList<string> ValidateBundle(
        QualityInspectionReplayBundle bundle)
    {
        ArgumentNullException.ThrowIfNull(bundle);

        var errors = QualityInspectionReplayBundleValidationRuntime
            .Validate(bundle)
            .ToList();

        if (errors.Count > 0)
            return errors;

        errors.AddRange(
            ValidateFingerprint(
                QualityInspectionReplayBundleFingerprintRuntime
                    .CreateFingerprint(bundle)));

        return errors;
    }

    public static bool IsValidBundle(
        QualityInspectionReplayBundle bundle) =>
        ValidateBundle(bundle).Count == 0;
}
