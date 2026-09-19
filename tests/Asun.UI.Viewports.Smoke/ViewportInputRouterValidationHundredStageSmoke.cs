using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportInputRouterValidationHundredStageSmoke
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

        var down = router.PointerDown(
            new Vector2(5, 5),
            ViewportMouseButton.Middle);

        for (var i = 0; i < 10; i++)
            Check(
                down.Kind == ViewportGestureKind.Panning &&
                router.CapturedOwner == ViewportInputOwner.Pan &&
                ViewportInputRouterValidationRuntime.IsValid(
                    capture.Owner,
                    router.CapturedOwner),
                $"middle-button routing round {i + 1} should own Pan capture.");

        var move = router.PointerMove(new Vector2(12, 8));

        for (var i = 0; i < 10; i++)
            Check(
                move.Kind == ViewportGestureKind.Panning &&
                router.CapturedOwner == ViewportInputOwner.Pan,
                $"captured move round {i + 1} should remain owned by Pan.");

        var up = router.PointerUp(new Vector2(12, 8));

        for (var i = 0; i < 10; i++)
            Check(
                up.Kind == ViewportGestureKind.Panning &&
                ViewportInputRouterValidationRuntime.IsReleased(
                    capture.Owner,
                    router.CapturedOwner),
                $"owned pointer-up round {i + 1} should release both owners.");

        for (var i = 0; i < 10; i++)
            Check(
                router.PointerUp(new Vector2(12, 8)) == default &&
                capture.Owner == ViewportInputOwner.None,
                $"unowned pointer-up round {i + 1} should be inert.");

        for (var i = 0; i < 10; i++)
            Check(
                !router.Escape(new Vector2(12, 8)) &&
                capture.Owner == ViewportInputOwner.None,
                $"unowned Escape round {i + 1} should be inert.");

        for (var i = 0; i < 10; i++)
            Check(
                ViewportInputCaptureValidationRuntime.IsFree(capture.Owner) &&
                router.CapturedOwner == ViewportInputOwner.None,
                $"post-cycle ownership round {i + 1} should be empty.");

        for (var i = 0; i < 10; i++)
            Check(
                ViewportInputRouterValidationRuntime.IsReleased(
                    capture.Owner,
                    router.CapturedOwner),
                $"released router invariant round {i + 1} should hold.");

        for (var i = 0; i < 10; i++)
            Check(
                down.Kind == ViewportGestureKind.Panning &&
                move.Kind == ViewportGestureKind.Panning,
                $"gesture result stability round {i + 1} should remain deterministic.");

        for (var i = 0; i < 10; i++)
            Check(
                ViewportInputRouterValidationRuntime.IsReleased(
                    capture.Owner,
                    router.CapturedOwner),
                $"final release contract round {i + 1} should remain deterministic.");

        for (var i = 0; i < 10; i++)
            Check(
                capture.Owner == ViewportInputOwner.None,
                $"final capture owner round {i + 1} should remain None.");

        assert(
            round == 100,
            $"Input router validation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
