using System.Drawing;
using System.Numerics;

namespace Asun.Vision.Contracts;

/// <summary>
/// Immutable simple-polygon geometry represented by an ordered, non-self-interpreting point ring.
/// The type provides geometric utilities only; it does not define inspection semantics.
/// </summary>
public sealed class Polygon2D
{
    private readonly Vector2[] _points;

    public Polygon2D(IEnumerable<Vector2> points)
    {
        ArgumentNullException.ThrowIfNull(points);

        _points = points.ToArray();

        if (_points.Length < 3)
            throw new ArgumentException("A polygon requires at least three points.", nameof(points));

        if (_points.Any(point => !float.IsFinite(point.X) || !float.IsFinite(point.Y)))
            throw new ArgumentException("Polygon points must be finite.", nameof(points));

        if (_points[0] != _points[^1])
            _points = _points.Append(_points[0]).ToArray();

        if (_points.Length < 4)
            throw new ArgumentException("A polygon ring requires at least three distinct positions.", nameof(points));
    }

    public int VertexCount => _points.Length - 1;

    public IReadOnlyList<Vector2> Vertices =>
        Array.AsReadOnly(_points);

    public bool IsClockwise => SignedArea < 0;

    public double SignedArea
    {
        get
        {
            double areaTwice = 0;

            for (var index = 0; index < _points.Length - 1; index++)
            {
                var current = _points[index];
                var next = _points[index + 1];

                areaTwice +=
                    (double)current.X * next.Y -
                    (double)next.X * current.Y;
            }

            return areaTwice / 2d;
        }
    }

    public double Area => Math.Abs(SignedArea);

    public Vector2 Centroid
    {
        get
        {
            var area = SignedArea;

            if (Math.Abs(area) <= double.Epsilon)
                throw new InvalidOperationException("A polygon with zero area has no unique centroid.");

            double sumX = 0;
            double sumY = 0;

            for (var index = 0; index < _points.Length - 1; index++)
            {
                var current = _points[index];
                var next = _points[index + 1];
                var cross =
                    (double)current.X * next.Y -
                    (double)next.X * current.Y;

                sumX += (current.X + next.X) * cross;
                sumY += (current.Y + next.Y) * cross;
            }

            var factor = 1d / (6d * area);

            return new Vector2(
                (float)(sumX * factor),
                (float)(sumY * factor));
        }
    }

    public double Perimeter
    {
        get
        {
            double perimeter = 0;

            for (var index = 1; index < _points.Length; index++)
                perimeter += Vector2.Distance(_points[index - 1], _points[index]);

            return perimeter;
        }
    }

    public RectangleF Bounds =>
        RectangleF.FromLTRB(
            _points.Min(point => point.X),
            _points.Min(point => point.Y),
            _points.Max(point => point.X),
            _points.Max(point => point.Y));

    public bool Contains(Vector2 point)
    {
        if (!float.IsFinite(point.X) || !float.IsFinite(point.Y))
            return false;

        var inside = false;

        for (var index = 0, previous = _points.Length - 1;
             index < _points.Length;
             previous = index++)
        {
            var currentPoint = _points[index];
            var previousPoint = _points[previous];

            var intersects =
                ((currentPoint.Y > point.Y) != (previousPoint.Y > point.Y)) &&
                point.X <
                (previousPoint.X - currentPoint.X) *
                (point.Y - currentPoint.Y) /
                (previousPoint.Y - currentPoint.Y) +
                currentPoint.X;

            if (intersects)
                inside = !inside;
        }

        return inside;
    }

    public Polygon2D Reverse() =>
        new(_points.Take(_points.Length - 1).Reverse());

    public Polygon2D Translate(Vector2 delta)
    {
        if (!float.IsFinite(delta.X) || !float.IsFinite(delta.Y))
            throw new ArgumentOutOfRangeException(nameof(delta));

        return new Polygon2D(
            _points.Take(_points.Length - 1)
                .Select(point => point + delta));
    }

    public Vector2 GetVertex(int index)
    {
        if (index < 0 || index >= VertexCount)
            throw new ArgumentOutOfRangeException(nameof(index));

        return _points[index];
    }
}
