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

    public Vector2 RenderedImageSize =>
        ImageSize * (float)Scale;

    public Vector2 ViewportCenter =>
        ViewportSize * 0.5f;

    public Vector2 ImageCenter =>
        ImageSize * 0.5f;

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
}
