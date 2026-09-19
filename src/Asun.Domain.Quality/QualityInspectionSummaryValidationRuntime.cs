namespace Asun.Domain.Quality;

public static class QualityInspectionSummaryValidationRuntime
{
    public static IReadOnlyList<string> ValidateShape(
        QualityInspectionSummary summary)
    {
        ArgumentNullException.ThrowIfNull(summary);

        var errors = new List<string>();

        if (summary.ResultId == Guid.Empty)
            errors.Add("Summary result id cannot be empty.");

        if (summary.SnapshotId == Guid.Empty)
            errors.Add("Summary snapshot id cannot be empty.");

        if (summary.Sequence < 0)
            errors.Add("Summary sequence cannot be negative.");

        if (summary.Outcomes is null)
            errors.Add("Summary outcomes cannot be null.");

        if (summary.Severities is null)
            errors.Add("Summary severities cannot be null.");

        if (summary.Evidence is null)
            errors.Add("Summary evidence cannot be null.");

        if (summary.ContentFingerprint is null)
            errors.Add("Summary content fingerprint cannot be null.");

        if (summary.ContentFingerprint is not null)
        {
            errors.AddRange(
                QualityInspectionReplayBundleFingerprintValidationRuntime
                    .ValidateFingerprint(summary.ContentFingerprint));
        }

        return errors;
    }

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

        errors.AddRange(ValidateShape(summary));

        if (summary.ResultId != result.ResultId)
            errors.Add("Summary result id does not match the inspection result.");

        if (summary.SnapshotId != result.SnapshotId)
            errors.Add("Summary snapshot id does not match the inspection result.");

        if (summary.Sequence != result.Sequence)
            errors.Add("Summary sequence does not match the inspection result.");

        if (summary.Outcomes is not null)
        {
            errors.AddRange(
                QualityInspectionOutcomeSummaryValidationRuntime.Validate(
                    result,
                    summary.Outcomes));
        }

        if (summary.Severities is not null)
        {
            errors.AddRange(
                QualityInspectionSeveritySummaryValidationRuntime.Validate(
                    result,
                    summary.Severities));
        }

        if (summary.Evidence is not null)
        {
            errors.AddRange(
                QualityInspectionEvidenceSummaryValidationRuntime.Validate(
                    result,
                    summary.Evidence));
        }

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
