namespace Asun.Domain.Quality;

public sealed class QualityInspectionFindingAuditWindow
{
    private readonly QualityInspectionFindingAuditEnvelope[] _envelopes;

    public QualityInspectionFindingAuditWindow(
        IEnumerable<QualityInspectionFindingAuditEnvelope> envelopes)
    {
        ArgumentNullException.ThrowIfNull(envelopes);

        _envelopes=envelopes
            .Select(envelope=>envelope ??
                throw new ArgumentException(
                    "Finding audit window cannot contain null envelopes.",
                    nameof(envelopes)))
            .ToArray();
    }

    public int Count=>_envelopes.Length;

    public IReadOnlyList<QualityInspectionFindingAuditEnvelope> Envelopes =>
        Array.AsReadOnly(_envelopes);
}
