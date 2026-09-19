using System.Drawing;
using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportReplayStateFingerprintHundredStageSmoke
{
    private static readonly Guid StableRoiId =
        Guid.Parse("11111111-2222-3333-4444-555555555555");

    public static void Run(Action<bool, string> assert)
    {
        var events = new[]
        {
            new ViewportInputEvent(1, ViewportInputEventKind.PointerMove, new Vector2(80, 90), 0, ViewportMouseButton.Left),
            new ViewportInputEvent(2, ViewportInputEventKind.PointerDown, new Vector2(80, 90), 0, ViewportMouseButton.Left),
            new ViewportInputEvent(3, ViewportInputEventKind.PointerMove, new Vector2(120, 130), 0, ViewportMouseButton.Left),
            new ViewportInputEvent(4, ViewportInputEventKind.PointerUp, new Vector2(120, 130), 0, ViewportMouseButton.Left),
            new ViewportInputEvent(5, ViewportInputEventKind.Wheel, new Vector2(140, 140), 120, ViewportMouseButton.Left),
            new ViewportInputEvent(6, ViewportInputEventKind.PointerDown, new Vector2(140, 140), 0, ViewportMouseButton.Middle),
            new ViewportInputEvent(7, ViewportInputEventKind.PointerMove, new Vector2(150, 145), 0, ViewportMouseButton.Middle),
            new ViewportInputEvent(8, ViewportInputEventKind.PointerUp, new Vector2(150, 145), 0, ViewportMouseButton.Middle)
        };

        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        static ViewportCompositeInputRuntime<string> CreateInput()
        {
            var composite = new ViewportCompositeRuntime<string>(
                new Vector2(1600, 1200),
                new Vector2(500, 400),
                new Vector2(100, 100),
                1,
                64,
                4,
                new LocalTileSource());

            composite.AddRoi(
                RoiGeometry.CreateRectangle(
                    new Vector2(180, 160),
                    new Vector2(80, 60)),
                StableRoiId);

            return new ViewportCompositeInputRuntime<string>(composite);
        }

        for (var i = 0; i < 10; i++)
        {
            var first = CreateInput();
            var second = CreateInput();

            using (first.Composite)
            using (second.Composite)
            {
                var left = ViewportReplayExecutionStateRuntime.Execute(
                    first,
                    events);

                var right = ViewportReplayExecutionStateRuntime.Execute(
                    second,
                    events);

                Check(
                    ViewportReplayExecutionRuntime.HasSameInput(
                        left.Execution,
                        right.Execution) &&
                    ViewportReplayExecutionRuntime.HasSameResult(
                        left.Execution,
                        right.Execution) &&
                    ViewportReplayExecutionStateRuntime.HasSameFinalState(
                        left,
                        right) &&
                    left.FinalStateHash == right.FinalStateHash,
                    $"deterministic state execution {i + 1} should match.");
            }
        }

        for (var i = 0; i < 10; i++)
        {
            var input = CreateInput();

            using (input.Composite)
            {
                var report =
                    ViewportReplayExecutionStateRuntime.Execute(
                        input,
                        events);

                Check(
                    report.StateChanged &&
                    report.InitialState.StateHash.Length == 64 &&
                    report.FinalState.StateHash.Length == 64 &&
                    report.Execution.FinalGeneration >=
                    report.Execution.InitialGeneration,
                    $"state metrics {i + 1} should be complete.");
            }
        }

        for (var i = 0; i < 10; i++)
        {
            var first = CreateInput();
            var second = CreateInput();

            using (first.Composite)
            using (second.Composite)
            {
                var left =
                    ViewportReplayExecutionStateRuntime.Execute(
                        first,
                        events);

                var right =
                    ViewportReplayExecutionStateRuntime.Execute(
                        second,
                        events);

                second.Composite.PanBy(
                    new Vector2(i + 1, 0));

                var comparison =
                    ViewportReplayExecutionStateRuntime.CompareFinalState(
                        left,
                        right);

                Check(
                    !comparison.IsEquivalent &&
                    comparison.Differences.Count > 0 &&
                    comparison.Differences.Any(
                        item => item.StartsWith(
                            "Transform.",
                            StringComparison.Ordinal)),
                    $"state mutation detection {i + 1} should identify transform differences.");
            }
        }

        for (var i = 0; i < 10; i++)
        {
            var input = CreateInput();

            using (input.Composite)
            {
                var initial =
                    ViewportReplayStateFingerprintRuntime.Capture(
                        input.Composite);

                var empty =
                    ViewportReplayExecutionStateRuntime.Execute(
                        input,
                        Array.Empty<ViewportInputEvent>());

                Check(
                    ViewportReplayStateFingerprintRuntime.AreEquivalent(
                        initial,
                        empty.FinalState) &&
                    empty.Execution.Count == 0,
                    $"empty state execution {i + 1} should preserve state.");
            }
        }

        for (var i = 0; i < 10; i++)
        {
            var input = CreateInput();

            using (input.Composite)
            {
                var fingerprint =
                    ViewportReplayStateFingerprintRuntime.Capture(
                        input.Composite);

                Check(
                    fingerprint.RoiCount == 1 &&
                    fingerprint.SelectedRoiId == StableRoiId &&
                    fingerprint.RoiDocument.Mode == RoiEditorMode.Select &&
                    fingerprint.StateHash.Length == 64,
                    $"fingerprint metadata {i + 1} should expose stable ROI identity.");
            }
        }

        for (var i = 0; i < 10; i++)
        {
            var input = CreateInput();

            using (input.Composite)
            {
                var before =
                    ViewportReplayStateFingerprintRuntime.Capture(
                        input.Composite);

                input.Composite.TranslateSelected(
                    new Vector2(i + 1, i + 2));

                var after =
                    ViewportReplayStateFingerprintRuntime.Capture(
                        input.Composite);

                var comparison =
                    ViewportReplayStateFingerprintRuntime.Compare(
                        before,
                        after);

                Check(
                    !comparison.IsEquivalent &&
                    comparison.Differences.Any(
                        item => item.Contains(
                            "Geometry",
                            StringComparison.Ordinal)) &&
                    before.StateHash != after.StateHash,
                    $"ROI geometry mutation {i + 1} should change the fingerprint.");
            }
        }

        for (var i = 0; i < 10; i++)
        {
            var input = CreateInput();

            using (input.Composite)
            {
                var before =
                    ViewportReplayStateFingerprintRuntime.Capture(
                        input.Composite);

                input.Composite.ZoomAt(
                    1.01 + i * 0.001,
                    0.01,
                    20,
                    input.Composite.Transform.ViewportCenter);

                var after =
                    ViewportReplayStateFingerprintRuntime.Capture(
                        input.Composite);

                Check(
                    !ViewportReplayStateFingerprintRuntime.AreEquivalent(
                        before,
                        after) &&
                    before.Transform != after.Transform,
                    $"zoom mutation {i + 1} should change transform state.");
            }
        }

        for (var i = 0; i < 10; i++)
        {
            var input = CreateInput();

            using (input.Composite)
            {
                var before =
                    ViewportReplayStateFingerprintRuntime.Capture(
                        input.Composite);

                input.Composite.SelectRoi(null);

                var after =
                    ViewportReplayStateFingerprintRuntime.Capture(
                        input.Composite);

                var comparison =
                    ViewportReplayStateFingerprintRuntime.Compare(
                        before,
                        after);

                Check(
                    !comparison.IsEquivalent &&
                    comparison.Differences.Any(
                        item => item == "Roi.SelectedId"),
                    $"selection mutation {i + 1} should identify selected ROI changes.");
            }
        }

        for (var i = 0; i < 10; i++)
        {
            var input = CreateInput();

            using (input.Composite)
            {
                var report =
                    ViewportReplayExecutionStateRuntime.Execute(
                        input,
                        events);

                var captured =
                    ViewportReplayStateFingerprintRuntime.Capture(
                        input.Composite);

                Check(
                    ViewportReplayStateFingerprintRuntime.AreEquivalent(
                        report.FinalState,
                        captured) &&
                    report.FinalStateHash == captured.StateHash,
                    $"captured final state {i + 1} should match the execution report.");
            }
        }

        for (var i = 0; i < 10; i++)
        {
            var first = CreateInput();

            using (first.Composite)
            {
                var report =
                    ViewportReplayExecutionStateRuntime.Execute(
                        first,
                        events);

                var bundle = CreateBundle(events);
                var second = CreateInput();

                using (second.Composite)
                {
                    var bundledExecution =
                        ViewportReplayExecutionRuntime.Execute(
                            second,
                            bundle);

                    var bundledState =
                        ViewportReplayStateFingerprintRuntime.Capture(
                            second.Composite);

                    Check(
                        ViewportReplayExecutionRuntime.HasSameResult(
                            report.Execution,
                            bundledExecution) &&
                        report.FinalStateHash ==
                        bundledState.StateHash,
                        $"bundle and direct execution {i + 1} should converge to the same state.");
                }
            }
        }

        assert(
            round == 100,
            $"Replay state fingerprint smoke should execute exactly 100 numbered rounds; actual {round}.");
    }

    private static ViewportReplaySessionBundle CreateBundle(
        IReadOnlyList<ViewportInputEvent> inputs)
    {
        var manifest = ViewportReplaySessionBundleRuntime.CreateManifest(
            "state-fingerprint-test",
            DateTimeOffset.UnixEpoch,
            inputs,
            Array.Empty<ViewportRenderEvidenceManifest>(),
            Array.Empty<ViewportPresentationAuditEvent>());

        return ViewportReplaySessionBundleRuntime.Capture(
            manifest,
            inputs,
            Array.Empty<ViewportRenderEvidenceManifest>(),
            Array.Empty<ViewportPresentationAuditEvent>());
    }

    private sealed class LocalTileSource : ITileSource<string>
    {
        public ValueTask<string> LoadAsync(
            TileRequest request,
            RectangleF imageRectangle,
            CancellationToken cancellationToken = default) =>
            ValueTask.FromResult(
                $"tile:{request.Index.X},{request.Index.Y}:{imageRectangle.Width:0.###}x{imageRectangle.Height:0.###}");
    }
}
