using System.Numerics;

namespace Asun.UI.Viewports;

public sealed class ViewportInputRouterRuntime
{
    private readonly ViewportGestureRuntime _gestures;
    private readonly ViewportInputCaptureRuntime _capture;
    private ViewportInputOwner _capturedOwner;

    public ViewportInputRouterRuntime(
        ViewportGestureRuntime gestures,
        ViewportInputCaptureRuntime capture)
    {
        ArgumentNullException.ThrowIfNull(gestures);
        ArgumentNullException.ThrowIfNull(capture);
        _gestures = gestures;
        _capture = capture;
        _capturedOwner = ViewportInputOwner.None;
    }

    public ViewportGestureEvent PointerDown(
        Vector2 point,
        ViewportMouseButton button = ViewportMouseButton.Left)
    {
        var owner = button switch
        {
            ViewportMouseButton.Middle => ViewportInputOwner.Pan,
            ViewportMouseButton.Left => ViewportInputOwner.Roi,
            _ => ViewportInputOwner.None
        };

        if (owner == ViewportInputOwner.None ||
            !_capture.TryCapture(owner))
            return default;

        try
        {
            var result = _gestures.PointerDown(point, button);

            if (!result.Kind.Equals(ViewportGestureKind.Idle))
            {
                _capturedOwner = owner;
                return result;
            }

            _capture.Release(owner);
            return result;
        }
        catch
        {
            _capture.Release(owner);
            throw;
        }
    }

    public ViewportGestureEvent PointerMove(Vector2 point)
    {
        if (_capturedOwner == ViewportInputOwner.None)
            return _gestures.PointerMove(point);

        return _gestures.PointerMove(point);
    }

    public ViewportGestureEvent PointerUp(Vector2 point)
    {
        var result = _gestures.PointerUp(point);

        if (_capturedOwner != ViewportInputOwner.None)
        {
            _capture.Release(_capturedOwner);
            _capturedOwner = ViewportInputOwner.None;
        }

        return result;
    }

    public bool Escape(Vector2 point)
    {
        var result = _gestures.KeyDown(ViewportKey.Escape, point);

        if (_capturedOwner != ViewportInputOwner.None)
        {
            _capture.Release(_capturedOwner);
            _capturedOwner = ViewportInputOwner.None;
        }

        return result;
    }

    public ViewportInputOwner CapturedOwner => _capturedOwner;
}
