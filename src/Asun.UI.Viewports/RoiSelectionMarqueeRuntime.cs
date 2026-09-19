using System.Drawing;
using System.Numerics;

namespace Asun.UI.Viewports;

public static class RoiSelectionMarqueeRuntime
{
    public static RectangleF Normalize(Vector2 first, Vector2 second) =>
        RectangleF.FromLTRB(
            MathF.Min(first.X, second.X),
            MathF.Min(first.Y, second.Y),
            MathF.Max(first.X, second.X),
            MathF.Max(first.Y, second.Y));

    public static IReadOnlyList<Guid> Intersections(
        RectangleF rectangle,
        IEnumerable<RoiDocumentItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        return items.Where(x => x.Geometry.GetBounds().IntersectsWith(rectangle)).Select(x => x.Id).ToArray();
    }
}
