namespace Asun.UI.Viewports;

public static class ViewportInputCaptureValidationRuntime
{
    public static bool IsValid(ViewportInputOwner owner)
    {
        return owner is ViewportInputOwner.None or
            ViewportInputOwner.Pan or
            ViewportInputOwner.Roi or
            ViewportInputOwner.Marquee or
            ViewportInputOwner.Overlay;
    }

    public static bool IsFree(ViewportInputOwner owner) =>
        owner == ViewportInputOwner.None;

    public static bool IsOwnedBy(
        ViewportInputOwner owner,
        ViewportInputOwner expected) =>
        expected != ViewportInputOwner.None &&
        owner == expected;
}
