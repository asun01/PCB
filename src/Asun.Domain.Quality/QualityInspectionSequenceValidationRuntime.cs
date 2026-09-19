namespace Asun.Domain.Quality;

public static class QualityInspectionSequenceValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        QualityInspectionSnapshot previous,
        QualityInspectionSnapshot current,
        QualityInspectionSequenceRelation relation)
    {
        ArgumentNullException.ThrowIfNull(previous);
        ArgumentNullException.ThrowIfNull(current);
        ArgumentNullException.ThrowIfNull(relation);

        var errors = new List<string>();

        if (!QualityInspectionSnapshotValidationRuntime.IsValid(previous))
            errors.Add("Previous inspection snapshot is invalid.");

        if (!QualityInspectionSnapshotValidationRuntime.IsValid(current))
            errors.Add("Current inspection snapshot is invalid.");

        if (relation.Delta != current.Sequence - previous.Sequence)
            errors.Add("Sequence relation delta does not match snapshot sequences.");

        return errors;
    }

    public static bool IsValid(
        QualityInspectionSnapshot previous,
        QualityInspectionSnapshot current,
        QualityInspectionSequenceRelation relation) =>
        Validate(previous, current, relation).Count == 0;

    public static IReadOnlyList<string> Validate(
        QualityInspectionSnapshot previous,
        QualityInspectionSnapshot current)
    {
        ArgumentNullException.ThrowIfNull(previous);
        ArgumentNullException.ThrowIfNull(current);

        var errors = new List<string>();

        if (!QualityInspectionSnapshotValidationRuntime.IsValid(previous))
            errors.Add("Previous inspection snapshot is invalid.");

        if (!QualityInspectionSnapshotValidationRuntime.IsValid(current))
            errors.Add("Current inspection snapshot is invalid.");

        return errors;
    }

    public static bool IsValid(
        QualityInspectionSnapshot previous,
        QualityInspectionSnapshot current) =>
        Validate(previous, current).Count == 0;
}
