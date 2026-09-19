using System.Drawing;
using System.Numerics;

namespace Asun.UI.Viewports;

public readonly record struct TileIndex(int X, int Y);

public readonly record struct VisibleTileRange(
    TileIndex Minimum,
    TileIndex Maximum)
{
    public int Width => Maximum.X - Minimum.X + 1;
    public int Height => Maximum.Y - Minimum.Y + 1;
    public int Count => checked(Width * Height);

    public IEnumerable<TileIndex> Enumerate()
    {
        for (var y = Minimum.Y; y <= Maximum.Y; y++)
        {
            for (var x = Minimum.X; x <= Maximum.X; x++)
                yield return new TileIndex(x, y);
        }
    }
}

/// <summary>
/// Computes the finite tile window needed to cover a visible image rectangle.
/// This is pure geometry and does not load or own image data.
/// </summary>
public static class ImageTileGeometry
{
    public static VisibleTileRange CalculateVisibleTiles(
        Vector2 imageSize,
        Vector2 tileSize,
        RectangleF visibleImageRectangle)
    {
        ValidatePositiveFinite(imageSize, nameof(imageSize));
        ValidatePositiveFinite(tileSize, nameof(tileSize));

        if (float.IsNaN(visibleImageRectangle.X) ||
            float.IsNaN(visibleImageRectangle.Y) ||
            float.IsNaN(visibleImageRectangle.Width) ||
            float.IsNaN(visibleImageRectangle.Height) ||
            float.IsInfinity(visibleImageRectangle.X) ||
            float.IsInfinity(visibleImageRectangle.Y) ||
            float.IsInfinity(visibleImageRectangle.Width) ||
            float.IsInfinity(visibleImageRectangle.Height))
        {
            throw new ArgumentOutOfRangeException(
                nameof(visibleImageRectangle),
                "Visible rectangle must contain finite values.");
        }

        if (visibleImageRectangle.Width < 0 || visibleImageRectangle.Height < 0)
            throw new ArgumentOutOfRangeException(
                nameof(visibleImageRectangle),
                "Visible rectangle dimensions cannot be negative.");

        var left = Math.Clamp(visibleImageRectangle.Left, 0f, imageSize.X);
        var top = Math.Clamp(visibleImageRectangle.Top, 0f, imageSize.Y);
        var right = Math.Clamp(visibleImageRectangle.Right, 0f, imageSize.X);
        var bottom = Math.Clamp(visibleImageRectangle.Bottom, 0f, imageSize.Y);

        if (right < left || bottom < top)
        {
            return new VisibleTileRange(
                new TileIndex(0, 0),
                new TileIndex(-1, -1));
        }

        var tileCountX = Math.Max(1, (int)Math.Ceiling(imageSize.X / tileSize.X));
        var tileCountY = Math.Max(1, (int)Math.Ceiling(imageSize.Y / tileSize.Y));

        var minX = Math.Clamp((int)Math.Floor(left / tileSize.X), 0, tileCountX - 1);
        var minY = Math.Clamp((int)Math.Floor(top / tileSize.Y), 0, tileCountY - 1);

        var maxX = Math.Clamp(
            (int)Math.Ceiling(right / tileSize.X) - 1,
            minX,
            tileCountX - 1);

        var maxY = Math.Clamp(
            (int)Math.Ceiling(bottom / tileSize.Y) - 1,
            minY,
            tileCountY - 1);

        return new VisibleTileRange(
            new TileIndex(minX, minY),
            new TileIndex(maxX, maxY));
    }

    private static void ValidatePositiveFinite(Vector2 value, string parameterName)
    {
        if (!float.IsFinite(value.X) ||
            !float.IsFinite(value.Y) ||
            value.X <= 0 ||
            value.Y <= 0)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                value,
                "Size must contain finite values greater than zero.");
        }
    }
}
