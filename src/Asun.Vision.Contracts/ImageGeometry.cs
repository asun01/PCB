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

    public long PixelCount => (long)Width * Height;

    public bool IsSquare => Width == Height;

    public double AspectRatio => (double)Width / Height;

    public Vector2 Vector => new(Width, Height);

    public double DiagonalLength =>
        Math.Sqrt((double)Width * Width + (double)Height * Height);

    public PixelRect Bounds =>
        new(0, 0, Width, Height);

    public Vector2 Center => new(Width / 2f, Height / 2f);

    public bool Contains(PixelPoint point) =>
        point.IsFinite &&
        point.X >= 0 &&
        point.Y >= 0 &&
        point.X <= Width &&
        point.Y <= Height;

    public PixelPoint Clamp(PixelPoint point)
    {
        if (!point.IsFinite)
            throw new ArgumentOutOfRangeException(nameof(point));

        return new PixelPoint(
            Math.Clamp(point.X, 0, Width),
            Math.Clamp(point.Y, 0, Height));
    }

    public bool TryClamp(PixelPoint point, out PixelPoint clamped)
    {
        if (!point.IsFinite)
        {
            clamped = default;
            return false;
        }

        clamped = Clamp(point);
        return true;
    }
}

public readonly record struct PixelPoint(double X, double Y)
{
    public bool IsFinite => double.IsFinite(X) && double.IsFinite(Y);

    public double LengthSquared => X * X + Y * Y;

    public double Length => Math.Sqrt(LengthSquared);

    public double DistanceSquaredTo(PixelPoint other)
    {
        if (!IsFinite || !other.IsFinite)
            throw new ArgumentOutOfRangeException(nameof(other));

        var dx = X - other.X;
        var dy = Y - other.Y;
        return dx * dx + dy * dy;
    }

    public double DistanceTo(PixelPoint other) =>
        Math.Sqrt(DistanceSquaredTo(other));

    public PixelPoint Translate(double deltaX, double deltaY)
    {
        if (!double.IsFinite(deltaX) || !double.IsFinite(deltaY))
            throw new ArgumentOutOfRangeException(nameof(deltaX));

        var x = X + deltaX;
        var y = Y + deltaY;

        if (!double.IsFinite(x) || !double.IsFinite(y))
            throw new ArgumentOutOfRangeException(nameof(deltaX));

        return new PixelPoint(x, y);
    }

    public PixelPoint Midpoint(PixelPoint other)
    {
        if (!IsFinite || !other.IsFinite)
            throw new ArgumentOutOfRangeException(nameof(other));

        return new PixelPoint(
            (X + other.X) / 2d,
            (Y + other.Y) / 2d);
    }

    public PixelPoint Lerp(PixelPoint other, double amount)
    {
        if (!IsFinite || !other.IsFinite)
            throw new ArgumentOutOfRangeException(nameof(other));

        if (!double.IsFinite(amount))
            throw new ArgumentOutOfRangeException(nameof(amount));

        var x = X + (other.X - X) * amount;
        var y = Y + (other.Y - Y) * amount;

        if (!double.IsFinite(x) || !double.IsFinite(y))
            throw new ArgumentOutOfRangeException(nameof(amount));

        return new PixelPoint(x, y);
    }
}

public readonly record struct PixelRect(double X, double Y, double Width, double Height)
{
    public static PixelRect FromCenter(
        PixelPoint center,
        double width,
        double height)
    {
        if (!center.IsFinite)
            throw new ArgumentOutOfRangeException(nameof(center));

        if (!double.IsFinite(width) || width < 0)
            throw new ArgumentOutOfRangeException(nameof(width));

        if (!double.IsFinite(height) || height < 0)
            throw new ArgumentOutOfRangeException(nameof(height));

        return new PixelRect(
            center.X - width / 2d,
            center.Y - height / 2d,
            width,
            height);
    }

    public static PixelRect FromLTRB(
        double left,
        double top,
        double right,
        double bottom)
    {
        if (!double.IsFinite(left) ||
            !double.IsFinite(top) ||
            !double.IsFinite(right) ||
            !double.IsFinite(bottom))
        {
            throw new ArgumentOutOfRangeException(nameof(left));
        }

        return new PixelRect(
            Math.Min(left, right),
            Math.Min(top, bottom),
            Math.Abs(right - left),
            Math.Abs(bottom - top));
    }

    public static PixelRect FromPoints(PixelPoint first, PixelPoint second)
    {
        if (!first.IsFinite)
            throw new ArgumentOutOfRangeException(nameof(first));

        if (!second.IsFinite)
            throw new ArgumentOutOfRangeException(nameof(second));

        var left = Math.Min(first.X, second.X);
        var top = Math.Min(first.Y, second.Y);
        var right = Math.Max(first.X, second.X);
        var bottom = Math.Max(first.Y, second.Y);

        var width = right - left;
        var height = bottom - top;

        if (!double.IsFinite(width) || !double.IsFinite(height))
            throw new ArgumentOutOfRangeException(
                nameof(first),
                "The point pair produces non-finite rectangle dimensions.");

        return new PixelRect(left, top, width, height);
    }

    public bool IsFinite =>
        double.IsFinite(X) && double.IsFinite(Y) &&
        double.IsFinite(Width) && double.IsFinite(Height);

    public bool IsValid => IsFinite && Width >= 0 && Height >= 0;

    public bool IsEmpty => IsValid && (Width == 0 || Height == 0);

    public bool IsDegenerate => IsEmpty;

    public double Perimeter =>
        IsValid ? 2d * (Width + Height) : throw new InvalidOperationException("The rectangle is invalid.");

    public double Area =>
        IsValid ? Width * Height : throw new InvalidOperationException("The rectangle is invalid.");

    public bool AreBoundsFinite =>
        IsFinite &&
        double.IsFinite(Right) &&
        double.IsFinite(Bottom);

    public PixelRect EnsureValid()
    {
        if (!IsValid || !AreBoundsFinite)
            throw new InvalidOperationException("The rectangle is not finite and valid.");

        return this;
    }

    public PixelPoint TopLeft => new(Left, Top);
    public PixelPoint TopRight => new(Right, Top);
    public PixelPoint BottomLeft => new(Left, Bottom);
    public PixelPoint BottomRight => new(Right, Bottom);

    public double Left => X;
    public double Top => Y;
    public double Right => X + Width;
    public double Bottom => Y + Height;

    public PixelPoint Center => new(X + Width / 2d, Y + Height / 2d);

    public PixelPoint SizePoint => new(Width, Height);

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

    public bool Intersects(PixelRect rectangle) =>
        IsValid &&
        rectangle.IsValid &&
        Left <= rectangle.Right &&
        Right >= rectangle.Left &&
        Top <= rectangle.Bottom &&
        Bottom >= rectangle.Top;

    public PixelRect Normalized =>
        Normalize();

    public PixelRect Normalize()
    {
        if (!IsFinite)
            throw new InvalidOperationException("The rectangle contains non-finite values.");

        var left = Math.Min(Left, Right);
        var top = Math.Min(Top, Bottom);
        var right = Math.Max(Left, Right);
        var bottom = Math.Max(Top, Bottom);

        return new PixelRect(left, top, right - left, bottom - top);
    }

    public PixelRect Translate(PixelPoint delta)
    {
        if (!delta.IsFinite)
            throw new ArgumentOutOfRangeException(nameof(delta));

        return Translate(delta.X, delta.Y);
    }

    public PixelRect Translate(double deltaX, double deltaY)
    {
        ValidateFinite(deltaX, nameof(deltaX));
        ValidateFinite(deltaY, nameof(deltaY));

        var translatedX = X + deltaX;
        var translatedY = Y + deltaY;

        if (!double.IsFinite(translatedX) || !double.IsFinite(translatedY))
            throw new ArgumentOutOfRangeException(nameof(deltaX), "Translation produces non-finite coordinates.");

        return new PixelRect(translatedX, translatedY, Width, Height);
    }

    public PixelRect ScaleUniform(double factor)
    {
        ValidateFinite(factor, nameof(factor));
        return ScaleAroundCenter(factor, factor);
    }

    public PixelRect ScaleAroundCenter(double factorX, double factorY)
    {
        ValidateFinite(factorX, nameof(factorX));
        ValidateFinite(factorY, nameof(factorY));

        if (!IsValid)
            throw new InvalidOperationException("The rectangle is invalid.");

        if (factorX < 0 || factorY < 0)
            throw new ArgumentOutOfRangeException(nameof(factorX), "Scale factors cannot be negative.");

        var center = Center;
        var width = Width * factorX;
        var height = Height * factorY;

        if (!double.IsFinite(width) || !double.IsFinite(height))
            throw new ArgumentOutOfRangeException(nameof(factorX), "Scaling produces non-finite dimensions.");

        return new PixelRect(
            center.X - width / 2d,
            center.Y - height / 2d,
            width,
            height);
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

        var newX = X - horizontal;
        var newY = Y - vertical;

        if (!double.IsFinite(newX) || !double.IsFinite(newY))
        {
            throw new ArgumentOutOfRangeException(
                nameof(horizontal),
                "Inflation must not produce non-finite coordinates.");
        }

        return new PixelRect(
            newX,
            newY,
            newWidth,
            newHeight);
    }

    public PixelRect ExpandToInclude(PixelRect other)
    {
        if (!IsValid)
            throw new InvalidOperationException("The current rectangle is invalid.");

        if (!other.IsValid)
            throw new ArgumentException("The other rectangle is invalid.", nameof(other));

        return Union(other);
    }

    public PixelRect ExpandToInclude(PixelPoint point)
    {
        if (!IsValid)
            throw new InvalidOperationException("The rectangle is invalid.");

        if (!point.IsFinite)
            throw new ArgumentOutOfRangeException(nameof(point));

        var left = Math.Min(Left, point.X);
        var top = Math.Min(Top, point.Y);
        var right = Math.Max(Right, point.X);
        var bottom = Math.Max(Bottom, point.Y);

        return new PixelRect(left, top, right - left, bottom - top);
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

    public double DistanceSquaredTo(PixelPoint point)
    {
        if (!IsValid)
            throw new InvalidOperationException("The rectangle is invalid.");

        if (!point.IsFinite)
            throw new ArgumentOutOfRangeException(nameof(point));

        var dx = point.X < Left
            ? Left - point.X
            : point.X > Right
                ? point.X - Right
                : 0d;

        var dy = point.Y < Top
            ? Top - point.Y
            : point.Y > Bottom
                ? point.Y - Bottom
                : 0d;

        return dx * dx + dy * dy;
    }

    public PixelRect Union(PixelRect other)
    {
        if (!IsValid)
            throw new InvalidOperationException("The current rectangle is invalid.");

        if (!other.IsValid)
            throw new ArgumentException("The other rectangle is invalid.", nameof(other));

        var left = Math.Min(Left, other.Left);
        var top = Math.Min(Top, other.Top);
        var right = Math.Max(Right, other.Right);
        var bottom = Math.Max(Bottom, other.Bottom);

        return new PixelRect(left, top, right - left, bottom - top);
    }

    public bool TryIntersect(PixelRect other, out PixelRect intersection)
    {
        if (!IsValid)
            throw new InvalidOperationException("The current rectangle is invalid.");

        if (!other.IsValid)
            throw new ArgumentException("The other rectangle is invalid.", nameof(other));

        var left = Math.Max(Left, other.Left);
        var top = Math.Max(Top, other.Top);
        var right = Math.Min(Right, other.Right);
        var bottom = Math.Min(Bottom, other.Bottom);

        if (right < left || bottom < top)
        {
            intersection = default;
            return false;
        }

        intersection = new PixelRect(left, top, right - left, bottom - top);
        return true;
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
