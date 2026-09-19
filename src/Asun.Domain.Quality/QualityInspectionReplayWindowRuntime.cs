namespace Asun.Domain.Quality;

public static class QualityInspectionReplayWindowRuntime
{
    public static QualityInspectionReplayWindow Create(
        IEnumerable<QualityInspectionReplayEnvelope> envelopes)
    {
        ArgumentNullException.ThrowIfNull(envelopes);

        var ordered = envelopes
            .ToArray();

        foreach (var envelope in ordered)
        {
            if (!QualityInspectionReplayEnvelopeValidationRuntime.IsValid(envelope))
                throw new ArgumentException(
                    "Replay window cannot contain an invalid envelope.",
                    nameof(envelopes));
        }

        return new QualityInspectionReplayWindow(
            ordered
                .OrderBy(envelope => envelope.Bundle.Current.Sequence)
                .ThenBy(
                    envelope => envelope.Bundle.Current.SnapshotId)
                .ThenBy(
                    envelope => envelope.Bundle.Current.ResultId)
                .ToArray());
    }
}
