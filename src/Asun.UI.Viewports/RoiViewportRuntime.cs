using System.Collections.Immutable;
using System.Drawing;
using System.Numerics;

namespace Asun.UI.Viewports;

public readonly record struct RoiViewportPointerEvent(
    Vector2 ViewportPoint,
    Vector2 ImagePoint,
    RoiDocumentEvent DocumentEvent,
    RoiDocumentHit Hit);

public readonly record struct RoiViewportItem(
    Guid Id,
    bool IsSelected,
    int ZIndex,
    RoiGeometry Geometry,
    IReadOnlyList<RoiControlPoint> ControlPoints,
    RectangleF Bounds);

public sealed class RoiViewportSnapshot
{
    internal RoiViewportSnapshot(
        ViewportTransform transform,
        Vector2? viewportPointer,
        Vector2? imagePointer,
        RoiDocumentSnapshot document,
        RoiDocumentHit hover,
        IEnumerable<RoiViewportItem> items)
    {
        Transform = transform;
        ViewportPointer = viewportPointer;
        ImagePointer = imagePointer;
        Document = document;
        Hover = hover;
        Items = items.ToImmutableArray();
    }

    public ViewportTransform Transform { get; }

    public Vector2? ViewportPointer { get; }

    public Vector2? ImagePointer { get; }

    public RoiDocumentSnapshot Document { get; }

    public RoiDocumentHit Hover { get; }

    public IReadOnlyList<RoiViewportItem> Items { get; }
}

/// <summary>
/// Bridges viewport coordinates and the multi-ROI editor in one UI-neutral runtime.
/// Pointer tolerances are supplied in viewport pixels and converted to image units,
/// so ROI editing remains visually stable across zoom levels.
/// </summary>
public sealed class RoiViewportRuntime
{
    private readonly object _sync = new();
    private readonly RoiDocumentRuntime _document;

    private ViewportTransform _transform;
    private Vector2? _viewportPointer;
    private RoiDocumentHit _hover = new(Guid.Empty, RoiHitResult.None);

    public RoiViewportRuntime(
        Vector2 imageSize,
        Vector2 viewportSize,
        RoiEditorMode mode = RoiEditorMode.Select)
    {
        _transform = ViewportTransform.Fit(imageSize, viewportSize);
        _document = new RoiDocumentRuntime
        {
            Mode = mode
        };
    }

    public ViewportTransform Transform
    {
        get
        {
            lock (_sync)
                return _transform;
        }
    }

    public RoiDocumentRuntime Document => _document;

    public Guid? SelectedId => _document.SelectedId;

    public RoiDocumentHit Hover
    {
        get
        {
            lock (_sync)
                return _hover;
        }
    }

    public Vector2? ViewportPointer
    {
        get
        {
            lock (_sync)
                return _viewportPointer;
        }
    }

    public Vector2? ImagePointer
    {
        get
        {
            lock (_sync)
            {
                return _viewportPointer is Vector2 point
                    ? _transform.ViewportToImage(point)
                    : null;
            }
        }
    }

    public RoiEditorMode Mode
    {
        get => _document.Mode;
        set => _document.Mode = value;
    }

    public void SetTransform(ViewportTransform transform)
    {
        lock (_sync)
        {
            _transform = transform;
            RefreshHoverUnsafe();
        }
    }

    public void FitToViewport()
    {
        lock (_sync)
        {
            _transform = ViewportTransform.Fit(
                _transform.ImageSize,
                _transform.ViewportSize);
            RefreshHoverUnsafe();
        }
    }

    public void ResizeViewport(Vector2 viewportSize)
    {
        lock (_sync)
        {
            _transform = _transform.WithViewportSize(viewportSize);
            RefreshHoverUnsafe();
        }
    }

    public void PanBy(Vector2 viewportDelta, bool clamp = true)
    {
        ValidateFinite(viewportDelta, nameof(viewportDelta));

        lock (_sync)
        {
            _transform = clamp
                ? _transform.PanByClamped(viewportDelta)
                : _transform.PanBy(viewportDelta);

            RefreshHoverUnsafe();
        }
    }

    public void ZoomAt(
        double zoomFactor,
        double minScale,
        double maxScale,
        Vector2 viewportAnchor)
    {
        ValidateFinite(viewportAnchor, nameof(viewportAnchor));

        lock (_sync)
        {
            _transform = _transform.WithZoomFactorClamped(
                zoomFactor,
                minScale,
                maxScale,
                viewportAnchor);

            RefreshHoverUnsafe();
        }
    }

    public void CenterOnImagePoint(Vector2 imagePoint)
    {
        ValidateFinite(imagePoint, nameof(imagePoint));

        lock (_sync)
        {
            _transform = _transform.CenterOnImagePoint(imagePoint);
            RefreshHoverUnsafe();
        }
    }

    public RoiViewportPointerEvent PointerDown(
        Vector2 viewportPoint,
        float handleTolerancePixels = 8f,
        float bodyTolerancePixels = 0f)
    {
        ValidateFinite(viewportPoint, nameof(viewportPoint));
        ValidateTolerance(handleTolerancePixels, nameof(handleTolerancePixels));
        ValidateTolerance(bodyTolerancePixels, nameof(bodyTolerancePixels));

        lock (_sync)
        {
            var imagePoint = _transform.ViewportToImage(viewportPoint);
            var documentEvent = _document.PointerDown(
                imagePoint,
                ToImageTolerance(handleTolerancePixels),
                ToImageTolerance(bodyTolerancePixels));

            _viewportPointer = viewportPoint;
            RefreshHoverUnsafe(viewportPoint);

            return new RoiViewportPointerEvent(
                viewportPoint,
                imagePoint,
                documentEvent,
                _hover);
        }
    }

    public RoiViewportPointerEvent PointerMove(
        Vector2 viewportPoint,
        float handleTolerancePixels = 8f)
    {
        ValidateFinite(viewportPoint, nameof(viewportPoint));
        ValidateTolerance(handleTolerancePixels, nameof(handleTolerancePixels));

        lock (_sync)
        {
            var imagePoint = _transform.ViewportToImage(viewportPoint);
            var documentEvent = _document.PointerMove(
                imagePoint,
                ToImageTolerance(handleTolerancePixels));

            _viewportPointer = viewportPoint;
            RefreshHoverUnsafe(viewportPoint);

            return new RoiViewportPointerEvent(
                viewportPoint,
                imagePoint,
                documentEvent,
                _hover);
        }
    }

    public RoiViewportPointerEvent PointerUp(Vector2 viewportPoint)
    {
        ValidateFinite(viewportPoint, nameof(viewportPoint));

        lock (_sync)
        {
            var imagePoint = _transform.ViewportToImage(viewportPoint);
            var documentEvent = _document.PointerUp(imagePoint);

            _viewportPointer = viewportPoint;
            RefreshHoverUnsafe(viewportPoint);

            return new RoiViewportPointerEvent(
                viewportPoint,
                imagePoint,
                documentEvent,
                _hover);
        }
    }

    public RoiViewportPointerEvent Cancel(Vector2 viewportPoint)
    {
        ValidateFinite(viewportPoint, nameof(viewportPoint));

        lock (_sync)
        {
            var imagePoint = _transform.ViewportToImage(viewportPoint);
            var documentEvent = _document.Cancel(imagePoint);

            _viewportPointer = viewportPoint;
            RefreshHoverUnsafe(viewportPoint);

            return new RoiViewportPointerEvent(
                viewportPoint,
                imagePoint,
                documentEvent,
                _hover);
        }
    }

    public bool Undo()
    {
        lock (_sync)
        {
            var changed = _document.Undo();
            RefreshHoverUnsafe();
            return changed;
        }
    }

    public bool Redo()
    {
        lock (_sync)
        {
            var changed = _document.Redo();
            RefreshHoverUnsafe();
            return changed;
        }
    }

    public RoiViewportSnapshot CreateSnapshot()
    {
        lock (_sync)
        {
            return CreateSnapshotUnsafe();
        }
    }

    public RoiDocumentHit HitTestViewport(
        Vector2 viewportPoint,
        float handleTolerancePixels = 8f,
        float bodyTolerancePixels = 0f)
    {
        ValidateFinite(viewportPoint, nameof(viewportPoint));
        ValidateTolerance(handleTolerancePixels, nameof(handleTolerancePixels));
        ValidateTolerance(bodyTolerancePixels, nameof(bodyTolerancePixels));

        lock (_sync)
        {
            var imagePoint = _transform.ViewportToImage(viewportPoint);
            return _document.HitTest(
                imagePoint,
                ToImageTolerance(handleTolerancePixels),
                ToImageTolerance(bodyTolerancePixels));
        }
    }

    private RoiViewportSnapshot CreateSnapshotUnsafe()
    {
        var items = new List<RoiViewportItem>();
        var selectedId = _document.SelectedId;

        foreach (var item in _document.Items)
        {
            var viewportGeometry = TransformGeometry(
                item.Geometry,
                _transform);

            items.Add(new RoiViewportItem(
                item.Id,
                item.Id == selectedId,
                item.ZIndex,
                viewportGeometry,
                viewportGeometry.GetControlPoints(),
                viewportGeometry.GetBounds()));
        }

        return new RoiViewportSnapshot(
            _transform,
            _viewportPointer,
            _viewportPointer is Vector2 point
                ? _transform.ViewportToImage(point)
                : null,
            _document.CreateSnapshot(),
            _hover,
            items);
    }

    private void RefreshHoverUnsafe(Vector2? viewportPoint = null)
    {
        var point = viewportPoint ?? _viewportPointer;

        if (point is not Vector2 value)
        {
            _hover = new RoiDocumentHit(Guid.Empty, RoiHitResult.None);
            return;
        }

        var imagePoint = _transform.ViewportToImage(value);
        _hover = _document.HitTest(
            imagePoint,
            ToImageTolerance(8f),
            0f);
    }

    private float ToImageTolerance(float viewportPixels)
    {
        var scale = Math.Max(_transform.Scale, double.Epsilon);
        var imageTolerance = viewportPixels / scale;

        if (!double.IsFinite(imageTolerance))
            throw new InvalidOperationException("The viewport scale is invalid.");

        return (float)imageTolerance;
    }

    private static RoiGeometry TransformGeometry(
        RoiGeometry geometry,
        ViewportTransform transform)
    {
        if (geometry.IsPolygon)
        {
            return RoiGeometry.CreatePolygon(
                geometry.Vertices.Select(transform.ImageToViewport));
        }

        var center = transform.ImageToViewport(geometry.Center);
        var size = geometry.Size * (float)transform.Scale;

        return geometry.Kind switch
        {
            RoiShapeKind.Rectangle =>
                RoiGeometry.CreateRectangle(center, size),
            RoiShapeKind.RotatedRectangle =>
                RoiGeometry.CreateRotatedRectangle(
                    center,
                    size,
                    geometry.RotationRadians),
            RoiShapeKind.Ellipse =>
                RoiGeometry.CreateEllipse(
                    center,
                    size,
                    geometry.RotationRadians),
            _ => geometry
        };
    }

    private static void ValidateTolerance(float value, string parameterName)
    {
        if (!float.IsFinite(value) || value < 0f)
            throw new ArgumentOutOfRangeException(parameterName);
    }

    private static void ValidateFinite(Vector2 value, string parameterName)
    {
        if (!float.IsFinite(value.X) || !float.IsFinite(value.Y))
            throw new ArgumentOutOfRangeException(parameterName);
    }
}
