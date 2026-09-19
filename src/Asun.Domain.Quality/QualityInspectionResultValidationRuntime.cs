namespace Asun.Domain.Quality;

public static class QualityInspectionResultValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        QualityInspectionResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        var errors = new List<string>();

        if (result.ResultId == Guid.Empty)
            errors.Add("Inspection result id cannot be empty.");

        if (!QualityInspectionSnapshotValidationRuntime.IsValid(result.Snapshot))
            errors.Add("Inspection result snapshot is invalid.");

        return errors;
    }

    public static bool IsValid(QualityInspectionResult result) =>
        Validate(result).Count == 0;
}
