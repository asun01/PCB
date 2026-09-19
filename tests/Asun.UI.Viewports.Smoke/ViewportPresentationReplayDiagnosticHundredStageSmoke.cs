using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportPresentationReplayDiagnosticHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        static ViewportPresentationRuntime<string> CreateRuntime()
        {
            return new ViewportPresentationRuntime<string>(
                new Vector2(1200, 900),
                new Vector2(400, 300),
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

            var snapshot =
                ViewportPresentationReplayDiagnosticRuntime.Capture(
                    runtime);

            Check(
                ViewportPresentationReplayDiagnosticRuntime.Validate(
                    snapshot).Count == 0 &&
                snapshot.DiagnosticHash.Length == 64 &&
                snapshot.StateFingerprint.StateHash.Length == 64,
                $"initial presentation diagnostic {i + 1} should validate.");
        }

        for (var i = 0; i < 10; i++)
        {
            using var first = CreateRuntime();
            using var second = CreateRuntime();

            var left =
                ViewportPresentationReplayDiagnosticRuntime.Capture(
                    first);
            var right =
                ViewportPresentationReplayDiagnosticRuntime.Capture(
                    second);

            Check(
                ViewportPresentationReplayDiagnosticRuntime.IsEquivalent(
                    left,
                    right),
                $"deterministic presentation diagnostic {i + 1} should match.");
        }

        for (var i = 0; i < 10; i++)
        {
            using var runtime = CreateRuntime();

            runtime.Submit(
                ViewportInputEventKind.Wheel,
                new Vector2(200, 150),
                120 + i);

            runtime.Continuous.ProcessInputs();

            var snapshot =
                ViewportPresentationReplayDiagnosticRuntime.Capture(
                    runtime);

            Check(
                snapshot.StateFingerprint.Generation > 0 &&
                snapshot.ReplayBundle.Inputs.Length == 1 &&
                snapshot.ReplayBundle.Inputs[0].Kind ==
                ViewportInputEventKind.Wheel,
                $"processed presentation input {i + 1} should appear in the diagnostic snapshot.");
        }

        for (var i = 0; i < 10; i++)
        {
            using var runtime = CreateRuntime();

            runtime.Submit(
                ViewportInputEventKind.Wheel,
                new Vector2(200, 150),
                120);

            runtime.Continuous.ProcessInputs();

            var snapshot =
                ViewportPresentationReplayDiagnosticRuntime.Capture(
                    runtime);

            Check(
                snapshot.Presentation.Generation ==
                snapshot.StateFingerprint.Generation,
                $"presentation/state generation {i + 1} should converge.");
        }

        for (var i = 0; i < 10; i++)
        {
            using var runtime = CreateRuntime();

            var snapshot =
                ViewportPresentationReplayDiagnosticRuntime.Capture(
                    runtime);

            Check(
                snapshot.ReplayBundle.Manifest.InputEventCount ==
                snapshot.ReplayBundle.Inputs.Length &&
                snapshot.ReplayBundle.Manifest.EvidenceManifestCount ==
                snapshot.ReplayBundle.Evidence.Length &&
                snapshot.ReplayBundle.Manifest.AuditEventCount ==
                snapshot.ReplayBundle.Audit.Length,
                $"diagnostic bundle counts {i + 1} should be coherent.");
        }

        for (var i = 0; i < 10; i++)
        {
            using var runtime = CreateRuntime();

            runtime.Submit(
                ViewportInputEventKind.PointerMove,
                new Vector2(50 + i, 60));

            runtime.Continuous.ProcessInputs();

            var snapshot =
                ViewportPresentationReplayDiagnosticRuntime.Capture(
                    runtime);

            Check(
                snapshot.ReplayBundle.Inputs.Count == 1 &&
                snapshot.ReplayBundle.Inputs[0].Position.X ==
                50 + i,
                $"pointer diagnostic capture {i + 1} should preserve input data.");
        }

        for (var i = 0; i < 10; i++)
        {
            using var runtime = CreateRuntime();

            runtime.Composite.AddRoi(
                RoiGeometry.CreateRectangle(
                    new Vector2(100 + i, 120),
                    new Vector2(40, 30)));

            var snapshot =
                ViewportPresentationReplayDiagnosticRuntime.Capture(
                    runtime);

            Check(
                snapshot.StateFingerprint.RoiCount == 1 &&
                snapshot.StateFingerprint.StateHash.Length == 64,
                $"ROI diagnostic capture {i + 1} should expose state.");
        }

        for (var i = 0; i < 10; i++)
        {
            using var runtime = CreateRuntime();

            var snapshot =
                ViewportPresentationReplayDiagnosticRuntime.Capture(
                    runtime);

            var validation =
                ViewportPresentationReplayDiagnosticRuntime.Validate(
                    snapshot);

            Check(
                validation.Count == 0 &&
                snapshot.Presentation.Generation >= 0 &&
                snapshot.Presentation.PendingInput >= 0,
                $"presentation diagnostic validity {i + 1} should hold.");
        }

        for (var i = 0; i < 10; i++)
        {
            using var runtime = CreateRuntime();

            runtime.Submit(
                ViewportInputEventKind.Wheel,
                new Vector2(200, 150),
                120);

            runtime.Continuous.ProcessInputs();

            var first =
                ViewportPresentationReplayDiagnosticRuntime.Capture(
                    runtime);

            var second =
                ViewportPresentationReplayDiagnosticRuntime.Capture(
                    runtime);

            Check(
                ViewportPresentationReplayDiagnosticRuntime.IsEquivalent(
                    first,
                    second),
                $"repeated presentation capture {i + 1} should be deterministic.");
        }

        assert(
            round == 100,
            $"Presentation replay diagnostic smoke should execute exactly 100 numbered rounds; actual {round}.");
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
