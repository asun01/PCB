namespace Asun.Domain.Quality;

public static class QualityInspectionSummaryValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        QualityInspectionResult result,
        QualityInspectionSummary summary)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(summary);

        var errors = new List<string>();

        if (!QualityInspectionResultValidationRuntime.IsValid(result))
        {
            errors.Add("Inspection result is invalid.");
            return errors;
        }

        if (summary.ResultId != result.ResultId)
            errors.Add("Summary result id does not match the inspection result.");

        if (summary.SnapshotId != result.SnapshotId)
            errors.Add("Summary snapshot id does not match the inspection result.");

        if (summary.Sequence != result.Sequence)
            errors.Add("Summary sequence does not match the inspection result.");

        errors.AddRange(
            QualityInspectionOutcomeSummaryValidationRuntime.Validate(
                result,
                summary.Outcomes));

        errors.AddRange(
            QualityInspectionSeveritySummaryValidationRuntime.Validate(
                result,
                summary.Severities));

        errors.AddRange(
            QualityInspectionEvidenceSummaryValidationRuntime.Validate(
                result,
                summary.Evidence));

        errors.AddRange(
            QualityInspectionReplayBundleFingerprintValidationRuntime
                .ValidateFingerprint(summary.ContentFingerprint));

        var expectedFingerprint =
            QualityInspectionResultDeterminismRuntime
                .CreateContentFingerprint(result);

        if (!string.Equals(
                expectedFingerprint,
                summary.ContentFingerprint,
                StringComparison.Ordinal))
        {
            errors.Add("Summary content fingerprint does not match the inspection result.");
        }

        return errors;
    }

    public static bool IsValid(
        QualityInspectionResult result,
        QualityInspectionSummary summary) =>
        Validate(result, summary).Count == 0;
}
