using System.Numerics;

namespace Asun.UI.Viewports;

public static class ViewportTransformGuardRuntime
{
    public static bool IsUsable(ViewportTransform transform) =>
        double.IsFinite(transform.Scale) &&
        transform.Scale > 0 &&
        float.IsFinite(transform.Translation.X) &&
        float.IsFinite(transform.Translation.Y) &&
        float.IsFinite(transform.ImageSize.X) &&
        float.IsFinite(transform.ImageSize.Y) &&
        float.IsFinite(transform.ViewportSize.X) &&
        float.IsFinite(transform.ViewportSize.Y);

    public static ViewportTransform EnsureUsable(ViewportTransform transform)
    {
        if (!IsUsable(transform)) throw new InvalidOperationException("Viewport transform is invalid.");
        return transform;
    }
}
