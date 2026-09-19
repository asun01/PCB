namespace Asun.UI.Viewports;

public readonly record struct ViewportRenderFrameMetrics(
    int RequestedTiles,
    int LoadedTiles,
    int VisibleTiles,
    int RoiCommands,
    int SceneCommands,
    int SceneDiffs,
    bool Complete,
    long Generation);

public static class ViewportRenderFrameRuntime
{
    public static ViewportRenderFrameMetrics Capture<TTile>(
        ViewportCompositeFrame<TTile> frame)
    {
        ArgumentNullException.ThrowIfNull(frame);

        return new ViewportRenderFrameMetrics(
            frame.Tiles.RequestedCount,
            frame.Tiles.LoadedCount,
            frame.Tiles.RequestedVisibleCount,
            frame.RoiCommands.Count,
            frame.SceneCommands.Count,
            frame.SceneDiff.Count,
            frame.IsReady,
            frame.Generation);
    }
}
