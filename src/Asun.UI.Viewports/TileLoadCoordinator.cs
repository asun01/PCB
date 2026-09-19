namespace Asun.UI.Viewports;

using System.Collections.Concurrent;

/// <summary>
/// Deduplicates concurrent requests for the same tile and caches successful loads.
/// Caller cancellation cancels only its wait; shared loading remains reusable by other callers.
/// </summary>
public sealed class TileLoadCoordinator<TTile> : IDisposable
{
    private readonly ITileSource<TTile> _source;
    private readonly TileCache<TTile> _cache;
    private readonly ConcurrentDictionary<TileIndex, Task<TTile>> _inflight = new();
    private readonly CancellationTokenSource _lifetime = new();
    private int _disposed;

    public TileLoadCoordinator(
        ITileSource<TTile> source,
        TileCache<TTile> cache)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(cache);

        _source = source;
        _cache = cache;
    }

    public TileCache<TTile> Cache => _cache;

    public int InFlightCount => _inflight.Count;

    public async ValueTask<TTile> LoadAsync(
        TileRequest request,
        System.Drawing.RectangleF imageRectangle,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        if (_cache.TryGet(request.Index, out var cached))
            return cached;

        var task = _inflight.GetOrAdd(
            request.Index,
            _ => LoadAndCacheAsync(request, imageRectangle));

        try
        {
            return await task
                .WaitAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        finally
        {
            if (task.IsCompleted)
                _inflight.TryRemove(
                    new KeyValuePair<TileIndex, Task<TTile>>(
                        request.Index,
                        task));
        }
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
            return;

        _lifetime.Cancel();
        _lifetime.Dispose();
    }

    private async Task<TTile> LoadAndCacheAsync(
        TileRequest request,
        System.Drawing.RectangleF imageRectangle)
    {
        try
        {
            var tile = await _source
                .LoadAsync(request, imageRectangle, _lifetime.Token)
                .ConfigureAwait(false);

            _cache.Set(request.Index, tile);
            return tile;
        }
        finally
        {
            _inflight.TryRemove(request.Index, out _);
        }
    }

    private void ThrowIfDisposed() =>
        ObjectDisposedException.ThrowIf(_disposed != 0, this);
}
