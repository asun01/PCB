namespace Asun.UI.Viewports;

public readonly record struct ViewportRenderFrameState(
    ViewportRenderDeliveryStatus Status,
    long Generation,
    int PlannedUnits,
    int RenderedUnits,
    int DeferredUnits,
    int RegionCount,
    IReadOnlyList<ViewportRenderWorkItem> DeferredWorkItems,
    Exception? Error)
{
    public bool IsComplete =>
        Status == ViewportRenderDeliveryStatus.Succeeded &&
        DeferredUnits == 0;

    public bool IsPartial =>
        RenderedUnits > 0 &&
        DeferredUnits > 0;

    public bool HasDeferredWork =>
        DeferredUnits > 0 ||
        DeferredWorkItems.Count != 0;

    public bool HasError =>
        Error is not null;
}
