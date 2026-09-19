namespace Asun.Domain.Quality;

public sealed class QualityInspectionAuditWindow
{
    private readonly QualityInspectionAuditEnvelope[] _envelopes;

    public QualityInspectionAuditWindow(
        IEnumerable<QualityInspectionAuditEnvelope> envelopes)
    {
        ArgumentNullException.ThrowIfNull(envelopes);

        _envelopes=envelopes
            .Select(envelope=>envelope ??
                throw new ArgumentException(
                    "Audit window cannot contain null envelopes.",
                    nameof(envelopes)))
            .ToArray();
    }

    public int Count=>_envelopes.Length;

    public IReadOnlyList<QualityInspectionAuditEnvelope> Envelopes =>
        Array.AsReadOnly(_envelopes);
}
