namespace Asun.Domain.Quality;

public static class QualityInspectionEvidenceDiffRuntime
{
    public static QualityInspectionEvidenceDiff Diff(
        QualityInspectionSnapshot previous,
        QualityInspectionSnapshot current)
    {
        ArgumentNullException.ThrowIfNull(previous);
        ArgumentNullException.ThrowIfNull(current);

        if (!QualityInspectionSnapshotValidationRuntime.IsValid(previous))
            throw new ArgumentException(
                "Previous inspection snapshot is invalid.",
                nameof(previous));

        if (!QualityInspectionSnapshotValidationRuntime.IsValid(current))
            throw new ArgumentException(
                "Current inspection snapshot is invalid.",
                nameof(current));

        var previousLinks = previous.Evidence.Links.ToHashSet();
        var currentLinks = current.Evidence.Links.ToHashSet();

        var added = currentLinks
            .Except(previousLinks)
            .OrderBy(link => link.FindingId.Value, StringComparer.Ordinal)
            .ThenBy(link => link.EvidenceKey.Value, StringComparer.Ordinal)
            .ToArray();

        var removed = previousLinks
            .Except(currentLinks)
            .OrderBy(link => link.FindingId.Value, StringComparer.Ordinal)
            .ThenBy(link => link.EvidenceKey.Value, StringComparer.Ordinal)
            .ToArray();

        return new QualityInspectionEvidenceDiff(added, removed);
    }
}
