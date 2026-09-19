namespace Asun.Domain.Quality;

public static class QualityInspectionEvidenceSummaryValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        QualityInspectionResult result,
        QualityInspectionEvidenceSummary summary)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(summary);

        var errors = new List<string>();

        if (!QualityInspectionResultValidationRuntime.IsValid(result))
        {
            errors.Add("Inspection result is invalid.");
            return errors;
        }

        if (summary.LinkCount < 0 ||
            summary.DistinctEvidenceKeyCount < 0 ||
            summary.LinkedFindingCount < 0)
        {
            errors.Add("Inspection evidence summary counts cannot be negative.");
            return errors;
        }

        if (summary.LinkCount != result.Evidence.Count)
            errors.Add("Evidence link count does not match the inspection result.");

        var distinctKeys = result.Evidence.Links
            .Select(link => link.EvidenceKey)
            .Distinct()
            .Count();
        var linkedFindings = result.Evidence.Links
            .Select(link => link.FindingId)
            .Distinct()
            .Count();

        if (summary.DistinctEvidenceKeyCount != distinctKeys)
            errors.Add("Distinct evidence-key count does not match the inspection result.");

        if (summary.LinkedFindingCount != linkedFindings)
            errors.Add("Linked-finding count does not match the inspection result.");

        return errors;
    }

    public static bool IsValid(
        QualityInspectionResult result,
        QualityInspectionEvidenceSummary summary) =>
        Validate(result, summary).Count == 0;
}
