namespace Asun.Domain.Quality;

public static class QualityInspectionChainValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        QualityInspectionResult previous,
        QualityInspectionResult current)
    {
        ArgumentNullException.ThrowIfNull(previous);
        ArgumentNullException.ThrowIfNull(current);

        var errors = new List<string>();

        if (!QualityInspectionResultValidationRuntime.IsValid(previous))
            errors.Add("Previous inspection result is invalid.");

        if (!QualityInspectionResultValidationRuntime.IsValid(current))
            errors.Add("Current inspection result is invalid.");

        if (previous.ResultId == current.ResultId)
            errors.Add("Inspection result ids must be distinct in a chain.");

        if (previous.SnapshotId == current.SnapshotId)
            errors.Add("Inspection snapshot ids must be distinct in a chain.");

        if (errors.Count > 0)
            return errors;

        var relation = QualityInspectionSequenceRuntime.Observe(
            previous.Snapshot,
            current.Snapshot);

        errors.AddRange(
            QualityInspectionSequenceValidationRuntime.Validate(
                previous.Snapshot,
                current.Snapshot,
                relation));

        errors.AddRange(
            QualityInspectionDiffValidationRuntime.Validate(
                QualityInspectionDiffRuntime.Diff(
                    previous.Snapshot,
                    current.Snapshot)));

        errors.AddRange(
            QualityInspectionEvidenceDiffValidationRuntime.Validate(
                QualityInspectionEvidenceDiffRuntime.Diff(
                    previous.Snapshot,
                    current.Snapshot)));

        errors.AddRange(
            QualityInspectionAuditValidationRuntime.Validate(
                QualityInspectionAuditRuntime.Create(previous)));

        errors.AddRange(
            QualityInspectionAuditValidationRuntime.Validate(
                QualityInspectionAuditRuntime.Create(current)));

        return errors;
    }

    public static bool IsValid(
        QualityInspectionResult previous,
        QualityInspectionResult current) =>
        Validate(previous, current).Count == 0;
}
