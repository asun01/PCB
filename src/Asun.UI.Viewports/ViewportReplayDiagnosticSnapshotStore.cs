namespace Asun.UI.Viewports;

public sealed class ViewportReplayDiagnosticSnapshotStore
{
    private readonly object _sync = new();
    private readonly int _capacity;
    private readonly List<ViewportReplayDiagnosticSnapshot> _entries = new();
    private long _dropped;

    public ViewportReplayDiagnosticSnapshotStore(int capacity = 64)
    {
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity));

        _capacity = capacity;
    }

    public int Capacity => _capacity;

    public long DroppedCount
    {
        get
        {
            lock (_sync)
                return _dropped;
        }
    }

    public int Count
    {
        get
        {
            lock (_sync)
                return _entries.Count;
        }
    }

    public ViewportReplayDiagnosticSnapshot? Latest
    {
        get
        {
            lock (_sync)
                return _entries.Count == 0
                    ? null
                    : _entries[^1];
        }
    }

    public void Add(
        ViewportReplayDiagnosticSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var errors =
            ViewportReplayDiagnosticSnapshotRuntime.Validate(
                snapshot);

        if (errors.Count != 0)
        {
            throw new InvalidOperationException(
                $"Invalid diagnostic snapshot: {errors[0]}");
        }

        lock (_sync)
        {
            if (_entries.Count != 0)
            {
                var previous = _entries[^1];

                if (snapshot.Generation <
                    previous.Generation)
                {
                    throw new InvalidOperationException(
                        "Diagnostic snapshot generations must be monotonic.");
                }
            }

            if (_entries.Count >= _capacity)
            {
                _entries.RemoveAt(0);
                _dropped++;
            }

            _entries.Add(snapshot);
        }
    }

    public IReadOnlyList<ViewportReplayDiagnosticSnapshot> Snapshot()
    {
        lock (_sync)
            return _entries.ToArray();
    }

    public IReadOnlyList<ViewportReplayDiagnosticSnapshot> FindByGeneration(
        long generation)
    {
        if (generation < 0)
            throw new ArgumentOutOfRangeException(nameof(generation));

        lock (_sync)
        {
            return _entries
                .Where(item => item.Generation == generation)
                .ToArray();
        }
    }

    public void Clear()
    {
        lock (_sync)
        {
            _entries.Clear();
            _dropped = 0;
        }
    }
}
