using System.Numerics;

namespace Asun.UI.Viewports;

public enum RoiAlignmentMode { Left, Center, Right, Top, Middle, Bottom }

public static class RoiAlignmentRuntime
{
    public static IReadOnlyList<RoiDocumentItem> Align(
        IEnumerable<RoiDocumentItem> items, RoiAlignmentMode mode)
    {
        ArgumentNullException.ThrowIfNull(items);
        var source = items.ToArray();
        if (source.Length == 0) return Array.Empty<RoiDocumentItem>();
        var bounds = source.Select(x => x.Geometry.GetBounds()).ToArray();
        var left = bounds.Min(x => x.Left);
        var right = bounds.Max(x => x.Right);
        var top = bounds.Min(x => x.Top);
        var bottom = bounds.Max(x => x.Bottom);
        var centerX = (left + right) / 2f;
        var centerY = (top + bottom) / 2f;
        return source.Select((item, i) =>
        {
            var b = item.Geometry.GetBounds();
            var dx = mode switch
            {
                RoiAlignmentMode.Left => left - b.Left,
                RoiAlignmentMode.Center => centerX - b.X - b.Width / 2f,
                RoiAlignmentMode.Right => right - b.Right,
                _ => 0f
            };
            var dy = mode switch
            {
                RoiAlignmentMode.Top => top - b.Top,
                RoiAlignmentMode.Middle => centerY - b.Y - b.Height / 2f,
                RoiAlignmentMode.Bottom => bottom - b.Bottom,
                _ => 0f
            };
            return item with { Geometry = item.Geometry.Translate(new Vector2(dx, dy)) };
        }).ToArray();
    }
}
