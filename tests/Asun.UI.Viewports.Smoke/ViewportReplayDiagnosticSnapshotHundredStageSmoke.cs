using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportReplayDiagnosticSnapshotHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var events = new[]
        {
            new ViewportInputEvent(
                1,
                ViewportInputEventKind.PointerMove,
                new Vector2(10, 20),
                0,
                ViewportMouseButton.Left),
            new ViewportInputEvent(
                2,
                ViewportInputEventKind.Wheel,
                new Vector2(20, 30),
                120,
                ViewportMouseButton.Left),
            new ViewportInputEvent(
                3,
                ViewportInputEventKind.PointerMove,
                new Vector2(30, 40),
                0,
                ViewportMouseButton.Left)
        };

        static ViewportCompositeInputRuntime<string> CreateInput()
        {
            var composite = new ViewportCompositeRuntime<string>(
                new Vector2(1200, 900),
                new Vector2(400, 300),
                new Vector2(100, 100),
                1,
                32,
                2,
                new LocalTileSource());

            return new ViewportCompositeInputRuntime<string>(
                composite);
        }

        static ViewportReplaySessionBundle CreateBundle(
            IReadOnlyList<ViewportInputEvent> inputs)
        {
            var manifest =
                ViewportReplaySessionBundleRuntime.CreateManifest(
                    "snapshot-test",
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

        static ViewportReplayDiagnosticSnapshot CreateSnapshot(
            IReadOnlyList<ViewportInputEvent> inputs)
        {
            var input = CreateInput();

            using (input.Composite)
            {
                var execution =
                    ViewportReplayExecutionStateRuntime.Execute(
                        input,
                        inputs);

                var bundle = CreateBundle(inputs);

                var checkpoint =
                    ViewportReplayCheckpointRuntime.Capture(
                        execution,
                        inputs);

                return ViewportReplayDiagnosticSnapshotRuntime.Capture(
                    execution,
                    bundle,
                    checkpoint);
            }
        }

        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        for (var i = 0; i < 10; i++)
        {
            var snapshot = CreateSnapshot(events);

            Check(
                ViewportReplayDiagnosticSnapshotRuntime.Validate(
                    snapshot).Count == 0 &&
                snapshot.DiagnosticHash.Length == 64,
                $"clean snapshot {i + 1} should validate.");
        }

        for (var i = 0; i < 10; i++)
        {
            var first = CreateSnapshot(events);
            var second = CreateSnapshot(events);

            Check(
                ViewportReplayDiagnosticSnapshotRuntime.AreEquivalent(
                    first,
                    second),
                $"deterministic snapshot {i + 1} should match.");
        }

        for (var i = 0; i < 10; i++)
        {
            var snapshot = CreateSnapshot(events);
            var mutated = snapshot with
            {
                Checkpoint =
                    snapshot.Checkpoint with
                    {
                        StateHash = new string((char)('a' + i), 64)
                    },
                DiagnosticHash = snapshot.DiagnosticHash
            };

            Check(
                ViewportReplayDiagnosticSnapshotRuntime.Validate(
                    mutated).Count > 0,
                $"checkpoint mutation {i + 1} should invalidate the snapshot.");
        }

        for (var i = 0; i < 10; i++)
        {
            var snapshot = CreateSnapshot(events);
            var invalid = snapshot with
            {
                Checkpoint =
                    snapshot.Checkpoint with
                    {
                        InputHash = new string((char)('b' + i), 64)
                    }
            };

            var rejected = false;

            try
            {
                ViewportReplayDiagnosticSnapshotRuntime.Capture(
                    invalid.Execution,
                    invalid.Bundle,
                    invalid.Checkpoint);
            }
            catch (InvalidOperationException)
            {
                rejected = true;
            }

            Check(
                rejected,
                $"checkpoint input mismatch {i + 1} should be rejected.");
        }

        for (var i = 0; i < 10; i++)
        {
            var snapshot = CreateSnapshot(events);
            var invalidBundle =
                snapshot.Bundle with
                {
                    Manifest =
                        snapshot.Bundle.Manifest with
                        {
                            InputHash =
                                new string((char)('c' + i), 64)
                        }
                };

            var rejected = false;

            try
            {
                ViewportReplayDiagnosticSnapshotRuntime.Capture(
                    snapshot.Execution,
                    invalidBundle,
                    snapshot.Checkpoint);
            }
            catch (InvalidOperationException)
            {
                rejected = true;
            }

            Check(
                rejected,
                $"bundle input mismatch {i + 1} should be rejected.");
        }

        for (var i = 0; i < 10; i++)
        {
            var snapshot = CreateSnapshot(events);

            var comparison =
                ViewportReplayDiagnosticSnapshotRuntime.Compare(
                    snapshot,
                    snapshot);

            Check(
                comparison.IsEquivalent &&
                comparison.Differences.Count == 0,
                $"self comparison {i + 1} should be exact.");
        }

        for (var i = 0; i < 10; i++)
        {
            var first = CreateSnapshot(events);
            var second = first with
            {
                DiagnosticHash = new string((char)('d' + i), 64)
            };

            var comparison =
                ViewportReplayDiagnosticSnapshotRuntime.Compare(
                    first,
                    second);

            Check(
                !comparison.IsEquivalent &&
                comparison.Differences.Any(
                    item => item.StartsWith(
                        "DiagnosticHash",
                        StringComparison.Ordinal)),
                $"diagnostic hash mismatch {i + 1} should be visible.");
        }

        for (var i = 0; i < 10; i++)
        {
            var snapshot = CreateSnapshot(events.Take(2).ToArray());

            Check(
                snapshot.Bundle.Manifest.InputEventCount == 2 &&
                snapshot.Checkpoint.InputCount == 2 &&
                snapshot.Execution.Execution.Count == 2,
                $"prefix snapshot {i + 1} should retain matching counts.");
        }

        for (var i = 0; i < 10; i++)
        {
            var snapshot = CreateSnapshot(events);

            Check(
                snapshot.Execution.Execution.InputHash ==
                snapshot.Bundle.Manifest.InputHash &&
                snapshot.Execution.Execution.InputHash ==
                snapshot.Checkpoint.InputHash &&
                snapshot.Execution.Execution.ResultHash ==
                snapshot.Checkpoint.ResultHash &&
                snapshot.Execution.FinalState.StateHash ==
                snapshot.Checkpoint.StateHash,
                $"cross-layer hashes {i + 1} should converge.");
        }

        for (var i = 0; i < 10; i++)
        {
            var snapshot = CreateSnapshot(Array.Empty<ViewportInputEvent>());

            Check(
                snapshot.Execution.Execution.Count == 0 &&
                snapshot.Checkpoint.IsEmpty &&
                snapshot.DiagnosticHash.Length == 64,
                $"empty snapshot {i + 1} should remain valid.");
        }

        assert(
            round == 100,
            $"Replay diagnostic snapshot smoke should execute exactly 100 numbered rounds; actual {round}.");
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
