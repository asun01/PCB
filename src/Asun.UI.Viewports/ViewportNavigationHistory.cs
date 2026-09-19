namespace Asun.UI.Viewports;

public sealed class ViewportNavigationHistory
{
    private readonly object _sync = new();
    private readonly int _capacity;
    private readonly List<ViewportTransform> _back = new();
    private readonly List<ViewportTransform> _forward = new();

    public ViewportNavigationHistory(
        ViewportTransform initial,
        int capacity = 32)
    {
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity));

        _capacity = capacity;
        _back.Add(initial);
    }

    public int Capacity => _capacity;

    public bool CanBack
    {
        get
        {
            lock (_sync)
                return _back.Count > 1;
        }
    }

    public bool CanForward
    {
        get
        {
            lock (_sync)
                return _forward.Count > 0;
        }
    }

    public ViewportTransform Current
    {
        get
        {
            lock (_sync)
                return _back[^1];
        }
    }

    public void Record(ViewportTransform transform)
    {
        lock (_sync)
        {
            if (_back[^1].Equals(transform))
                return;

            _back.Add(transform);
            _forward.Clear();

            if (_back.Count > _capacity)
                _back.RemoveAt(0);
        }
    }

    public bool Back(out ViewportTransform transform)
    {
        lock (_sync)
        {
            if (_back.Count <= 1)
            {
                transform = Current;
                return false;
            }

            var current = _back[^1];
            _back.RemoveAt(_back.Count - 1);
            _forward.Add(current);
            transform = _back[^1];
            return true;
        }
    }

    public bool Forward(out ViewportTransform transform)
    {
        lock (_sync)
        {
            if (_forward.Count == 0)
            {
                transform = Current;
                return false;
            }

            transform = _forward[^1];
            _forward.RemoveAt(_forward.Count - 1);
            _back.Add(transform);
            return true;
        }
    }

    public void Clear()
    {
        lock (_sync)
        {
            var current = _back[^1];
            _back.Clear();
            _back.Add(current);
            _forward.Clear();
        }
    }
}
