namespace Asun.Domain.Pcb;

public static class PcbFeatureReferenceValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        PcbFeatureReference feature,
        PcbBoardDefinition? board = null)
    {
        ArgumentNullException.ThrowIfNull(feature);

        var errors = new List<string>();

        if (!feature.Id.IsValid)
            errors.Add("Feature id must be valid.");

        if (string.IsNullOrWhiteSpace(feature.Name))
            errors.Add("Feature name cannot be blank.");

        if (feature.Kind == PcbFeatureKind.Unknown)
            errors.Add("Feature kind cannot be Unknown for a concrete reference.");

        if (feature.LayerIndex < 0)
            errors.Add("Feature layer index cannot be negative.");

        if (board is not null &&
            board.IsValid &&
            feature.LayerIndex >= board.LayerCount)
        {
            errors.Add("Feature layer index must be inside the board layer count.");
        }

        if (!feature.Position.IsFinite ||
            !double.IsFinite(feature.RotationRadians) ||
            !double.IsFinite(feature.SizeXmm) ||
            !double.IsFinite(feature.SizeYmm) ||
            feature.SizeXmm < 0 ||
            feature.SizeYmm < 0)
        {
            errors.Add("Feature geometry must be finite and non-negative.");
        }

        return errors;
    }

    public static bool IsValid(
        PcbFeatureReference feature,
        PcbBoardDefinition? board = null) =>
        Validate(feature, board).Count == 0;
}
