using System.Numerics;

namespace Asun.UI.Viewports;

public readonly record struct RoiGuideline(Vector2 Start, Vector2 End);

public static class RoiGuidelineRuntime
{
    public static IReadOnlyList<RoiGuideline> Build(IEnumerable<RoiDocumentItem> items, float extension = 10f)
    {
        ArgumentNullException.ThrowIfNull(items);
        if (!float.IsFinite(extension) || extension < 0) throw new ArgumentOutOfRangeException(nameof(extension));
        var list = items.ToArray();
        var guides = new List<RoiGuideline>();
        foreach (var item in list)
        {
            var b = item.Geometry.GetBounds();
            guides.Add(new RoiGuideline(new Vector2(b.Left - extension, b.Top), new Vector2(b.Right + extension, b.Top)));
            guides.Add(new RoiGuideline(new Vector2(b.Left, b.Top - extension), new Vector2(b.Left, b.Bottom + extension)));
        }
        return guides;
    }
}
