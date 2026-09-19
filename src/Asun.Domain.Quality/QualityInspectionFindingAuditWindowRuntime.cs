namespace Asun.Domain.Quality;

public static class QualityInspectionFindingAuditWindowRuntime
{
    public static QualityInspectionFindingAuditWindow Create(
        IEnumerable<QualityInspectionResult> results)
    {
        ArgumentNullException.ThrowIfNull(results);

        var envelopes=results
            .Select(QualityInspectionFindingAuditEnvelopeRuntime.Create)
            .ToArray();

        return new QualityInspectionFindingAuditWindow(
            envelopes
                .OrderBy(envelope=>envelope.Sequence)
                .ThenBy(envelope=>envelope.SnapshotId)
                .ThenBy(envelope=>envelope.ResultId)
                .ToArray());
    }
}
