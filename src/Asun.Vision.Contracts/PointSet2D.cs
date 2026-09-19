using System.Numerics;

namespace Asun.Vision.Contracts;

/// <summary>
/// Immutable 2D point collection with deterministic geometry summaries.
/// </summary>
public sealed class PointSet2D
{
    private readonly Vector2[] _points;

    public PointSet2D(IEnumerable<Vector2> points)
    {
        ArgumentNullException.ThrowIfNull(points);

        _points = points.ToArray();

        if (_points.Any(point =>
                !float.IsFinite(point.X) ||
                !float.IsFinite(point.Y)))
        {
            throw new ArgumentException("Point-set coordinates must be finite.", nameof(points));
        }
    }

    public int Count => _points.Length;

    public IReadOnlyList<Vector2> Points =>
        Array.AsReadOnly(_points);

    public PixelRect Bounds
    {
        get
        {
            if (_points.Length == 0)
                return new PixelRect(0, 0, 0, 0);

            return new PixelRect(
                _points.Min(point => point.X),
                _points.Min(point => point.Y),
                _points.Max(point => point.X) - _points.Min(point => point.X),
                _points.Max(point => point.Y) - _points.Min(point => point.Y));
        }
    }

    public bool TryGetCentroid(out Vector2 centroid)
    {
        if (_points.Length == 0)
        {
            centroid = default;
            return false;
        }

        centroid = Centroid;
        return true;
    }

    public Vector2 Centroid
    {
        get
        {
            if (_points.Length == 0)
                throw new InvalidOperationException("An empty point set has no centroid.");

            var sum = Vector2.Zero;

            foreach (var point in _points)
                sum += point;

            return sum / _points.Length;
        }
    }

    public Vector2 GetPoint(int index) =>
        _points[index];

    public Vector2 ClosestPoint(Vector2 point)
    {
        if (_points.Length == 0)
            throw new InvalidOperationException("An empty point set has no closest point.");

        if (!float.IsFinite(point.X) || !float.IsFinite(point.Y))
            throw new ArgumentOutOfRangeException(nameof(point));

        var best = _points[0];
        var bestDistance = Vector2.DistanceSquared(best, point);

        for (var index = 1; index < _points.Length; index++)
        {
            var distance = Vector2.DistanceSquared(_points[index], point);

            if (distance < bestDistance)
            {
                best = _points[index];
                bestDistance = distance;
            }
        }

        return best;
    }

    public double DistanceSquaredTo(Vector2 point)
    {
        var closest = ClosestPoint(point);
        return Vector2.DistanceSquared(closest, point);
    }

    public PointSet2D Translate(Vector2 delta)
    {
        if (!float.IsFinite(delta.X) || !float.IsFinite(delta.Y))
            throw new ArgumentOutOfRangeException(nameof(delta));

        return new PointSet2D(
            _points.Select(point => point + delta));
    }

    public LineSegment2D GetAxisAlignedDiagonal()
    {
        var bounds = Bounds;

        return new LineSegment2D(
            new Vector2((float)bounds.Left, (float)bounds.Top),
            new Vector2((float)bounds.Right, (float)bounds.Bottom));
    }

    public Polyline2D ToPolyline()
    {
        if (_points.Length < 2)
            throw new InvalidOperationException("At least two points are required to create a polyline.");

        return new Polyline2D(_points);
    }
}
