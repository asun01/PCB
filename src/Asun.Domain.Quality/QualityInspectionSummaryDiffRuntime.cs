namespace Asun.Domain.Quality;

public static class QualityInspectionSummaryDiffRuntime
{
    public static QualityInspectionSummaryDiff Diff(
        QualityInspectionSummary previous,
        QualityInspectionSummary current)
    {
        ArgumentNullException.ThrowIfNull(previous);
        ArgumentNullException.ThrowIfNull(current);

        if (!QualityInspectionSummaryValidationRuntime.IsValid(
                new QualityInspectionResult(
                    previous.ResultId,
                    new QualityInspectionSnapshot(
                        previous.SnapshotId,
                        previous.Sequence,
                        new QualityFindingSet(Array.Empty<QualityFinding>()),
                        new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))),
                previous))
        {
            throw new ArgumentException(
                "Previous inspection summary is not independently reconstructible.");
        }

        if (previous.ResultId == current.ResultId &&
            previous.SnapshotId == current.SnapshotId &&
            previous.Sequence == current.Sequence)
        {
            return new QualityInspectionSummaryDiff(
                previous.Outcomes != current.Outcomes,
                previous.Severities != current.Severities,
                previous.Evidence != current.Evidence,
                !string.Equals(
                    previous.ContentFingerprint,
                    current.ContentFingerprint,
                    StringComparison.Ordinal));
        }

        return new QualityInspectionSummaryDiff(
            previous.Outcomes != current.Outcomes,
            previous.Severities != current.Severities,
            previous.Evidence != current.Evidence,
            !string.Equals(
                previous.ContentFingerprint,
                current.ContentFingerprint,
                StringComparison.Ordinal));
    }
}
