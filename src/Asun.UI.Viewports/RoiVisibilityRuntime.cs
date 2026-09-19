namespace Asun.UI.Viewports;

public static class RoiVisibilityRuntime
{
    public static IReadOnlyList<RoiDocumentItem> Filter(
        IEnumerable<RoiDocumentItem> items,
        RoiLayerRuntime layers)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(layers);
        var map = layers.Snapshot().ToDictionary(x => x.RoiId);
        return items.Where(item => !map.TryGetValue(item.Id, out var layer) || layer.Visible).ToArray();
    }
}
