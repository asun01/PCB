using System.Drawing;
using System.Numerics;

namespace Asun.Vision.Contracts;

/// <summary>
/// Immutable circle geometry independent of any vision backend or production schema.
/// </summary>
public readonly record struct Circle2D(
    Vector2 Center,
    double Radius)
{
    public bool IsValid =>
        float.IsFinite(Center.X) &&
        float.IsFinite(Center.Y) &&
        double.IsFinite(Radius) &&
        Radius >= 0;

    public double Diameter =>
        IsValid ? Radius * 2d : throw new InvalidOperationException("The circle is invalid.");

    public double Area =>
        IsValid ? Math.PI * Radius * Radius : throw new InvalidOperationException("The circle is invalid.");

    public double Circumference =>
        IsValid ? Math.PI * 2d * Radius : throw new InvalidOperationException("The circle is invalid.");

    public RectangleF Bounds =>
        IsValid
            ? new RectangleF(
                (float)(Center.X - Radius),
                (float)(Center.Y - Radius),
                (float)(Radius * 2d),
                (float)(Radius * 2d))
            : throw new InvalidOperationException("The circle is invalid.");

    public bool Contains(Vector2 point)
    {
        EnsureValid();

        if (!float.IsFinite(point.X) || !float.IsFinite(point.Y))
            return false;

        return Vector2.DistanceSquared(point, Center) <= Radius * Radius;
    }

    public bool ContainsStrict(Vector2 point)
    {
        EnsureValid();

        if (!float.IsFinite(point.X) || !float.IsFinite(point.Y))
            return false;

        return Vector2.DistanceSquared(point, Center) < Radius * Radius;
    }

    public Vector2 PointAtAngle(double radians)
    {
        EnsureValid();

        if (!double.IsFinite(radians))
            throw new ArgumentOutOfRangeException(nameof(radians));

        return Center + new Vector2(
            (float)(Math.Cos(radians) * Radius),
            (float)(Math.Sin(radians) * Radius));
    }

    public Vector2 ClosestPoint(Vector2 point)
    {
        EnsureValid();

        if (!float.IsFinite(point.X) || !float.IsFinite(point.Y))
            throw new ArgumentOutOfRangeException(nameof(point));

        var delta = point - Center;
        var lengthSquared = delta.LengthSquared();

        if (lengthSquared <= float.Epsilon)
            return Center + new Vector2((float)Radius, 0);

        var scale = (float)(Radius / Math.Sqrt(lengthSquared));
        return Center + delta * scale;
    }

    public double DistanceSquaredToCenter(Vector2 point)
    {
        EnsureValid();

        if (!float.IsFinite(point.X) || !float.IsFinite(point.Y))
            throw new ArgumentOutOfRangeException(nameof(point));

        return Vector2.DistanceSquared(point, Center);
    }

    public Circle2D Translate(Vector2 delta)
    {
        EnsureValid();

        if (!float.IsFinite(delta.X) || !float.IsFinite(delta.Y))
            throw new ArgumentOutOfRangeException(nameof(delta));

        return this with { Center = Center + delta };
    }

    public Circle2D Resize(double radius)
    {
        if (!double.IsFinite(radius) || radius < 0)
            throw new ArgumentOutOfRangeException(nameof(radius));

        EnsureValid();
        return this with { Radius = radius };
    }

    private void EnsureValid()
    {
        if (!IsValid)
            throw new InvalidOperationException("The circle is invalid.");
    }
}
