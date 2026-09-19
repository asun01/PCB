using System.Numerics;

namespace Asun.UI.Viewports;

public sealed record RoiTemplate(
    string Name,
    RoiGeometry Geometry);

public sealed class RoiTemplateRuntime
{
    private readonly object _sync = new();
    private readonly Dictionary<string, RoiTemplate> _templates = new(StringComparer.OrdinalIgnoreCase);

    public void Save(string name, RoiGeometry geometry)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException(nameof(name));
        ArgumentNullException.ThrowIfNull(geometry);
        lock (_sync) _templates[name] = new RoiTemplate(name, geometry);
    }

    public bool Remove(string name)
    {
        lock (_sync) return _templates.Remove(name);
    }

    public bool TryCreate(string name, Vector2 center, out RoiGeometry geometry)
    {
        Validate(center);
        lock (_sync)
        {
            if (!_templates.TryGetValue(name, out var template))
            {
                geometry = default!;
                return false;
            }
            geometry = template.Geometry.WithCenter(center);
            return true;
        }
    }

    public IReadOnlyList<RoiTemplate> Snapshot()
    {
        lock (_sync) return _templates.Values.OrderBy(x => x.Name).ToArray();
    }

    private static void Validate(Vector2 p)
    {
        if (!float.IsFinite(p.X) || !float.IsFinite(p.Y)) throw new ArgumentOutOfRangeException(nameof(p));
    }
}
