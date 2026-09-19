namespace Asun.UI.Viewports;

/// <summary>
/// Thread-safe bounded LRU cache for image tiles.
/// </summary>
public sealed class TileCache<TTile>
{
    private readonly object _sync = new();
    private readonly int _capacity;
    private readonly Dictionary<TileIndex, LinkedListNode<Entry>> _entries = new();
    private readonly LinkedList<Entry> _lru = new();
    private long _hits;
    private long _misses;
    private long _evictions;

    public TileCache(int capacity)
    {
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity));

        _capacity = capacity;
    }

    public int Capacity => _capacity;

    public int Count
    {
        get
        {
            lock (_sync)
                return _entries.Count;
        }
    }

    public TileCacheStatistics Statistics
    {
        get
        {
            lock (_sync)
            {
                return new TileCacheStatistics(
                    Count,
                    _hits,
                    _misses,
                    _evictions);
            }
        }
    }

    public bool TryGet(TileIndex index, out TTile tile)
    {
        lock (_sync)
        {
            if (!_entries.TryGetValue(index, out var node))
            {
                _misses++;
                tile = default!;
                return false;
            }

            _hits++;
            Touch(node);
            tile = node.Value.Tile;
            return true;
        }
    }

    public void Set(TileIndex index, TTile tile)
    {
        lock (_sync)
        {
            if (_entries.TryGetValue(index, out var existing))
            {
                existing.Value = new Entry(index, tile);
                Touch(existing);
                return;
            }

            var node = _lru.AddLast(new Entry(index, tile));
            _entries[index] = node;

            while (_entries.Count > _capacity)
            {
                var oldest = _lru.First!;
                _lru.RemoveFirst();
                _entries.Remove(oldest.Value.Index);
                _evictions++;
            }
        }
    }

    public bool Remove(TileIndex index)
    {
        lock (_sync)
        {
            if (!_entries.Remove(index, out var node))
                return false;

            _lru.Remove(node);
            return true;
        }
    }

    public IReadOnlyList<TileIndex> GetMostRecentFirst()
    {
        lock (_sync)
        {
            return _lru
                .Reverse()
                .Select(entry => entry.Index)
                .ToArray();
        }
    }

    public void Clear()
    {
        lock (_sync)
        {
            _entries.Clear();
            _lru.Clear();
        }
    }

    private void Touch(LinkedListNode<Entry> node)
    {
        if (node.List != _lru || node == _lru.Last)
            return;

        _lru.Remove(node);
        _lru.AddLast(node);
    }

    private sealed record Entry(
        TileIndex Index,
        TTile Tile);
}

public readonly record struct TileCacheStatistics(
    int Count,
    long Hits,
    long Misses,
    long Evictions);
