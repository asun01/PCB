using System.Numerics;

namespace Asun.UI.Viewports;

public static class ViewportFocusRuntime
{
    public static ViewportTransform FocusPoint(
        ViewportTransform transform,
        Vector2 imagePoint) =>
        transform.CenterOnImagePoint(imagePoint);

    public static ViewportTransform FocusRectangle(
        ViewportTransform transform,
        RectangleF rectangle,
        double padding = 0.9) =>
        transform.ZoomToImageRectangle(rectangle, padding);
}
