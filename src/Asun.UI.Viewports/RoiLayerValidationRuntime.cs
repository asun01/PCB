namespace Asun.UI.Viewports;

public static class RoiLayerValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        IReadOnlyList<RoiLayer> layers)
    {
        ArgumentNullException.ThrowIfNull(layers);

        var errors = new List<string>();

        if (layers.Select(layer => layer.RoiId).Distinct().Count() != layers.Count)
            errors.Add("ROI layer ids must be unique.");

        for (var index = 0; index < layers.Count; index++)
        {
            var layer = layers[index];

            if (layer.RoiId == Guid.Empty)
                errors.Add("ROI layer id cannot be empty.");

            if (string.IsNullOrWhiteSpace(layer.Name))
                errors.Add("ROI layer name cannot be blank.");

            if (index > 0)
            {
                var previous = layers[index - 1];

                if (previous.Order > layer.Order ||
                    (previous.Order == layer.Order &&
                     string.Compare(
                         previous.Name,
                         layer.Name,
                         StringComparison.Ordinal) > 0))
                {
                    errors.Add("ROI layers are not deterministically ordered.");
                }
            }
        }

        return errors;
    }

    public static bool IsValid(IReadOnlyList<RoiLayer> layers) =>
        Validate(layers).Count == 0;
}
