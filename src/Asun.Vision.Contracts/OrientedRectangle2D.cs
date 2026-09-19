using System.Drawing;
using System.Numerics;

namespace Asun.Vision.Contracts;

/// <summary>
/// Immutable oriented rectangle geometry. It carries no inspection or production-schema semantics.
/// </summary>
public readonly record struct OrientedRectangle2D(
    Vector2 Center,
    Vector2 Size,
    double AngleRadians)
{
    public bool IsValid =>
        float.IsFinite(Center.X) &&
        float.IsFinite(Center.Y) &&
        float.IsFinite(Size.X) &&
        float.IsFinite(Size.Y) &&
        Size.X >= 0 &&
        Size.Y >= 0 &&
        double.IsFinite(AngleRadians);

    public double Area =>
        IsValid
            ? (double)Size.X * Size.Y
            : throw new InvalidOperationException("The oriented rectangle is invalid.");

    public double HalfDiagonalSquared =>
        IsValid
            ? ((double)Size.X * Size.X + (double)Size.Y * Size.Y) / 4d
            : throw new InvalidOperationException("The oriented rectangle is invalid.");

    public Vector2 LocalXAxis =>
        new((float)Math.Cos(AngleRadians), (float)Math.Sin(AngleRadians));

    public Vector2 LocalYAxis =>
        new(-(float)Math.Sin(AngleRadians), (float)Math.Cos(AngleRadians));

    public Vector2[] GetCorners()
    {
        EnsureValid();

        var half = Size * 0.5f;
        var x = LocalXAxis * half.X;
        var y = LocalYAxis * half.Y;

        return new[]
        {
            Center - x - y,
            Center + x - y,
            Center + x + y,
            Center - x + y
        };
    }

    public RectangleF GetAxisAlignedBounds()
    {
        var corners = GetCorners();

        var left = corners.Min(point => point.X);
        var top = corners.Min(point => point.Y);
        var right = corners.Max(point => point.X);
        var bottom = corners.Max(point => point.Y);

        return new RectangleF(left, top, right - left, bottom - top);
    }

    public bool Contains(Vector2 point)
    {
        EnsureValid();

        if (!float.IsFinite(point.X) || !float.IsFinite(point.Y))
            return false;

        var delta = point - Center;
        var localX = Vector2.Dot(delta, LocalXAxis);
        var localY = Vector2.Dot(delta, LocalYAxis);

        return Math.Abs(localX) <= Size.X / 2f &&
               Math.Abs(localY) <= Size.Y / 2f;
    }

    public double DistanceSquaredToCenter(Vector2 point)
    {
        EnsureValid();

        if (!float.IsFinite(point.X) || !float.IsFinite(point.Y))
            throw new ArgumentOutOfRangeException(nameof(point));

        return Vector2.DistanceSquared(point, Center);
    }

    public OrientedRectangle2D Translate(Vector2 delta)
    {
        if (!float.IsFinite(delta.X) || !float.IsFinite(delta.Y))
            throw new ArgumentOutOfRangeException(nameof(delta));

        EnsureValid();

        return this with { Center = Center + delta };
    }

    public OrientedRectangle2D Rotate(double deltaRadians)
    {
        if (!double.IsFinite(deltaRadians))
            throw new ArgumentOutOfRangeException(nameof(deltaRadians));

        EnsureValid();

        return this with { AngleRadians = AngleRadians + deltaRadians };
    }

    public AffineTransform2D ToAffineTransform()
    {
        EnsureValid();

        var translation = AffineTransform2D.Translation(Center.X, Center.Y);
        var rotation = AffineTransform2D.Rotation(AngleRadians);
        var scale = AffineTransform2D.Scale(Size.X, Size.Y);

        return scale
            .Combine(rotation)
            .Combine(translation);
    }

    private void EnsureValid()
    {
        if (!IsValid)
            throw new InvalidOperationException("The oriented rectangle is invalid.");
    }
}
