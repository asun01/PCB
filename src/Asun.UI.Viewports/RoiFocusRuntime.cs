using System.Numerics;

namespace Asun.UI.Viewports;

public static class RoiFocusRuntime
{
    public static ViewportTransform Focus(
        ViewportTransform transform,
        RoiDocumentItem item,
        double padding = 0.85) =>
        transform.ZoomToImageRectangle(item.Geometry.GetBounds(), padding);

    public static ViewportTransform FocusSelection(
        ViewportTransform transform,
        IEnumerable<RoiDocumentItem> items,
        double padding = 0.85)
    {
        var array = items.ToArray();
        if (array.Length == 0) return transform;
        var bounds = array[0].Geometry.GetBounds();
        foreach (var item in array.Skip(1))
            bounds = RectangleF.Union(bounds, item.Geometry.GetBounds());
        return transform.ZoomToImageRectangle(bounds, padding);
    }
}
