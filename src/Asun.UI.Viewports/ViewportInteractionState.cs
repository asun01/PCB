using System.Numerics;

namespace Asun.UI.Viewports;

/// <summary>
/// Pure interaction state for a zoomable/pannable image viewport.
/// It contains no WPF or vendor control dependencies.
/// </summary>
public readonly record struct ViewportInteractionState(
    ViewportTransform Transform,
    bool IsPanning,
    Vector2 PointerPosition)
{
    public static ViewportInteractionState Create(ViewportTransform transform) =>
        new(transform, false, Vector2.Zero);

    public ViewportInteractionState BeginPan(Vector2 viewportPoint)
    {
        Validate(viewportPoint, nameof(viewportPoint));

        return this with
        {
            IsPanning = true,
            PointerPosition = viewportPoint
        };
    }

    public ViewportInteractionState UpdatePan(Vector2 viewportPoint)
    {
        Validate(viewportPoint, nameof(viewportPoint));

        if (!IsPanning)
            return this;

        var delta = viewportPoint - PointerPosition;

        return this with
        {
            Transform = Transform.PanBy(delta),
            PointerPosition = viewportPoint
        };
    }

    public ViewportInteractionState EndPan() =>
        this with { IsPanning = false };

    public ViewportInteractionState CancelPan() =>
        this with { IsPanning = false };

    public ViewportInteractionState ApplyZoomFactor(
        double zoomFactor,
        double minScale,
        double maxScale,
        Vector2 viewportAnchor)
    {
        Validate(viewportAnchor, nameof(viewportAnchor));

        var requestedScale = Transform.Scale * zoomFactor;
        return this with
        {
            Transform = Transform.WithScaleAroundClamped(
                requestedScale,
                minScale,
                maxScale,
                viewportAnchor)
        };
    }

    public ViewportInteractionState ApplyZoom(
        double requestedScale,
        double minScale,
        double maxScale,
        Vector2 viewportAnchor)
    {
        Validate(viewportAnchor, nameof(viewportAnchor));

        return this with
        {
            Transform = Transform.WithScaleAroundClamped(
                requestedScale,
                minScale,
                maxScale,
                viewportAnchor)
        };
    }

    private static void Validate(Vector2 point, string parameterName)
    {
        if (!float.IsFinite(point.X) || !float.IsFinite(point.Y))
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                "Viewport point must contain finite values.");
        }
    }
}
