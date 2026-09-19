using System.Numerics;

namespace Asun.Vision.Contracts;

public readonly record struct ImageSize
{
    public ImageSize(int width, int height)
    {
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

        Width = width;
        Height = height;
    }

    public int Width { get; }
    public int Height { get; }
    public Vector2 Center => new(Width / 2f, Height / 2f);
}

public readonly record struct PixelPoint(double X, double Y)
{
    public bool IsFinite => double.IsFinite(X) && double.IsFinite(Y);
}

public readonly record struct PixelRect(double X, double Y, double Width, double Height)
{
    public bool IsFinite =>
        double.IsFinite(X) && double.IsFinite(Y) &&
        double.IsFinite(Width) && double.IsFinite(Height);

    public bool IsValid => IsFinite && Width >= 0 && Height >= 0;

    public bool IsEmpty => IsValid && (Width == 0 || Height == 0);

    public double Left => X;
    public double Top => Y;
    public double Right => X + Width;
    public double Bottom => Y + Height;

    public PixelPoint Center => new(X + Width / 2d, Y + Height / 2d);

    public bool Contains(PixelPoint point) =>
        IsValid &&
        point.IsFinite &&
        point.X >= Left &&
        point.X <= Right &&
        point.Y >= Top &&
        point.Y <= Bottom;

    public bool Contains(PixelRect rectangle) =>
        IsValid &&
        rectangle.IsValid &&
        rectangle.Left >= Left &&
        rectangle.Top >= Top &&
        rectangle.Right <= Right &&
        rectangle.Bottom <= Bottom;

    public PixelRect Translate(double deltaX, double deltaY)
    {
        ValidateFinite(deltaX, nameof(deltaX));
        ValidateFinite(deltaY, nameof(deltaY));
        return new PixelRect(X + deltaX, Y + deltaY, Width, Height);
    }

    public PixelRect Inflate(double horizontal, double vertical)
    {
        ValidateFinite(horizontal, nameof(horizontal));
        ValidateFinite(vertical, nameof(vertical));

        var newWidth = Width + horizontal * 2d;
        var newHeight = Height + vertical * 2d;

        if (!double.IsFinite(newWidth) || !double.IsFinite(newHeight) ||
            newWidth < 0 || newHeight < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(horizontal),
                "Inflation must not produce a negative or non-finite rectangle size.");
        }

        return new PixelRect(
            X - horizontal,
            Y - vertical,
            newWidth,
            newHeight);
    }

    public PixelRect ClampTo(PixelRect bounds)
    {
        if (!IsValid)
            throw new InvalidOperationException("The current rectangle is invalid.");

        if (!bounds.IsValid)
            throw new ArgumentException("The bounds rectangle is invalid.", nameof(bounds));

        var left = Math.Clamp(Left, bounds.Left, bounds.Right);
        var top = Math.Clamp(Top, bounds.Top, bounds.Bottom);
        var right = Math.Clamp(Right, bounds.Left, bounds.Right);
        var bottom = Math.Clamp(Bottom, bounds.Top, bounds.Bottom);

        return new PixelRect(
            left,
            top,
            Math.Max(0, right - left),
            Math.Max(0, bottom - top));
    }

    public PixelRect Intersect(PixelRect other)
    {
        if (!IsValid)
            throw new InvalidOperationException("The current rectangle is invalid.");

        if (!other.IsValid)
            throw new ArgumentException("The other rectangle is invalid.", nameof(other));

        var left = Math.Max(Left, other.Left);
        var top = Math.Max(Top, other.Top);
        var right = Math.Min(Right, other.Right);
        var bottom = Math.Min(Bottom, other.Bottom);

        return right < left || bottom < top
            ? new PixelRect(left, top, 0, 0)
            : new PixelRect(left, top, right - left, bottom - top);
    }

    private static void ValidateFinite(double value, string parameterName)
    {
        if (!double.IsFinite(value))
            throw new ArgumentOutOfRangeException(parameterName, value, "Value must be finite.");
    }
}
