namespace Asun.Domain.Quality;

public static class QualityInspectionAuditEnvelopeRuntime
{
    public static QualityInspectionAuditEnvelope Create(
        QualityInspectionResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        var projection=QualityInspectionAuditProjectionRuntime.Create(result);
        var fingerprint=
            QualityInspectionAuditProjectionFingerprintRuntime
                .CreateFingerprint(projection);

        return new QualityInspectionAuditEnvelope(
            result.ResultId,
            result.SnapshotId,
            result.Sequence,
            projection,
            fingerprint);
    }
}
