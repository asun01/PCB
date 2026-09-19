using System.Drawing;
using System.Numerics;

namespace Asun.Vision.Contracts;

/// <summary>
/// Immutable 2D line segment geometry independent of any vision backend.
/// </summary>
public readonly record struct LineSegment2D(
    Vector2 Start,
    Vector2 End)
{
    public bool IsFinite =>
        float.IsFinite(Start.X) &&
        float.IsFinite(Start.Y) &&
        float.IsFinite(End.X) &&
        float.IsFinite(End.Y);

    public double LengthSquared =>
        IsFinite
            ? Vector2.DistanceSquared(Start, End)
            : throw new InvalidOperationException("The line segment is non-finite.");

    public double Length => Math.Sqrt(LengthSquared);

    public Vector2 Delta => End - Start;

    public Vector2 Direction =>
        LengthSquared <= double.Epsilon
            ? Vector2.Zero
            : Vector2.Normalize(Delta);

    public Vector2 Midpoint =>
        (Start + End) * 0.5f;

    public Circle2D GetBoundingCircle()
    {
        EnsureFinite();

        return new Circle2D(
            Midpoint,
            Length / 2d);
    }

    public RectangleF Bounds
    {
        get
        {
            EnsureFinite();

            return RectangleF.FromLTRB(
                Math.Min(Start.X, End.X),
                Math.Min(Start.Y, End.Y),
                Math.Max(Start.X, End.X),
                Math.Max(Start.Y, End.Y));
        }
    }

    public Vector2 PointAt(double fraction)
    {
        EnsureFinite();

        if (!double.IsFinite(fraction))
            throw new ArgumentOutOfRangeException(nameof(fraction));

        return Start + Delta * (float)fraction;
    }

    public Vector2 ClosestPoint(Vector2 point)
    {
        EnsureFinite();

        if (!float.IsFinite(point.X) || !float.IsFinite(point.Y))
            throw new ArgumentOutOfRangeException(nameof(point));

        var lengthSquared = Delta.LengthSquared();

        if (lengthSquared <= float.Epsilon)
            return Start;

        var projection = Vector2.Dot(point - Start, Delta) / lengthSquared;
        var clamped = Math.Clamp(projection, 0f, 1f);

        return Start + Delta * clamped;
    }

    public double DistanceSquaredTo(Vector2 point)
    {
        var closest = ClosestPoint(point);
        return Vector2.DistanceSquared(closest, point);
    }

    public LineSegment2D Reverse() =>
        this with
        {
            Start = End,
            End = Start
        };

    public LineSegment2D Translate(Vector2 delta)
    {
        EnsureFinite();

        if (!float.IsFinite(delta.X) || !float.IsFinite(delta.Y))
            throw new ArgumentOutOfRangeException(nameof(delta));

        return new LineSegment2D(
            Start + delta,
            End + delta);
    }

    private void EnsureFinite()
    {
        if (!IsFinite)
            throw new InvalidOperationException("The line segment contains non-finite coordinates.");
    }
}
