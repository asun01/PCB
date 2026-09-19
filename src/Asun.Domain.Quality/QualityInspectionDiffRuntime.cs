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

        var previousEvidence = previous.Evidence.Links
            .Select(link => (link.FindingId, link.EvidenceKey))
            .ToHashSet();
        var currentEvidence = current.Evidence.Links
            .Select(link => (link.FindingId, link.EvidenceKey))
            .ToHashSet();

        var addedEvidence = currentEvidence
            .Except(previousEvidence)
            .Select(pair => pair.EvidenceKey)
            .Distinct()
            .OrderBy(key => key.Value, StringComparer.Ordinal)
            .ToArray();

        var removedEvidence = previousEvidence
            .Except(currentEvidence)
            .Select(pair => pair.EvidenceKey)
            .Distinct()
            .OrderBy(key => key.Value, StringComparer.Ordinal)
            .ToArray();

        return new QualityInspectionDiff(
            addedFindings,
            removedFindings,
            changedFindings,
            addedEvidence,
            removedEvidence);
    }
}
