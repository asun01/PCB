namespace Asun.Domain.Pcb;

public static class PcbFeatureCollectionValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        PcbBoardDefinition board,
        IReadOnlyList<PcbFeatureReference> features)
    {
        ArgumentNullException.ThrowIfNull(board);
        ArgumentNullException.ThrowIfNull(features);

        var errors = new List<string>();

        if (!board.IsValid)
            errors.Add("Feature collections require a valid board definition.");

        var ids = new HashSet<PcbFeatureId>();

        foreach (var feature in features)
        {
            if (!ids.Add(feature.Id))
                errors.Add("Feature ids must be unique within a board.");

            errors.AddRange(
                PcbFeatureReferenceValidationRuntime.Validate(
                    feature,
                    board));
        }

        return errors;
    }

    public static bool IsValid(
        PcbBoardDefinition board,
        IReadOnlyList<PcbFeatureReference> features) =>
        Validate(board, features).Count == 0;
}
