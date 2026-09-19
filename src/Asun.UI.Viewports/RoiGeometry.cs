using System.Collections.Immutable;
using System.Numerics;
using System.Drawing;

namespace Asun.UI.Viewports;

public enum RoiShapeKind
{
    Rectangle,
    RotatedRectangle,
    Ellipse,
    Polygon
}

public enum RoiHandleKind
{
    None,
    Body,
    TopLeft,
    Top,
    TopRight,
    Right,
    BottomRight,
    Bottom,
    BottomLeft,
    Left,
    Rotation,
    Vertex
}

public readonly record struct RoiControlPoint(
    RoiHandleKind Kind,
    int Index,
    Vector2 Position);

/// <summary>
/// Immutable, framework-neutral ROI geometry used by the viewport interaction layer.
/// It contains only editor geometry and no measurement-tool semantics.
/// </summary>
public sealed class RoiGeometry : IEquatable<RoiGeometry>
{
    private const float MinimumSize = 1e-4f;
    private readonly ImmutableArray<Vector2> _vertices;

    private RoiGeometry(
        RoiShapeKind kind,
        Vector2 center,
        Vector2 size,
        float rotationRadians,
        ImmutableArray<Vector2> vertices)
    {
        Kind = kind;
        Center = center;
        Size = size;
        RotationRadians = rotationRadians;
        _vertices = vertices;
    }

    public RoiShapeKind Kind { get; }

    public Vector2 Center { get; }

    public Vector2 Size { get; }

    public float RotationRadians { get; }

    public IReadOnlyList<Vector2> Vertices => _vertices;

    public bool IsPolygon => Kind == RoiShapeKind.Polygon;

    public bool IsEllipse => Kind == RoiShapeKind.Ellipse;

    public bool IsRectangle =>
        Kind is RoiShapeKind.Rectangle or RoiShapeKind.RotatedRectangle;

    public bool IsValid
    {
        get
        {
            if (!IsFinite(Center) ||
                !IsFinite(Size) ||
                !float.IsFinite(RotationRadians) ||
                Size.X < MinimumSize ||
                Size.Y < MinimumSize)
            {
                return false;
            }

            if (Kind != RoiShapeKind.Polygon)
                return true;

            return _vertices.Length >= 3 &&
                   _vertices.All(IsFinite);
        }
    }

    public static RoiGeometry CreateRectangle(
        Vector2 center,
        Vector2 size) =>
        Create(RoiShapeKind.Rectangle, center, size, 0f);

    public static RoiGeometry CreateRotatedRectangle(
        Vector2 center,
        Vector2 size,
        float rotationRadians) =>
        Create(RoiShapeKind.RotatedRectangle, center, size, rotationRadians);

    public static RoiGeometry CreateEllipse(
        Vector2 center,
        Vector2 size,
        float rotationRadians = 0f) =>
        Create(RoiShapeKind.Ellipse, center, size, rotationRadians);

    public static RoiGeometry CreatePolygon(IEnumerable<Vector2> vertices)
    {
        ArgumentNullException.ThrowIfNull(vertices);

        var points = vertices.ToImmutableArray();

        if (points.Length < 3)
            throw new ArgumentException("A polygon ROI requires at least three vertices.", nameof(vertices));

        if (points.Any(point => !IsFinite(point)))
            throw new ArgumentOutOfRangeException(nameof(vertices));

        if (MathF.Abs(SignedArea(points)) < MinimumSize)
            throw new ArgumentException("A polygon ROI must enclose a non-zero area.", nameof(vertices));

        var bounds = GetBounds(points);
        return new RoiGeometry(
            RoiShapeKind.Polygon,
            bounds.Center,
            bounds.Size,
            0f,
            points);
    }

    public RoiGeometry Translate(Vector2 delta)
    {
        ValidateFinite(delta);

        if (IsPolygon)
            return CreatePolygon(_vertices.Select(point => point + delta));

        return Create(
            Kind,
            Center + delta,
            Size,
            RotationRadians);
    }

    public RoiGeometry Rotate(float deltaRadians)
    {
        if (!float.IsFinite(deltaRadians))
            throw new ArgumentOutOfRangeException(nameof(deltaRadians));

        if (IsPolygon)
        {
            var cos = MathF.Cos(deltaRadians);
            var sin = MathF.Sin(deltaRadians);
            var rotated = _vertices.Select(point =>
            {
                var local = point - Center;
                return Center + new Vector2(
                    local.X * cos - local.Y * sin,
                    local.X * sin + local.Y * cos);
            });

            return CreatePolygon(rotated);
        }

        return Create(Kind, Center, Size, NormalizeAngle(RotationRadians + deltaRadians));
    }

    public RoiGeometry WithCenter(Vector2 center)
    {
        ValidateFinite(center);

        if (IsPolygon)
            return CreatePolygon(_vertices.Select(point => point + (center - Center)));

        return Create(Kind, center, Size, RotationRadians);
    }

    public RoiGeometry WithSize(Vector2 size)
    {
        ValidateSize(size);

        if (IsPolygon)
            throw new InvalidOperationException("Polygon ROI size must be edited by vertex operations.");

        return Create(Kind, Center, size, RotationRadians);
    }

    public RoiGeometry WithRotation(float rotationRadians)
    {
        if (!float.IsFinite(rotationRadians))
            throw new ArgumentOutOfRangeException(nameof(rotationRadians));

        if (IsPolygon)
            throw new InvalidOperationException("Polygon ROI rotation is represented by its vertices.");

        return Create(Kind, Center, Size, NormalizeAngle(rotationRadians));
    }

    public RoiGeometry WithVertex(int index, Vector2 point)
    {
        if (!IsPolygon)
            throw new InvalidOperationException("Only polygon ROIs expose vertex editing.");

        ValidateFinite(point);

        if ((uint)index >= (uint)_vertices.Length)
            throw new ArgumentOutOfRangeException(nameof(index));

        var points = _vertices.ToArray();
        points[index] = point;
        return CreatePolygon(points);
    }

    public Vector2 ToLocal(Vector2 worldPoint)
    {
        ValidateFinite(worldPoint);

        var translated = worldPoint - Center;
        var cos = MathF.Cos(-RotationRadians);
        var sin = MathF.Sin(-RotationRadians);

        return new Vector2(
            translated.X * cos - translated.Y * sin,
            translated.X * sin + translated.Y * cos);
    }

    public Vector2 ToWorld(Vector2 localPoint)
    {
        ValidateFinite(localPoint);

        var cos = MathF.Cos(RotationRadians);
        var sin = MathF.Sin(RotationRadians);

        return Center + new Vector2(
            localPoint.X * cos - localPoint.Y * sin,
            localPoint.X * sin + localPoint.Y * cos);
    }

    public bool Contains(Vector2 point, float boundaryTolerance = 0f)
    {
        ValidateFinite(point);

        if (!float.IsFinite(boundaryTolerance) || boundaryTolerance < 0f)
            throw new ArgumentOutOfRangeException(nameof(boundaryTolerance));

        if (!IsValid)
            return false;

        if (IsPolygon)
            return ContainsPolygon(point, boundaryTolerance);

        var local = ToLocal(point);
        var half = Size / 2f;

        if (Kind is RoiShapeKind.Rectangle or RoiShapeKind.RotatedRectangle)
        {
            return MathF.Abs(local.X) <= half.X + boundaryTolerance &&
                   MathF.Abs(local.Y) <= half.Y + boundaryTolerance;
        }

        var x = MathF.Max(0f, MathF.Abs(local.X) - boundaryTolerance);
        var y = MathF.Max(0f, MathF.Abs(local.Y) - boundaryTolerance);
        var normalizedX = x / half.X;
        var normalizedY = y / half.Y;

        return normalizedX * normalizedX + normalizedY * normalizedY <= 1f;
    }

    public RectangleF GetBounds()
    {
        if (!IsValid)
            throw new InvalidOperationException("The ROI geometry is invalid.");

        if (IsPolygon)
            return GetBounds(_vertices);

        var controlPoints = GetCornerPoints();
        var minX = controlPoints.Min(point => point.X);
        var maxX = controlPoints.Max(point => point.X);
        var minY = controlPoints.Min(point => point.Y);
        var maxY = controlPoints.Max(point => point.Y);

        return RectangleF.FromLTRB(minX, minY, maxX, maxY);
    }

    public IReadOnlyList<RoiControlPoint> GetControlPoints()
    {
        if (!IsValid)
            return Array.Empty<RoiControlPoint>();

        if (IsPolygon)
        {
            var polygonControls = new RoiControlPoint[_vertices.Length];
            for (var i = 0; i < _vertices.Length; i++)
            {
                polygonControls[i] = new RoiControlPoint(
                    RoiHandleKind.Vertex,
                    i,
                    _vertices[i]);
            }

            return polygonControls;
        }

        var half = Size / 2f;
        var points = new[]
        {
            new RoiControlPoint(RoiHandleKind.TopLeft, 0, ToWorld(new Vector2(-half.X, -half.Y))),
            new RoiControlPoint(RoiHandleKind.Top, 0, ToWorld(new Vector2(0f, -half.Y))),
            new RoiControlPoint(RoiHandleKind.TopRight, 0, ToWorld(new Vector2(half.X, -half.Y))),
            new RoiControlPoint(RoiHandleKind.Right, 0, ToWorld(new Vector2(half.X, 0f))),
            new RoiControlPoint(RoiHandleKind.BottomRight, 0, ToWorld(new Vector2(half.X, half.Y))),
            new RoiControlPoint(RoiHandleKind.Bottom, 0, ToWorld(new Vector2(0f, half.Y))),
            new RoiControlPoint(RoiHandleKind.BottomLeft, 0, ToWorld(new Vector2(-half.X, half.Y))),
            new RoiControlPoint(RoiHandleKind.Left, 0, ToWorld(new Vector2(-half.X, 0f))),
            new RoiControlPoint(
                RoiHandleKind.Rotation,
                0,
                ToWorld(new Vector2(0f, -half.Y - MathF.Max(12f, MathF.Min(Size.X, Size.Y) * 0.2f))))
        };

        return points;
    }

    public IReadOnlyList<Vector2> GetCornerPoints()
    {
        if (IsPolygon)
            return _vertices;

        var half = Size / 2f;
        return new[]
        {
            ToWorld(new Vector2(-half.X, -half.Y)),
            ToWorld(new Vector2(half.X, -half.Y)),
            ToWorld(new Vector2(half.X, half.Y)),
            ToWorld(new Vector2(-half.X, half.Y))
        };
    }

    public bool Equals(RoiGeometry? other)
    {
        if (ReferenceEquals(this, other))
            return true;

        if (other is null ||
            Kind != other.Kind ||
            Center != other.Center ||
            Size != other.Size ||
            RotationRadians != other.RotationRadians ||
            _vertices.Length != other._vertices.Length)
        {
            return false;
        }

        return _vertices.AsSpan().SequenceEqual(other._vertices.AsSpan());
    }

    public override bool Equals(object? obj) =>
        obj is RoiGeometry other && Equals(other);

    public override int GetHashCode()
    {
        var hash = HashCode.Combine(
            Kind,
            Center,
            Size,
            RotationRadians);

        foreach (var point in _vertices)
            hash = HashCode.Combine(hash, point);

        return hash;
    }

    private bool ContainsPolygon(Vector2 point, float boundaryTolerance)
    {
        var bounds = GetBounds(_vertices);
        var expandedBounds = RectangleF.Inflate(
            bounds,
            boundaryTolerance,
            boundaryTolerance);

        if (!expandedBounds.Contains(point.X, point.Y))
            return false;

        for (var i = 0; i < _vertices.Length; i++)
        {
            var start = _vertices[i];
            var end = _vertices[(i + 1) % _vertices.Length];
            if (DistancePointToSegment(point, start, end) <= boundaryTolerance)
                return true;
        }

        var inside = false;
        for (var i = 0, j = _vertices.Length - 1; i < _vertices.Length; j = i++)
        {
            var a = _vertices[i];
            var b = _vertices[j];

            var intersects =
                (a.Y > point.Y) != (b.Y > point.Y) &&
                point.X < (b.X - a.X) * (point.Y - a.Y) / (b.Y - a.Y) + a.X;

            if (intersects)
                inside = !inside;
        }

        return inside;
    }

    private static RoiGeometry Create(
        RoiShapeKind kind,
        Vector2 center,
        Vector2 size,
        float rotationRadians)
    {
        ValidateFinite(center);
        ValidateSize(size);

        if (!float.IsFinite(rotationRadians))
            throw new ArgumentOutOfRangeException(nameof(rotationRadians));

        if (kind == RoiShapeKind.Polygon)
            throw new ArgumentException("Use CreatePolygon for polygon ROIs.", nameof(kind));

        return new RoiGeometry(
            kind,
            center,
            size,
            NormalizeAngle(rotationRadians),
            ImmutableArray<Vector2>.Empty);
    }

    private static RectangleF GetBounds(ImmutableArray<Vector2> points)
    {
        var minX = points.Min(point => point.X);
        var maxX = points.Max(point => point.X);
        var minY = points.Min(point => point.Y);
        var maxY = points.Max(point => point.Y);

        return RectangleF.FromLTRB(minX, minY, maxX, maxY);
    }

    private static float SignedArea(ImmutableArray<Vector2> points)
    {
        double area = 0d;
        for (var i = 0; i < points.Length; i++)
        {
            var current = points[i];
            var next = points[(i + 1) % points.Length];
            area += (double)current.X * next.Y - (double)next.X * current.Y;
        }

        return (float)(area / 2d);
    }

    private static float DistancePointToSegment(
        Vector2 point,
        Vector2 start,
        Vector2 end)
    {
        var delta = end - start;
        var lengthSquared = delta.LengthSquared();

        if (lengthSquared <= MinimumSize * MinimumSize)
            return Vector2.Distance(point, start);

        var amount = Vector2.Dot(point - start, delta) / lengthSquared;
        amount = Math.Clamp(amount, 0f, 1f);
        var closest = start + delta * amount;
        return Vector2.Distance(point, closest);
    }

    private static float NormalizeAngle(float radians)
    {
        var fullTurn = 2f * MathF.PI;
        var normalized = radians % fullTurn;

        if (normalized > MathF.PI)
            normalized -= fullTurn;
        else if (normalized < -MathF.PI)
            normalized += fullTurn;

        return normalized;
    }

    private static void ValidateSize(Vector2 size)
    {
        if (!IsFinite(size) ||
            size.X < MinimumSize ||
            size.Y < MinimumSize)
        {
            throw new ArgumentOutOfRangeException(nameof(size));
        }
    }

    private static void ValidateFinite(Vector2 value)
    {
        if (!IsFinite(value))
            throw new ArgumentOutOfRangeException(nameof(value));
    }

    private static bool IsFinite(Vector2 value) =>
        float.IsFinite(value.X) && float.IsFinite(value.Y);
}

public readonly record struct RoiHitResult(
    bool Hit,
    RoiHandleKind Handle,
    int Index,
    float Distance)
{
    public static RoiHitResult None => new(false, RoiHandleKind.None, -1, float.PositiveInfinity);

    public bool IsHandleHit =>
        Hit && Handle is not RoiHandleKind.None and not RoiHandleKind.Body;
}
