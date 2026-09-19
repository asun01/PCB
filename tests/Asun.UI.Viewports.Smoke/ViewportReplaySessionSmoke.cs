using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportReplaySessionSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var runtime = new RoiViewportRuntime(
            new Vector2(1000, 1000),
            new Vector2(400, 300));

        var input = new ViewportCompositeInputRuntime<string>(runtime);

        var replay = new ViewportInputReplayRuntime();

        replay.Record(
            ViewportInputEventKind.PointerDown,
            new Vector2(100, 80),
            button: ViewportMouseButton.Middle,
            sequence: 1);

        replay.Record(
            ViewportInputEventKind.PointerMove,
            new Vector2(130, 95),
            button: ViewportMouseButton.Middle,
            sequence: 2);

        replay.Record(
            ViewportInputEventKind.PointerUp,
            new Vector2(130, 95),
            button: ViewportMouseButton.Middle,
            sequence: 3);

        replay.Record(
            ViewportInputEventKind.Wheel,
            new Vector2(200, 150),
            wheelDelta: 120,
            sequence: 4);

        var firstResults = replay.Replay(input);
        var firstTransform = runtime.Transform;
        var firstHash = replay.EvidenceHash();

        assert(
            replay.Count == 4 &&
            firstResults.Count == 4 &&
            firstResults[^1].TransformChanged &&
            firstHash.Length == 64,
            "Input replay should apply all recorded events and produce deterministic evidence.");

        var secondRuntime = new RoiViewportRuntime(
            new Vector2(1000, 1000),
            new Vector2(400, 300));

        var secondInput = new ViewportCompositeInputRuntime<string>(
            secondRuntime);

        replay.Replay(secondInput);

        assert(
            secondRuntime.Transform == firstTransform &&
            replay.EvidenceHash() == firstHash,
            "Replaying the same immutable input sequence should reproduce transform state and evidence hash.");

        var invalidReplay = new ViewportInputReplayRuntime();

        var invalidEventRejected = false;

        try
        {
            invalidReplay.Record(
                new ViewportInputEvent(
                    1,
                    ViewportInputEventKind.PointerMove,
                    new Vector2(float.NaN, 0),
                    0,
                    ViewportMouseButton.Left));
        }
        catch (ArgumentOutOfRangeException)
        {
            invalidEventRejected = true;
        }

        assert(
            invalidEventRejected,
            "Input replay should reject non-finite pointer coordinates.");

        var session = new ViewportReplaySessionRuntime(
            sessionId: "session-001",
            createdAtUtc: DateTimeOffset.UnixEpoch);

        foreach (var item in replay.Snapshot())
            session.RecordInput(item);

        var evidence = new ViewportRenderEvidenceManifest(
            Generation: 10,
            SubmissionSequence: 20,
            DirtyFlags: ViewportDirtyFlags.Image,
            BatchItemCount: 2,
            RegionCount: 1,
            TileCount: 1,
            RoiCount: 1,
            OverlayCount: 0,
            InvalidationCount: 0,
            FullSurfaceCount: 0,
            PlannedUnits: 2,
            RenderedUnits: 2,
            DeferredUnits: 0,
            BatchHash: new string('a', 64),
            CommandHash: new string('b', 64),
            FrameHash: new string('c', 64),
            ReplayHash: new string('d', 64));

        session.RecordEvidence(evidence);

        session.RecordAudit(
            new ViewportPresentationAuditEvent(
                Sequence: 1,
                Stage: "Presented",
                Generation: 10,
                SubmissionSequence: 20,
                DeliveryStatus: ViewportRenderDeliveryStatus.Succeeded,
                RenderedUnits: 2,
                DeferredUnits: 0,
                EvidenceKey: evidence.StableKey));

        var manifest = session.CreateManifest();
        var json = session.ToJson();

        assert(
            manifest.SessionId == "session-001" &&
            manifest.InputEventCount == 4 &&
            manifest.EvidenceManifestCount == 1 &&
            manifest.AuditEventCount == 1 &&
            manifest.InputHash.Length == 64 &&
            manifest.EvidenceHash.Length == 64 &&
            manifest.AuditHash.Length == 64 &&
            manifest.SessionHash.Length == 64,
            "Replay session manifest should bind input, evidence, audit, and session hashes.");

        assert(
            json.Contains("\"manifest\":") &&
            json.Contains("\"inputs\":") &&
            json.Contains("\"evidence\":") &&
            json.Contains("\"audit\":"),
            "Replay session JSON should expose all evidence layers.");

        var beforeResetHash = manifest.SessionHash;

        session.Reset();

        var empty = session.CreateManifest();

        assert(
            empty.IsEmpty &&
            empty.SessionId == manifest.SessionId &&
            empty.SessionHash != beforeResetHash,
            "Resetting a replay session should clear evidence while retaining the session identity.");

        replay.Reset();

        assert(
            replay.Count == 0 &&
            replay.EvidenceHash().Length == 64,
            "Input replay reset should produce an empty deterministic evidence hash.");
    }
}
