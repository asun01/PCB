using System.Numerics;
using System.Drawing;

namespace Asun.UI.Viewports;

/// <summary>
/// Pure image-to-viewport geometry used by the viewport layer.
/// It contains no WPF, DevExpress, imaging-library, or device dependencies.
/// </summary>
public readonly record struct ViewportTransform(
    double Scale,
    Vector2 Translation,
    Vector2 ImageSize,
    Vector2 ViewportSize)
{
    public static ViewportTransform Fit(
        Vector2 imageSize,
        Vector2 viewportSize)
    {
        ValidateSize(imageSize, nameof(imageSize));
        ValidateSize(viewportSize, nameof(viewportSize));

        var scale = Math.Min(
            viewportSize.X / imageSize.X,
            viewportSize.Y / imageSize.Y);

        var renderedSize = imageSize * (float)scale;
        var translation = (viewportSize - renderedSize) * 0.5f;

        return new ViewportTransform(
            scale,
            translation,
            imageSize,
            viewportSize);
    }

    public static ViewportTransform Create(
        Vector2 imageSize,
        Vector2 viewportSize,
        double scale,
        Vector2 translation)
    {
        ValidateSize(imageSize, nameof(imageSize));
        ValidateSize(viewportSize, nameof(viewportSize));

        if (!double.IsFinite(scale) || scale <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(scale), scale, "Scale must be finite and greater than zero.");
        }

        if (!IsFinite(translation))
        {
            throw new ArgumentOutOfRangeException(nameof(translation), "Translation must contain finite values.");
        }

        return new ViewportTransform(scale, translation, imageSize, viewportSize);
    }

    public Vector2 ImageToViewport(Vector2 imagePoint) =>
        Translation + imagePoint * (float)Scale;

    public Vector2 ViewportToImage(Vector2 viewportPoint) =>
        (viewportPoint - Translation) / (float)Scale;

    public RectangleF ImageToViewportRectangle(RectangleF imageRectangle) =>
        new(
            Translation.X + imageRectangle.X * (float)Scale,
            Translation.Y + imageRectangle.Y * (float)Scale,
            imageRectangle.Width * (float)Scale,
            imageRectangle.Height * (float)Scale);

    public RectangleF ViewportToImageRectangle(RectangleF viewportRectangle) =>
        new(
            (viewportRectangle.X - Translation.X) / (float)Scale,
            (viewportRectangle.Y - Translation.Y) / (float)Scale,
            viewportRectangle.Width / (float)Scale,
            viewportRectangle.Height / (float)Scale);

    public double FitScale =>
        Math.Min(
            ViewportSize.X / ImageSize.X,
            ViewportSize.Y / ImageSize.Y);

    public double ZoomRatioToFit =>
        Scale / FitScale;

    public Vector2 RenderedImageSize =>
        ImageSize * (float)Scale;

    public RectangleF RenderedImageRectangle =>
        new(Translation.X, Translation.Y, RenderedImageSize.X, RenderedImageSize.Y);

    public Vector2 ViewportCenter =>
        ViewportSize * 0.5f;

    public Vector2 ImageCenter =>
        ImageSize * 0.5f;

    public bool IsImageFullyVisible =>
        RenderedImageRectangle.X >= 0 &&
        RenderedImageRectangle.Y >= 0 &&
        RenderedImageRectangle.Right <= ViewportSize.X &&
        RenderedImageRectangle.Bottom <= ViewportSize.Y;

    public ViewportTransform WithZoomFactor(
        double zoomFactor,
        Vector2 viewportAnchor)
    {
        if (!double.IsFinite(zoomFactor) || zoomFactor <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(zoomFactor),
                zoomFactor,
                "Zoom factor must be finite and greater than zero.");
        }

        return WithScaleAround(Scale * zoomFactor, viewportAnchor);
    }

    public Vector2 ClampViewportPointToImage(Vector2 viewportPoint)
    {
        var imagePoint = ViewportToImage(viewportPoint);
        var clamped = new Vector2(
            Math.Clamp(imagePoint.X, 0f, ImageSize.X),
            Math.Clamp(imagePoint.Y, 0f, ImageSize.Y));

        return ImageToViewport(clamped);
    }

    public bool ContainsViewportPoint(Vector2 viewportPoint)
    {
        var imagePoint = ViewportToImage(viewportPoint);

        return imagePoint.X >= 0 &&
               imagePoint.Y >= 0 &&
               imagePoint.X <= ImageSize.X &&
               imagePoint.Y <= ImageSize.Y;
    }

    public ViewportTransform ZoomToImageRectangle(
        RectangleF imageRectangle,
        double paddingFactor = 0.9)
    {
        if (!double.IsFinite(paddingFactor) || paddingFactor <= 0 || paddingFactor > 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(paddingFactor),
                paddingFactor,
                "Padding factor must be greater than zero and at most one.");
        }

        if (!IsFiniteRectangle(imageRectangle) ||
            imageRectangle.Width <= 0 ||
            imageRectangle.Height <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(imageRectangle),
                "Image rectangle must contain finite positive dimensions.");
        }

        var scale = Math.Min(
            ViewportSize.X / imageRectangle.Width,
            ViewportSize.Y / imageRectangle.Height) * paddingFactor;

        if (!double.IsFinite(scale) || scale <= 0)
            throw new InvalidOperationException("Unable to compute a valid viewport scale.");

        var imageCenter = new Vector2(
            imageRectangle.X + imageRectangle.Width / 2f,
            imageRectangle.Y + imageRectangle.Height / 2f);

        var translation = ViewportCenter - imageCenter * (float)scale;

        return this with
        {
            Scale = scale,
            Translation = translation
        };
    }

    public ViewportTransform WithScaleAround(
        double newScale,
        Vector2 viewportAnchor)
    {
        if (!double.IsFinite(newScale) || newScale <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(newScale), newScale, "Scale must be finite and greater than zero.");
        }

        var imageAnchor = ViewportToImage(viewportAnchor);
        var newTranslation = viewportAnchor - imageAnchor * (float)newScale;

        return this with
        {
            Scale = newScale,
            Translation = newTranslation
        };
    }

    public ViewportTransform CenterOnImagePoint(Vector2 imagePoint)
    {
        if (!IsFinite(imagePoint))
            throw new ArgumentOutOfRangeException(nameof(imagePoint));

        return this with
        {
            Translation = ViewportCenter - imagePoint * (float)Scale
        };
    }

    public ViewportTransform CenterOnImageRectangle(RectangleF imageRectangle)
    {
        if (!IsFiniteRectangle(imageRectangle) ||
            imageRectangle.Width <= 0 ||
            imageRectangle.Height <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(imageRectangle),
                "Image rectangle must contain finite positive dimensions.");
        }

        var center = new Vector2(
            imageRectangle.X + imageRectangle.Width / 2f,
            imageRectangle.Y + imageRectangle.Height / 2f);

        return CenterOnImagePoint(center);
    }

    public ViewportTransform WithScaleAroundClamped(
        double requestedScale,
        double minScale,
        double maxScale,
        Vector2 viewportAnchor)
    {
        if (!double.IsFinite(minScale) || minScale <= 0)
            throw new ArgumentOutOfRangeException(nameof(minScale));

        if (!double.IsFinite(maxScale) || maxScale < minScale)
            throw new ArgumentOutOfRangeException(nameof(maxScale));

        if (!double.IsFinite(requestedScale) || requestedScale <= 0)
            throw new ArgumentOutOfRangeException(nameof(requestedScale));

        var clampedScale = Math.Clamp(requestedScale, minScale, maxScale);
        return WithScaleAround(clampedScale, viewportAnchor);
    }

    public ViewportTransform PanBy(Vector2 viewportDelta)
    {
        if (!IsFinite(viewportDelta))
        {
            throw new ArgumentOutOfRangeException(
                nameof(viewportDelta),
                "Viewport delta must contain finite values.");
        }

        return this with
        {
            Translation = Translation + viewportDelta
        };
    }

    public VisibleTileRange GetVisibleTileRange(Vector2 tileSize) =>
        ImageTileGeometry.CalculateVisibleTiles(
            ImageSize,
            tileSize,
            GetVisibleImageRectangle());

    public VisibleTileRange GetPrefetchTileRange(
        Vector2 tileSize,
        int marginTiles)
    {
        var visibleRange = GetVisibleTileRange(tileSize);

        return ImageTileGeometry.ExpandTileRange(
            ImageSize,
            tileSize,
            visibleRange,
            marginTiles);
    }

    public RectangleF GetVisibleImageRectangle()
    {
        var topLeft = ViewportToImage(Vector2.Zero);
        var bottomRight = ViewportToImage(ViewportSize);

        var left = Math.Clamp(Math.Min(topLeft.X, bottomRight.X), 0f, ImageSize.X);
        var top = Math.Clamp(Math.Min(topLeft.Y, bottomRight.Y), 0f, ImageSize.Y);
        var right = Math.Clamp(Math.Max(topLeft.X, bottomRight.X), 0f, ImageSize.X);
        var bottom = Math.Clamp(Math.Max(topLeft.Y, bottomRight.Y), 0f, ImageSize.Y);

        return new RectangleF(
            left,
            top,
            Math.Max(0f, right - left),
            Math.Max(0f, bottom - top));
    }

    /// <summary>
    /// Changes the viewport size while keeping the image point currently under
    /// the old viewport center centered in the new viewport.
    /// </summary>
    public ViewportTransform WithViewportSize(Vector2 viewportSize)
    {
        ValidateSize(viewportSize, nameof(viewportSize));

        var imageCenterAnchor = ViewportToImage(ViewportCenter);
        var translation = viewportSize * 0.5f - imageCenterAnchor * (float)Scale;

        return this with
        {
            ViewportSize = viewportSize,
            Translation = translation
        };
    }

    /// <summary>
    /// Clamps panning so the rendered image covers the viewport whenever possible.
    /// When the rendered image is smaller than the viewport on an axis, it is centered.
    /// </summary>
    public ViewportTransform WithTranslationClamped(Vector2 translation)
    {
        if (!IsFinite(translation))
            throw new ArgumentOutOfRangeException(nameof(translation));

        var rendered = RenderedImageSize;

        var minX = rendered.X <= ViewportSize.X
            ? (ViewportSize.X - rendered.X) / 2f
            : ViewportSize.X - rendered.X;
        var maxX = rendered.X <= ViewportSize.X
            ? minX
            : 0f;

        var minY = rendered.Y <= ViewportSize.Y
            ? (ViewportSize.Y - rendered.Y) / 2f
            : ViewportSize.Y - rendered.Y;
        var maxY = rendered.Y <= ViewportSize.Y
            ? minY
            : 0f;

        return this with
        {
            Translation = new Vector2(
                Math.Clamp(translation.X, minX, maxX),
                Math.Clamp(translation.Y, minY, maxY))
        };
    }

    public ViewportTransform WithTranslation(Vector2 translation)
    {
        if (!IsFinite(translation))
        {
            throw new ArgumentOutOfRangeException(nameof(translation), "Translation must contain finite values.");
        }

        return this with { Translation = translation };
    }

    private static void ValidateSize(Vector2 size, string parameterName)
    {
        if (!IsFinite(size) || size.X <= 0 || size.Y <= 0)
        {
            throw new ArgumentOutOfRangeException(parameterName, size, "Size must contain finite values greater than zero.");
        }
    }

    private static bool IsFinite(Vector2 value) =>
        float.IsFinite(value.X) && float.IsFinite(value.Y);

    private static bool IsFiniteRectangle(RectangleF rectangle) =>
        float.IsFinite(rectangle.X) &&
        float.IsFinite(rectangle.Y) &&
        float.IsFinite(rectangle.Width) &&
        float.IsFinite(rectangle.Height);
}
