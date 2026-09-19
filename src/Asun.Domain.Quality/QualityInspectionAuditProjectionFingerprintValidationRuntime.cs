namespace Asun.Domain.Quality;

public static class QualityInspectionAuditProjectionFingerprintValidationRuntime
{
    public static IReadOnlyList<string> ValidateFingerprint(
        string fingerprint)
    {
        ArgumentNullException.ThrowIfNull(fingerprint);

        var errors=new List<string>();

        if(fingerprint.Length!=64)
            errors.Add("Top-level audit projection fingerprint must be 64 hexadecimal characters.");

        if(fingerprint.Any(character=>!Uri.IsHexDigit(character)))
            errors.Add("Top-level audit projection fingerprint must contain only hexadecimal characters.");

        return errors;
    }

    public static IReadOnlyList<string> ValidateProjection(
        QualityInspectionResult result,
        QualityInspectionAuditProjection projection,
        string fingerprint)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(projection);
        ArgumentNullException.ThrowIfNull(fingerprint);

        var errors=QualityInspectionAuditProjectionValidationRuntime
            .Validate(result,projection)
            .ToList();

        errors.AddRange(ValidateFingerprint(fingerprint));

        if(errors.Count==0)
        {
            var expected=
                QualityInspectionAuditProjectionFingerprintRuntime
                    .CreateFingerprint(projection);

            if(!string.Equals(expected,fingerprint,StringComparison.Ordinal))
                errors.Add("Top-level audit projection fingerprint does not match the projection.");
        }

        return errors;
    }

    public static bool IsValidProjection(
        QualityInspectionResult result,
        QualityInspectionAuditProjection projection,
        string fingerprint) =>
        ValidateProjection(result,projection,fingerprint).Count==0;
}
