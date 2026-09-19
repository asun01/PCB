namespace Asun.Domain.Quality;

public static class QualityInspectionDiffRuntime
{
    public static QualityInspectionDiff Diff(
        QualityInspectionSnapshot previous,
        QualityInspectionSnapshot current)
    {
        ArgumentNullException.ThrowIfNull(previous);
        ArgumentNullException.ThrowIfNull(current);

        if (!QualityInspectionSnapshotValidationRuntime.IsValid(previous) ||
            !QualityInspectionSnapshotValidationRuntime.IsValid(current))
        {
            throw new ArgumentException(
                "Quality inspection diff requires valid snapshots.");
        }

        var previousFindings = previous.Findings.Findings
            .ToDictionary(finding => finding.Id);
        var currentFindings = current.Findings.Findings
            .ToDictionary(finding => finding.Id);

        var addedFindings = currentFindings.Keys
            .Except(previousFindings.Keys)
            .OrderBy(id => id.Value, StringComparer.Ordinal)
            .ToArray();

        var removedFindings = previousFindings.Keys
            .Except(currentFindings.Keys)
            .OrderBy(id => id.Value, StringComparer.Ordinal)
            .ToArray();

        var changedFindings = currentFindings.Keys
            .Intersect(previousFindings.Keys)
            .Where(id => previousFindings[id] != currentFindings[id])
            .OrderBy(id => id.Value, StringComparer.Ordinal)
            .ToArray();

        var previousLinks = previous.Evidence.Links.ToHashSet();
        var currentLinks = current.Evidence.Links.ToHashSet();

        var previousKeys = previousLinks
            .Select(link => link.EvidenceKey)
            .ToHashSet();
        var currentKeys = currentLinks
            .Select(link => link.EvidenceKey)
            .ToHashSet();

        var relinkedEvidence = previousKeys
            .Intersect(currentKeys)
            .Where(key =>
            {
                var previousFindingsForKey = previousLinks
                    .Where(link => link.EvidenceKey == key)
                    .Select(link => link.FindingId)
                    .ToHashSet();
                var currentFindingsForKey = currentLinks
                    .Where(link => link.EvidenceKey == key)
                    .Select(link => link.FindingId)
                    .ToHashSet();

                return !previousFindingsForKey.SetEquals(currentFindingsForKey);
            })
            .OrderBy(key => key.Value, StringComparer.Ordinal)
            .ToArray();

        var addedEvidence = currentKeys
            .Except(previousKeys)
            .OrderBy(key => key.Value, StringComparer.Ordinal)
            .ToArray();

        var removedEvidence = previousKeys
            .Except(currentKeys)
            .OrderBy(key => key.Value, StringComparer.Ordinal)
            .ToArray();

        return new QualityInspectionDiff(
            addedFindings,
            removedFindings,
            changedFindings,
            addedEvidence,
            removedEvidence)
        {
            RelinkedEvidenceKeys = relinkedEvidence
        };
    }
}
