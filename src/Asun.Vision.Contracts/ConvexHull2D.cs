using System.Numerics;

namespace Asun.Vision.Contracts;

/// <summary>
/// Deterministic 2D convex-hull construction using monotonic chain ordering.
/// </summary>
public static class ConvexHull2D
{
    public static IReadOnlyList<Vector2> Compute(IEnumerable<Vector2> points)
    {
        ArgumentNullException.ThrowIfNull(points);

        var unique = points
            .ToArray();

        if (unique.Any(point => !float.IsFinite(point.X) || !float.IsFinite(point.Y)))
            throw new ArgumentException("Convex-hull points must be finite.", nameof(points));

        Array.Sort(unique, ComparePoints);

        var deduplicated = new List<Vector2>(unique.Length);

        foreach (var point in unique)
        {
            if (deduplicated.Count == 0 || point != deduplicated[^1])
                deduplicated.Add(point);
        }

        if (deduplicated.Count <= 2)
            return deduplicated.ToArray();

        var lower = new List<Vector2>(deduplicated.Count);

        foreach (var point in deduplicated)
        {
            while (
                lower.Count >= 2 &&
                Cross(
                    lower[^1] - lower[^2],
                    point - lower[^1]) <= 0)
            {
                lower.RemoveAt(lower.Count - 1);
            }

            lower.Add(point);
        }

        var upper = new List<Vector2>(deduplicated.Count);

        for (var index = deduplicated.Count - 1; index >= 0; index--)
        {
            var point = deduplicated[index];

            while (
                upper.Count >= 2 &&
                Cross(
                    upper[^1] - upper[^2],
                    point - upper[^1]) <= 0)
            {
                upper.RemoveAt(upper.Count - 1);
            }

            upper.Add(point);
        }

        lower.RemoveAt(lower.Count - 1);
        upper.RemoveAt(upper.Count - 1);
        lower.AddRange(upper);

        return lower.ToArray();
    }

    public static Polygon2D ComputePolygon(IEnumerable<Vector2> points)
    {
        var hull = Compute(points);

        if (hull.Count < 3)
            throw new ArgumentException("At least three non-collinear points are required.", nameof(points));

        return new Polygon2D(hull);
    }

    private static int ComparePoints(Vector2 left, Vector2 right)
    {
        var x = left.X.CompareTo(right.X);
        return x != 0
            ? x
            : left.Y.CompareTo(right.Y);
    }

    private static double Cross(Vector2 left, Vector2 right) =>
        (double)left.X * right.Y -
        (double)left.Y * right.X;
}
