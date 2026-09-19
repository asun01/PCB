using System.Drawing;

namespace Asun.UI.Viewports;

public static class ViewportRenderRegionRuntime
{
    public static RectangleF ClipToViewport(
        RectangleF bounds,
        ViewportTransform transform)
    {
        var viewport = new RectangleF(
            0,
            0,
            transform.ViewportSize.X,
            transform.ViewportSize.Y);

        return RectangleF.Intersect(bounds, viewport);
    }

    public static IReadOnlyList<RectangleF> Merge(
        IEnumerable<RectangleF> regions)
    {
        ArgumentNullException.ThrowIfNull(regions);

        var pending = regions
            .Where(region =>
                !region.IsEmpty &&
                float.IsFinite(region.X) &&
                float.IsFinite(region.Y) &&
                float.IsFinite(region.Width) &&
                float.IsFinite(region.Height))
            .ToList();

        var changed = true;

        while (changed)
        {
            changed = false;

            for (var i = 0; i < pending.Count && !changed; i++)
            {
                for (var j = i + 1; j < pending.Count; j++)
                {
                    if (!pending[i].IntersectsWith(pending[j]))
                        continue;

                    pending[i] = RectangleF.Union(
                        pending[i],
                        pending[j]);

                    pending.RemoveAt(j);
                    changed = true;
                    break;
                }
            }
        }

        return pending;
    }

    public static IReadOnlyList<RectangleF> ClipAndMerge(
        IEnumerable<RectangleF> regions,
        ViewportTransform transform)
    {
        ArgumentNullException.ThrowIfNull(regions);

        return Merge(
            regions.Select(
                region => ClipToViewport(region, transform)));
    }
}
