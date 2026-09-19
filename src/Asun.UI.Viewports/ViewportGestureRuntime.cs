using System.Drawing;
using System.Numerics;

namespace Asun.UI.Viewports;

public enum ViewportGestureKind
{
    Idle,
    Panning,
    RoiEditing,
    CreatingRoi
}

public enum ViewportMouseButton
{
    Left,
    Middle,
    Right
}

public enum ViewportKey
{
    Escape
}

public readonly record struct ViewportGestureSnapshot(
    ViewportGestureKind Kind,
    bool IsPointerDown,
    Vector2 PointerPosition,
    Vector2? PointerDownPosition);

public readonly record struct ViewportGestureEvent(
    ViewportGestureKind Kind,
    Vector2 ViewportPoint,
    Vector2 ImagePoint,
    bool TransformChanged,
    bool DocumentChanged,
    bool SelectionChanged);

/// <summary>
/// UI-neutral input state machine for an interactive image viewport.
/// It routes pointer gestures to ROI editing or panning, converts wheel deltas
/// into anchored zoom, supports double-click focus, and provides one consistent
/// Escape cancellation path.
/// </summary>
public sealed class ViewportGestureRuntime
{
    private readonly object _sync = new();
    private readonly RoiViewportRuntime _viewport;
    private readonly float _handleTolerancePixels;
    private readonly float _bodyTolerancePixels;
    private readonly double _minScale;
    private readonly double _maxScale;
    private readonly double _wheelStep;

    private ViewportGestureKind _kind;
    private bool _pointerDown;
    private Vector2 _pointerPosition;
    private Vector2 _pointerDownPosition;

    public ViewportGestureRuntime(
        RoiViewportRuntime viewport,
        float handleTolerancePixels = 8f,
        float bodyTolerancePixels = 0f,
        double minScale = 0.01,
        double maxScale = 64d,
        double wheelStep = 1.1)
    {
        ArgumentNullException.ThrowIfNull(viewport);

        ValidateTolerance(handleTolerancePixels, nameof(handleTolerancePixels));
        ValidateTolerance(bodyTolerancePixels, nameof(bodyTolerancePixels));

        if (!double.IsFinite(minScale) || minScale <= 0)
            throw new ArgumentOutOfRangeException(nameof(minScale));

        if (!double.IsFinite(maxScale) || maxScale < minScale)
            throw new ArgumentOutOfRangeException(nameof(maxScale));

        if (!double.IsFinite(wheelStep) || wheelStep <= 1)
            throw new ArgumentOutOfRangeException(nameof(wheelStep));

        _viewport = viewport;
        _handleTolerancePixels = handleTolerancePixels;
        _bodyTolerancePixels = bodyTolerancePixels;
        _minScale = minScale;
        _maxScale = maxScale;
        _wheelStep = wheelStep;
        _kind = ViewportGestureKind.Idle;
    }

    public ViewportGestureKind Kind
    {
        get
        {
            lock (_sync)
                return _kind;
        }
    }

    public bool IsPointerDown
    {
        get
        {
            lock (_sync)
                return _pointerDown;
        }
    }

    public ViewportGestureSnapshot Snapshot
    {
        get
        {
            lock (_sync)
            {
                return new ViewportGestureSnapshot(
                    _kind,
                    _pointerDown,
                    _pointerPosition,
                    _pointerDown ? _pointerDownPosition : null);
            }
        }
    }

    public RoiViewportRuntime Viewport => _viewport;

    public ViewportGestureEvent PointerDown(
        Vector2 viewportPoint,
        ViewportMouseButton button = ViewportMouseButton.Left)
    {
        ValidatePoint(viewportPoint);

        lock (_sync)
        {
            if (button == ViewportMouseButton.Middle)
            {
                BeginPointerUnsafe(
                    viewportPoint,
                    ViewportGestureKind.Panning);

                return CreateEventUnsafe(
                    ViewportGestureKind.Panning,
                    viewportPoint,
                    transformChanged: false,
                    documentChanged: false,
                    selectionChanged: false);
            }

            if (button != ViewportMouseButton.Left)
            {
                _kind = ViewportGestureKind.Idle;
                _pointerDown = false;

                return CreateEventUnsafe(
                    ViewportGestureKind.Idle,
                    viewportPoint,
                    false,
                    false,
                    false);
            }

            var previousTransform = _viewport.Transform;
            var documentEvent = _viewport.PointerDown(
                viewportPoint,
                _handleTolerancePixels,
                _bodyTolerancePixels);

            BeginPointerUnsafe(
                viewportPoint,
                documentEvent.EditorEvent.Interaction switch
                {
                    RoiInteractionKind.Creating => ViewportGestureKind.CreatingRoi,
                    RoiInteractionKind.Moving or
                    RoiInteractionKind.Resizing or
                    RoiInteractionKind.Rotating => ViewportGestureKind.RoiEditing,
                    _ => ViewportGestureKind.Panning
                });

            if (documentEvent.EditorEvent.Interaction == RoiInteractionKind.Idle)
                _kind = ViewportGestureKind.Panning;

            return CreateEventUnsafe(
                _kind,
                viewportPoint,
                previousTransform != _viewport.Transform,
                documentEvent.DocumentEventChanged(),
                documentEvent.SelectionChanged);
        }
    }

    public ViewportGestureEvent PointerMove(Vector2 viewportPoint)
    {
        ValidatePoint(viewportPoint);

        lock (_sync)
        {
            if (!_pointerDown)
            {
                var hoverEvent = _viewport.PointerMove(
                    viewportPoint,
                    _handleTolerancePixels);

                _pointerPosition = viewportPoint;

                return CreateEventUnsafe(
                    ViewportGestureKind.Idle,
                    viewportPoint,
                    false,
                    hoverEvent.DocumentEventChanged(),
                    hoverEvent.SelectionChanged);
            }

            if (_kind is ViewportGestureKind.RoiEditing or
                ViewportGestureKind.CreatingRoi)
            {
                var documentEvent = _viewport.PointerMove(
                    viewportPoint,
                    _handleTolerancePixels);

                _pointerPosition = viewportPoint;

                return CreateEventUnsafe(
                    _kind,
                    viewportPoint,
                    false,
                    documentEvent.DocumentEventChanged(),
                    documentEvent.SelectionChanged);
            }

            var delta = viewportPoint - _pointerPosition;
            _viewport.PanBy(delta, clamp: true);
            _pointerPosition = viewportPoint;

            return CreateEventUnsafe(
                ViewportGestureKind.Panning,
                viewportPoint,
                delta != Vector2.Zero,
                false,
                false);
        }
    }

    public ViewportGestureEvent PointerUp(Vector2 viewportPoint)
    {
        ValidatePoint(viewportPoint);

        lock (_sync)
        {
            if (!_pointerDown)
            {
                _pointerPosition = viewportPoint;

                return CreateEventUnsafe(
                    ViewportGestureKind.Idle,
                    viewportPoint,
                    false,
                    false,
                    false);
            }

            var completedKind = _kind;

            if (_kind is ViewportGestureKind.RoiEditing or
                ViewportGestureKind.CreatingRoi)
            {
                var documentEvent = _viewport.PointerUp(viewportPoint);
                EndPointerUnsafe(viewportPoint);

                return CreateEventUnsafe(
                    completedKind,
                    viewportPoint,
                    false,
                    documentEvent.DocumentEventChanged(),
                    documentEvent.SelectionChanged);
            }

            EndPointerUnsafe(viewportPoint);

            return CreateEventUnsafe(
                completedKind,
                viewportPoint,
                false,
                false,
                false);
        }
    }

    public ViewportGestureEvent Wheel(
        Vector2 viewportPoint,
        int wheelDelta)
    {
        ValidatePoint(viewportPoint);

        if (wheelDelta == 0)
        {
            return new ViewportGestureEvent(
                ViewportGestureKind.Idle,
                viewportPoint,
                _viewport.Transform.ViewportToImage(viewportPoint),
                false,
                false,
                false);
        }

        lock (_sync)
        {
            var before = _viewport.Transform;
            var steps = wheelDelta / 120d;
            var factor = Math.Pow(_wheelStep, steps);

            _viewport.ZoomAt(
                factor,
                _minScale,
                _maxScale,
                viewportPoint);

            var after = _viewport.Transform;

            return CreateEventUnsafe(
                _kind,
                viewportPoint,
                before != after,
                false,
                false);
        }
    }

    public ViewportGestureEvent DoubleClick(Vector2 viewportPoint)
    {
        ValidatePoint(viewportPoint);

        lock (_sync)
        {
            var before = _viewport.Transform;
            var hit = _viewport.HitTestViewport(
                viewportPoint,
                _handleTolerancePixels,
                _bodyTolerancePixels);

            if (hit.Id == Guid.Empty)
            {
                _viewport.FitToViewport();
            }
            else
            {
                _viewport.Document.Select(hit.Id);

                var item = _viewport.Document.Items
                    .Single(item => item.Id == hit.Id);

                _viewport.SetTransform(
                    before.ZoomToImageRectangle(
                        item.Geometry.GetBounds(),
                        0.85));
            }

            var after = _viewport.Transform;
            return CreateEventUnsafe(
                _kind,
                viewportPoint,
                before != after,
                false,
                hit.Id != Guid.Empty &&
                _viewport.SelectedId == hit.Id);
        }
    }

    public bool KeyDown(ViewportKey key, Vector2 currentViewportPoint)
    {
        ValidatePoint(currentViewportPoint);

        lock (_sync)
        {
            if (key != ViewportKey.Escape)
                return false;

            var changed = false;

            if (_kind is ViewportGestureKind.RoiEditing or
                ViewportGestureKind.CreatingRoi)
            {
                _viewport.Cancel(currentViewportPoint);
                changed = true;
            }

            _kind = ViewportGestureKind.Idle;
            _pointerDown = false;
            _pointerPosition = currentViewportPoint;

            return changed;
        }
    }

    public void ResetGestureState()
    {
        lock (_sync)
        {
            _kind = ViewportGestureKind.Idle;
            _pointerDown = false;
            _pointerPosition = default;
            _pointerDownPosition = default;
        }
    }

    private void BeginPointerUnsafe(
        Vector2 viewportPoint,
        ViewportGestureKind kind)
    {
        _pointerDown = true;
        _pointerDownPosition = viewportPoint;
        _pointerPosition = viewportPoint;
        _kind = kind;
    }

    private void EndPointerUnsafe(Vector2 viewportPoint)
    {
        _pointerDown = false;
        _pointerPosition = viewportPoint;
        _kind = ViewportGestureKind.Idle;
    }

    private ViewportGestureEvent CreateEventUnsafe(
        ViewportGestureKind kind,
        Vector2 viewportPoint,
        bool transformChanged,
        bool documentChanged,
        bool selectionChanged)
    {
        return new ViewportGestureEvent(
            kind,
            viewportPoint,
            _viewport.Transform.ViewportToImage(viewportPoint),
            transformChanged,
            documentChanged,
            selectionChanged);
    }

    private static void ValidateTolerance(
        float value,
        string parameterName)
    {
        if (!float.IsFinite(value) || value < 0f)
            throw new ArgumentOutOfRangeException(parameterName);
    }

    private static void ValidatePoint(Vector2 point)
    {
        if (!float.IsFinite(point.X) || !float.IsFinite(point.Y))
            throw new ArgumentOutOfRangeException(nameof(point));
    }
}

internal static class RoiViewportEventExtensions
{
    public static bool DocumentEventChanged(
        this RoiDocumentEvent value) =>
        value.DocumentChanged;
}
