namespace Asun.Domain.Quality;

public static class QualityInspectionReplayBundleRuntime
{
    public static QualityInspectionReplayBundle Create(
        QualityInspectionResult current,
        QualityInspectionResult? previous = null)
    {
        ArgumentNullException.ThrowIfNull(current);

        var currentProjection =
            QualityInspectionReplayProjectionRuntime.Create(current);

        var previousProjection = previous is null
            ? null
            : QualityInspectionReplayProjectionRuntime.Create(previous);

        var diff = previousProjection is null
            ? QualityInspectionReplayProjectionDiffRuntime.Diff(
                currentProjection,
                currentProjection)
            : QualityInspectionReplayProjectionDiffRuntime.Diff(
                previousProjection,
                currentProjection);

        return new QualityInspectionReplayBundle(
            previousProjection,
            currentProjection,
            diff);
    }
}
