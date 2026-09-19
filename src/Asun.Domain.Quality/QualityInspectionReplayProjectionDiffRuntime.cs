namespace Asun.Domain.Quality;

public static class QualityInspectionReplayProjectionDiffRuntime
{
    public static QualityInspectionReplayProjectionDiff Diff(
        QualityInspectionReplayProjection previous,
        QualityInspectionReplayProjection current)
    {
        ArgumentNullException.ThrowIfNull(previous);
        ArgumentNullException.ThrowIfNull(current);

        if (!QualityInspectionReplayProjectionValidationRuntime.IsValid(previous))
            throw new ArgumentException(
                "Previous replay projection is invalid.",
                nameof(previous));

        if (!QualityInspectionReplayProjectionValidationRuntime.IsValid(current))
            throw new ArgumentException(
                "Current replay projection is invalid.",
                nameof(current));

        var previousFindings = previous.FindingIds.ToHashSet();
        var currentFindings = current.FindingIds.ToHashSet();

        var addedFindings = currentFindings
            .Except(previousFindings)
            .OrderBy(id => id.Value, StringComparer.Ordinal)
            .ToArray();

        var removedFindings = previousFindings
            .Except(currentFindings)
            .OrderBy(id => id.Value, StringComparer.Ordinal)
            .ToArray();

        var previousEvidence = previous.EvidenceManifest.Links.ToHashSet();
        var currentEvidence = current.EvidenceManifest.Links.ToHashSet();

        var addedEvidence = currentEvidence
            .Except(previousEvidence)
            .OrderBy(link => link.FindingId.Value, StringComparer.Ordinal)
            .ThenBy(link => link.EvidenceKey.Value, StringComparer.Ordinal)
            .ToArray();

        var removedEvidence = previousEvidence
            .Except(currentEvidence)
            .OrderBy(link => link.FindingId.Value, StringComparer.Ordinal)
            .ThenBy(link => link.EvidenceKey.Value, StringComparer.Ordinal)
            .ToArray();

        return new QualityInspectionReplayProjectionDiff(
            addedFindings,
            removedFindings,
            addedEvidence,
            removedEvidence,
            !string.Equals(
                previous.ContentFingerprint,
                current.ContentFingerprint,
                StringComparison.Ordinal));
    }
}
