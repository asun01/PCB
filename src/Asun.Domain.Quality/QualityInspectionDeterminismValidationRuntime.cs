namespace Asun.Domain.Quality;

public static class QualityInspectionDeterminismValidationRuntime
{
    public static IReadOnlyList<string> ValidateFingerprint(string fingerprint)
    {
        ArgumentNullException.ThrowIfNull(fingerprint);

        var errors = new List<string>();

        if (fingerprint.Length != 64)
            errors.Add("Inspection content fingerprint must be 64 hexadecimal characters.");

        if (fingerprint.Any(character =>
                !Uri.IsHexDigit(character)))
        {
            errors.Add("Inspection content fingerprint must contain only hexadecimal characters.");
        }

        return errors;
    }

    public static bool IsValidFingerprint(string fingerprint) =>
        ValidateFingerprint(fingerprint).Count == 0;

    public static IReadOnlyList<string> ValidateSnapshot(
        QualityInspectionSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var errors = new List<string>();

        if (!QualityInspectionSnapshotValidationRuntime.IsValid(snapshot))
        {
            errors.Add("Inspection snapshot is invalid.");
            return errors;
        }

        var fingerprint = QualityInspectionDeterminismRuntime
            .CreateContentFingerprint(snapshot);

        errors.AddRange(ValidateFingerprint(fingerprint));
        return errors;
    }

    public static bool IsValidSnapshot(
        QualityInspectionSnapshot snapshot) =>
        ValidateSnapshot(snapshot).Count == 0;
}
