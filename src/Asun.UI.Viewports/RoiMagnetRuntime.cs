using System.Numerics;

namespace Asun.UI.Viewports;

public readonly record struct RoiMagnetResult(Vector2 Point, bool Snapped, float Distance);

public static class RoiMagnetRuntime
{
    public static RoiMagnetResult SnapToNearest(
        Vector2 point, IEnumerable<Vector2> candidates, float tolerance)
    {
        Validate(point);
        if (!float.IsFinite(tolerance) || tolerance < 0) throw new ArgumentOutOfRangeException(nameof(tolerance));
        var best = point;
        var bestDistance = float.PositiveInfinity;
        foreach (var candidate in candidates)
        {
            Validate(candidate);
            var distance = Vector2.Distance(point, candidate);
            if (distance < bestDistance) { bestDistance = distance; best = candidate; }
        }
        return new RoiMagnetResult(best, bestDistance <= tolerance, bestDistance);
    }

    private static void Validate(Vector2 p)
    {
        if (!float.IsFinite(p.X) || !float.IsFinite(p.Y)) throw new ArgumentOutOfRangeException(nameof(p));
    }
}
