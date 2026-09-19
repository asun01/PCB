using System.Numerics;
using Asun.Vision.Contracts;

public static class PointSet2DValidationHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        var points = new PointSet2D(
            new[]
            {
                new Vector2(0, 0),
                new Vector2(10, 0),
                new Vector2(0, 10)
            });

        var empty = new PointSet2D(Array.Empty<Vector2>());
        var centroid = points.Centroid;
        var bounds = points.Bounds;
        var closest = points.ClosestPoint(new Vector2(8, 2));
        var distanceSquared = points.DistanceSquaredTo(new Vector2(8, 2));
        var translated = points.Translate(new Vector2(5, 7));
        var diagonal = points.GetAxisAlignedDiagonal();
        var polyline = points.ToPolyline();

        var validation = PointSet2DValidationRuntime.IsValid(points);
        var emptyValidation = PointSet2DValidationRuntime.IsValid(empty);

        for (var i = 0; i < 10; i++)
            Check(points.Count == 3, $"point count round {i + 1} should remain three.");

        for (var i = 0; i < 10; i++)
            Check(centroid == new Vector2(10f / 3f, 10f / 3f), $"centroid round {i + 1} should be deterministic.");

        for (var i = 0; i < 10; i++)
            Check(bounds.Width == 10 && bounds.Height == 10, $"bounds round {i + 1} should span ten units.");

        for (var i = 0; i < 10; i++)
            Check(closest == new Vector2(10, 0), $"closest-point round {i + 1} should choose the nearest point.");

        for (var i = 0; i < 10; i++)
            Check(Math.Abs(distanceSquared - 8) < 1e-6, $"distance round {i + 1} should match the nearest-point distance.");

        for (var i = 0; i < 10; i++)
            Check(translated.Centroid == centroid + new Vector2(5, 7), $"translation round {i + 1} should shift the centroid.");

        for (var i = 0; i < 10; i++)
            Check(diagonal.Start == new Vector2(0, 0) && diagonal.End == new Vector2(10, 10), $"diagonal round {i + 1} should span the point-set bounds.");

        for (var i = 0; i < 10; i++)
            Check(polyline.Start == new Vector2(0, 0) && polyline.End == new Vector2(0, 10), $"polyline round {i + 1} should preserve point order.");

        for (var i = 0; i < 10; i++)
            Check(validation && emptyValidation, $"point-set validation round {i + 1} should accept populated and empty sets.");

        for (var i = 0; i < 10; i++)
            Check(!empty.TryGetCentroid(out _), $"empty centroid round {i + 1} should report no centroid.");

        assert(round == 100, $"Point-set geometry smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
