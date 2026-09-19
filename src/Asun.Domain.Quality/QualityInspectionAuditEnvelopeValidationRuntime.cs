namespace Asun.Domain.Quality;

public static class QualityInspectionAuditEnvelopeValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        QualityInspectionResult result,
        QualityInspectionAuditEnvelope envelope)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(envelope);

        var errors=new List<string>();

        if(envelope.ResultId!=result.ResultId)
            errors.Add("Audit envelope result id does not match the source result.");

        if(envelope.SnapshotId!=result.SnapshotId)
            errors.Add("Audit envelope snapshot id does not match the source result.");

        if(envelope.Sequence!=result.Sequence)
            errors.Add("Audit envelope sequence does not match the source result.");

        errors.AddRange(
            QualityInspectionAuditProjectionFingerprintValidationRuntime
                .ValidateProjection(
                    result,
                    envelope.Projection,
                    envelope.ProjectionFingerprint));

        return errors;
    }

    public static bool IsValid(
        QualityInspectionResult result,
        QualityInspectionAuditEnvelope envelope) =>
        Validate(result,envelope).Count==0;
}
