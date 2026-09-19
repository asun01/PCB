using System.Numerics;

namespace Asun.UI.Viewports;

public static class RoiBatchEditRuntime
{
    public static IReadOnlyList<RoiDocumentItem> Translate(IEnumerable<RoiDocumentItem> items, Vector2 delta) =>
        items.Select(item => item with { Geometry = item.Geometry.Translate(delta) }).ToArray();

    public static IReadOnlyList<RoiDocumentItem> Constrain(
        IEnumerable<RoiDocumentItem> items,
        RoiConstraintProfile profile,
        Vector2 imageSize) =>
        items.Select(item => item with { Geometry = RoiConstraintRuntime.Apply(item.Geometry, profile, imageSize) }).ToArray();

    public static IReadOnlyList<RoiDocumentItem> WithZOrder(
        IEnumerable<RoiDocumentItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        return items.Select((item, index) => item with { ZIndex = index }).ToArray();
    }
}
