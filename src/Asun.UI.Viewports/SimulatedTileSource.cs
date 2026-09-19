using System.Drawing;

namespace Asun.UI.Viewports;

/// <summary>
/// Deterministic, vendor-neutral tile source for simulation, diagnostics, and
/// integration tests. It exposes bounded concurrency and controlled failure
/// behavior without depending on a camera, decoder, or imaging vendor.
/// </summary>
public sealed class SimulatedTileSource<TTile> : ITileSource<TTile>
{
    private readonly TimeSpan _delay;
    private readonly Func<TileRequest, bool>? _shouldFail;
    private readonly Func<TileRequest, RectangleF, TTile> _factory;
    private readonly bool _signalFirstLoad;
    private readonly TaskCompletionSource<bool> _firstLoadStarted =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    private int _loadCount;
    private int _activeLoads;
    private int _maxConcurrentLoads;
    private int _firstLoadSignaled;

    public SimulatedTileSource(
        Func<TileRequest, RectangleF, TTile> factory,
        TimeSpan delay = default,
        Func<TileRequest, bool>? shouldFail = null,
        bool signalFirstLoad = false)
    {
        ArgumentNullException.ThrowIfNull(factory);

        if (delay < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(delay));

        _factory = factory;
        _delay = delay;
        _shouldFail = shouldFail;
        _signalFirstLoad = signalFirstLoad;
    }

    public int LoadCount =>
        Volatile.Read(ref _loadCount);

    public int ActiveLoadCount =>
        Volatile.Read(ref _activeLoads);

    public int MaxConcurrentLoads =>
        Volatile.Read(ref _maxConcurrentLoads);

    public Task FirstLoadStarted =>
        _firstLoadStarted.Task;

    public async ValueTask<TTile> LoadAsync(
        TileRequest request,
        RectangleF imageRectangle,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Interlocked.Increment(ref _loadCount);

        var active = Interlocked.Increment(ref _activeLoads);
        UpdateMaximum(active);

        if (_signalFirstLoad &&
            Interlocked.Exchange(ref _firstLoadSignaled, 1) == 0)
        {
            _firstLoadStarted.TrySetResult(true);
        }

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (_shouldFail?.Invoke(request) == true)
            {
                throw new InvalidOperationException(
                    $"Simulated tile load failure for {request.Index}.");
            }

            if (_delay > TimeSpan.Zero)
                await Task.Delay(_delay, cancellationToken).ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();

            return _factory(request, imageRectangle);
        }
        finally
        {
            Interlocked.Decrement(ref _activeLoads);
        }
    }

    private void UpdateMaximum(int active)
    {
        while (true)
        {
            var current = Volatile.Read(ref _maxConcurrentLoads);

            if (active <= current)
                return;

            if (Interlocked.CompareExchange(
                    ref _maxConcurrentLoads,
                    active,
                    current) == current)
            {
                return;
            }
        }
    }
}
