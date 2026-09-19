namespace Asun.Domain.Quality;

public static class QualityInspectionSequenceRuntime
{
    public static QualityInspectionSequenceRelation Observe(
        QualityInspectionSnapshot previous,
        QualityInspectionSnapshot current)
    {
        ArgumentNullException.ThrowIfNull(previous);
        ArgumentNullException.ThrowIfNull(current);

        if (!QualityInspectionSnapshotValidationRuntime.IsValid(previous))
            throw new ArgumentException(
                "Previous inspection snapshot is invalid.",
                nameof(previous));

        if (!QualityInspectionSnapshotValidationRuntime.IsValid(current))
            throw new ArgumentException(
                "Current inspection snapshot is invalid.",
                nameof(current));

        return new QualityInspectionSequenceRelation(
            current.Sequence - previous.Sequence);
    }
}
