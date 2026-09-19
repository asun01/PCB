using System.Numerics;

namespace Asun.UI.Viewports;

public enum ViewportCommand
{
    Fit,
    ZoomIn,
    ZoomOut,
    PanLeft,
    PanRight,
    PanUp,
    PanDown,
    Undo,
    Redo
}

public static class ViewportCommandRuntime
{
    public static bool Execute(
        ViewportCommand command,
        RoiViewportRuntime viewport,
        double zoomFactor = 1.25)
    {
        ArgumentNullException.ThrowIfNull(viewport);
        if (!double.IsFinite(zoomFactor) || zoomFactor <= 1) throw new ArgumentOutOfRangeException(nameof(zoomFactor));

        var before = viewport.Transform;
        switch (command)
        {
            case ViewportCommand.Fit:
                viewport.FitToViewport();
                break;
            case ViewportCommand.ZoomIn:
                viewport.ZoomAt(zoomFactor, 0.05, 64, viewport.Transform.ViewportCenter);
                break;
            case ViewportCommand.ZoomOut:
                viewport.ZoomAt(1d / zoomFactor, 0.05, 64, viewport.Transform.ViewportCenter);
                break;
            case ViewportCommand.PanLeft:
                viewport.PanBy(new Vector2(32, 0));
                break;
            case ViewportCommand.PanRight:
                viewport.PanBy(new Vector2(-32, 0));
                break;
            case ViewportCommand.PanUp:
                viewport.PanBy(new Vector2(0, 32));
                break;
            case ViewportCommand.PanDown:
                viewport.PanBy(new Vector2(0, -32));
                break;
            case ViewportCommand.Undo:
                return viewport.Undo();
            case ViewportCommand.Redo:
                return viewport.Redo();
            default:
                throw new ArgumentOutOfRangeException(nameof(command));
        }
        return before != viewport.Transform;
    }
}
