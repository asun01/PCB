using System.Numerics;

namespace Asun.UI.Viewports;

public enum RoiDistributionMode { HorizontalCenters, VerticalCenters, HorizontalGaps, VerticalGaps }

public static class RoiDistributionRuntime
{
    public static IReadOnlyList<RoiDocumentItem> Distribute(
        IEnumerable<RoiDocumentItem> items, RoiDistributionMode mode)
    {
        ArgumentNullException.ThrowIfNull(items);
        var source = items.OrderBy(x => x.Geometry.Center.X).ToArray();
        if (source.Length < 3) return source;
        var horizontal = mode is RoiDistributionMode.HorizontalCenters or RoiDistributionMode.HorizontalGaps;
        if (!horizontal) source = source.OrderBy(x => x.Geometry.Center.Y).ToArray();
        var first = source.First().Geometry.Center;
        var last = source.Last().Geometry.Center;
        var result = source.ToArray();
        if (mode is RoiDistributionMode.HorizontalCenters or RoiDistributionMode.VerticalCenters)
        {
            var step = horizontal
                ? (last.X - first.X) / (source.Length - 1)
                : (last.Y - first.Y) / (source.Length - 1);
            for (var i = 1; i < source.Length - 1; i++)
            {
                var center = source[i].Geometry.Center;
                var target = horizontal
                    ? new Vector2(first.X + step * i, center.Y)
                    : new Vector2(center.X, first.Y + step * i);
                result[i] = source[i] with { Geometry = source[i].Geometry.WithCenter(target) };
            }
        }
        else
        {
            var bounds = source.Select(x => x.Geometry.GetBounds()).ToArray();
            var total = horizontal ? bounds.Sum(x => x.Width) : bounds.Sum(x => x.Height);
            var span = horizontal ? last.X - first.X : last.Y - first.Y;
            var gap = (span - total) / (source.Length - 1);
            var cursor = horizontal ? bounds[0].Right + gap : bounds[0].Bottom + gap;
            for (var i = 1; i < source.Length; i++)
            {
                var b = bounds[i];
                var shift = horizontal ? cursor - b.Left : cursor - b.Top;
                result[i] = source[i] with { Geometry = source[i].Geometry.Translate(horizontal ? new Vector2(shift, 0) : new Vector2(0, shift)) };
                cursor = horizontal ? b.Right + gap : b.Bottom + gap;
            }
        }
        return result;
    }
}
