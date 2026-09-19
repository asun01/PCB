using System.Numerics;

namespace Asun.UI.Viewports;

public readonly record struct ViewportCompositeInputResult(
    ViewportGestureKind Kind,
    Vector2 ViewportPoint,
    Vector2 ImagePoint,
    bool TransformChanged,
    bool DocumentChanged,
    bool SelectionChanged,
    ViewportDirtyFlags DirtyFlags);

public sealed class ViewportCompositeInputRuntime<TTile>
{
    private readonly object _sync = new();
    private readonly ViewportCompositeRuntime<TTile> _composite;
    private readonly float _handleTolerancePixels;
    private readonly float _bodyTolerancePixels;
    private readonly double _minScale;
    private readonly double _maxScale;
    private readonly double _wheelStep;

    private ViewportGestureKind _kind;
    private bool _pointerDown;
    private Vector2 _pointerPosition;

    public ViewportCompositeInputRuntime(
        ViewportCompositeRuntime<TTile> composite,
        float handleTolerancePixels = 8f,
        float bodyTolerancePixels = 0f,
        double minScale = 0.01,
        double maxScale = 64d,
        double wheelStep = 1.1)
    {
        ArgumentNullException.ThrowIfNull(composite);

        ValidateTolerance(handleTolerancePixels, nameof(handleTolerancePixels));
        ValidateTolerance(bodyTolerancePixels, nameof(bodyTolerancePixels));

        if (!double.IsFinite(minScale) || minScale <= 0)
            throw new ArgumentOutOfRangeException(nameof(minScale));

        if (!double.IsFinite(maxScale) || maxScale < minScale)
            throw new ArgumentOutOfRangeException(nameof(maxScale));

        if (!double.IsFinite(wheelStep) || wheelStep <= 1)
            throw new ArgumentOutOfRangeException(nameof(wheelStep));

        _composite = composite;
        _handleTolerancePixels = handleTolerancePixels;
        _bodyTolerancePixels = bodyTolerancePixels;
        _minScale = minScale;
        _maxScale = maxScale;
        _wheelStep = wheelStep;
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
                    _pointerDown ? _pointerPosition : null);
            }
        }
    }

    public ViewportCompositeRuntime<TTile> Composite => _composite;

    public ViewportCompositeInputResult Apply(
        ViewportInputEvent input)
    {
        lock (_sync)
        {
            return input.Kind switch
            {
                ViewportInputEventKind.PointerDown =>
                    ApplyPointerDownUnsafe(input),
                ViewportInputEventKind.PointerMove =>
                    ApplyPointerMoveUnsafe(input),
                ViewportInputEventKind.PointerUp =>
                    ApplyPointerUpUnsafe(input),
                ViewportInputEventKind.Wheel =>
                    ApplyWheelUnsafe(input),
                ViewportInputEventKind.DoubleClick =>
                    ApplyDoubleClickUnsafe(input),
                ViewportInputEventKind.Escape =>
                    ApplyEscapeUnsafe(input),
                _ => throw new ArgumentOutOfRangeException(
                    nameof(input),
                    input.Kind,
                    "Unsupported viewport input event.")
            };
        }
    }

    public void Reset()
    {
        lock (_sync)
        {
            _kind = ViewportGestureKind.Idle;
            _pointerDown = false;
            _pointerPosition = default;
        }
    }

    private ViewportCompositeInputResult ApplyPointerDownUnsafe(
        ViewportInputEvent input)
    {
        if (input.Button == ViewportMouseButton.Middle)
        {
            BeginPointerUnsafe(
                input.Position,
                ViewportGestureKind.Panning);

            return CreateResultUnsafe(
                input.Position,
                false,
                false,
                false,
                ViewportDirtyFlags.Overlay);
        }

        if (input.Button != ViewportMouseButton.Left)
        {
            _kind = ViewportGestureKind.Idle;
            _pointerDown = false;

            return CreateResultUnsafe(
                input.Position,
                false,
                false,
                false,
                ViewportDirtyFlags.None);
        }

        var documentEvent = _composite.PointerDown(
            input.Position,
            _handleTolerancePixels,
            _bodyTolerancePixels);

        var interaction = documentEvent.EditorEvent.Interaction;

        var kind = interaction switch
        {
            RoiInteractionKind.Creating =>
                ViewportGestureKind.CreatingRoi,
            RoiInteractionKind.Moving or
            RoiInteractionKind.Resizing or
            RoiInteractionKind.Rotating =>
                ViewportGestureKind.RoiEditing,
            _ => ViewportGestureKind.Panning
        };

        BeginPointerUnsafe(input.Position, kind);

        var transformChanged = false;
        var dirty =
            ViewportDirtyFlags.Roi |
            (documentEvent.SelectionChanged
                ? ViewportDirtyFlags.Selection
                : ViewportDirtyFlags.None);

        return CreateResultUnsafe(
            input.Position,
            transformChanged,
            documentEvent.DocumentChanged,
            documentEvent.SelectionChanged,
            dirty);
    }

    private ViewportCompositeInputResult ApplyPointerMoveUnsafe(
        ViewportInputEvent input)
    {
        if (!_pointerDown)
        {
            var result = _composite.PointerMove(
                input.Position,
                _handleTolerancePixels);

            _pointerPosition = input.Position;

            return CreateResultUnsafe(
                input.Position,
                false,
                result.DocumentEvent.DocumentChanged,
                result.DocumentEvent.SelectionChanged,
                ViewportDirtyFlags.Roi | ViewportDirtyFlags.Selection);
        }

        if (_kind is ViewportGestureKind.RoiEditing or
            ViewportGestureKind.CreatingRoi)
        {
            var result = _composite.PointerMove(
                input.Position,
                _handleTolerancePixels);

            _pointerPosition = input.Position;

            return CreateResultUnsafe(
                input.Position,
                false,
                result.DocumentEvent.DocumentChanged,
                result.DocumentEvent.SelectionChanged,
                ViewportDirtyFlags.Roi | ViewportDirtyFlags.Selection);
        }

        var delta = input.Position - _pointerPosition;

        if (delta != Vector2.Zero)
            _composite.PanBy(delta);

        _pointerPosition = input.Position;

        return CreateResultUnsafe(
            input.Position,
            delta != Vector2.Zero,
            false,
            false,
            delta != Vector2.Zero
                ? ViewportDirtyFlags.Image |
                  ViewportDirtyFlags.Transform |
                  ViewportDirtyFlags.Roi
                : ViewportDirtyFlags.Overlay);
    }

    private ViewportCompositeInputResult ApplyPointerUpUnsafe(
        ViewportInputEvent input)
    {
        if (!_pointerDown)
        {
            _pointerPosition = input.Position;

            return CreateResultUnsafe(
                input.Position,
                false,
                false,
                false,
                ViewportDirtyFlags.None);
        }

        var completedKind = _kind;

        if (_kind is ViewportGestureKind.RoiEditing or
            ViewportGestureKind.CreatingRoi)
        {
            var result = _composite.PointerUp(input.Position);
            EndPointerUnsafe(input.Position);

            return CreateResultUnsafe(
                input.Position,
                false,
                result.DocumentEvent.DocumentChanged,
                result.DocumentEvent.SelectionChanged,
                ViewportDirtyFlags.Roi | ViewportDirtyFlags.Selection);
        }

        EndPointerUnsafe(input.Position);

        return CreateResultUnsafe(
            input.Position,
            false,
            false,
            false,
            completedKind == ViewportGestureKind.Panning
                ? ViewportDirtyFlags.Overlay
                : ViewportDirtyFlags.None);
    }

    private ViewportCompositeInputResult ApplyWheelUnsafe(
        ViewportInputEvent input)
    {
        if (input.WheelDelta == 0)
        {
            return CreateResultUnsafe(
                input.Position,
                false,
                false,
                false,
                ViewportDirtyFlags.None);
        }

        var before = _composite.Transform;
        var steps = input.WheelDelta / 120d;
        var factor = Math.Pow(_wheelStep, steps);

        _composite.ZoomAt(
            factor,
            _minScale,
            _maxScale,
            input.Position);

        return CreateResultUnsafe(
            input.Position,
            before != _composite.Transform,
            false,
            false,
            ViewportDirtyFlags.Image |
            ViewportDirtyFlags.Transform |
            ViewportDirtyFlags.Roi);
    }

    private ViewportCompositeInputResult ApplyDoubleClickUnsafe(
        ViewportInputEvent input)
    {
        var before = _composite.Transform;
        var hit = _composite.RoiRuntime.HitTestViewport(
            input.Position,
            _handleTolerancePixels,
            _bodyTolerancePixels);

        if (hit.Id == Guid.Empty)
        {
            _composite.FitToViewport();
        }
        else
        {
            _composite.SelectRoi(hit.Id);

            var item = _composite.RoiRuntime.Document.Items
                .Single(item => item.Id == hit.Id);

            _composite.CenterOnImagePoint(
                item.Geometry.Center);
        }

        return CreateResultUnsafe(
            input.Position,
            before != _composite.Transform,
            false,
            hit.Id != Guid.Empty &&
            _composite.RoiRuntime.SelectedId == hit.Id,
            ViewportDirtyFlags.Image |
            ViewportDirtyFlags.Transform |
            ViewportDirtyFlags.Roi |
            ViewportDirtyFlags.Selection);
    }

    private ViewportCompositeInputResult ApplyEscapeUnsafe(
        ViewportInputEvent input)
    {
        var changed = false;

        if (_kind is ViewportGestureKind.RoiEditing or
            ViewportGestureKind.CreatingRoi)
        {
            var result = _composite.CancelPointer(
                input.Position);

            changed = result.DocumentEvent.DocumentChanged;
        }

        _kind = ViewportGestureKind.Idle;
        _pointerDown = false;
        _pointerPosition = input.Position;

        return CreateResultUnsafe(
            input.Position,
            false,
            changed,
            false,
            changed
                ? ViewportDirtyFlags.Roi | ViewportDirtyFlags.Selection
                : ViewportDirtyFlags.Overlay);
    }

    private ViewportCompositeInputResult CreateResultUnsafe(
        Vector2 point,
        bool transformChanged,
        bool documentChanged,
        bool selectionChanged,
        ViewportDirtyFlags dirtyFlags)
    {
        return new ViewportCompositeInputResult(
            _kind,
            point,
            _composite.Transform.ViewportToImage(point),
            transformChanged,
            documentChanged,
            selectionChanged,
            dirtyFlags);
    }

    private void BeginPointerUnsafe(
        Vector2 point,
        ViewportGestureKind kind)
    {
        _pointerDown = true;
        _pointerPosition = point;
        _kind = kind;
    }

    private void EndPointerUnsafe(Vector2 point)
    {
        _pointerDown = false;
        _pointerPosition = point;
        _kind = ViewportGestureKind.Idle;
    }

    private static void ValidateTolerance(
        float value,
        string parameterName)
    {
        if (!float.IsFinite(value) || value < 0)
            throw new ArgumentOutOfRangeException(parameterName);
    }
}
