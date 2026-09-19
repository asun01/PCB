using System.Numerics;

namespace Asun.UI.Viewports;

public static class RoiFitRuntime
{
    public static ViewportTransform FitSelection(
        ViewportTransform transform,
        IEnumerable<RoiDocumentItem> items,
        double padding = 0.9)
    {
        var source = items.ToArray();
        if (source.Length == 0) return transform;

        var bounds = source[0].Geometry.GetBounds();
        foreach (var item in source.Skip(1))
            bounds = RectangleF.Union(bounds, item.Geometry.GetBounds());

        return transform.ZoomToImageRectangle(bounds, padding);
    }

    public static ViewportTransform FitAll(
        ViewportTransform transform,
        IEnumerable<RoiDocumentItem> items,
        double padding = 0.9) =>
        FitSelection(transform, items, padding);
}
