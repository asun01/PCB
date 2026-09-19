using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportInputCaptureValidationHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var viewport = new RoiViewportRuntime(
            imageSize: new Vector2(100, 100),
            viewportSize: new Vector2(100, 100));
        var gestures = new ViewportGestureRuntime(viewport);
        var capture = new ViewportInputCaptureRuntime();
        var router = new ViewportInputRouterRuntime(gestures, capture);

        var round = 0;
        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        var left = router.PointerDown(
            new Vector2(10, 10),
            ViewportMouseButton.Left);

        for (var i = 0; i < 10; i++)
            Check(
                left.Kind == ViewportGestureKind.Panning &&
                capture.Owner == ViewportInputOwner.Roi &&
                router.CapturedOwner == ViewportInputOwner.Roi,
                $"left capture round {i + 1} should belong to ROI.");

        router.PointerUp(new Vector2(10, 10));

        for (var i = 0; i < 10; i++)
            Check(
                ViewportInputCaptureValidationRuntime.IsFree(capture.Owner) &&
                router.CapturedOwner == ViewportInputOwner.None,
                $"owned release round {i + 1} should clear both owners.");

        var right = router.PointerDown(
            new Vector2(10, 10),
            ViewportMouseButton.Right);

        for (var i = 0; i < 10; i++)
            Check(
                right == default &&
                capture.Owner == ViewportInputOwner.None,
                $"right-button round {i + 1} should not acquire capture.");

        var externalCaptured = capture.TryCapture(ViewportInputOwner.Overlay);

        for (var i = 0; i < 10; i++)
            Check(
                externalCaptured &&
                capture.Owner == ViewportInputOwner.Overlay,
                $"external ownership round {i + 1} should be established.");

        var blockedRouter = new ViewportInputRouterRuntime(
            gestures,
            capture);

        var blockedDown = blockedRouter.PointerDown(
            new Vector2(10, 10),
            ViewportMouseButton.Left);

        for (var i = 0; i < 10; i++)
            Check(
                blockedDown == default &&
                capture.Owner == ViewportInputOwner.Overlay &&
                blockedRouter.CapturedOwner == ViewportInputOwner.None,
                $"blocked acquisition round {i + 1} should preserve the external owner.");

        var blockedMove = blockedRouter.PointerMove(new Vector2(20, 20));

        for (var i = 0; i < 10; i++)
            Check(
                capture.Owner == ViewportInputOwner.Overlay &&
                blockedRouter.CapturedOwner == ViewportInputOwner.None,
                $"blocked move round {i + 1} should not steal capture.");

        var blockedUp = blockedRouter.PointerUp(new Vector2(20, 20));

        for (var i = 0; i < 10; i++)
            Check(
                blockedUp == default &&
                capture.Owner == ViewportInputOwner.Overlay,
                $"foreign pointer-up round {i + 1} should not release the overlay owner.");

        Check(
            !blockedRouter.Escape(new Vector2(20, 20)) &&
            capture.Owner == ViewportInputOwner.Overlay,
            "foreign Escape should not release the overlay owner.");

        for (var i = 0; i < 10; i++)
            Check(
                ViewportInputCaptureValidationRuntime.IsOwnedBy(
                    capture.Owner,
                    ViewportInputOwner.Overlay) &&
                blockedMove == default,
                $"foreign ownership validation round {i + 1} should remain intact.");

        for (var i = 0; i < 10; i++)
            Check(
                blockedRouter.CapturedOwner == ViewportInputOwner.None,
                $"blocked router ownership round {i + 1} should remain empty.");

        assert(
            round == 100,
            $"Input capture validation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
