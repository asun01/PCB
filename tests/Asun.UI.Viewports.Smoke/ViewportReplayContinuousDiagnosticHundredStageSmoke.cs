using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportReplayContinuousDiagnosticHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        static ViewportPresentationRuntime<string> CreateRuntime()
        {
            return new ViewportPresentationRuntime<string>(
                new Vector2(1600, 1200),
                new Vector2(500, 400),
                new Vector2(100, 100),
                1,
                32,
                2,
                new LocalTileSource());
        }

        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        for (var i = 0; i < 10; i++)
        {
            using var runtime = CreateRuntime();

            var facade =
                runtime.CreateReplayDiagnosticSnapshot();

            var direct =
                ViewportPresentationReplayDiagnosticRuntime.Capture(
                    runtime);

            Check(
                ViewportPresentationReplayDiagnosticRuntime.IsEquivalent(
                    facade,
                    direct),
                $"runtime facade {i + 1} should match the diagnostic runtime.");
        }

        for (var i = 0; i < 10; i++)
        {
            using var runtime = CreateRuntime();

            runtime.Submit(
                ViewportInputEventKind.Wheel,
                new Vector2(250, 200),
                120);

            runtime.Continuous.ProcessInputs();

            var snapshot =
                runtime.CreateReplayDiagnosticSnapshot();

            Check(
                snapshot.ReplayBundle.Inputs.Count == 1 &&
                snapshot.ReplayBundle.Inputs[0].WheelDelta == 120 &&
                snapshot.StateFingerprint.Generation > 0,
                $"processed runtime facade {i + 1} should expose input and state.");
        }

        for (var i = 0; i < 10; i++)
        {
            using var runtime = CreateRuntime();

            runtime.Submit(
                ViewportInputEventKind.PointerMove,
                new Vector2(100 + i, 150));

            runtime.Continuous.ProcessInputs();

            var snapshot =
                runtime.CreateReplayDiagnosticSnapshot();

            Check(
                snapshot.Presentation.Generation ==
                snapshot.StateFingerprint.Generation &&
                snapshot.ReplayBundle.Manifest.InputEventCount == 1,
                $"generation convergence {i + 1} should hold.");
        }

        for (var i = 0; i < 10; i++)
        {
            using var runtime = CreateRuntime();

            runtime.Composite.AddRoi(
                RoiGeometry.CreateRectangle(
                    new Vector2(100 + i, 120),
                    new Vector2(50, 40)));

            var snapshot =
                runtime.CreateReplayDiagnosticSnapshot();

            Check(
                snapshot.StateFingerprint.RoiCount == 1 &&
                snapshot.StateFingerprint.SelectedRoiId is not null,
                $"ROI facade capture {i + 1} should expose selection.");
        }

        for (var i = 0; i < 10; i++)
        {
            using var runtime = CreateRuntime();

            var snapshot =
                runtime.CreateReplayDiagnosticSnapshot();

            var errors =
                ViewportPresentationReplayDiagnosticRuntime.Validate(
                    snapshot);

            Check(
                errors.Count == 0 &&
                snapshot.DiagnosticHash.Length == 64,
                $"facade validation {i + 1} should be clean.");
        }

        for (var i = 0; i < 10; i++)
        {
            using var runtime = CreateRuntime();

            runtime.Submit(
                ViewportInputEventKind.Wheel,
                new Vector2(250, 200),
                120);

            runtime.Continuous.ProcessInputs();

            var first =
                runtime.CreateReplayDiagnosticSnapshot();
            var second =
                runtime.CreateReplayDiagnosticSnapshot();

            Check(
                ViewportPresentationReplayDiagnosticRuntime.IsEquivalent(
                    first,
                    second),
                $"repeated facade capture {i + 1} should be deterministic.");
        }

        for (var i = 0; i < 10; i++)
        {
            using var runtime = CreateRuntime();

            runtime.Submit(
                ViewportInputEventKind.Wheel,
                new Vector2(250, 200),
                120 + i);

            runtime.Continuous.ProcessInputs();

            var snapshot =
                runtime.CreateReplayDiagnosticSnapshot();

            var exported =
                ViewportReplayDiagnosticManifestRuntime.Create(
                    ViewportReplayDiagnosticSnapshotRuntime.Capture(
                        snapshot.StateFingerprint is not null
                            ? CreateExecutionFromFacade(
                                snapshot,
                                runtime)
                            : throw new InvalidOperationException(),
                        snapshot.ReplayBundle,
                        ViewportReplayCheckpointRuntime.Capture(
                            CreateExecutionFromFacade(
                                snapshot,
                                runtime),
                            snapshot.ReplayBundle.Inputs)));

            Check(
                exported.FormatVersion ==
                ViewportReplayDiagnosticManifest.CurrentFormatVersion,
                $"facade export {i + 1} should preserve manifest version.");
        }

        for (var i = 0; i < 10; i++)
        {
            using var runtime = CreateRuntime();

            runtime.Submit(
                ViewportInputEventKind.PointerMove,
                new Vector2(20 + i, 30));

            runtime.Continuous.ProcessInputs();

            var snapshot =
                runtime.CreateReplayDiagnosticSnapshot();

            Check(
                snapshot.ReplayBundle.Inputs[0].Position.X == 20 + i &&
                snapshot.ReplayBundle.Manifest.InputHash.Length == 64,
                $"input evidence {i + 1} should remain deterministic.");
        }

        for (var i = 0; i < 10; i++)
        {
            using var runtime = CreateRuntime();

            var before =
                runtime.CreateReplayDiagnosticSnapshot();

            runtime.Composite.FitToViewport();

            var after =
                runtime.CreateReplayDiagnosticSnapshot();

            Check(
                ViewportPresentationReplayDiagnosticRuntime.Validate(
                    before).Count == 0 &&
                ViewportPresentationReplayDiagnosticRuntime.Validate(
                    after).Count == 0 &&
                after.StateFingerprint.StateHash.Length == 64,
                $"navigation diagnostic {i + 1} should remain valid.");
        }

        for (var i = 0; i < 10; i++)
        {
            using var runtime = CreateRuntime();

            var snapshot =
                runtime.CreateReplayDiagnosticSnapshot();

            runtime.Reset();

            var reset =
                runtime.CreateReplayDiagnosticSnapshot();

            Check(
                snapshot.ReplayBundle.Manifest.InputEventCount == 0 &&
                reset.ReplayBundle.Manifest.InputEventCount == 0 &&
                reset.StateFingerprint.StateHash.Length == 64,
                $"reset diagnostic {i + 1} should remain valid.");
        }

        assert(
            round == 100,
            $"Continuous diagnostic facade smoke should execute exactly 100 numbered rounds; actual {round}.");
    }

    private static ViewportReplayExecutionStateReport
        CreateExecutionFromFacade(
            ViewportPresentationReplayDiagnosticSnapshot snapshot,
            ViewportPresentationRuntime<string> runtime)
    {
        var bundle = snapshot.ReplayBundle;
        return ViewportReplayExecutionStateRuntime.Execute(
            new ViewportCompositeInputRuntime<string>(
                runtime.Composite),
            bundle.Inputs);
    }

    private sealed class LocalTileSource : ITileSource<string>
    {
        public ValueTask<string> LoadAsync(
            TileRequest request,
            System.Drawing.RectangleF imageRectangle,
            CancellationToken cancellationToken = default) =>
            ValueTask.FromResult(
                $"tile:{request.Index.X},{request.Index.Y}");
    }
}
