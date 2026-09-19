namespace Asun.UI.Viewports;

public static class TileCacheWarmupRuntime
{
    public static async ValueTask<int> WarmAsync<TTile>(
        TileLoadCoordinator<TTile> loader,
        IEnumerable<TileRequest> requests,
        Func<TileRequest, System.Drawing.RectangleF> rectangleFactory,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(loader);
        ArgumentNullException.ThrowIfNull(requests);
        ArgumentNullException.ThrowIfNull(rectangleFactory);

        var count = 0;
        foreach (var request in requests)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (loader.Cache.TryGet(request.Index, out _))
                continue;

            await loader.LoadAsync(
                request,
                rectangleFactory(request),
                cancellationToken);

            count++;
        }
        return count;
    }
}
