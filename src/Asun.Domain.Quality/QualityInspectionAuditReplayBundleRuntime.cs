namespace Asun.Domain.Quality;

public static class QualityInspectionAuditReplayBundleRuntime
{
    public static QualityInspectionAuditReplayBundle Create(
        QualityInspectionResult current,
        QualityInspectionResult? previous = null)
    {
        ArgumentNullException.ThrowIfNull(current);

        var currentProjection=QualityInspectionAuditProjectionRuntime.Create(current);
        var previousProjection=previous is null
            ? null
            : QualityInspectionAuditProjectionRuntime.Create(previous);

        var diff=previousProjection is null
            ? QualityInspectionAuditWindowDiffRuntime.Diff(
                new QualityInspectionAuditWindow(new[]{
                    QualityInspectionAuditEnvelopeRuntime.Create(current)}),
                new QualityInspectionAuditWindow(new[]{
                    QualityInspectionAuditEnvelopeRuntime.Create(current)}))
            : QualityInspectionAuditWindowDiffRuntime.Diff(
                new QualityInspectionAuditWindow(new[]{
                    QualityInspectionAuditEnvelopeRuntime.Create(previous!)}),
                new QualityInspectionAuditWindow(new[]{
                    QualityInspectionAuditEnvelopeRuntime.Create(current)}));

        return new QualityInspectionAuditReplayBundle(
            previousProjection,
            currentProjection,
            diff);
    }
}
