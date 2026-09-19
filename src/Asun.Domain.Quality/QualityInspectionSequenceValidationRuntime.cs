namespace Asun.Domain.Quality;

public static class QualityInspectionSequenceValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        QualityInspectionSequenceRelation relation)
    {
        ArgumentNullException.ThrowIfNull(relation);

        return Array.Empty<string>();
    }

    public static bool IsValid(
        QualityInspectionSequenceRelation relation) =>
        Validate(relation).Count == 0;

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
