using System.Drawing;
using System.Numerics;

namespace Asun.UI.Viewports;

public readonly record struct TileIndex(int X, int Y);

public readonly record struct VisibleTileRange(
    TileIndex Minimum,
    TileIndex Maximum)
{
    public bool IsEmpty =>
        Maximum.X < Minimum.X || Maximum.Y < Minimum.Y;

    public int Width =>
        IsEmpty ? 0 : Maximum.X - Minimum.X + 1;

    public int Height =>
        IsEmpty ? 0 : Maximum.Y - Minimum.Y + 1;

    public int Count =>
        checked(Width * Height);

    public IEnumerable<TileIndex> Enumerate()
    {
        if (IsEmpty)
            yield break;

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

        var rawLeft = visibleImageRectangle.Left;
        var rawTop = visibleImageRectangle.Top;
        var rawRight = visibleImageRectangle.Right;
        var rawBottom = visibleImageRectangle.Bottom;

        if (rawRight <= 0 ||
            rawBottom <= 0 ||
            rawLeft >= imageSize.X ||
            rawTop >= imageSize.Y)
        {
            return EmptyRange();
        }

        var left = Math.Clamp(rawLeft, 0f, imageSize.X);
        var top = Math.Clamp(rawTop, 0f, imageSize.Y);
        var right = Math.Clamp(rawRight, 0f, imageSize.X);
        var bottom = Math.Clamp(rawBottom, 0f, imageSize.Y);

        if (right <= left || bottom <= top)
            return EmptyRange();

        var gridSize = CalculateGridSize(imageSize, tileSize);

        var minX = Math.Clamp((int)Math.Floor(left / tileSize.X), 0, gridSize.X - 1);
        var minY = Math.Clamp((int)Math.Floor(top / tileSize.Y), 0, gridSize.Y - 1);

        var maxX = Math.Clamp(
            (int)Math.Ceiling(right / tileSize.X) - 1,
            minX,
            gridSize.X - 1);

        var maxY = Math.Clamp(
            (int)Math.Ceiling(bottom / tileSize.Y) - 1,
            minY,
            gridSize.Y - 1);

        return new VisibleTileRange(
            new TileIndex(minX, minY),
            new TileIndex(maxX, maxY));
    }

    /// <summary>
    /// Expands a visible tile range by a fixed number of neighboring tiles,
    /// clamped to the finite image tile grid. An empty range stays empty.
    /// </summary>
    public static VisibleTileRange ExpandTileRange(
        Vector2 imageSize,
        Vector2 tileSize,
        VisibleTileRange visibleRange,
        int marginTiles) =>
        ExpandTileRange(imageSize, tileSize, visibleRange, marginTiles, marginTiles);

    public static VisibleTileRange ExpandTileRange(
        Vector2 imageSize,
        Vector2 tileSize,
        VisibleTileRange visibleRange,
        int marginX,
        int marginY)
    {
        ValidatePositiveFinite(imageSize, nameof(imageSize));
        ValidatePositiveFinite(tileSize, nameof(tileSize));

        if (marginX < 0)
            throw new ArgumentOutOfRangeException(nameof(marginX));

        if (marginY < 0)
            throw new ArgumentOutOfRangeException(nameof(marginY));

        if (visibleRange.IsEmpty)
            return EmptyRange();

        var gridSize = CalculateGridSize(imageSize, tileSize);

        if (visibleRange.Minimum.X < 0 ||
            visibleRange.Minimum.Y < 0 ||
            visibleRange.Maximum.X >= gridSize.X ||
            visibleRange.Maximum.Y >= gridSize.Y)
        {
            throw new ArgumentOutOfRangeException(
                nameof(visibleRange),
                visibleRange,
                "Tile range must stay inside the image tile grid.");
        }

        return new VisibleTileRange(
            new TileIndex(
                Math.Max(0, visibleRange.Minimum.X - marginX),
                Math.Max(0, visibleRange.Minimum.Y - marginY)),
            new TileIndex(
                Math.Min(gridSize.X - 1, visibleRange.Maximum.X + marginX),
                Math.Min(gridSize.Y - 1, visibleRange.Maximum.Y + marginY)));
    }

    public static bool ContainsTile(
        Vector2 imageSize,
        Vector2 tileSize,
        TileIndex tileIndex)
    {
        ValidatePositiveFinite(imageSize, nameof(imageSize));
        ValidatePositiveFinite(tileSize, nameof(tileSize));

        var gridSize = CalculateGridSize(imageSize, tileSize);

        return tileIndex.X >= 0 &&
               tileIndex.Y >= 0 &&
               tileIndex.X < gridSize.X &&
               tileIndex.Y < gridSize.Y;
    }

    public static bool TryGetTileIndexAtImagePoint(
        Vector2 imageSize,
        Vector2 tileSize,
        Vector2 imagePoint,
        out TileIndex tileIndex)
    {
        ValidatePositiveFinite(imageSize, nameof(imageSize));
        ValidatePositiveFinite(tileSize, nameof(tileSize));

        if (!float.IsFinite(imagePoint.X) ||
            !float.IsFinite(imagePoint.Y) ||
            imagePoint.X < 0 ||
            imagePoint.Y < 0 ||
            imagePoint.X >= imageSize.X ||
            imagePoint.Y >= imageSize.Y)
        {
            tileIndex = default;
            return false;
        }

        var gridSize = CalculateGridSize(imageSize, tileSize);
        tileIndex = new TileIndex(
            Math.Clamp((int)Math.Floor(imagePoint.X / tileSize.X), 0, gridSize.X - 1),
            Math.Clamp((int)Math.Floor(imagePoint.Y / tileSize.Y), 0, gridSize.Y - 1));

        return true;
    }

    public static TileIndex GetTileIndexAtImagePoint(
        Vector2 imageSize,
        Vector2 tileSize,
        Vector2 imagePoint)
    {
        if (!TryGetTileIndexAtImagePoint(imageSize, tileSize, imagePoint, out var tileIndex))
            throw new ArgumentOutOfRangeException(nameof(imagePoint), imagePoint, "Point is outside the image bounds.");

        return tileIndex;
    }

    public static RectangleF GetTileRangeRectangle(
        Vector2 imageSize,
        Vector2 tileSize,
        VisibleTileRange range)
    {
        ValidatePositiveFinite(imageSize, nameof(imageSize));
        ValidatePositiveFinite(tileSize, nameof(tileSize));

        if (range.IsEmpty)
            return new RectangleF();

        if (!ContainsTile(imageSize, tileSize, range.Minimum) ||
            !ContainsTile(imageSize, tileSize, range.Maximum))
        {
            throw new ArgumentOutOfRangeException(nameof(range), range, "Tile range is outside the image grid.");
        }

        var first = GetTileRectangle(imageSize, tileSize, range.Minimum);
        var last = GetTileRectangle(imageSize, tileSize, range.Maximum);

        return new RectangleF(
            first.X,
            first.Y,
            last.Right - first.X,
            last.Bottom - first.Y);
    }

    public static RectangleF GetTileRectangle(
        Vector2 imageSize,
        Vector2 tileSize,
        TileIndex tileIndex)
    {
        ValidatePositiveFinite(imageSize, nameof(imageSize));
        ValidatePositiveFinite(tileSize, nameof(tileSize));

        if (tileIndex.X < 0 || tileIndex.Y < 0)
            throw new ArgumentOutOfRangeException(nameof(tileIndex));

        var gridSize = CalculateGridSize(imageSize, tileSize);

        if (tileIndex.X >= gridSize.X || tileIndex.Y >= gridSize.Y)
            throw new ArgumentOutOfRangeException(
                nameof(tileIndex),
                tileIndex,
                "Tile index is outside the image tile grid.");

        var left = tileIndex.X * tileSize.X;
        var top = tileIndex.Y * tileSize.Y;
        var right = Math.Min(left + tileSize.X, imageSize.X);
        var bottom = Math.Min(top + tileSize.Y, imageSize.Y);

        return new RectangleF(
            left,
            top,
            Math.Max(0, right - left),
            Math.Max(0, bottom - top));
    }

    private static (int X, int Y) CalculateGridSize(
        Vector2 imageSize,
        Vector2 tileSize) =>
        (
            Math.Max(1, (int)Math.Ceiling(imageSize.X / tileSize.X)),
            Math.Max(1, (int)Math.Ceiling(imageSize.Y / tileSize.Y)));

    private static VisibleTileRange EmptyRange() =>
        new(new TileIndex(0, 0), new TileIndex(-1, -1));

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
