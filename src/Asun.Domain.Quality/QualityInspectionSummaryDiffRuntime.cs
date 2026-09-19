namespace Asun.Domain.Quality;

public static class QualityInspectionSummaryDiffRuntime
{
    public static QualityInspectionSummaryDiff Diff(
        QualityInspectionSummary previous,
        QualityInspectionSummary current)
    {
        ArgumentNullException.ThrowIfNull(previous);
        ArgumentNullException.ThrowIfNull(current);

        if (!IsValidShape(previous))
            throw new ArgumentException(
                "Previous inspection summary is invalid.",
                nameof(previous));

        if (!IsValidShape(current))
            throw new ArgumentException(
                "Current inspection summary is invalid.",
                nameof(current));

        return new QualityInspectionSummaryDiff(
            previous.Outcomes != current.Outcomes,
            previous.Severities != current.Severities,
            previous.Evidence != current.Evidence,
            !string.Equals(
                previous.ContentFingerprint,
                current.ContentFingerprint,
                StringComparison.Ordinal));
    }

    private static bool IsValidShape(QualityInspectionSummary summary) =>
        QualityInspectionSummaryValidationRuntime
            .ValidateShape(summary)
            .Count == 0;
}
