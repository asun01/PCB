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

    public static OrientedRectangle2D FromAxisAligned(
        PixelRect rectangle) =>
        new(
            new Vector2(
                (float)rectangle.Center.X,
                (float)rectangle.Center.Y),
            new Vector2(
                (float)rectangle.Width,
                (float)rectangle.Height),
            0);

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

    public Vector2 ToLocalPoint(Vector2 point)
    {
        EnsureValid();

        if (!float.IsFinite(point.X) || !float.IsFinite(point.Y))
            throw new ArgumentOutOfRangeException(nameof(point));

        var delta = point - Center;

        return new Vector2(
            Vector2.Dot(delta, LocalXAxis),
            Vector2.Dot(delta, LocalYAxis));
    }

    public Vector2 ToWorldPoint(Vector2 localPoint)
    {
        EnsureValid();

        if (!float.IsFinite(localPoint.X) || !float.IsFinite(localPoint.Y))
            throw new ArgumentOutOfRangeException(nameof(localPoint));

        return Center +
               LocalXAxis * localPoint.X +
               LocalYAxis * localPoint.Y;
    }

    public bool Contains(Vector2 point)
    {
        EnsureValid();

        if (!float.IsFinite(point.X) || !float.IsFinite(point.Y))
            return false;

        var local = ToLocalPoint(point);

        return Math.Abs(local.X) <= Size.X / 2f &&
               Math.Abs(local.Y) <= Size.Y / 2f;
    }

    public Vector2 ClosestPoint(Vector2 point)
    {
        EnsureValid();

        if (!float.IsFinite(point.X) || !float.IsFinite(point.Y))
            throw new ArgumentOutOfRangeException(nameof(point));

        var local = ToLocalPoint(point);

        var clampedLocal = new Vector2(
            Math.Clamp(local.X, -Size.X / 2f, Size.X / 2f),
            Math.Clamp(local.Y, -Size.Y / 2f, Size.Y / 2f));

        return ToWorldPoint(clampedLocal);
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

    public OrientedRectangle2D NormalizeAngle()
    {
        EnsureValid();

        var normalized = AngleRadians % (Math.PI * 2);
        if (normalized <= -Math.PI)
            normalized += Math.PI * 2;
        else if (normalized > Math.PI)
            normalized -= Math.PI * 2;

        return this with { AngleRadians = normalized };
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
