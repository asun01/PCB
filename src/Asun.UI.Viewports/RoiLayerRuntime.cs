namespace Asun.UI.Viewports;

public readonly record struct RoiLayer(Guid RoiId, string Name, int Order, bool Visible, bool Locked);

public sealed class RoiLayerRuntime
{
    private readonly object _sync = new();
    private readonly Dictionary<Guid, RoiLayer> _layers = new();

    public void Ensure(Guid id, string name, int order = 0)
    {
        if (id == Guid.Empty || string.IsNullOrWhiteSpace(name))
            throw new ArgumentException();

        lock (_sync)
        {
            if (_layers.TryGetValue(id, out var existing))
            {
                _layers[id] = existing with
                {
                    Name = name,
                    Order = order
                };
                return;
            }

            _layers[id] = new RoiLayer(
                id,
                name,
                order,
                Visible: true,
                Locked: false);
        }
    }

    public bool Remove(Guid id) { lock (_sync) return _layers.Remove(id); }

    public bool SetVisible(Guid id, bool visible)
    {
        lock (_sync)
        {
            if (!_layers.TryGetValue(id, out var layer)) return false;
            _layers[id] = layer with { Visible = visible };
            return true;
        }
    }

    public bool SetLocked(Guid id, bool locked)
    {
        lock (_sync)
        {
            if (!_layers.TryGetValue(id, out var layer)) return false;
            _layers[id] = layer with { Locked = locked };
            return true;
        }
    }

    public IReadOnlyList<RoiLayer> Snapshot() { lock (_sync) return _layers.Values.OrderBy(x => x.Order).ThenBy(x => x.Name).ToArray(); }
}
