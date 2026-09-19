namespace Asun.Domain.Quality;

public static class QualityInspectionFindingAuditProjectionValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        QualityInspectionResult result,
        QualityInspectionFindingAuditProjection projection)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(projection);

        var errors=QualityInspectionFindingAuditIndexFingerprintValidationRuntime
            .ValidateIndex(result,projection.Index)
            .ToList();

        errors.AddRange(
            QualityInspectionFindingAuditIndexFingerprintValidationRuntime
                .ValidateFingerprint(projection.ContentFingerprint));

        if(errors.Count==0)
        {
            var expected=
                QualityInspectionFindingAuditIndexFingerprintRuntime
                    .CreateFingerprint(projection.Index);

            if(!string.Equals(
                    expected,
                    projection.ContentFingerprint,
                    StringComparison.Ordinal))
            {
                errors.Add(
                    "Finding audit projection fingerprint does not match the index.");
            }
        }

        return errors;
    }

    public static bool IsValid(
        QualityInspectionResult result,
        QualityInspectionFindingAuditProjection projection) =>
        Validate(result,projection).Count==0;
}
