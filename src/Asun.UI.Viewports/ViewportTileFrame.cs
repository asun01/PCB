using System.Collections.ObjectModel;
using System.Numerics;

namespace Asun.UI.Viewports;

/// <summary>
/// Immutable display snapshot produced by the viewport tile runtime.
/// </summary>
public sealed class ViewportTileFrame<TTile>
{
    internal ViewportTileFrame(
        ViewportTransform transform,
        Vector2 tileSize,
        IReadOnlyList<TileRequest> requests,
        IReadOnlyDictionary<TileIndex, TTile> loadedTiles,
        IReadOnlyList<TileLoadFailure<TTile>> failures)
    {
        Transform = transform;
        TileSize = tileSize;
        Requests = Array.AsReadOnly(requests.ToArray());
        LoadedTiles = new ReadOnlyDictionary<TileIndex, TTile>(
            new Dictionary<TileIndex, TTile>(loadedTiles));
        Failures = Array.AsReadOnly(failures.ToArray());
    }

    public ViewportTransform Transform { get; }

    public Vector2 TileSize { get; }

    public IReadOnlyList<TileRequest> Requests { get; }

    public IReadOnlyDictionary<TileIndex, TTile> LoadedTiles { get; }

    public IReadOnlyList<TileLoadFailure<TTile>> Failures { get; }

    public int RequestedCount =>
        Requests.Count;

    public int RequestedVisibleCount =>
        Requests.Count(request => request.IsVisible);

    public int RequestedPrefetchCount =>
        Requests.Count(request => request.IsPrefetch);

    public int LoadedCount => LoadedTiles.Count;

    public bool IsCompleteForVisible =>
        Failures.All(failure => failure.Request.IsPrefetch) &&
        LoadedTiles.Keys.Count(index =>
            Requests.Any(request =>
                request.IsVisible &&
                request.Index == index)) == RequestedVisibleCount;

    public bool IsCompleteForAllRequests =>
        Failures.Count == 0 &&
        LoadedCount == RequestedCount;

    public bool IsComplete => IsCompleteForVisible;

    public bool TryGetTile(TileIndex index, out TTile tile) =>
        LoadedTiles.TryGetValue(index, out tile!);

    public IReadOnlyList<TileRequest> MissingVisibleRequests() =>
        Requests
            .Where(request =>
                request.IsVisible &&
                !LoadedTiles.ContainsKey(request.Index))
            .ToArray();

    public Vector2 ImagePointAtViewportCenter =>
        Transform.ImagePointAtViewportCenter;
}

public readonly record struct TileLoadFailure<TTile>(
    TileRequest Request,
    Exception Error);
