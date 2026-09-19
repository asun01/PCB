namespace Asun.Domain.Quality;

public static class QualityInspectionFindingAuditEnvelopeValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        QualityInspectionResult result,
        QualityInspectionFindingAuditEnvelope envelope)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(envelope);

        var errors=new List<string>();

        if(envelope.ResultId!=result.ResultId)
            errors.Add("Finding audit envelope result id does not match the source result.");

        if(envelope.SnapshotId!=result.SnapshotId)
            errors.Add("Finding audit envelope snapshot id does not match the source result.");

        if(envelope.Sequence!=result.Sequence)
            errors.Add("Finding audit envelope sequence does not match the source result.");

        errors.AddRange(
            QualityInspectionFindingAuditProjectionValidationRuntime
                .Validate(result,envelope.Projection));

        errors.AddRange(
            QualityInspectionFindingAuditIndexFingerprintValidationRuntime
                .ValidateFingerprint(envelope.ProjectionFingerprint));

        if(errors.Count==0 &&
           !string.Equals(
               envelope.ProjectionFingerprint,
               envelope.Projection.ContentFingerprint,
               StringComparison.Ordinal))
        {
            errors.Add(
                "Finding audit envelope fingerprint does not match the projection.");
        }

        return errors;
    }

    public static bool IsValid(
        QualityInspectionResult result,
        QualityInspectionFindingAuditEnvelope envelope) =>
        Validate(result,envelope).Count==0;
}
