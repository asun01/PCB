namespace Asun.Domain.Quality;

public static class QualityInspectionReplayWindowDiffRuntime
{
    public static QualityInspectionReplayWindowDiff Diff(
        QualityInspectionReplayWindow previous,
        QualityInspectionReplayWindow current)
    {
        ArgumentNullException.ThrowIfNull(previous);
        ArgumentNullException.ThrowIfNull(current);

        if (!QualityInspectionReplayWindowValidationRuntime.IsValid(previous))
            throw new ArgumentException(
                "Previous replay window is invalid.",
                nameof(previous));

        if (!QualityInspectionReplayWindowValidationRuntime.IsValid(current))
            throw new ArgumentException(
                "Current replay window is invalid.",
                nameof(current));

        var previousByResult = previous.Envelopes.ToDictionary(
            envelope => envelope.Bundle.Current.ResultId);
        var currentByResult = current.Envelopes.ToDictionary(
            envelope => envelope.Bundle.Current.ResultId);

        var added = currentByResult.Keys
            .Except(previousByResult.Keys)
            .OrderBy(id => id)
            .ToArray();

        var removed = previousByResult.Keys
            .Except(currentByResult.Keys)
            .OrderBy(id => id)
            .ToArray();

        var changed = currentByResult.Keys
            .Intersect(previousByResult.Keys)
            .Where(id =>
                !string.Equals(
                    previousByResult[id].BundleFingerprint,
                    currentByResult[id].BundleFingerprint,
                    StringComparison.Ordinal))
            .OrderBy(id => id)
            .ToArray();

        return new QualityInspectionReplayWindowDiff(
            added,
            removed,
            changed);
    }
}
