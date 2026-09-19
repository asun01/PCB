namespace Asun.Domain.Quality;

public static class QualityInspectionFindingAuditWindowDiffRuntime
{
    public static QualityInspectionFindingAuditWindowDiff Diff(
        QualityInspectionFindingAuditWindow previous,
        QualityInspectionFindingAuditWindow current)
    {
        ArgumentNullException.ThrowIfNull(previous);
        ArgumentNullException.ThrowIfNull(current);

        var previousByResult=previous.Envelopes
            .ToDictionary(envelope=>envelope.ResultId);
        var currentByResult=current.Envelopes
            .ToDictionary(envelope=>envelope.ResultId);

        var added=currentByResult.Keys
            .Except(previousByResult.Keys)
            .OrderBy(id=>id)
            .ToArray();
        var removed=previousByResult.Keys
            .Except(currentByResult.Keys)
            .OrderBy(id=>id)
            .ToArray();
        var changed=currentByResult.Keys
            .Intersect(previousByResult.Keys)
            .Where(id=>
                previousByResult[id].ProjectionFingerprint!=
                currentByResult[id].ProjectionFingerprint)
            .OrderBy(id=>id)
            .ToArray();

        return new QualityInspectionFindingAuditWindowDiff(
            added,
            removed,
            changed);
    }
}
