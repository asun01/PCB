using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportInputRouterValidationHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        using var viewport = new RoiViewportRuntime(
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

        var down = router.PointerDown(
            new Vector2(5, 5),
            ViewportMouseButton.Middle);

        for (var i = 0; i < 10; i++)
        {
            Check(
                down.Kind == ViewportGestureKind.Panning &&
                router.CapturedOwner == ViewportInputOwner.Pan &&
                ViewportInputRouterValidationRuntime.IsValid(
                    capture.Owner,
                    router.CapturedOwner),
                $"middle-button routing round {i + 1} should own Pan capture.");
        }

        var move = router.PointerMove(new Vector2(12, 8));

        for (var i = 0; i < 10; i++)
        {
            Check(
                move.Kind == ViewportGestureKind.Panning,
                $"captured pointer move round {i + 1} should remain a pan gesture.");
        }

        var up = router.PointerUp(new Vector2(12, 8));

        for (var i = 0; i < 10; i++)
        {
            Check(
                up.Kind == ViewportGestureKind.Panning &&
                ViewportInputRouterValidationRuntime.IsReleased(
                    capture.Owner,
                    router.CapturedOwner),
                $"owned pointer-up round {i + 1} should release both router and capture.");
        }

        Check(
            router.PointerUp(new Vector2(12, 8)) == default &&
            capture.Owner == ViewportInputOwner.None,
            "unowned pointer-up should be inert.");

        Check(
            !router.Escape(new Vector2(12, 8)) &&
            capture.Owner == ViewportInputOwner.None,
            "unowned Escape should not affect capture.");

        Check(
            round == 100,
            $"Input router validation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
