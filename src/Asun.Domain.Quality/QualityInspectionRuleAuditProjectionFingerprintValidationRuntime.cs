namespace Asun.Domain.Quality;

public static class QualityInspectionRuleAuditProjectionFingerprintValidationRuntime
{
    public static IReadOnlyList<string> ValidateFingerprint(
        string fingerprint)
    {
        ArgumentNullException.ThrowIfNull(fingerprint);

        var errors=new List<string>();

        if(fingerprint.Length!=64)
            errors.Add("Rule audit projection fingerprint must be 64 hexadecimal characters.");

        if(fingerprint.Any(character=>!Uri.IsHexDigit(character)))
            errors.Add("Rule audit projection fingerprint must contain only hexadecimal characters.");

        return errors;
    }

    public static bool IsValidFingerprint(string fingerprint) =>
        ValidateFingerprint(fingerprint).Count==0;

    public static IReadOnlyList<string> ValidateProjection(
        QualityInspectionResult result,
        QualityInspectionRuleAuditProjection projection)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(projection);

        var errors=QualityInspectionRuleAuditProjectionValidationRuntime
            .Validate(result,projection)
            .ToList();

        if(errors.Count>0)
            return errors;

        errors.AddRange(
            ValidateFingerprint(
                QualityInspectionRuleAuditProjectionFingerprintRuntime
                    .CreateFingerprint(projection)));

        return errors;
    }

    public static bool IsValidProjection(
        QualityInspectionResult result,
        QualityInspectionRuleAuditProjection projection) =>
        ValidateProjection(result,projection).Count==0;
}
