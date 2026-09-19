using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportReplayDiagnosticManifestHundredStageSmoke
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
                ViewportMouseButton.Left)
        };

        static ViewportReplayDiagnosticSnapshot CreateSnapshot(
            IReadOnlyList<ViewportInputEvent> inputs)
        {
            var composite =
                new ViewportCompositeRuntime<string>(
                    new Vector2(800, 600),
                    new Vector2(400, 300),
                    new Vector2(100, 100),
                    1,
                    16,
                    2,
                    new LocalTileSource());

            var input =
                new ViewportCompositeInputRuntime<string>(
                    composite);

            var execution =
                ViewportReplayExecutionStateRuntime.Execute(
                    input,
                    inputs);

            var bundle =
                ViewportReplaySessionBundleRuntime.Capture(
                    ViewportReplaySessionBundleRuntime.CreateManifest(
                        "manifest-test",
                        DateTimeOffset.UnixEpoch,
                        inputs,
                        Array.Empty<ViewportRenderEvidenceManifest>(),
                        Array.Empty<ViewportPresentationAuditEvent>()),
                    inputs,
                    Array.Empty<ViewportRenderEvidenceManifest>(),
                    Array.Empty<ViewportPresentationAuditEvent>());

            var checkpoint =
                ViewportReplayCheckpointRuntime.Capture(
                    execution,
                    inputs);

            var snapshot =
                ViewportReplayDiagnosticSnapshotRuntime.Capture(
                    execution,
                    bundle,
                    checkpoint);

            composite.Dispose();
            return snapshot;
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
            var manifest =
                ViewportReplayDiagnosticManifestRuntime.Create(
                    snapshot);

            Check(
                ViewportReplayDiagnosticManifestRuntime.IsValid(
                    manifest) &&
                manifest.FormatVersion ==
                ViewportReplayDiagnosticManifest.CurrentFormatVersion,
                $"manifest creation {i + 1} should be valid.");
        }

        for (var i = 0; i < 10; i++)
        {
            var snapshot = CreateSnapshot(events);
            var manifest =
                ViewportReplayDiagnosticManifestRuntime.Create(
                    snapshot);
            var json =
                ViewportReplayDiagnosticManifestRuntime.ToJson(
                    manifest);
            var restored =
                ViewportReplayDiagnosticManifestRuntime.FromJson(
                    json);

            Check(
                restored == manifest &&
                ViewportReplayDiagnosticManifestRuntime.IsValid(
                    restored),
                $"manifest JSON roundtrip {i + 1} should be lossless.");
        }

        for (var i = 0; i < 10; i++)
        {
            var snapshot = CreateSnapshot(events);
            var manifest =
                ViewportReplayDiagnosticManifestRuntime.Create(
                    snapshot) with
                {
                    FormatVersion = 99 + i
                };

            Check(
                !ViewportReplayDiagnosticManifestRuntime.IsValid(
                    manifest),
                $"unsupported manifest version {i + 1} should fail.");
        }

        for (var i = 0; i < 10; i++)
        {
            var snapshot = CreateSnapshot(events);
            var manifest =
                ViewportReplayDiagnosticManifestRuntime.Create(
                    snapshot) with
                {
                    DiagnosticHash =
                        new string((char)('a' + i), 64)
                };

            Check(
                !ViewportReplayDiagnosticManifestRuntime.IsValid(
                    manifest),
                $"diagnostic hash mutation {i + 1} should fail.");
        }

        for (var i = 0; i < 10; i++)
        {
            var snapshot = CreateSnapshot(events);
            var manifest =
                ViewportReplayDiagnosticManifestRuntime.Create(
                    snapshot) with
                {
                    BundleJson = string.Empty
                };

            Check(
                ViewportReplayDiagnosticManifestRuntime.Validate(
                    manifest).Any(
                    item => item.Contains(
                        "BundleJson",
                        StringComparison.Ordinal)),
                $"empty bundle JSON {i + 1} should fail explicitly.");
        }

        for (var i = 0; i < 10; i++)
        {
            var snapshot = CreateSnapshot(events);
            var manifest =
                ViewportReplayDiagnosticManifestRuntime.Create(
                    snapshot) with
                {
                    ExecutionResultHash =
                        new string((char)('b' + i), 63)
                };

            Check(
                !ViewportReplayDiagnosticManifestRuntime.IsValid(
                    manifest),
                $"malformed execution hash {i + 1} should fail.");
        }

        for (var i = 0; i < 10; i++)
        {
            var snapshot = CreateSnapshot(events);
            var manifest =
                ViewportReplayDiagnosticManifestRuntime.Create(
                    snapshot);

            var json =
                ViewportReplayDiagnosticManifestRuntime.ToJson(
                    manifest);

            Check(
                json.Contains(
                    ""formatVersion"",
                    StringComparison.Ordinal) &&
                json.Contains(
                    ""bundleJson"",
                    StringComparison.Ordinal) &&
                json.Contains(
                    ""diagnosticHash"",
                    StringComparison.Ordinal),
                $"manifest schema {i + 1} should expose stable fields.");
        }

        for (var i = 0; i < 10; i++)
        {
            var snapshot = CreateSnapshot(events);
            var manifest =
                ViewportReplayDiagnosticManifestRuntime.Create(
                    snapshot);

            var malformedJson =
                "{"formatVersion":1,"bundleJson":"{"}";

            var rejected = false;

            try
            {
                ViewportReplayDiagnosticManifestRuntime.FromJson(
                    malformedJson);
            }
            catch (InvalidOperationException)
            {
                rejected = true;
            }

            Check(
                rejected &&
                manifest.FormatVersion == 1,
                $"malformed manifest JSON {i + 1} should be rejected.");
        }

        for (var i = 0; i < 10; i++)
        {
            var first = CreateSnapshot(events);
            var second = CreateSnapshot(events);
            var firstManifest =
                ViewportReplayDiagnosticManifestRuntime.Create(
                    first);
            var secondManifest =
                ViewportReplayDiagnosticManifestRuntime.Create(
                    second);

            Check(
                firstManifest.DiagnosticHash ==
                secondManifest.DiagnosticHash &&
                firstManifest.BundleJson ==
                secondManifest.BundleJson,
                $"deterministic manifest {i + 1} should match.");
        }

        for (var i = 0; i < 10; i++)
        {
            var snapshot =
                CreateSnapshot(
                    events.Take(1).ToArray());
            var manifest =
                ViewportReplayDiagnosticManifestRuntime.Create(
                    snapshot);

            Check(
                manifest.BundleJson.Contains(
                    ""inputEventCount":1",
                    StringComparison.Ordinal),
                $"prefix manifest {i + 1} should preserve bundle count.");
        }

        assert(
            round == 100,
            $"Replay diagnostic manifest smoke should execute exactly 100 numbered rounds; actual {round}.");
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
