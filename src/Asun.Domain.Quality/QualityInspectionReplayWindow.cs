namespace Asun.Domain.Quality;

public sealed class QualityInspectionReplayWindow
{
    private readonly QualityInspectionReplayEnvelope[] _envelopes;

    public QualityInspectionReplayWindow(
        IEnumerable<QualityInspectionReplayEnvelope> envelopes)
    {
        ArgumentNullException.ThrowIfNull(envelopes);

        _envelopes = envelopes
            .Select(envelope => envelope ??
                throw new ArgumentException(
                    "Replay window cannot contain null envelopes.",
                    nameof(envelopes)))
            .ToArray();
    }

    public int Count => _envelopes.Length;

    public IReadOnlyList<QualityInspectionReplayEnvelope> Envelopes =>
        Array.AsReadOnly(_envelopes);
}
