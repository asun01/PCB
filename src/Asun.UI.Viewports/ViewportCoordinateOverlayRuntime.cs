using System.Drawing;
using System.Numerics;

namespace Asun.UI.Viewports;

public readonly record struct ViewportCoordinateReadout(
    Vector2 ViewportPoint,
    Vector2 ImagePoint,
    string XText,
    string YText);

public readonly record struct ViewportGridLine(
    Vector2 Start,
    Vector2 End,
    bool Major,
    string Label);

public readonly record struct ViewportCrosshair(
    Vector2 HorizontalStart,
    Vector2 HorizontalEnd,
    Vector2 VerticalStart,
    Vector2 VerticalEnd);

public sealed class ViewportCoordinateOverlayRuntime
{
    private readonly ViewportTransform _transform;
    private readonly int _majorEvery;

    public ViewportCoordinateOverlayRuntime(
        ViewportTransform transform,
        int majorEvery = 5)
    {
        if (majorEvery <= 0)
            throw new ArgumentOutOfRangeException(nameof(majorEvery));

        _transform = transform;
        _majorEvery = majorEvery;
    }

    public ViewportCoordinateReadout GetReadout(
        Vector2 viewportPoint,
        int decimals = 3)
    {
        Validate(viewportPoint);

        if (decimals < 0 || decimals > 12)
            throw new ArgumentOutOfRangeException(nameof(decimals));

        var image = _transform.ViewportToImage(viewportPoint);
        var format = decimals == 0 ? "0" : $"0.{new string('0', decimals)}";

        return new ViewportCoordinateReadout(
            viewportPoint,
            image,
            image.X.ToString(format, System.Globalization.CultureInfo.InvariantCulture),
            image.Y.ToString(format, System.Globalization.CultureInfo.InvariantCulture));
    }

    public ViewportCrosshair GetCrosshair(
        Vector2 viewportPoint)
    {
        Validate(viewportPoint);

        return new ViewportCrosshair(
            new Vector2(0, viewportPoint.Y),
            new Vector2(_transform.ViewportSize.X, viewportPoint.Y),
            new Vector2(viewportPoint.X, 0),
            new Vector2(viewportPoint.X, _transform.ViewportSize.Y));
    }

    public IReadOnlyList<ViewportGridLine> BuildGrid(
        double imageGridSpacing,
        int maxLines = 200)
    {
        if (!double.IsFinite(imageGridSpacing) || imageGridSpacing <= 0)
            throw new ArgumentOutOfRangeException(nameof(imageGridSpacing));

        if (maxLines <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxLines));

        var visible = _transform.GetVisibleImageRectangle();
        var firstX = Math.Floor(visible.Left / imageGridSpacing) * imageGridSpacing;
        var firstY = Math.Floor(visible.Top / imageGridSpacing) * imageGridSpacing;

        var lines = new List<ViewportGridLine>();
        var xCount = (int)Math.Min(
            maxLines,
            Math.Ceiling((visible.Right - firstX) / imageGridSpacing) + 1);
        var yCount = (int)Math.Min(
            maxLines - xCount,
            Math.Ceiling((visible.Bottom - firstY) / imageGridSpacing) + 1);

        for (var i = 0; i < xCount; i++)
        {
            var x = firstX + i * imageGridSpacing;
            var start = _transform.ImageToViewport(
                new Vector2((float)x, visible.Top));
            var end = _transform.ImageToViewport(
                new Vector2((float)x, visible.Bottom));

            lines.Add(new ViewportGridLine(
                start,
                end,
                IsMajor(i),
                x.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture)));
        }

        for (var i = 0; i < yCount; i++)
        {
            var y = firstY + i * imageGridSpacing;
            var start = _transform.ImageToViewport(
                new Vector2(visible.Left, (float)y));
            var end = _transform.ImageToViewport(
                new Vector2(visible.Right, (float)y));

            lines.Add(new ViewportGridLine(
                start,
                end,
                IsMajor(i),
                y.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture)));
        }

        return lines;
    }

    public static double ChooseScaleBarLength(
        double visibleImageWidth,
        double targetScreenPixels,
        double scale)
    {
        if (!double.IsFinite(visibleImageWidth) || visibleImageWidth <= 0)
            throw new ArgumentOutOfRangeException(nameof(visibleImageWidth));
        if (!double.IsFinite(targetScreenPixels) || targetScreenPixels <= 0)
            throw new ArgumentOutOfRangeException(nameof(targetScreenPixels));
        if (!double.IsFinite(scale) || scale <= 0)
            throw new ArgumentOutOfRangeException(nameof(scale));

        var raw = targetScreenPixels / scale;
        var exponent = Math.Floor(Math.Log10(raw));
        var magnitude = Math.Pow(10, exponent);
        var normalized = raw / magnitude;

        var nice = normalized >= 5
            ? 5
            : normalized >= 2
                ? 2
                : 1;

        return nice * magnitude;
    }

    private bool IsMajor(int index) =>
        index % _majorEvery == 0;

    private static void Validate(Vector2 point)
    {
        if (!float.IsFinite(point.X) || !float.IsFinite(point.Y))
            throw new ArgumentOutOfRangeException(nameof(point));
    }
}
