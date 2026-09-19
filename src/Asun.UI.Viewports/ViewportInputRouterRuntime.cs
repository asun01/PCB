using System.Numerics;

namespace Asun.UI.Viewports;

public sealed class ViewportInputRouterRuntime
{
    private readonly ViewportGestureRuntime _gestures;
    private readonly ViewportInputCaptureRuntime _capture;

    public ViewportInputRouterRuntime(
        ViewportGestureRuntime gestures,
        ViewportInputCaptureRuntime capture)
    {
        ArgumentNullException.ThrowIfNull(gestures);
        ArgumentNullException.ThrowIfNull(capture);
        _gestures = gestures;
        _capture = capture;
    }

    public ViewportGestureEvent PointerDown(Vector2 point, ViewportMouseButton button = ViewportMouseButton.Left)
    {
        var owner = button == ViewportMouseButton.Middle ? ViewportInputOwner.Pan : ViewportInputOwner.Roi;
        if (!_capture.TryCapture(owner))
            return default;
        return _gestures.PointerDown(point, button);
    }

    public ViewportGestureEvent PointerMove(Vector2 point) =>
        _gestures.PointerMove(point);

    public ViewportGestureEvent PointerUp(Vector2 point)
    {
        var result = _gestures.PointerUp(point);
        _capture.ForceRelease();
        return result;
    }

    public bool Escape(Vector2 point)
    {
        var result = _gestures.KeyDown(ViewportKey.Escape, point);
        _capture.ForceRelease();
        return result;
    }
}
