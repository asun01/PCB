namespace Asun.Domain.Quality;

public static class QualityInspectionAuditWindowRuntime
{
    public static QualityInspectionAuditWindow Create(
        IEnumerable<QualityInspectionResult> results)
    {
        ArgumentNullException.ThrowIfNull(results);

        var envelopes=results
            .Select(QualityInspectionAuditEnvelopeRuntime.Create)
            .ToArray();

        return new QualityInspectionAuditWindow(
            envelopes
                .OrderBy(envelope=>envelope.Sequence)
                .ThenBy(envelope=>envelope.SnapshotId)
                .ThenBy(envelope=>envelope.ResultId)
                .ToArray());
    }
}
