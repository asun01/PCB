namespace Asun.Domain.Quality;

public static class QualityInspectionFindingAuditReplayBundleRuntime
{
    public static QualityInspectionFindingAuditReplayBundle Create(
        QualityInspectionResult current,
        QualityInspectionResult? previous = null)
    {
        ArgumentNullException.ThrowIfNull(current);

        var currentProjection=
            QualityInspectionFindingAuditProjectionRuntime.Create(current);
        var previousProjection=previous is null
            ? null
            : QualityInspectionFindingAuditProjectionRuntime.Create(previous);
        var diff=previousProjection is null
            ? QualityInspectionFindingAuditProjectionDiffRuntime.Diff(
                currentProjection,
                currentProjection)
            : QualityInspectionFindingAuditProjectionDiffRuntime.Diff(
                previousProjection,
                currentProjection);

        return new QualityInspectionFindingAuditReplayBundle(
            previousProjection,
            currentProjection,
            diff);
    }
}
