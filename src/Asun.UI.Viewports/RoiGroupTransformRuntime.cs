using System.Numerics;

namespace Asun.UI.Viewports;

public readonly record struct RoiGroupTransformResult(
    IReadOnlyList<RoiDocumentItem> Items,
    RectangleF Bounds,
    Vector2 Pivot);

public static class RoiGroupTransformRuntime
{
    public static RoiGroupTransformResult Translate(
        IEnumerable<RoiDocumentItem> items,
        Vector2 delta)
    {
        ArgumentNullException.ThrowIfNull(items);
        Validate(delta);

        var source = items.ToArray();
        if (source.Length == 0)
            return new RoiGroupTransformResult(
                Array.Empty<RoiDocumentItem>(),
                default,
                Vector2.Zero);

        var transformed = source
            .Select(item => item with { Geometry = item.Geometry.Translate(delta) })
            .ToArray();

        return BuildResult(transformed);
    }

    public static RoiGroupTransformResult Rotate(
        IEnumerable<RoiDocumentItem> items,
        float radians,
        Vector2? pivot = null)
    {
        ArgumentNullException.ThrowIfNull(items);

        if (!float.IsFinite(radians))
            throw new ArgumentOutOfRangeException(nameof(radians));

        var source = items.ToArray();
        if (source.Length == 0)
            return new RoiGroupTransformResult(
                Array.Empty<RoiDocumentItem>(),
                default,
                Vector2.Zero);

        var center = pivot ?? GetBoundsCenter(GetUnionBounds(source));
        var transformed = source
            .Select(item => item with
            {
                Geometry = RotateAround(item.Geometry, radians, center)
            })
            .ToArray();

        return BuildResult(transformed, center);
    }

    public static RoiGroupTransformResult Scale(
        IEnumerable<RoiDocumentItem> items,
        Vector2 factors,
        Vector2? pivot = null)
    {
        ArgumentNullException.ThrowIfNull(items);

        if (!float.IsFinite(factors.X) || !float.IsFinite(factors.Y) ||
            factors.X <= 0 || factors.Y <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(factors));
        }

        var source = items.ToArray();
        if (source.Length == 0)
            return new RoiGroupTransformResult(
                Array.Empty<RoiDocumentItem>(),
                default,
                Vector2.Zero);

        var center = pivot ?? GetUnionBounds(source).Center.ToVector2();

        var transformed = source
            .Select(item => item with
            {
                Geometry = ScaleAround(item.Geometry, factors, center)
            })
            .ToArray();

        return BuildResult(transformed, center);
    }

    private static RoiGeometry RotateAround(
        RoiGeometry geometry,
        float radians,
        Vector2 pivot)
    {
        var centerDelta = geometry.Center - pivot;
        var cos = MathF.Cos(radians);
        var sin = MathF.Sin(radians);

        var rotatedCenter = pivot + new Vector2(
            centerDelta.X * cos - centerDelta.Y * sin,
            centerDelta.X * sin + centerDelta.Y * cos);

        if (geometry.IsPolygon)
            return geometry.WithCenter(rotatedCenter).Rotate(radians);

        return geometry
            .WithCenter(rotatedCenter)
            .Rotate(radians);
    }

    private static RoiGeometry ScaleAround(
        RoiGeometry geometry,
        Vector2 factors,
        Vector2 pivot)
    {
        var centerDelta = geometry.Center - pivot;
        var newCenter = pivot + new Vector2(
            centerDelta.X * factors.X,
            centerDelta.Y * factors.Y);

        if (geometry.IsPolygon)
        {
            var points = geometry.Vertices.Select(point =>
                pivot + new Vector2(
                    (point.X - pivot.X) * factors.X,
                    (point.Y - pivot.Y) * factors.Y));

            return RoiGeometry.CreatePolygon(points);
        }

        return geometry.WithCenter(newCenter).WithSize(
            new Vector2(
                geometry.Size.X * factors.X,
                geometry.Size.Y * factors.Y));
    }

    private static RectangleF GetUnionBounds(
        IReadOnlyList<RoiDocumentItem> items)
    {
        var bounds = items[0].Geometry.GetBounds();

        for (var i = 1; i < items.Count; i++)
            bounds = RectangleF.Union(
                bounds,
                items[i].Geometry.GetBounds());

        return bounds;
    }

    private static RoiGroupTransformResult BuildResult(
        IReadOnlyList<RoiDocumentItem> items,
        Vector2? pivot = null)
    {
        var bounds = GetUnionBounds(items);
        return new RoiGroupTransformResult(
            items,
            bounds,
            pivot ?? GetBoundsCenter(bounds));
    }


    private static Vector2 GetBoundsCenter(RectangleF bounds) =>
        new(
            bounds.X + bounds.Width / 2f,
            bounds.Y + bounds.Height / 2f);

    private static void Validate(Vector2 value)
    {
        if (!float.IsFinite(value.X) || !float.IsFinite(value.Y))
            throw new ArgumentOutOfRangeException(nameof(value));
    }
}
