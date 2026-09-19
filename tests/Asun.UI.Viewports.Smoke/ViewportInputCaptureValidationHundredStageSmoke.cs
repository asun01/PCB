using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportInputCaptureValidationHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        using var viewport = CreateViewport();
        var gestures = new ViewportGestureRuntime(viewport);
        var capture = new ViewportInputCaptureRuntime();
        var router = new ViewportInputRouterRuntime(gestures, capture);

        var round = 0;
        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        var left = router.PointerDown(new Vector2(10, 10), ViewportMouseButton.Left);
        var ownerAfterLeft = capture.Owner;

        for (var i = 0; i < 10; i++)
            Check(
                left.Kind == ViewportGestureKind.Panning &&
                ownerAfterLeft == ViewportInputOwner.Roi,
                $"left capture round {i + 1} should belong to ROI routing.");

        router.PointerUp(new Vector2(10, 10));
        var releasedOwner = capture.Owner;

        for (var i = 0; i < 10; i++)
            Check(
                ViewportInputCaptureValidationRuntime.IsFree(releasedOwner),
                $"release round {i + 1} should return capture to None.");

        var right = router.PointerDown(new Vector2(10, 10), ViewportMouseButton.Right);

        for (var i = 0; i < 10; i++)
            Check(
                right == default &&
                capture.Owner == ViewportInputOwner.None,
                $"right-button round {i + 1} should not acquire gesture capture.");

        var held = new ViewportInputCaptureRuntime();
        Check(
            held.TryCapture(ViewportInputOwner.Overlay),
            "external owner should acquire capture.");

        var blockedRouter = new ViewportInputRouterRuntime(
            gestures,
            held);

        var blockedDown = blockedRouter.PointerDown(
            new Vector2(10, 10),
            ViewportMouseButton.Left);

        for (var i = 0; i < 10; i++)
            Check(
                blockedDown == default &&
                held.Owner == ViewportInputOwner.Overlay,
                $"blocked acquisition round {i + 1} should preserve the external owner.");

        var blockedUp = blockedRouter.PointerUp(new Vector2(10, 10));

        for (var i = 0; i < 10; i++)
            Check(
                blockedUp == default &&
                held.Owner == ViewportInputOwner.Overlay,
                $"foreign pointer-up round {i + 1} should not release the external owner.");

        Check(
            ViewportInputCaptureValidationRuntime.IsOwnedBy(
                held.Owner,
                ViewportInputOwner.Overlay),
            "capture validator should confirm the preserved external owner.");

        Check(
            round == 100,
            $"Input capture validation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }

    private static RoiViewportRuntime CreateViewport() =>
        new(
            imageSize: new Vector2(100, 100),
            viewportSize: new Vector2(100, 100));
}
