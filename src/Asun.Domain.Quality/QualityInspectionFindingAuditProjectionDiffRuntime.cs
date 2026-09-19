namespace Asun.Domain.Quality;

public static class QualityInspectionFindingAuditProjectionDiffRuntime
{
    public static QualityInspectionFindingAuditProjectionDiff Diff(
        QualityInspectionFindingAuditProjection previous,
        QualityInspectionFindingAuditProjection current)
    {
        ArgumentNullException.ThrowIfNull(previous);
        ArgumentNullException.ThrowIfNull(current);

        var previousIds=previous.Index.FindingIds.ToHashSet();
        var currentIds=current.Index.FindingIds.ToHashSet();

        var added=currentIds
            .Except(previousIds)
            .OrderBy(id=>id.Value,StringComparer.Ordinal)
            .ToArray();
        var removed=previousIds
            .Except(currentIds)
            .OrderBy(id=>id.Value,StringComparer.Ordinal)
            .ToArray();
        var changed=currentIds
            .Intersect(previousIds)
            .Where(id =>
                previous.Index.Find(id)!=current.Index.Find(id))
            .OrderBy(id=>id.Value,StringComparer.Ordinal)
            .ToArray();

        return new QualityInspectionFindingAuditProjectionDiff(
            added,
            removed,
            changed);
    }
}
