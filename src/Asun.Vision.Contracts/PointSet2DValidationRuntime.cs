using System.Numerics;

namespace Asun.Vision.Contracts;

public static class PointSet2DValidationRuntime
{
    public static IReadOnlyList<string> Validate(PointSet2D pointSet)
    {
        ArgumentNullException.ThrowIfNull(pointSet);

        var errors = new List<string>();

        if (pointSet.Points.Any(point =>
            !float.IsFinite(point.X) ||
            !float.IsFinite(point.Y)))
        {
            errors.Add("Point-set coordinates must remain finite.");
        }

        if (pointSet.Count == 0)
        {
            if (pointSet.TryGetCentroid(out _))
                errors.Add("Empty point sets cannot expose a centroid.");
        }
        else
        {
            if (!pointSet.TryGetCentroid(out var centroid) ||
                !float.IsFinite(centroid.X) ||
                !float.IsFinite(centroid.Y))
            {
                errors.Add("Non-empty point sets must expose a finite centroid.");
            }

            var bounds = pointSet.Bounds;

            if (!bounds.IsValid)
                errors.Add("Point-set bounds must be valid.");
        }

        return errors;
    }

    public static bool IsValid(PointSet2D pointSet) =>
        Validate(pointSet).Count == 0;
}
