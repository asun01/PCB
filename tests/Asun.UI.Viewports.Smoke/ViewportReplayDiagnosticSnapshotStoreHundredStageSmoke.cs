using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportReplayDiagnosticSnapshotStoreHundredStageSmoke
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
                ViewportMouseButton.Left)
        };

        static ViewportReplayDiagnosticSnapshot CreateSnapshot(
            long generationOffset = 0)
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
                    events);

            var bundle =
                ViewportReplaySessionBundleRuntime.Capture(
                    ViewportReplaySessionBundleRuntime.CreateManifest(
                        "snapshot-store",
                        DateTimeOffset.UnixEpoch,
                        events,
                        Array.Empty<ViewportRenderEvidenceManifest>(),
                        Array.Empty<ViewportPresentationAuditEvent>()),
                    events,
                    Array.Empty<ViewportRenderEvidenceManifest>(),
                    Array.Empty<ViewportPresentationAuditEvent>());

            var checkpoint =
                ViewportReplayCheckpointRuntime.Capture(
                    execution,
                    events);

            var snapshot =
                ViewportReplayDiagnosticSnapshotRuntime.Capture(
                    execution,
                    bundle,
                    checkpoint);

            composite.Dispose();

            return snapshot with
            {
                Execution =
                    snapshot.Execution with
                    {
                        FinalState =
                            snapshot.Execution.FinalState with
                            {
                                Generation =
                                    snapshot.Execution.FinalState.Generation +
                                    generationOffset
                            }
                    },
                DiagnosticHash =
                    snapshot.DiagnosticHash
            };
        }

        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        for (var i = 0; i < 10; i++)
        {
            var store = new ViewportReplayDiagnosticSnapshotStore(4);
            var snapshot = CreateSnapshot();

            store.Add(snapshot);

            Check(
                store.Count == 1 &&
                store.Latest is not null &&
                store.Latest.DiagnosticHash == snapshot.DiagnosticHash &&
                store.DroppedCount == 0,
                $"single snapshot retention {i + 1} should be correct.");
        }

        for (var i = 0; i < 10; i++)
        {
            var store = new ViewportReplayDiagnosticSnapshotStore(3);

            store.Add(CreateSnapshot());

            Check(
                store.Snapshot().Count == 1 &&
                store.Latest is not null,
                $"snapshot enumeration {i + 1} should be stable.");
        }

        for (var i = 0; i < 10; i++)
        {
            var store = new ViewportReplayDiagnosticSnapshotStore(2);
            store.Add(CreateSnapshot());
            store.Add(CreateSnapshot());
            store.Add(CreateSnapshot());

            Check(
                store.Count == 2 &&
                store.DroppedCount == 1 &&
                store.Snapshot().Count == 2,
                $"bounded eviction {i + 1} should retain a fixed tail.");
        }

        for (var i = 0; i < 10; i++)
        {
            var store = new ViewportReplayDiagnosticSnapshotStore(4);

            store.Add(CreateSnapshot());
            var generation = store.Latest!.Generation;

            Check(
                store.FindByGeneration(generation).Count == 1,
                $"generation lookup {i + 1} should find retained snapshot.");
        }

        for (var i = 0; i < 10; i++)
        {
            var store = new ViewportReplayDiagnosticSnapshotStore(4);
            store.Add(CreateSnapshot());
            store.Clear();

            Check(
                store.Count == 0 &&
                store.DroppedCount == 0 &&
                store.Latest is null,
                $"clear {i + 1} should reset the store.");
        }

        for (var i = 0; i < 10; i++)
        {
            var store = new ViewportReplayDiagnosticSnapshotStore(4);
            var first = CreateSnapshot();
            var second = CreateSnapshot();

            store.Add(first);
            store.Add(second);

            Check(
                store.Snapshot().Count == 2 &&
                ViewportReplayDiagnosticSnapshotRuntime.AreEquivalent(
                    store.Snapshot()[0],
                    first) &&
                ViewportReplayDiagnosticSnapshotRuntime.AreEquivalent(
                    store.Snapshot()[1],
                    second),
                $"snapshot equivalence retention {i + 1} should be stable.");
        }

        for (var i = 0; i < 10; i++)
        {
            var store = new ViewportReplayDiagnosticSnapshotStore(4);
            var snapshot = CreateSnapshot();

            var invalid =
                snapshot with
                {
                    DiagnosticHash =
                        new string((char)('a' + i), 64)
                };

            var rejected = false;

            try
            {
                store.Add(invalid);
            }
            catch (InvalidOperationException)
            {
                rejected = true;
            }

            Check(
                rejected &&
                store.Count == 0,
                $"invalid snapshot {i + 1} should never enter the store.");
        }

        for (var i = 0; i < 10; i++)
        {
            var store = new ViewportReplayDiagnosticSnapshotStore(4);
            var snapshot = CreateSnapshot();

            var rejected = false;

            try
            {
                store.FindByGeneration(-1);
            }
            catch (ArgumentOutOfRangeException)
            {
                rejected = true;
            }

            Check(
                rejected &&
                snapshot.Generation >= 0,
                $"negative generation lookup {i + 1} should be rejected.");
        }

        for (var i = 0; i < 10; i++)
        {
            var rejected = false;
            var store = new ViewportReplayDiagnosticSnapshotStore(4);
            var first = CreateSnapshot();

            store.Add(first);

            try
            {
                var invalid = first with
                {
                    Execution =
                        first.Execution with
                        {
                            FinalState =
                                first.Execution.FinalState with
                                {
                                    Generation =
                                        first.Generation - 1
                                }
                        }
                };

                store.Add(invalid);
            }
            catch (InvalidOperationException)
            {
                rejected = true;
            }

            Check(
                rejected,
                $"descending generation {i + 1} should be rejected.");
        }

        for (var i = 0; i < 10; i++)
        {
            var store = new ViewportReplayDiagnosticSnapshotStore(4);
            var snapshot = CreateSnapshot();

            store.Add(snapshot);

            Check(
                ViewportReplayDiagnosticSnapshotRuntime.Validate(
                    store.Latest!).Count == 0,
                $"retained validation {i + 1} should remain clean.");
        }

        assert(
            round == 100,
            $"Diagnostic snapshot store smoke should execute exactly 100 numbered rounds; actual {round}.");
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
