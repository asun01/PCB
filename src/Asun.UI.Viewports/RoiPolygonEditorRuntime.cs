using System.Numerics;

namespace Asun.UI.Viewports;

public readonly record struct RoiPolygonEdgeHit(
    int EdgeIndex,
    Vector2 ClosestPoint,
    float Distance);

public static class RoiPolygonEditorRuntime
{
    public static RoiGeometry InsertVertex(
        RoiGeometry polygon,
        Vector2 point,
        float maximumEdgeDistance = 12f)
    {
        EnsurePolygon(polygon);
        Validate(point);

        if (!float.IsFinite(maximumEdgeDistance) || maximumEdgeDistance < 0)
            throw new ArgumentOutOfRangeException(nameof(maximumEdgeDistance));

        var hit = FindClosestEdge(polygon, point);

        if (hit.EdgeIndex < 0 || hit.Distance > maximumEdgeDistance)
            throw new InvalidOperationException("No polygon edge is close enough to insert a vertex.");

        var points = polygon.Vertices.ToList();
        points.Insert(hit.EdgeIndex + 1, point);
        return RoiGeometry.CreatePolygon(points);
    }

    public static RoiGeometry RemoveVertex(
        RoiGeometry polygon,
        int index)
    {
        EnsurePolygon(polygon);

        if ((uint)index >= (uint)polygon.Vertices.Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (polygon.Vertices.Count <= 3)
            throw new InvalidOperationException("A polygon ROI must retain at least three vertices.");

        var points = polygon.Vertices.ToList();
        points.RemoveAt(index);
        return RoiGeometry.CreatePolygon(points);
    }

    public static RoiGeometry Reverse(RoiGeometry polygon)
    {
        EnsurePolygon(polygon);
        return RoiGeometry.CreatePolygon(polygon.Vertices.Reverse());
    }

    public static RoiGeometry MergeNearVertices(
        RoiGeometry polygon,
        float mergeDistance)
    {
        EnsurePolygon(polygon);

        if (!float.IsFinite(mergeDistance) || mergeDistance < 0)
            throw new ArgumentOutOfRangeException(nameof(mergeDistance));

        var points = polygon.Vertices.ToList();
        if (points.Count <= 3)
            return polygon;

        var changed = true;
        while (changed && points.Count > 3)
        {
            changed = false;

            for (var i = 0; i < points.Count; i++)
            {
                var next = (i + 1) % points.Count;
                if (Vector2.Distance(points[i], points[next]) <= mergeDistance)
                {
                    points.RemoveAt(next);
                    changed = true;
                    break;
                }
            }
        }

        return RoiGeometry.CreatePolygon(points);
    }

    public static RoiPolygonEdgeHit FindClosestEdge(
        RoiGeometry polygon,
        Vector2 point)
    {
        EnsurePolygon(polygon);
        Validate(point);

        var bestIndex = -1;
        var bestPoint = default(Vector2);
        var bestDistance = float.PositiveInfinity;

        for (var i = 0; i < polygon.Vertices.Count; i++)
        {
            var start = polygon.Vertices[i];
            var end = polygon.Vertices[(i + 1) % polygon.Vertices.Count];
            var delta = end - start;
            var lengthSquared = delta.LengthSquared();

            var amount = lengthSquared <= float.Epsilon
                ? 0f
                : Math.Clamp(
                    Vector2.Dot(point - start, delta) / lengthSquared,
                    0f,
                    1f);

            var closest = start + delta * amount;
            var distance = Vector2.Distance(point, closest);

            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestIndex = i;
                bestPoint = closest;
            }
        }

        return new RoiPolygonEdgeHit(
            bestIndex,
            bestPoint,
            bestDistance);
    }

    private static void EnsurePolygon(RoiGeometry polygon)
    {
        ArgumentNullException.ThrowIfNull(polygon);

        if (!polygon.IsPolygon || !polygon.IsValid)
            throw new ArgumentException("A valid polygon ROI is required.", nameof(polygon));
    }

    private static void Validate(Vector2 point)
    {
        if (!float.IsFinite(point.X) || !float.IsFinite(point.Y))
            throw new ArgumentOutOfRangeException(nameof(point));
    }
}
