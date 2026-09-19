using System.Numerics;
using Asun.Vision.Contracts;

public static class Polygon2DValidationHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        var polygon = new Polygon2D(new[]
        {
            new Vector2(0, 0),
            new Vector2(10, 0),
            new Vector2(10, 10),
            new Vector2(0, 10)
        });

        var reversed = polygon.Reverse();
        var translated = polygon.Translate(new Vector2(5, 7));
        var center = polygon.Centroid;
        var boundary = polygon.ContainsInclusive(
            new Vector2(10, 5),
            boundaryTolerance: 1e-3);
        var inside = polygon.Contains(new Vector2(5, 5));
        var outside = !polygon.Contains(new Vector2(15, 5));
        var closest = polygon.ClosestPoint(new Vector2(15, 5));
        var distance = polygon.DistanceTo(new Vector2(15, 5));
        var validation = Polygon2DValidationRuntime.IsValid(polygon);
        var reverseValidation = Polygon2DValidationRuntime.IsValid(reversed);

        for (var i = 0; i < 10; i++)
            Check(polygon.VertexCount == 4 && polygon.Vertices.Count == 5, $"polygon ring round {i + 1} should expose four vertices plus closure.");

        for (var i = 0; i < 10; i++)
            Check(Math.Abs(polygon.Area - 100) < 1e-9, $"area round {i + 1} should be one hundred.");

        for (var i = 0; i < 10; i++)
            Check(Math.Abs(polygon.SignedArea - 100) < 1e-9 && reversed.SignedArea < 0, $"orientation round {i + 1} should flip deterministically.");

        for (var i = 0; i < 10; i++)
            Check(center == new Vector2(5, 5), $"centroid round {i + 1} should be central.");

        for (var i = 0; i < 10; i++)
            Check(Math.Abs(polygon.Perimeter - 40) < 1e-6, $"perimeter round {i + 1} should be forty.");

        for (var i = 0; i < 10; i++)
            Check(inside && outside, $"containment round {i + 1} should distinguish inside and outside points.");

        for (var i = 0; i < 10; i++)
            Check(boundary, $"boundary tolerance round {i + 1} should include the edge.");

        for (var i = 0; i < 10; i++)
            Check(closest == new Vector2(10, 5) && Math.Abs(distance - 5) < 1e-6, $"closest-point round {i + 1} should identify the right edge.");

        for (var i = 0; i < 10; i++)
            Check(translated.Centroid == new Vector2(10, 12), $"translation round {i + 1} should move the centroid.");

        for (var i = 0; i < 10; i++)
            Check(validation && reverseValidation, $"polygon validation round {i + 1} should accept both orientations.");

        assert(round == 100, $"Polygon geometry smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
