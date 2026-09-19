using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportReplayDiagnosticIntegrityHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        static ViewportReplayDiagnosticSnapshot CreateSnapshot(
            IReadOnlyList<ViewportInputEvent> inputs)
        {
            var composite = new ViewportCompositeRuntime<string>(
                new Vector2(1200, 900),
                new Vector2(400, 300),
                new Vector2(100, 100),
                1,
                32,
                2,
                new LocalTileSource());

            using (composite)
            {
                var input = new ViewportCompositeInputRuntime<string>(composite);
                var execution =
                    ViewportReplayExecutionStateRuntime.Execute(
                        input,
                        inputs);

                var bundle =
                    ViewportReplaySessionBundleRuntime.Capture(
                        ViewportReplaySessionBundleRuntime.CreateManifest(
                            "diagnostic-integrity",
                            DateTimeOffset.UnixEpoch,
                            inputs,
                            Array.Empty<ViewportRenderEvidenceManifest>(),
                            Array.Empty<ViewportPresentationAuditEvent>()),
                        inputs,
                        Array.Empty<ViewportRenderEvidenceManifest>(),
                        Array.Empty<ViewportPresentationAuditEvent>());

                var checkpoint =
                    ViewportReplayCheckpointRuntime.Capture(
                        execution);

                return ViewportReplayDiagnosticSnapshotRuntime.Capture(
                    execution,
                    bundle,
                    checkpoint);
            }
        }

        var events = new[]
        {
            new ViewportInputEvent(10, ViewportInputEventKind.PointerMove, new Vector2(10, 20), 0, ViewportMouseButton.Left),
            new ViewportInputEvent(30, ViewportInputEventKind.Wheel, new Vector2(20, 30), 120, ViewportMouseButton.Left)
        };

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
                ViewportReplayDiagnosticSnapshotRuntime.Validate(snapshot).Count == 0,
                $"clean diagnostic snapshot {i + 1} should validate.");
        }

        for (var i = 0; i < 10; i++)
        {
            var snapshot = CreateSnapshot(events);
            Check(
                snapshot.Checkpoint.LastInputSequence == 30 &&
                snapshot.Checkpoint.InputHash == snapshot.Execution.Execution.InputHash,
                $"diagnostic checkpoint sequence {i + 1} should preserve source identity.");
        }

        for (var i = 0; i < 10; i++)
        {
            var snapshot = CreateSnapshot(events);
            var manifest =
                ViewportReplayDiagnosticManifestRuntime.Create(snapshot);
            var restored =
                ViewportReplayDiagnosticManifestRuntime.FromJson(
                    ViewportReplayDiagnosticManifestRuntime.ToJson(manifest));

            Check(
                restored == manifest,
                $"diagnostic manifest roundtrip {i + 1} should be lossless.");
        }

        for (var i = 0; i < 10; i++)
        {
            var first = CreateSnapshot(events);
            var second = CreateSnapshot(events);

            Check(
                ViewportReplayDiagnosticSnapshotRuntime.AreEquivalent(
                    first,
                    second),
                $"diagnostic repeatability {i + 1} should be deterministic.");
        }

        for (var i = 0; i < 10; i++)
        {
            var snapshot = CreateSnapshot(events);
            var invalid = snapshot.Checkpoint with
            {
                LastInputSequence = 0
            };

            Check(
                ViewportReplayCheckpointRuntime.Validate(invalid).Count > 0,
                $"diagnostic checkpoint mutation {i + 1} should be visible.");
        }

        for (var i = 0; i < 10; i++)
        {
            var snapshot = CreateSnapshot(events);
            var invalid =
                snapshot.Bundle with
                {
                    Inputs = new[]
                    {
                        events[0] with { Sequence = 11 }
                    }
                };

            Check(
                ViewportReplaySessionBundleRuntime.Validate(invalid).Count > 0,
                $"diagnostic bundle mutation {i + 1} should be rejected.");
        }

        for (var i = 0; i < 10; i++)
        {
            var snapshot = CreateSnapshot(events);
            var manifest =
                ViewportReplayDiagnosticManifestRuntime.Create(snapshot);

            Check(
                ViewportReplayDiagnosticManifestRuntime.IsValid(manifest),
                $"diagnostic export validity {i + 1} should remain clean.");
        }

        for (var i = 0; i < 10; i++)
        {
            var snapshot = CreateSnapshot(events);
            var store = new ViewportReplayDiagnosticSnapshotStore(2);
            store.Add(snapshot);
            store.Add(snapshot);

            Check(
                store.Count == 2 &&
                store.Latest is not null &&
                ViewportReplayDiagnosticSnapshotRuntime.AreEquivalent(
                    snapshot,
                    store.Latest!),
                $"diagnostic bounded store {i + 1} should retain valid snapshots.");
        }

        for (var i = 0; i < 10; i++)
        {
            var snapshot = CreateSnapshot(events.Take(1).ToArray());
            Check(
                snapshot.Checkpoint.InputCount == 1 &&
                snapshot.Checkpoint.LastInputSequence == 10 &&
                snapshot.Bundle.Manifest.InputEventCount == 1,
                $"diagnostic prefix {i + 1} should preserve count and sequence.");
        }

        for (var i = 0; i < 10; i++)
        {
            var snapshot = CreateSnapshot(Array.Empty<ViewportInputEvent>());
            Check(
                snapshot.Checkpoint.IsEmpty &&
                snapshot.Checkpoint.LastInputSequence == 0 &&
                snapshot.Bundle.Manifest.InputEventCount == 0,
                $"diagnostic empty case {i + 1} should remain coherent.");
        }

        assert(
            round == 100,
            $"Replay diagnostic integrity smoke should execute exactly 100 numbered rounds; actual {round}.");
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
