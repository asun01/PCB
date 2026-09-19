namespace Asun.Domain.Quality;

public static class QualityInspectionFindingAuditDiffRuntime
{
    public static QualityInspectionFindingAuditDiff Diff(
        QualityInspectionFindingAuditIndex previous,
        QualityInspectionFindingAuditIndex current)
    {
        ArgumentNullException.ThrowIfNull(previous);
        ArgumentNullException.ThrowIfNull(current);

        var previousById=previous.FindingIds
            .Select(id=>previous.Find(id)!)
            .ToDictionary(record=>record.FindingId);
        var currentById=current.FindingIds
            .Select(id=>current.Find(id)!)
            .ToDictionary(record=>record.FindingId);

        var added=currentById.Keys
            .Except(previousById.Keys)
            .OrderBy(id=>id.Value,StringComparer.Ordinal)
            .ToArray();
        var removed=previousById.Keys
            .Except(currentById.Keys)
            .OrderBy(id=>id.Value,StringComparer.Ordinal)
            .ToArray();
        var changed=currentById.Keys
            .Intersect(previousById.Keys)
            .Where(id=>previousById[id]!=currentById[id])
            .OrderBy(id=>id.Value,StringComparer.Ordinal)
            .ToArray();

        return new QualityInspectionFindingAuditDiff(
            added,
            removed,
            changed);
    }
}
