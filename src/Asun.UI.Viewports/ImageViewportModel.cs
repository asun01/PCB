using System.Numerics;

namespace Asun.UI.Viewports;

/// <summary>
/// Framework-neutral viewport model combining transform, interaction state,
/// and deterministic tile-request planning.
/// </summary>
public sealed class ImageViewportModel
{
    private ViewportInteractionState _interaction;

    public ImageViewportModel(
        Vector2 imageSize,
        Vector2 viewportSize,
        Vector2 tileSize,
        int prefetchMarginTiles)
    {
        ValidatePositiveFinite(tileSize, nameof(tileSize));

        if (prefetchMarginTiles < 0)
            throw new ArgumentOutOfRangeException(nameof(prefetchMarginTiles));

        Transform = ViewportTransform.Fit(imageSize, viewportSize);
        TileSize = tileSize;
        PrefetchMarginTiles = prefetchMarginTiles;
        _interaction = ViewportInteractionState.Create(Transform);
    }

    public ViewportTransform Transform { get; private set; }

    public Vector2 TileSize { get; }

    public int PrefetchMarginTiles { get; }

    public bool IsPanning =>
        _interaction.IsPanning;

    public Vector2 PointerPosition =>
        _interaction.PointerPosition;

    public IReadOnlyList<TileRequest> GetTileRequests() =>
        TileRequestPlanner.PlanForViewport(
            Transform,
            TileSize,
            PrefetchMarginTiles);

    public void BeginPan(Vector2 viewportPoint)
    {
        _interaction = _interaction.BeginPan(viewportPoint);
        Transform = _interaction.Transform;
    }

    public void UpdatePan(Vector2 viewportPoint)
    {
        _interaction = _interaction.UpdatePan(viewportPoint);
        Transform = _interaction.Transform;
    }

    public void EndPan()
    {
        _interaction = _interaction.EndPan();
        Transform = _interaction.Transform;
    }

    public void CancelPan()
    {
        _interaction = _interaction.CancelPan();
        Transform = _interaction.Transform;
    }

    public void PanBy(Vector2 viewportDelta)
    {
        Transform = Transform.PanBy(viewportDelta);
        _interaction = ViewportInteractionState.Create(Transform);
    }

    public void PanByClamped(Vector2 viewportDelta)
    {
        Transform = Transform.PanByClamped(viewportDelta);
        _interaction = ViewportInteractionState.Create(Transform);
    }

    public void ZoomFactor(
        double zoomFactor,
        double minScale,
        double maxScale,
        Vector2 viewportAnchor)
    {
        _interaction = _interaction.ApplyZoomFactor(
            zoomFactor,
            minScale,
            maxScale,
            viewportAnchor);

        Transform = _interaction.Transform;
    }

    public void FitToViewport()
    {
        _interaction = _interaction.FitToViewport();
        Transform = _interaction.Transform;
    }

    public void ResizeViewport(Vector2 viewportSize)
    {
        _interaction = _interaction.WithViewportSize(viewportSize);
        Transform = _interaction.Transform;
    }

    public void CenterOnImagePoint(Vector2 imagePoint)
    {
        _interaction = _interaction.CenterOnImagePoint(imagePoint);
        Transform = _interaction.Transform;
    }

    private static void ValidatePositiveFinite(Vector2 value, string parameterName)
    {
        if (!float.IsFinite(value.X) ||
            !float.IsFinite(value.Y) ||
            value.X <= 0 ||
            value.Y <= 0)
        {
            throw new ArgumentOutOfRangeException(parameterName, value);
        }
    }
}
