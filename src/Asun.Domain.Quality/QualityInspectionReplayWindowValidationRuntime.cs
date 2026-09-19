namespace Asun.Domain.Quality;

public static class QualityInspectionReplayWindowValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        QualityInspectionReplayWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);

        var errors = new List<string>();
        var resultIds = new HashSet<Guid>();
        var snapshotIds = new HashSet<Guid>();

        foreach (var envelope in window.Envelopes)
        {
            var envelopeErrors =
                QualityInspectionReplayEnvelopeValidationRuntime
                    .Validate(envelope);

            errors.AddRange(envelopeErrors);

            if (envelopeErrors.Count > 0 ||
                envelope.Bundle is null ||
                envelope.Bundle.Current is null)
            {
                continue;
            }

            if (!resultIds.Add(envelope.Bundle.Current.ResultId))
                errors.Add("Replay window result ids must be unique.");

            if (!snapshotIds.Add(envelope.Bundle.Current.SnapshotId))
                errors.Add("Replay window snapshot ids must be unique.");
        }

        return errors;
    }

    public static bool IsValid(
        QualityInspectionReplayWindow window) =>
        Validate(window).Count == 0;
}
