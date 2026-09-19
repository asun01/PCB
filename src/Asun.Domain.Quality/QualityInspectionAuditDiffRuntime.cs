namespace Asun.Domain.Quality;

public static class QualityInspectionAuditDiffRuntime
{
    public static QualityInspectionAuditDiff Diff(
        QualityInspectionAuditRecord previous,
        QualityInspectionAuditRecord current)
    {
        ArgumentNullException.ThrowIfNull(previous);
        ArgumentNullException.ThrowIfNull(current);

        if (!QualityInspectionAuditValidationRuntime.IsValid(previous))
            throw new ArgumentException(
                "Previous audit record is invalid.",
                nameof(previous));

        if (!QualityInspectionAuditValidationRuntime.IsValid(current))
            throw new ArgumentException(
                "Current audit record is invalid.",
                nameof(current));

        return new QualityInspectionAuditDiff(
            previous.ResultId != current.ResultId,
            previous.SnapshotId != current.SnapshotId,
            previous.Sequence != current.Sequence,
            previous.FindingCount != current.FindingCount,
            previous.EvidenceLinkCount != current.EvidenceLinkCount,
            !string.Equals(
                previous.ContentFingerprint,
                current.ContentFingerprint,
                StringComparison.Ordinal));
    }
}
