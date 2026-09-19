namespace Asun.Domain.Quality;

public static class QualityInspectionSnapshotValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        QualityInspectionSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var errors = new List<string>();

        if (snapshot.SnapshotId == Guid.Empty)
            errors.Add("Snapshot id cannot be empty.");

        if (snapshot.Sequence < 0)
            errors.Add("Snapshot sequence cannot be negative.");

        if (!QualityFindingSetValidationRuntime.IsValid(snapshot.Findings))
            errors.Add("Snapshot finding set is invalid.");

        if (!QualityFindingEvidenceValidationRuntime.IsValid(
                snapshot.Findings,
                snapshot.Evidence))
        {
            errors.Add("Snapshot evidence links contain an unknown finding.");
        }

        if (!QualityFindingEvidenceValidationRuntime.IsValid(snapshot.Evidence))
            errors.Add("Snapshot evidence link set is invalid.");

        return errors;
    }

    public static bool IsValid(QualityInspectionSnapshot snapshot) =>
        Validate(snapshot).Count == 0;
}
