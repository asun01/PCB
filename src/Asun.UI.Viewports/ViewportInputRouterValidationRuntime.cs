namespace Asun.UI.Viewports;

public static class ViewportInputRouterValidationRuntime
{
    public static bool IsValid(
        ViewportInputOwner captureOwner,
        ViewportInputOwner routerOwner)
    {
        if (!ViewportInputCaptureValidationRuntime.IsValid(captureOwner) ||
            !ViewportInputCaptureValidationRuntime.IsValid(routerOwner))
            return false;

        return captureOwner == routerOwner;
    }

    public static bool IsReleased(
        ViewportInputOwner captureOwner,
        ViewportInputOwner routerOwner) =>
        captureOwner == ViewportInputOwner.None &&
        routerOwner == ViewportInputOwner.None;
}
