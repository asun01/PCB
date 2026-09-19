namespace Asun.Domain.Quality;

public static class QualityInspectionFindingAuditEnvelopeRuntime
{
    public static QualityInspectionFindingAuditEnvelope Create(
        QualityInspectionResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        var projection=
            QualityInspectionFindingAuditProjectionRuntime.Create(result);

        return new QualityInspectionFindingAuditEnvelope(
            result.ResultId,
            result.SnapshotId,
            result.Sequence,
            projection,
            projection.ContentFingerprint);
    }
}
