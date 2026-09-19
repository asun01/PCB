namespace Asun.UI.Viewports;

public sealed record ViewportInteractionSnapshot(
    ViewportTransform Transform,
    ViewportGestureSnapshot Gesture,
    RoiViewportSnapshot Roi,
    ViewportDirtyFlags DirtyFlags,
    ViewportInputOwner InputOwner);
