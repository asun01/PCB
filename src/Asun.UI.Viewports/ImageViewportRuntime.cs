namespace Asun.UI.Viewports;

using System.Numerics;

/// <summary>
/// End-to-end framework-neutral large-image viewport runtime:
/// viewport interaction, deterministic request planning, cache lookup,
/// deduplicated asynchronous loading, visible-first refresh, and prefetch.
/// </summary>
public sealed class ImageViewportRuntime<TTile> : IDisposable
{
    private readonly object _sync = new();
    private readonly ImageViewportModel _viewport;
    private readonly TileLoadCoordinator<TTile> _loader;
    private readonly int _maxConcurrency;

    private CancellationTokenSource? _refreshCancellation;
    private long _refreshGeneration;
    private int _disposed;

    public ImageViewportRuntime(
        Vector2 imageSize,
        Vector2 viewportSize,
        Vector2 tileSize,
        int prefetchMarginTiles,
        int cacheCapacity,
        int maxConcurrency,
        ITileSource<TTile> tileSource)
    {
        if (maxConcurrency <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxConcurrency));

        if (cacheCapacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(cacheCapacity));

        ArgumentNullException.ThrowIfNull(tileSource);

        _viewport = new ImageViewportModel(
            imageSize,
            viewportSize,
            tileSize,
            prefetchMarginTiles);

        _loader = new TileLoadCoordinator<TTile>(
            tileSource,
            new TileCache<TTile>(cacheCapacity));

        _maxConcurrency = maxConcurrency;
    }

    public ViewportTransform Transform
    {
        get
        {
            lock (_sync)
                return _viewport.Transform;
        }
    }

    public TileCache<TTile> Cache => _loader.Cache;

    public int InFlightCount => _loader.InFlightCount;

    public bool IsPanning
    {
        get
        {
            lock (_sync)
                return _viewport.IsPanning;
        }
    }

    public void BeginPan(Vector2 viewportPoint)
    {
        lock (_sync)
        {
            ThrowIfDisposed();
            _viewport.BeginPan(viewportPoint);
            CancelRefreshUnsafe();
        }
    }

    public void UpdatePan(Vector2 viewportPoint)
    {
        lock (_sync)
        {
            ThrowIfDisposed();
            _viewport.UpdatePan(viewportPoint);
            CancelRefreshUnsafe();
        }
    }

    public void EndPan()
    {
        lock (_sync)
        {
            ThrowIfDisposed();
            _viewport.EndPan();
        }
    }

    public void CancelPan()
    {
        lock (_sync)
        {
            ThrowIfDisposed();
            _viewport.CancelPan();
        }
    }

    public void PanBy(Vector2 viewportDelta)
    {
        lock (_sync)
        {
            ThrowIfDisposed();
            _viewport.PanBy(viewportDelta);
            CancelRefreshUnsafe();
        }
    }

    public void PanByClamped(Vector2 viewportDelta)
    {
        lock (_sync)
        {
            ThrowIfDisposed();
            _viewport.PanByClamped(viewportDelta);
            CancelRefreshUnsafe();
        }
    }

    public void ZoomFactor(
        double zoomFactor,
        double minScale,
        double maxScale,
        Vector2 viewportAnchor)
    {
        lock (_sync)
        {
            ThrowIfDisposed();
            _viewport.ZoomFactor(
                zoomFactor,
                minScale,
                maxScale,
                viewportAnchor);
            CancelRefreshUnsafe();
        }
    }

    public void FitToViewport()
    {
        lock (_sync)
        {
            ThrowIfDisposed();
            _viewport.FitToViewport();
            CancelRefreshUnsafe();
        }
    }

    public void ResizeViewport(Vector2 viewportSize)
    {
        lock (_sync)
        {
            ThrowIfDisposed();
            _viewport.ResizeViewport(viewportSize);
            CancelRefreshUnsafe();
        }
    }

    public void CenterOnImagePoint(Vector2 imagePoint)
    {
        lock (_sync)
        {
            ThrowIfDisposed();
            _viewport.CenterOnImagePoint(imagePoint);
            CancelRefreshUnsafe();
        }
    }

    public IReadOnlyList<TileRequest> GetCurrentRequests()
    {
        lock (_sync)
        {
            ThrowIfDisposed();
            return _viewport.GetTileRequests();
        }
    }

    public ValueTask<ViewportTileFrame<TTile>> RefreshAsync(
        CancellationToken cancellationToken = default) =>
        RefreshCoreAsync(
            includePrefetch: false,
            cancellationToken);

    public ValueTask<ViewportTileFrame<TTile>> RefreshAndPrefetchAsync(
        CancellationToken cancellationToken = default) =>
        RefreshCoreAsync(
            includePrefetch: true,
            cancellationToken);

    private async ValueTask<ViewportTileFrame<TTile>> RefreshCoreAsync(
        bool includePrefetch,
        CancellationToken cancellationToken)
    {
        var operation = BeginRefresh(cancellationToken);

        try
        {
            var visible = operation.Requests
                .Where(request => request.IsVisible)
                .ToArray();

            var prefetch = operation.Requests
                .Where(request => request.IsPrefetch)
                .ToArray();

            var loaded = new Dictionary<TileIndex, TTile>();
            var failures = new List<TileLoadFailure<TTile>>();

            await LoadRequestsAsync(
                operation,
                visible,
                loaded,
                failures);

            operation.Token.ThrowIfCancellationRequested();

            if (includePrefetch)
            {
                await LoadRequestsAsync(
                    operation,
                    prefetch,
                    loaded,
                    failures);

                operation.Token.ThrowIfCancellationRequested();
            }

            return CreateFrame(
                operation.Transform,
                operation.Requests,
                loaded,
                failures);
        }
        finally
        {
            EndRefresh(operation);
        }
    }

    public async ValueTask<int> PrefetchAsync(
        CancellationToken cancellationToken = default)
    {
        var operation = BeginRefresh(cancellationToken);

        try
        {
            var prefetch = operation.Requests
                .Where(request => request.IsPrefetch)
                .ToArray();

            var loaded = new Dictionary<TileIndex, TTile>();
            var failures = new List<TileLoadFailure<TTile>>();

            await LoadRequestsAsync(
                operation,
                prefetch,
                loaded,
                failures);

            operation.Token.ThrowIfCancellationRequested();

            return loaded.Count;
        }
        finally
        {
            EndRefresh(operation);
        }
    }

    public void Dispose()
    {
        CancellationTokenSource? refresh;

        lock (_sync)
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0)
                return;

            refresh = _refreshCancellation;
            _refreshCancellation = null;
        }

        refresh?.Cancel();
        refresh?.Dispose();
        _loader.Dispose();
    }

    private RefreshOperation BeginRefresh(CancellationToken callerCancellation)
    {
        lock (_sync)
        {
            ThrowIfDisposed();

            CancelRefreshUnsafe();

            var linked = CancellationTokenSource.CreateLinkedTokenSource(
                callerCancellation);

            _refreshCancellation = linked;
            var generation = ++_refreshGeneration;

            var transform = _viewport.Transform;
            var requests = _viewport.GetTileRequests();

            return new RefreshOperation(
                generation,
                linked,
                transform,
                requests);
        }
    }

    private async Task LoadRequestsAsync(
        RefreshOperation operation,
        IReadOnlyList<TileRequest> requests,
        Dictionary<TileIndex, TTile> loaded,
        List<TileLoadFailure<TTile>> failures)
    {
        using var limiter = new SemaphoreSlim(_maxConcurrency);

        var results = await Task.WhenAll(
            requests.Select(async request =>
            {
                await limiter
                    .WaitAsync(operation.Token)
                    .ConfigureAwait(false);

                try
                {
                    var rectangle = TileRequestPlanner.GetRequestRectangle(
                        operation.Transform.ImageSize,
                        _viewport.TileSize,
                        request);

                    try
                    {
                        var tile = await _loader
                            .LoadAsync(
                                request,
                                rectangle,
                                operation.Token)
                            .ConfigureAwait(false);

                        return TileLoadResult.Success(request, tile);
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception exception)
                    {
                        return TileLoadResult.Failed<TTile>(
                            request,
                            exception);
                    }
                }
                finally
                {
                    limiter.Release();
                }
            }));

        foreach (var result in results)
        {
            if (result.HasValue)
                loaded[result.Request.Index] = result.Tile!;
            else
                failures.Add(result.Failure!);
        }
    }

    private ViewportTileFrame<TTile> CreateFrame(
        ViewportTransform transform,
        IReadOnlyList<TileRequest> requests,
        IReadOnlyDictionary<TileIndex, TTile> loaded,
        IReadOnlyList<TileLoadFailure<TTile>> failures) =>
        new(
            transform,
            requests,
            new Dictionary<TileIndex, TTile>(loaded),
            failures.ToArray());

    private void EndRefresh(RefreshOperation operation)
    {
        lock (_sync)
        {
            if (_refreshGeneration == operation.Generation &&
                ReferenceEquals(_refreshCancellation, operation.Cancellation))
            {
                _refreshCancellation = null;
            }
        }

        operation.Cancellation.Dispose();
    }

    private void CancelRefreshUnsafe()
    {
        _refreshCancellation?.Cancel();
    }

    private void ThrowIfDisposed() =>
        ObjectDisposedException.ThrowIf(_disposed != 0, this);

    private sealed record RefreshOperation(
        long Generation,
        CancellationTokenSource Cancellation,
        ViewportTransform Transform,
        IReadOnlyList<TileRequest> Requests)
    {
        public CancellationToken Token => Cancellation.Token;
    }

    private readonly record struct TileLoadResult(
        TileRequest Request,
        TTile Tile,
        TileLoadFailure<TTile>? Failure,
        bool HasValue)
    {
        public static TileLoadResult Success(
            TileRequest request,
            TTile tile) =>
            new(request, tile, null, true);

        public static TileLoadResult Failed<T>(
            TileRequest request,
            Exception exception) =>
            new(
                request,
                default!,
                new TileLoadFailure<T>(request, exception),
                false);
    }
}
