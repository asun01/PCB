namespace Asun.UI.Viewports;

public sealed record ViewportStateSnapshot(
    ViewportTransform Transform,
    ViewportGestureSnapshot Gesture,
    RoiSelectionSnapshot Selection,
    ViewportDirtyFlags DirtyFlags,
    ViewportInputOwner InputOwner,
    IReadOnlyList<RoiDocumentItem> Items);
