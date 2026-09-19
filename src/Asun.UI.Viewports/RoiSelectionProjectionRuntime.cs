using System.Numerics;

namespace Asun.UI.Viewports;

public readonly record struct RoiSelectionProjection(
    IReadOnlyList<RoiDocumentItem> Items,
    RectangleF Bounds,
    Vector2 Center);

public static class RoiSelectionProjectionRuntime
{
    public static RoiSelectionProjection Build(IEnumerable<RoiDocumentItem> items)
    {
        var selected = items.ToArray();
        if (selected.Length == 0)
            return new RoiSelectionProjection(Array.Empty<RoiDocumentItem>(), default, Vector2.Zero);

        var bounds = selected[0].Geometry.GetBounds();
        for (var i = 1; i < selected.Length; i++)
            bounds = RectangleF.Union(bounds, selected[i].Geometry.GetBounds());

        return new RoiSelectionProjection(
            selected,
            bounds,
            new Vector2(bounds.X + bounds.Width / 2f, bounds.Y + bounds.Height / 2f));
    }
}
