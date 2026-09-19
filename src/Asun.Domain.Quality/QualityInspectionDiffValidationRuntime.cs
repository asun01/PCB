namespace Asun.Domain.Quality;

public static class QualityInspectionDiffValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        QualityInspectionDiff diff)
    {
        ArgumentNullException.ThrowIfNull(diff);

        var errors = new List<string>();

        ValidateUnique(
            diff.AddedFindingIds,
            "Added finding ids",
            errors);
        ValidateUnique(
            diff.RemovedFindingIds,
            "Removed finding ids",
            errors);
        ValidateUnique(
            diff.ChangedFindingIds,
            "Changed finding ids",
            errors);
        ValidateUnique(
            diff.AddedEvidenceKeys,
            "Added evidence keys",
            errors);
        ValidateUnique(
            diff.RemovedEvidenceKeys,
            "Removed evidence keys",
            errors);
        ValidateUnique(
            diff.RelinkedEvidenceKeys,
            "Relinked evidence keys",
            errors);

        ValidateFindingIds(diff.AddedFindingIds, "Added finding ids", errors);
        ValidateFindingIds(diff.RemovedFindingIds, "Removed finding ids", errors);
        ValidateFindingIds(diff.ChangedFindingIds, "Changed finding ids", errors);
        ValidateEvidenceKeys(diff.AddedEvidenceKeys, "Added evidence keys", errors);
        ValidateEvidenceKeys(diff.RemovedEvidenceKeys, "Removed evidence keys", errors);
        ValidateEvidenceKeys(diff.RelinkedEvidenceKeys, "Relinked evidence keys", errors);

        if (diff.AddedFindingIds.Intersect(diff.RemovedFindingIds).Any())
            errors.Add("A finding cannot be both added and removed.");

        if (diff.AddedFindingIds.Intersect(diff.ChangedFindingIds).Any() ||
            diff.RemovedFindingIds.Intersect(diff.ChangedFindingIds).Any())
        {
            errors.Add("A finding cannot be both added/removed and changed.");
        }

        if (diff.AddedEvidenceKeys.Intersect(diff.RemovedEvidenceKeys).Any())
            errors.Add("An evidence key cannot be both added and removed.");

        if (diff.RelinkedEvidenceKeys.Intersect(diff.AddedEvidenceKeys).Any() ||
            diff.RelinkedEvidenceKeys.Intersect(diff.RemovedEvidenceKeys).Any())
        {
            errors.Add(
                "A relinked evidence key cannot also be newly added or removed.");
        }

        return errors;
    }

    public static bool IsValid(QualityInspectionDiff diff) =>
        Validate(diff).Count == 0;

    private static void ValidateUnique<T>(
        IReadOnlyList<T> values,
        string label,
        List<string> errors)
        where T : notnull
    {
        if (values.Count != values.Distinct().Count())
            errors.Add($"{label} must contain unique values.");
    }

    private static void ValidateFindingIds(
        IReadOnlyList<QualityFindingId> values,
        string label,
        List<string> errors)
    {
        if (values.Any(id => !id.IsValid))
            errors.Add($"{label} must contain only valid ids.");
    }

    private static void ValidateEvidenceKeys(
        IReadOnlyList<QualityEvidenceKey> values,
        string label,
        List<string> errors)
    {
        if (values.Any(key => !key.IsValid))
            errors.Add($"{label} must contain only valid evidence keys.");
    }
}
