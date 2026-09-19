using System.Numerics;

namespace Asun.UI.Viewports;

public static class ViewportAutoPanRuntime
{
    public static Vector2 GetDelta(
        Vector2 pointer,
        Vector2 viewportSize,
        float edgePixels,
        float speed)
    {
        Validate(pointer);
        if (!float.IsFinite(edgePixels) || edgePixels <= 0 ||
            !float.IsFinite(speed) || speed < 0)
            throw new ArgumentOutOfRangeException();

        var delta = Vector2.Zero;
        if (pointer.X < edgePixels) delta.X = -speed * (1f - pointer.X / edgePixels);
        else if (pointer.X > viewportSize.X - edgePixels)
            delta.X = speed * (1f - (viewportSize.X - pointer.X) / edgePixels);

        if (pointer.Y < edgePixels) delta.Y = -speed * (1f - pointer.Y / edgePixels);
        else if (pointer.Y > viewportSize.Y - edgePixels)
            delta.Y = speed * (1f - (viewportSize.Y - pointer.Y) / edgePixels);

        return delta;
    }

    private static void Validate(Vector2 p)
    {
        if (!float.IsFinite(p.X) || !float.IsFinite(p.Y)) throw new ArgumentOutOfRangeException(nameof(p));
    }
}
