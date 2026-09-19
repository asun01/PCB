using System.Numerics;

namespace Asun.UI.Viewports;

public static class ViewportSelectionProjectionRuntime
{
    public static IReadOnlyList<RoiViewportItem> Project(
        RoiViewportSnapshot snapshot,
        IEnumerable<Guid> selectedIds)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(selectedIds);

        var set = selectedIds.ToHashSet();
        return snapshot.Items
            .Select(item => item with { IsSelected = set.Contains(item.Id) })
            .ToArray();
    }
}
