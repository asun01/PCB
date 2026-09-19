namespace Asun.Platform.Core;

/// <summary>
/// Provides bounded, independently schedulable capacity for named resources.
/// It contains no domain ownership or production-state semantics.
/// </summary>
public sealed class ResourceLeasePool<TKey> : IDisposable
    where TKey : notnull
{
    private readonly IReadOnlyDictionary<TKey, ResourceEntry> _resources;
    private int _disposed;

    public ResourceLeasePool(IEnumerable<KeyValuePair<TKey, int>> capacities)
        : this(capacities, null)
    {
    }

    public ResourceLeasePool(
        IEnumerable<KeyValuePair<TKey, int>> capacities,
        IEqualityComparer<TKey>? comparer)
    {
        ArgumentNullException.ThrowIfNull(capacities);

        var resources = new Dictionary<TKey, ResourceEntry>(comparer);

        foreach (var pair in capacities)
        {
            if (pair.Value <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(capacities),
                    pair.Value,
                    "Resource capacity must be greater than zero.");
            }

            if (!resources.TryAdd(pair.Key, new ResourceEntry(pair.Value)))
            {
                throw new ArgumentException(
                    "Duplicate resource identifiers are not allowed.",
                    nameof(capacities));
            }
        }

        if (resources.Count == 0)
        {
            throw new ArgumentException(
                "At least one resource must be configured.",
                nameof(capacities));
        }

        _resources = resources;
    }

    public int Capacity(TKey resource) =>
        GetEntry(resource).Capacity;

    public int Available(TKey resource) =>
        GetEntry(resource).Semaphore.CurrentCount;

    public bool TryAcquire(TKey resource, out Lease? lease)
    {
        ThrowIfDisposed();

        var entry = GetEntry(resource);

        if (!entry.Semaphore.Wait(0))
        {
            lease = null;
            return false;
        }

        lease = new Lease(resource, entry);
        return true;
    }

    public async ValueTask<Lease> AcquireAsync(
        TKey resource,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        var entry = GetEntry(resource);
        await entry.Semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

        return new Lease(resource, entry);
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
            return;

        foreach (var entry in _resources.Values)
            entry.Semaphore.Dispose();
    }

    private ResourceEntry GetEntry(TKey resource)
    {
        if (!_resources.TryGetValue(resource, out var entry))
        {
            throw new KeyNotFoundException(
                $"The resource '{resource}' is not configured.");
        }

        return entry;
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed != 0, this);
    }

    internal sealed class ResourceEntry
    {
        public ResourceEntry(int capacity)
        {
            Capacity = capacity;
            Semaphore = new SemaphoreSlim(capacity, capacity);
        }

        public int Capacity { get; }
        public SemaphoreSlim Semaphore { get; }
    }

    public sealed class Lease : IDisposable
    {
        private ResourceEntry? _entry;

        internal Lease(TKey resource, ResourceEntry entry)
        {
            Resource = resource;
            _entry = entry;
        }

        public TKey Resource { get; }

        public void Dispose()
        {
            var entry = Interlocked.Exchange(ref _entry, null);
            entry?.Semaphore.Release();
        }
    }
}
