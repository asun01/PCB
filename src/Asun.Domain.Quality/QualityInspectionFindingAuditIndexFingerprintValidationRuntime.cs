namespace Asun.Domain.Quality;

public static class QualityInspectionFindingAuditIndexFingerprintValidationRuntime
{
    public static IReadOnlyList<string> ValidateFingerprint(
        string fingerprint)
    {
        ArgumentNullException.ThrowIfNull(fingerprint);

        var errors=new List<string>();

        if(fingerprint.Length!=64)
            errors.Add("Finding audit index fingerprint must be 64 hexadecimal characters.");

        if(fingerprint.Any(character=>!Uri.IsHexDigit(character)))
            errors.Add("Finding audit index fingerprint must contain only hexadecimal characters.");

        return errors;
    }

    public static bool IsValidFingerprint(string fingerprint) =>
        ValidateFingerprint(fingerprint).Count==0;

    public static IReadOnlyList<string> ValidateIndex(
        QualityInspectionResult result,
        QualityInspectionFindingAuditIndex index)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(index);

        var errors=QualityInspectionFindingAuditIndexValidationRuntime
            .Validate(result,index)
            .ToList();

        if(errors.Count>0)
            return errors;

        errors.AddRange(
            ValidateFingerprint(
                QualityInspectionFindingAuditIndexFingerprintRuntime
                    .CreateFingerprint(index)));

        return errors;
    }

    public static bool IsValidIndex(
        QualityInspectionResult result,
        QualityInspectionFindingAuditIndex index) =>
        ValidateIndex(result,index).Count==0;
}
