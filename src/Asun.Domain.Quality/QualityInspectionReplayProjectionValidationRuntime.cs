namespace Asun.Domain.Quality;

public static class QualityInspectionReplayProjectionValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        QualityInspectionReplayProjection projection)
    {
        ArgumentNullException.ThrowIfNull(projection);

        var errors = new List<string>();

        if (projection.ResultId == Guid.Empty)
            errors.Add("Replay projection result id cannot be empty.");

        if (projection.SnapshotId == Guid.Empty)
            errors.Add("Replay projection snapshot id cannot be empty.");

        if (projection.Sequence < 0)
            errors.Add("Replay projection sequence cannot be negative.");

        var findingIds = new HashSet<QualityFindingId>();
        foreach (var findingId in projection.FindingIds)
        {
            if (!findingId.IsValid)
                errors.Add("Replay projection finding ids must be valid.");

            if (!findingIds.Add(findingId))
                errors.Add("Replay projection finding ids must be unique.");
        }

        foreach (var link in projection.EvidenceManifest.Links)
        {
            if (!findingIds.Contains(link.FindingId))
                errors.Add("Replay projection evidence points to an unknown finding.");

            if (!QualityFindingEvidenceValidationRuntime.IsValid(link))
                errors.Add("Replay projection contains an invalid evidence link.");
        }

        errors.AddRange(
            QualityInspectionDeterminismValidationRuntime
                .ValidateFingerprint(projection.ContentFingerprint));

        return errors;
    }

    public static bool IsValid(
        QualityInspectionReplayProjection projection) =>
        Validate(projection).Count == 0;
}
