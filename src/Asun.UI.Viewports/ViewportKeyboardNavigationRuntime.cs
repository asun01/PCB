using System.Numerics;

namespace Asun.UI.Viewports;

public enum ViewportNavigationKey { Left, Right, Up, Down, Home, End }

public static class ViewportKeyboardNavigationRuntime
{
    public static ViewportTransform Apply(
        ViewportTransform transform,
        ViewportNavigationKey key,
        float stepPixels = 32f)
    {
        if (!float.IsFinite(stepPixels) || stepPixels <= 0) throw new ArgumentOutOfRangeException(nameof(stepPixels));
        return key switch
        {
            ViewportNavigationKey.Left => transform.PanByClamped(new Vector2(stepPixels, 0)),
            ViewportNavigationKey.Right => transform.PanByClamped(new Vector2(-stepPixels, 0)),
            ViewportNavigationKey.Up => transform.PanByClamped(new Vector2(0, stepPixels)),
            ViewportNavigationKey.Down => transform.PanByClamped(new Vector2(0, -stepPixels)),
            ViewportNavigationKey.Home => transform.ResetToFit(),
            ViewportNavigationKey.End => transform.CenterOnImagePoint(transform.ImageCenter),
            _ => transform
        };
    }
}
