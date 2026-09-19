namespace Asun.UI.Viewports;

public readonly record struct RoiStatus(
    int Count,
    int Selected,
    int Visible,
    int Locked,
    bool HasChanges);

public static class RoiStatusRuntime
{
    public static RoiStatus Capture(
        IEnumerable<RoiDocumentItem> items,
        RoiLayerRuntime layers,
        IEnumerable<Guid>? selectedIds,
        bool hasChanges)
    {
        var array = items.ToArray();
        var selected = selectedIds?.ToHashSet() ?? new HashSet<Guid>();
        var layerMap = layers.Snapshot().ToDictionary(x => x.RoiId);
        return new RoiStatus(
            array.Length,
            selected.Count,
            array.Count(x => !layerMap.TryGetValue(x.Id, out var l) || l.Visible),
            array.Count(x => layerMap.TryGetValue(x.Id, out var l) && l.Locked),
            hasChanges);
    }
}
