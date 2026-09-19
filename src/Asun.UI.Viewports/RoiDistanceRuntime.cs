using System.Numerics;

namespace Asun.UI.Viewports;

public readonly record struct RoiDistanceResult(Guid FirstId, Guid SecondId, float Distance);

public static class RoiDistanceRuntime
{
    public static RoiDistanceResult Between(RoiDocumentItem first, RoiDocumentItem second)
    {
        var a = first.Geometry.GetBounds();
        var b = second.Geometry.GetBounds();
        var dx = MathF.Max(MathF.Max(a.Left - b.Right, b.Left - a.Right), 0f);
        var dy = MathF.Max(MathF.Max(a.Top - b.Bottom, b.Top - a.Bottom), 0f);
        return new RoiDistanceResult(first.Id, second.Id, MathF.Sqrt(dx * dx + dy * dy));
    }

    public static float CenterDistance(RoiDocumentItem first, RoiDocumentItem second) =>
        Vector2.Distance(first.Geometry.Center, second.Geometry.Center);
}
