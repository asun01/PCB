using System.Drawing;
using System.Numerics;

namespace Asun.Vision.Contracts;

/// <summary>
/// Immutable ellipse geometry independent of any vision backend or production schema.
/// </summary>
public readonly record struct Ellipse2D(
    Vector2 Center,
    Vector2 Radii,
    double AngleRadians)
{
    public bool IsValid =>
        float.IsFinite(Center.X) &&
        float.IsFinite(Center.Y) &&
        float.IsFinite(Radii.X) &&
        float.IsFinite(Radii.Y) &&
        Radii.X >= 0 &&
        Radii.Y >= 0 &&
        double.IsFinite(AngleRadians);

    public double Area =>
        IsValid
            ? Math.PI * Radii.X * Radii.Y
            : throw new InvalidOperationException("The ellipse is invalid.");

    public Vector2 LocalXAxis =>
        new((float)Math.Cos(AngleRadians), (float)Math.Sin(AngleRadians));

    public Vector2 LocalYAxis =>
        new(-(float)Math.Sin(AngleRadians), (float)Math.Cos(AngleRadians));

    public RectangleF Bounds
    {
        get
        {
            EnsureValid();

            var cos = Math.Cos(AngleRadians);
            var sin = Math.Sin(AngleRadians);

            var extentX = Math.Sqrt(
                Radii.X * Radii.X * cos * cos +
                Radii.Y * Radii.Y * sin * sin);

            var extentY = Math.Sqrt(
                Radii.X * Radii.X * sin * sin +
                Radii.Y * Radii.Y * cos * cos);

            return new RectangleF(
                (float)(Center.X - extentX),
                (float)(Center.Y - extentY),
                (float)(extentX * 2d),
                (float)(extentY * 2d));
        }
    }

    public bool Contains(Vector2 point)
    {
        EnsureValid();

        if (!float.IsFinite(point.X) || !float.IsFinite(point.Y))
            return false;

        var local = ToLocalPoint(point);

        if (Radii.X == 0 || Radii.Y == 0)
            return local == Vector2.Zero;

        var x = local.X / Radii.X;
        var y = local.Y / Radii.Y;

        return x * x + y * y <= 1d;
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

    public Vector2 PointAtParameter(double radians)
    {
        EnsureValid();

        if (!double.IsFinite(radians))
            throw new ArgumentOutOfRangeException(nameof(radians));

        return ToWorldPoint(new Vector2(
            (float)(Math.Cos(radians) * Radii.X),
            (float)(Math.Sin(radians) * Radii.Y)));
    }

    public Ellipse2D Translate(Vector2 delta)
    {
        EnsureValid();

        if (!float.IsFinite(delta.X) || !float.IsFinite(delta.Y))
            throw new ArgumentOutOfRangeException(nameof(delta));

        return this with { Center = Center + delta };
    }

    public Ellipse2D Rotate(double deltaRadians)
    {
        EnsureValid();

        if (!double.IsFinite(deltaRadians))
            throw new ArgumentOutOfRangeException(nameof(deltaRadians));

        return this with { AngleRadians = AngleRadians + deltaRadians };
    }

    public Ellipse2D Resize(Vector2 radii)
    {
        EnsureValid();

        if (!float.IsFinite(radii.X) ||
            !float.IsFinite(radii.Y) ||
            radii.X < 0 ||
            radii.Y < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(radii));
        }

        return this with { Radii = radii };
    }

    private void EnsureValid()
    {
        if (!IsValid)
            throw new InvalidOperationException("The ellipse is invalid.");
    }
}
