using System.Collections.Immutable;
using System.Numerics;
using System.Drawing;

namespace Asun.UI.Viewports;

public enum RoiSelectionMode
{
    Replace,
    Add,
    Toggle,
    Subtract
}

public readonly record struct RoiSelectionSnapshot(
    IReadOnlySet<Guid> SelectedIds,
    bool IsMarqueeActive,
    RectangleF? Marquee);

public sealed class RoiSelectionRuntime
{
    private readonly object _sync = new();
    private readonly HashSet<Guid> _selected = new();

    private bool _marqueeActive;
    private Vector2 _marqueeStart;
    private Vector2 _marqueeCurrent;

    public IReadOnlySet<Guid> SelectedIds
    {
        get
        {
            lock (_sync)
                return _selected.ToImmutableHashSet();
        }
    }

    public bool IsMarqueeActive
    {
        get
        {
            lock (_sync)
                return _marqueeActive;
        }
    }

    public void Clear() 
    {
        lock (_sync)
            _selected.Clear();
    }

    public bool Select(
        IEnumerable<Guid> ids,
        RoiSelectionMode mode = RoiSelectionMode.Replace)
    {
        ArgumentNullException.ThrowIfNull(ids);

        lock (_sync)
        {
            var incoming = ids.Where(id => id != Guid.Empty).ToHashSet();
            var before = _selected.ToHashSet();

            switch (mode)
            {
                case RoiSelectionMode.Replace:
                    _selected.Clear();
                    _selected.UnionWith(incoming);
                    break;
                case RoiSelectionMode.Add:
                    _selected.UnionWith(incoming);
                    break;
                case RoiSelectionMode.Toggle:
                    foreach (var id in incoming)
                    {
                        if (!_selected.Add(id))
                            _selected.Remove(id);
                    }
                    break;
                case RoiSelectionMode.Subtract:
                    _selected.ExceptWith(incoming);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode));
            }

            return !before.SetEquals(_selected);
        }
    }

    public bool SelectFromPoint(
        RoiSpatialIndex index,
        Vector2 imagePoint,
        RoiSelectionMode mode = RoiSelectionMode.Replace,
        float handleTolerance = 0f)
    {
        ArgumentNullException.ThrowIfNull(index);

        var hit = index.HitTest(imagePoint, handleTolerance, 0f);
        if (!hit.Hit)
            return Select(Array.Empty<Guid>(), mode == RoiSelectionMode.Replace
                ? RoiSelectionMode.Replace
                : mode);

        return Select(new[] { hit.Id }, mode);
    }

    public void BeginMarquee(Vector2 imagePoint)
    {
        Validate(imagePoint);

        lock (_sync)
        {
            _marqueeActive = true;
            _marqueeStart = imagePoint;
            _marqueeCurrent = imagePoint;
        }
    }

    public RectangleF UpdateMarquee(Vector2 imagePoint)
    {
        Validate(imagePoint);

        lock (_sync)
        {
            if (!_marqueeActive)
                return default;

            _marqueeCurrent = imagePoint;
            return RectangleF.FromLTRB(
                MathF.Min(_marqueeStart.X, _marqueeCurrent.X),
                MathF.Min(_marqueeStart.Y, _marqueeCurrent.Y),
                MathF.Max(_marqueeStart.X, _marqueeCurrent.X),
                MathF.Max(_marqueeStart.Y, _marqueeCurrent.Y));
        }
    }

    public bool CompleteMarquee(
        IEnumerable<RoiDocumentItem> items,
        RoiSelectionMode mode = RoiSelectionMode.Replace,
        bool requireFullyContained = false)
    {
        ArgumentNullException.ThrowIfNull(items);

        lock (_sync)
        {
            if (!_marqueeActive)
                return false;

            var rectangle = RectangleF.FromLTRB(
                MathF.Min(_marqueeStart.X, _marqueeCurrent.X),
                MathF.Min(_marqueeStart.Y, _marqueeCurrent.Y),
                MathF.Max(_marqueeStart.X, _marqueeCurrent.X),
                MathF.Max(_marqueeStart.Y, _marqueeCurrent.Y));

            var ids = items
                .Where(item =>
                {
                    var bounds = item.Geometry.GetBounds();
                    return requireFullyContained
                        ? rectangle.Contains(bounds)
                        : bounds.IntersectsWith(rectangle);
                })
                .Select(item => item.Id)
                .ToArray();

            _marqueeActive = false;

            var incoming = ids.ToHashSet();
            var before = _selected.ToHashSet();

            switch (mode)
            {
                case RoiSelectionMode.Replace:
                    _selected.Clear();
                    _selected.UnionWith(incoming);
                    break;
                case RoiSelectionMode.Add:
                    _selected.UnionWith(incoming);
                    break;
                case RoiSelectionMode.Toggle:
                    foreach (var id in incoming)
                    {
                        if (!_selected.Add(id))
                            _selected.Remove(id);
                    }
                    break;
                case RoiSelectionMode.Subtract:
                    _selected.ExceptWith(incoming);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode));
            }

            return !before.SetEquals(_selected);
        }
    }

    public void CancelMarquee()
    {
        lock (_sync)
            _marqueeActive = false;
    }

    public RoiSelectionSnapshot CreateSnapshot()
    {
        lock (_sync)
        {
            RectangleF? marquee = _marqueeActive
                ? RectangleF.FromLTRB(
                    MathF.Min(_marqueeStart.X, _marqueeCurrent.X),
                    MathF.Min(_marqueeStart.Y, _marqueeCurrent.Y),
                    MathF.Max(_marqueeStart.X, _marqueeCurrent.X),
                    MathF.Max(_marqueeStart.Y, _marqueeCurrent.Y))
                : null;

            return new RoiSelectionSnapshot(
                _selected.ToImmutableHashSet(),
                _marqueeActive,
                marquee);
        }
    }

    private static void Validate(Vector2 value)
    {
        if (!float.IsFinite(value.X) || !float.IsFinite(value.Y))
            throw new ArgumentOutOfRangeException(nameof(value));
    }
}
