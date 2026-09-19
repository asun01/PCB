namespace Asun.Domain.Quality;

public static class QualityInspectionAuditWindowDiffRuntime
{
    public static QualityInspectionAuditWindowDiff Diff(
        QualityInspectionAuditWindow previous,
        QualityInspectionAuditWindow current)
    {
        ArgumentNullException.ThrowIfNull(previous);
        ArgumentNullException.ThrowIfNull(current);

        var previousById=previous.Envelopes.ToDictionary(item=>item.ResultId);
        var currentById=current.Envelopes.ToDictionary(item=>item.ResultId);

        var added=currentById.Keys
            .Except(previousById.Keys)
            .OrderBy(id=>id)
            .ToArray();
        var removed=previousById.Keys
            .Except(currentById.Keys)
            .OrderBy(id=>id)
            .ToArray();
        var changed=currentById.Keys
            .Intersect(previousById.Keys)
            .Where(id=>
                previousById[id].ProjectionFingerprint!=
                currentById[id].ProjectionFingerprint)
            .OrderBy(id=>id)
            .ToArray();

        return new QualityInspectionAuditWindowDiff(
            added,
            removed,
            changed);
    }
}
