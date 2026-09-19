namespace Asun.Domain.Quality;

public static class QualityInspectionFindingAuditProjectionRuntime
{
    public static QualityInspectionFindingAuditProjection Create(
        QualityInspectionResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        var index=QualityInspectionFindingAuditIndexRuntime.Create(result);

        return new QualityInspectionFindingAuditProjection(
            index,
            QualityInspectionFindingAuditIndexFingerprintRuntime
                .CreateFingerprint(index));
    }
}
