using System.Drawing;
using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportReplayDiagnosticGateHundredStageSmoke
{
    private static readonly Guid StableRoiId =
        Guid.Parse("11111111-2222-3333-4444-555555555555");

    public static void Run(Action<bool, string> assert)
    {
        var events = new[]
        {
            new ViewportInputEvent(1, ViewportInputEventKind.PointerMove, new Vector2(40, 50), 0, ViewportMouseButton.Left),
            new ViewportInputEvent(2, ViewportInputEventKind.PointerDown, new Vector2(50, 60), 0, ViewportMouseButton.Left),
            new ViewportInputEvent(3, ViewportInputEventKind.PointerMove, new Vector2(90, 100), 0, ViewportMouseButton.Left),
            new ViewportInputEvent(4, ViewportInputEventKind.PointerUp, new Vector2(90, 100), 0, ViewportMouseButton.Left)
        };

        static ViewportCompositeInputRuntime<string> CreateInput()
        {
            var composite = new ViewportCompositeRuntime<string>(
                new Vector2(1400, 1000),
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

        static ViewportReplaySessionBundle CreateBundle(
            IReadOnlyList<ViewportInputEvent> inputEvents)
        {
            var evidence =
                new[]
                {
                    new ViewportRenderEvidenceManifest(
                        1,
                        1,
                        ViewportDirtyFlags.Image,
                        2,
                        1,
                        1,
                        1,
                        0,
                        0,
                        0,
                        2,
                        2,
                        0,
                        new string('a', 64),
                        new string('b', 64),
                        new string('c', 64),
                        new string('d', 64))
                };

            var audit =
                new[]
                {
                    new ViewportPresentationAuditEvent(
                        1,
                        "Presented",
                        1,
                        1,
                        ViewportRenderDeliveryStatus.Succeeded,
                        2,
                        0,
                        evidence[0].StableKey)
                };

            var manifest =
                ViewportReplaySessionBundleRuntime.CreateManifest(
                    "diagnostic-gate",
                    DateTimeOffset.UnixEpoch,
                    inputEvents,
                    evidence,
                    audit);

            return ViewportReplaySessionBundleRuntime.Capture(
                manifest,
                inputEvents,
                evidence,
                audit);
        }

        static (
            ViewportReplayExecutionStateReport Execution,
            ViewportReplaySessionBundle Bundle,
            ViewportReplayCheckpoint Checkpoint)
            CreateScenario(
                IReadOnlyList<ViewportInputEvent> inputEvents)
        {
            var input = CreateInput();

            var report =
                ViewportReplayExecutionStateRuntime.Execute(
                    input,
                    inputEvents);

            var bundle = CreateBundle(inputEvents);

            var checkpoint =
                ViewportReplayCheckpointRuntime.Capture(
                    report,
                    inputEvents);

            input.Composite.Dispose();

            return (report, bundle, checkpoint);
        }

        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        for (var i = 0; i < 10; i++)
        {
            var expected = CreateScenario(events);
            var actual = CreateScenario(events);

            var result =
                ViewportReplayDiagnosticGateRuntime.Verify(
                    expected.Execution,
                    actual.Execution,
                    expected.Bundle,
                    actual.Bundle,
                    expected.Checkpoint,
                    actual.Checkpoint);

            Check(
                result.IsValid &&
                ViewportReplayDiagnosticGateRuntime.IsValid(result),
                $"clean diagnostic gate {i + 1} should pass.");
        }

        for (var i = 0; i < 10; i++)
        {
            var expected = CreateScenario(events);
            var changedEvents =
                events.Select(item =>
                    item.Sequence == 2
                        ? item with
                        {
                            Position =
                                item.Position +
                                new Vector2(i + 1, 0)
                        }
                        : item).ToArray();

            var actual = CreateScenario(changedEvents);

            var result =
                ViewportReplayDiagnosticGateRuntime.Verify(
                    expected.Execution,
                    actual.Execution,
                    expected.Bundle,
                    actual.Bundle,
                    expected.Checkpoint,
                    actual.Checkpoint);

            Check(
                !result.IsValid &&
                !result.ExecutionMatches &&
                !result.BundleMatches &&
                result.Differences.Count > 0,
                $"input mutation {i + 1} should fail the integrated gate.");
        }

        for (var i = 0; i < 10; i++)
        {
            var expected = CreateScenario(events);
            var actual = CreateScenario(events);

            var mutatedState =
                actual.Execution with
                {
                    FinalState =
                        actual.Execution.FinalState with
                        {
                            StateHash =
                                new string((char)('a' + i), 64)
                        }
                };

            var result =
                ViewportReplayDiagnosticGateRuntime.Verify(
                    expected.Execution,
                    mutatedState,
                    expected.Bundle,
                    actual.Bundle,
                    expected.Checkpoint,
                    actual.Checkpoint);

            Check(
                !result.IsValid &&
                result.ExecutionMatches &&
                !result.StateMatches &&
                result.Differences.Any(
                    item => item.StartsWith(
                        "State.",
                        StringComparison.Ordinal)),
                $"state mutation {i + 1} should be isolated.");
        }

        for (var i = 0; i < 10; i++)
        {
            var expected = CreateScenario(events);
            var actual = CreateScenario(events);

            var actualBundle =
                actual.Bundle with
                {
                    Audit = new[]
                    {
                        actual.Bundle.Audit[0] with
                        {
                            Stage = $"Changed-{i}"
                        }
                    }
                };

            actualBundle = actualBundle with
            {
                Manifest =
                    ViewportReplaySessionBundleRuntime.CreateManifest(
                        actualBundle.Manifest.SessionId,
                        actualBundle.Manifest.CreatedAtUtc,
                        actualBundle.Inputs,
                        actualBundle.Evidence,
                        actualBundle.Audit)
            };

            var result =
                ViewportReplayDiagnosticGateRuntime.Verify(
                    expected.Execution,
                    actual.Execution,
                    expected.Bundle,
                    actualBundle,
                    expected.Checkpoint,
                    actual.Checkpoint);

            Check(
                !result.IsValid &&
                !result.BundleMatches &&
                !result.PresentationEvidenceMatches &&
                result.Differences.Any(
                    item => item.StartsWith(
                        "Presentation.",
                        StringComparison.Ordinal)),
                $"presentation mutation {i + 1} should be isolated.");
        }

        for (var i = 0; i < 10; i++)
        {
            var expected = CreateScenario(events);
            var actual = CreateScenario(events);

            var mutatedCheckpoint =
                actual.Checkpoint with
                {
                    Generation =
                        actual.Checkpoint.Generation + i + 1
                };

            var result =
                ViewportReplayDiagnosticGateRuntime.Verify(
                    expected.Execution,
                    actual.Execution,
                    expected.Bundle,
                    actual.Bundle,
                    expected.Checkpoint,
                    mutatedCheckpoint);

            Check(
                !result.IsValid &&
                !result.CheckpointMatches &&
                result.Differences.Any(
                    item => item.StartsWith(
                        "Checkpoint.",
                        StringComparison.Ordinal)),
                $"checkpoint mutation {i + 1} should be isolated.");
        }

        for (var i = 0; i < 10; i++)
        {
            var expected = CreateScenario(events);
            var actual = CreateScenario(events);

            var evidence =
                actual.Bundle.Evidence[0] with
                {
                    ReplayHash =
                        new string((char)('z' - i), 64)
                };

            var actualBundle = actual.Bundle with
            {
                Evidence = new[] { evidence }
            };

            actualBundle = actualBundle with
            {
                Manifest =
                    ViewportReplaySessionBundleRuntime.CreateManifest(
                        actualBundle.Manifest.SessionId,
                        actualBundle.Manifest.CreatedAtUtc,
                        actualBundle.Inputs,
                        actualBundle.Evidence,
                        actualBundle.Audit)
            };

            var result =
                ViewportReplayDiagnosticGateRuntime.Verify(
                    expected.Execution,
                    actual.Execution,
                    expected.Bundle,
                    actualBundle,
                    expected.Checkpoint,
                    actual.Checkpoint);

            Check(
                !result.IsValid &&
                !result.BundleMatches &&
                !result.PresentationEvidenceMatches,
                $"evidence mutation {i + 1} should fail both bundle and presentation gates.");
        }

        for (var i = 0; i < 10; i++)
        {
            var scenario = CreateScenario(events);
            var result =
                ViewportReplayDiagnosticGateRuntime.Verify(
                    scenario.Execution,
                    scenario.Execution,
                    scenario.Bundle,
                    scenario.Bundle,
                    scenario.Checkpoint,
                    scenario.Checkpoint);

            Check(
                result.IsValid &&
                result.Differences.Count == 0,
                $"self diagnostic verification {i + 1} should be exact.");
        }

        for (var i = 0; i < 10; i++)
        {
            var input = CreateInput();

            using (input.Composite)
            {
                var report =
                    ViewportReplayExecutionStateRuntime.Execute(
                        input,
                        Array.Empty<ViewportInputEvent>());

                var bundle =
                    CreateBundle(Array.Empty<ViewportInputEvent>());

                var checkpoint =
                    ViewportReplayCheckpointRuntime.Capture(
                        report);

                var result =
                    ViewportReplayDiagnosticGateRuntime.Verify(
                        report,
                        report,
                        bundle,
                        bundle,
                        checkpoint,
                        checkpoint);

                Check(
                    result.IsValid &&
                    !report.StateChanged,
                    $"empty diagnostic gate {i + 1} should pass.");
            }
        }

        for (var i = 0; i < 10; i++)
        {
            var expected = CreateScenario(events);
            var actual = CreateScenario(events);

            var jsonExpected =
                ViewportReplaySessionBundleRuntime.FromJson(
                    ViewportReplaySessionBundleRuntime.ToJson(
                        expected.Bundle));

            var jsonActual =
                ViewportReplaySessionBundleRuntime.FromJson(
                    ViewportReplaySessionBundleRuntime.ToJson(
                        actual.Bundle));

            var result =
                ViewportReplayDiagnosticGateRuntime.Verify(
                    expected.Execution,
                    actual.Execution,
                    jsonExpected,
                    jsonActual,
                    expected.Checkpoint,
                    actual.Checkpoint);

            Check(
                result.IsValid,
                $"JSON-restored diagnostic scenario {i + 1} should pass.");
        }

        for (var i = 0; i < 10; i++)
        {
            var expected = CreateScenario(events);
            var actual = CreateScenario(events);

            var checkpoint =
                actual.Checkpoint with
                {
                    StateHash =
                        new string((char)('m' + i), 64)
                };

            var result =
                ViewportReplayDiagnosticGateRuntime.Verify(
                    expected.Execution,
                    actual.Execution,
                    expected.Bundle,
                    actual.Bundle,
                    expected.Checkpoint,
                    checkpoint);

            Check(
                !result.IsValid &&
                !result.CheckpointMatches &&
                result.ExecutionMatches &&
                result.StateMatches,
                $"checkpoint state-hash mutation {i + 1} should be isolated.");
        }

        assert(
            round == 100,
            $"Replay diagnostic gate smoke should execute exactly 100 numbered rounds; actual {round}.");
    }

    private sealed class LocalTileSource : ITileSource<string>
    {
        public ValueTask<string> LoadAsync(
            TileRequest request,
            RectangleF imageRectangle,
            CancellationToken cancellationToken = default) =>
            ValueTask.FromResult(
                $"tile:{request.Index.X},{request.Index.Y}");
    }
}
