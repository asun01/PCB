using System.Drawing;
using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportNavigationFeatureSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var viewportSize = new Vector2(300, 200);

        var leftDelta = ViewportAutoPanRuntime.GetDelta(
            new Vector2(5, 100),
            viewportSize,
            edgePixels: 50,
            speed: 20);

        var rightDelta = ViewportAutoPanRuntime.GetDelta(
            new Vector2(295, 100),
            viewportSize,
            edgePixels: 50,
            speed: 20);

        assert(
            leftDelta.X < 0 &&
            rightDelta.X > 0,
            "Auto-pan should produce directional horizontal deltas near both viewport edges.");

        var topDelta = ViewportAutoPanRuntime.GetDelta(
            new Vector2(150, 5),
            viewportSize,
            edgePixels: 50,
            speed: 20);

        var bottomDelta = ViewportAutoPanRuntime.GetDelta(
            new Vector2(150, 195),
            viewportSize,
            edgePixels: 50,
            speed: 20);

        assert(
            topDelta.Y < 0 &&
            bottomDelta.Y > 0,
            "Auto-pan should produce directional vertical deltas near both viewport edges.");

        var inertial = new ViewportInertialPanRuntime();
        inertial.SetVelocity(new Vector2(100, 50));
        var displacement = inertial.Step(0.1f);

        assert(
            displacement == new Vector2(10, 5) &&
            inertial.Velocity.Length() < new Vector2(100, 50).Length(),
            "Inertial panning should integrate displacement and apply damping.");

        inertial.Stop();

        assert(
            inertial.Velocity == Vector2.Zero &&
            inertial.Step(0.1f) == Vector2.Zero,
            "Stopping inertial panning should clear all subsequent displacement.");

        var gate = new ViewportFrameRateGate(60);
        var now = DateTimeOffset.UnixEpoch.AddSeconds(1);

        assert(
            gate.TryEnter(now) &&
            !gate.TryEnter(now.AddMilliseconds(1)) &&
            gate.GetDelay(now.AddMilliseconds(1)) > TimeSpan.Zero,
            "Frame-rate gate should admit the first frame and throttle an early second frame.");

        gate.Reset();

        assert(
            gate.TryEnter(now.AddMilliseconds(2)),
            "Frame-rate gate reset should permit a new frame immediately.");

        var transform = ViewportTransform.Fit(
            new Vector2(1000, 500),
            viewportSize);

        assert(
            ViewportKeyboardNavigationRuntime.Apply(
                transform,
                ViewportNavigationKey.Home) == transform.ResetToFit() &&
            ViewportKeyboardNavigationRuntime.Apply(
                transform,
                ViewportNavigationKey.End).ImagePointAtViewportCenter ==
                transform.ImageCenter,
            "Keyboard Home and End navigation should map to Fit and ImageCenter semantics.");

        var miniMap = ViewportMiniMapRuntime.CreateSnapshot(
            transform,
            new Vector2(120, 80));

        var miniMapCenter = miniMap.MiniMapTransform.ImageToViewport(
            transform.ImageCenter);

        var centered = ViewportMiniMapRuntime.CenterMainViewportOnMiniMapPoint(
            transform,
            miniMap,
            miniMapCenter);

        assert(
            centered.ImagePointAtViewportCenter == transform.ImageCenter,
            "Mini-map center navigation should preserve the selected image center.");

        var history = new ViewportNavigationHistory(transform, capacity: 3);
        var translated = transform.PanBy(new Vector2(20, 0));
        var zoomed = translated.WithZoomFactor(2, translated.ViewportCenter);

        history.Record(translated);
        history.Record(zoomed);

        assert(
            history.CanBack &&
            history.Back(out var backTransform) &&
            backTransform == translated &&
            history.Forward(out var forwardTransform) &&
            forwardTransform == zoomed,
            "Navigation history should preserve back/forward transform ordering.");

        history.Clear();

        assert(
            !history.CanBack &&
            !history.CanForward &&
            history.Current == zoomed,
            "Clearing navigation history should preserve the current transform only.");

        var clamped = ViewportPanPolicyRuntime.Apply(
            transform,
            new Vector2(100000, -100000),
            new ViewportPanPolicy(
                ClampToImage: true,
                AllowOverscroll: false,
                OverscrollPixels: 0));

        assert(
            transform.WithTranslationClamped(
                new Vector2(100000, -100000)).Translation == clamped,
            "Pan policy without overscroll should match the transform's canonical clamp.");

        var overscrolled = ViewportPanPolicyRuntime.Apply(
            transform,
            new Vector2(100000, -100000),
            new ViewportPanPolicy(
                ClampToImage: true,
                AllowOverscroll: true,
                OverscrollPixels: 12));

        assert(
            Vector2.Distance(
                overscrolled,
                transform.WithTranslationClamped(
                    new Vector2(100000, -100000)).Translation) <= 12.0001f,
            "Pan policy overscroll should remain inside the configured overscroll radius.");

        var pointer = new ViewportPointerCoalescer();
        pointer.Submit(new Vector2(1, 1));
        pointer.Submit(new Vector2(2, 2));
        pointer.Submit(new Vector2(3, 3));

        assert(
            pointer.HasPending &&
            pointer.SubmittedCount == 3 &&
            pointer.CoalescedCount == 2 &&
            pointer.TryTakeLatest(out var latest) &&
            latest.Position == new Vector2(3, 3) &&
            !pointer.HasPending,
            "Pointer coalescing should retain only the latest position while preserving sequence counters.");

        pointer.Clear();

        var profile = ViewportZoomProfileRuntime.Default;
        var stepped = ViewportZoomProfileRuntime.StepScale(
            1,
            120,
            profile);

        assert(
            stepped > 1 &&
            stepped <= profile.MaximumScale,
            "Zoom profile stepping should apply one wheel step inside the validated scale range.");

        var usable = ViewportTransformGuardRuntime.IsUsable(transform);
        var invalid = transform with { Scale = double.NaN };

        assert(
            usable &&
            !ViewportTransformGuardRuntime.IsUsable(invalid),
            "Viewport transform guard should distinguish finite usable transforms from invalid scale state.");

        var visible = transform.GetVisibleImageRectangle();
        var imagePoint = new Vector2(250, 125);

        assert(
            transform.TryImageToViewport(imagePoint, out var viewportPoint) &&
            transform.TryViewportToImage(viewportPoint, out var roundTrip) &&
            Vector2.Distance(roundTrip, imagePoint) < 1e-4f &&
            transform.ContainsViewportPoint(viewportPoint) &&
            RectangleF.Contains(
                visible,
                imagePoint.X,
                imagePoint.Y),
            "Transform conversion helpers should round-trip finite points consistently.");
    }
}
