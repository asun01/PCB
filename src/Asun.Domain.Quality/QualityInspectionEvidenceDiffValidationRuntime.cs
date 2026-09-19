namespace Asun.Domain.Quality;

public static class QualityInspectionEvidenceDiffValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        QualityInspectionEvidenceDiff diff)
    {
        ArgumentNullException.ThrowIfNull(diff);

        var errors = new List<string>();

        ValidateLinks(diff.AddedLinks, "Added evidence links", errors);
        ValidateLinks(diff.RemovedLinks, "Removed evidence links", errors);

        if (diff.AddedLinks.Intersect(diff.RemovedLinks).Any())
            errors.Add("An evidence link cannot be both added and removed.");

        return errors;
    }

    public static bool IsValid(QualityInspectionEvidenceDiff diff) =>
        Validate(diff).Count == 0;

    private static void ValidateLinks(
        IReadOnlyList<QualityFindingEvidenceLink> links,
        string label,
        List<string> errors)
    {
        var seen = new HashSet<QualityFindingEvidenceLink>();

        foreach (var link in links)
        {
            if (!seen.Add(link))
                errors.Add($"{label} must contain unique links.");

            if (!QualityFindingEvidenceValidationRuntime.IsValid(link))
                errors.Add($"{label} must contain only valid links.");
        }
    }
}
