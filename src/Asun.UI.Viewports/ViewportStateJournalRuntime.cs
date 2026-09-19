namespace Asun.UI.Viewports;

public sealed class ViewportStateJournalRuntime
{
    private readonly object _sync = new();
    private readonly int _capacity;
    private readonly Queue<ViewportTransform> _entries = new();

    public ViewportStateJournalRuntime(int capacity = 128)
    {
        if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity));
        _capacity = capacity;
    }

    public int Count { get { lock (_sync) return _entries.Count; } }

    public void Append(ViewportTransform transform)
    {
        lock (_sync)
        {
            _entries.Enqueue(transform);
            while (_entries.Count > _capacity) _entries.Dequeue();
        }
    }

    public IReadOnlyList<ViewportTransform> Snapshot()
    {
        lock (_sync) return _entries.ToArray();
    }

    public void Clear() { lock (_sync) _entries.Clear(); }
}
