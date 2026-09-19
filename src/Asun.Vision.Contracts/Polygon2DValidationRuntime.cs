using System.Numerics;

namespace Asun.Vision.Contracts;

public static class Polygon2DValidationRuntime
{
    public static IReadOnlyList<string> Validate(Polygon2D polygon)
    {
        ArgumentNullException.ThrowIfNull(polygon);

        var errors = new List<string>();

        if (polygon.VertexCount < 3)
            errors.Add("Polygon must contain at least three vertices.");

        if (polygon.Vertices.Any(point =>
            !float.IsFinite(point.X) ||
            !float.IsFinite(point.Y)))
        {
            errors.Add("Polygon vertices must remain finite.");
        }

        if (!double.IsFinite(polygon.SignedArea) ||
            !double.IsFinite(polygon.Perimeter))
        {
            errors.Add("Polygon area and perimeter must remain finite.");
        }

        if (polygon.Area <= 0)
            errors.Add("Polygon area must be positive for centroid operations.");

        return errors;
    }

    public static bool IsValid(Polygon2D polygon) =>
        Validate(polygon).Count == 0;
}
