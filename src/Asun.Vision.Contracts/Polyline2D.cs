using System.Drawing;
using System.Numerics;

namespace Asun.Vision.Contracts;

/// <summary>
/// Immutable polyline geometry. Input points are copied at construction.
/// </summary>
public sealed class Polyline2D
{
    private readonly Vector2[] _points;
    private readonly double[] _cumulativeLengths;

    public Polyline2D(IEnumerable<Vector2> points)
    {
        ArgumentNullException.ThrowIfNull(points);

        _points = points.ToArray();

        if (_points.Length < 2)
            throw new ArgumentException("A polyline requires at least two points.", nameof(points));

        if (_points.Any(point => !float.IsFinite(point.X) || !float.IsFinite(point.Y)))
            throw new ArgumentException("Polyline points must be finite.", nameof(points));

        _cumulativeLengths = new double[_points.Length];

        for (var index = 1; index < _points.Length; index++)
        {
            _cumulativeLengths[index] =
                _cumulativeLengths[index - 1] +
                Vector2.Distance(_points[index - 1], _points[index]);
        }
    }

    public int Count => _points.Length;

    public double Length => _cumulativeLengths[^1];

    public bool IsClosed =>
        _points[0] == _points[^1];

    public double ClosedLength =>
        IsClosed
            ? Length
            : Length + Vector2.Distance(_points[^1], _points[0]);

    public double SignedArea
    {
        get
        {
            if (!IsClosed || _points.Length < 3)
                throw new InvalidOperationException("A closed polyline with at least three points is required.");

            double areaTwice = 0;

            for (var index = 0; index < _points.Length - 1; index++)
            {
                var current = _points[index];
                var next = _points[index + 1];
                areaTwice += (double)current.X * next.Y - (double)next.X * current.Y;
            }

            return areaTwice / 2d;
        }
    }

    public IReadOnlyList<Vector2> Points =>
        Array.AsReadOnly(_points);

    public RectangleF Bounds
    {
        get
        {
            var minX = _points.Min(point => point.X);
            var minY = _points.Min(point => point.Y);
            var maxX = _points.Max(point => point.X);
            var maxY = _points.Max(point => point.Y);

            return RectangleF.FromLTRB(minX, minY, maxX, maxY);
        }
    }

    public Vector2 GetPoint(int index) =>
        _points[index];

    public Vector2 PointAtFraction(double fraction)
    {
        if (!double.IsFinite(fraction) || fraction < 0 || fraction > 1)
            throw new ArgumentOutOfRangeException(nameof(fraction));

        return PointAtDistance(Length * fraction);
    }

    public Vector2 PointAtDistance(double distance)
    {
        if (!double.IsFinite(distance) || distance < 0 || distance > Length)
            throw new ArgumentOutOfRangeException(nameof(distance));

        if (distance == 0)
            return _points[0];

        if (distance == Length)
            return _points[^1];

        var segmentIndex = Array.BinarySearch(_cumulativeLengths, distance);

        if (segmentIndex >= 0)
            return _points[segmentIndex];

        segmentIndex = ~segmentIndex;

        var startIndex = segmentIndex - 1;
        var startDistance = _cumulativeLengths[startIndex];
        var segmentLength = _cumulativeLengths[segmentIndex] - startDistance;

        if (segmentLength <= double.Epsilon)
            return _points[segmentIndex];

        var fraction = (distance - startDistance) / segmentLength;

        return Vector2.Lerp(
            _points[startIndex],
            _points[segmentIndex],
            (float)fraction);
    }

    public Polyline2D Reverse() =>
        new(_points.Reverse());

    public bool IsClosedWithin(double tolerance)
    {
        if (!double.IsFinite(tolerance) || tolerance < 0)
            throw new ArgumentOutOfRangeException(nameof(tolerance));

        return Vector2.DistanceSquared(_points[0], _points[^1]) <= tolerance * tolerance;
    }

    public Vector2 ClosestPoint(Vector2 point)
    {
        if (!float.IsFinite(point.X) || !float.IsFinite(point.Y))
            throw new ArgumentOutOfRangeException(nameof(point));

        var bestPoint = _points[0];
        var bestDistance = double.PositiveInfinity;

        for (var index = 1; index < _points.Length; index++)
        {
            var segment = new LineSegment2D(
                _points[index - 1],
                _points[index]);

            var candidate = segment.ClosestPoint(point);
            var distance = Vector2.DistanceSquared(candidate, point);

            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestPoint = candidate;
            }
        }

        return bestPoint;
    }

    public double DistanceSquaredTo(Vector2 point)
    {
        var closest = ClosestPoint(point);
        return Vector2.DistanceSquared(closest, point);
    }
}
