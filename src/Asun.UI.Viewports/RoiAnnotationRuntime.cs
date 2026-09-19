using System.Numerics;

namespace Asun.UI.Viewports;

public readonly record struct RoiAnnotation(
    Guid Id,
    string Text,
    Vector2 Anchor,
    bool Visible);

public sealed class RoiAnnotationRuntime
{
    private readonly object _sync = new();
    private readonly Dictionary<Guid, RoiAnnotation> _annotations = new();

    public void Set(Guid roiId, string text, Vector2 anchor)
    {
        if (roiId == Guid.Empty || string.IsNullOrWhiteSpace(text)) throw new ArgumentException();
        Validate(anchor);
        lock (_sync) _annotations[roiId] = new RoiAnnotation(roiId, text, anchor, true);
    }

    public bool Remove(Guid roiId)
    {
        lock (_sync) return _annotations.Remove(roiId);
    }

    public IReadOnlyList<RoiAnnotation> Snapshot()
    {
        lock (_sync) return _annotations.Values.OrderBy(x => x.Id).ToArray();
    }

    public void Clear()
    {
        lock (_sync) _annotations.Clear();
    }

    private static void Validate(Vector2 p)
    {
        if (!float.IsFinite(p.X) || !float.IsFinite(p.Y)) throw new ArgumentOutOfRangeException(nameof(p));
    }
}
