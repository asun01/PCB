using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportReplayIntegrationHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        using var runtime = new ViewportPresentationRuntime<string>(
            new Vector2(200, 200),
            new Vector2(100, 100),
            new Vector2(100, 100),
            0,
            8,
            1,
            new StableTileSource());

        var empty = runtime.ReplayBundle;

        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        Check(empty.IsEmpty, "initial presentation replay bundle should be empty.");

        Check(
            empty.Inputs.Length == 0,
            "initial replay bundle should contain no input events.");

        Check(
            empty.Evidence.Length == 0,
            "initial replay bundle should contain no evidence.");

        Check(
            empty.Audit.Length == 0,
            "initial replay bundle should contain no audit.");

        Check(
            runtime.ReplayBundleValidation.Count == 0,
            "initial replay bundle should validate.");

        Check(
            runtime.Continuous.InputReplay.Count == 0,
            "continuous input replay should start empty.");

        Check(
            runtime.Continuous.InputReplay.Capacity == 2048,
            "continuous input replay should use bounded history.");

        for (var i = 0; i < 10; i++)
        {
            runtime.Submit(
                ViewportInputEventKind.PointerMove,
                new Vector2(i + 1, i + 2));
            runtime.Continuous.ProcessInputs();

            Check(
                runtime.Continuous.InputReplay.Count == i + 1,
                $"processed input {i + 1} should enter bounded replay history.");

            Check(
                runtime.ReplayBundle.Inputs.Length == i + 1,
                $"replay bundle should expose input history {i + 1}.");
        }

        var evidence = new ViewportRenderEvidenceManifest(
            20,
            30,
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
            new string('d', 64));

        runtime.EvidenceHistory.Add(evidence);

        Check(
            runtime.ReplayBundle.Evidence.Length == 1,
            "presentation facade should expose retained evidence through the replay bundle.");

        Check(
            runtime.ReplayBundle.Evidence[0] == evidence,
            "replay bundle should preserve the exact retained evidence manifest.");

        runtime.AuditTrace.Record(
            "Presented",
            evidence.Generation,
            evidence.SubmissionSequence,
            ViewportRenderDeliveryStatus.Succeeded,
            2,
            0,
            evidence.StableKey);

        Check(
            runtime.ReplayBundle.Audit.Length == 1,
            "replay bundle should retain an audit event linked to retained evidence.");

        Check(
            runtime.ReplayBundle.Audit[0].EvidenceKey == evidence.StableKey,
            "audit evidence key should remain linked inside the bundle.");

        Check(
            runtime.ReplayBundleValidation.Count == 0,
            "presentation replay bundle should validate after evidence and audit insertion.");

        var json = ViewportReplaySessionBundleRuntime.ToJson(
            runtime.ReplayBundle);

        Check(
            json.Contains(
                ""manifest":",
                StringComparison.Ordinal),
            "bundle json should contain the manifest.");

        Check(
            json.Contains(
                ""inputs":",
                StringComparison.Ordinal),
            "bundle json should contain inputs.");

        Check(
            json.Contains(
                ""evidence":",
                StringComparison.Ordinal),
            "bundle json should contain evidence.");

        Check(
            json.Contains(
                ""audit":",
                StringComparison.Ordinal),
            "bundle json should contain audit.");

        var roundTripped =
            ViewportReplaySessionBundleRuntime.FromJson(json);

        Check(
            ViewportReplaySessionBundleRuntime.AreEquivalent(
                runtime.ReplayBundle,
                roundTripped),
            "json roundtrip should preserve the complete bundle.");

        for (var i = 0; i < 10; i++)
        {
            var rebuilt =
                ViewportReplaySessionBundleRuntime.FromJson(
                    ViewportReplaySessionBundleRuntime.ToJson(
                        runtime.ReplayBundle));

            Check(
                ViewportReplaySessionBundleRuntime.AreEquivalent(
                    runtime.ReplayBundle,
                    rebuilt),
                $"json replay cycle {i + 1} should remain deterministic.");
        }

        for (var i = 0; i < 10; i++)
        {
            var changedInput = roundTripped.Inputs
                .Select((item, index) =>
                    index == i
                        ? item with { WheelDelta = i + 1 }
                        : item)
                .ToArray();

            var changed = roundTripped with
            {
                Inputs = changedInput
            };

            var comparison =
                ViewportReplaySessionBundleRuntime.Compare(
                    roundTripped,
                    changed);

            Check(
                !comparison.IsEquivalent,
                $"input mutation {i + 1} should break bundle equivalence.");
        }

        for (var i = 0; i < 10; i++)
        {
            var changedEvidence = roundTripped.Evidence
                .Select(item =>
                    item with
                    {
                        RenderedUnits = i % 2,
                        DeferredUnits = i % 2 == 0 ? 0 : 1
                    })
                .ToArray();

            var changed = roundTripped with
            {
                Evidence = changedEvidence
            };

            var comparison =
                ViewportReplaySessionBundleRuntime.Compare(
                    roundTripped,
                    changed);

            Check(
                !comparison.IsEquivalent,
                $"evidence mutation {i + 1} should break bundle equivalence.");
        }

        for (var i = 0; i < 10; i++)
        {
            var changedAudit = roundTripped.Audit
                .Select(item =>
                    item with
                    {
                        Stage = $"Stage-{i}"
                    })
                .ToArray();

            var changed = roundTripped with
            {
                Audit = changedAudit
            };

            var comparison =
                ViewportReplaySessionBundleRuntime.Compare(
                    roundTripped,
                    changed);

            Check(
                !comparison.IsEquivalent,
                $"audit mutation {i + 1} should break bundle equivalence.");
        }

        var oldInputCount = runtime.Continuous.InputReplay.Count;

        for (var i = 0; i < 5; i++)
        {
            runtime.Continuous.Reset();

            Check(
                runtime.Continuous.InputReplay.Count == 0,
                $"continuous reset {i + 1} should clear input replay.");

            Check(
                runtime.EvidenceHistory.Count == 0,
                $"continuous reset {i + 1} should clear evidence history.");

            Check(
                runtime.AuditTrace.Count == 0,
                $"continuous reset {i + 1} should clear audit history.");

            Check(
                runtime.ReplayBundle.IsEmpty,
                $"continuous reset {i + 1} should produce an empty bundle.");

            Check(
                runtime.ValidateReplayBundle().Count == 0,
                $"continuous reset {i + 1} should leave a valid empty bundle.");

            runtime.Submit(
                ViewportInputEventKind.PointerMove,
                new Vector2(i, i));

            runtime.Continuous.ProcessInputs();

            Check(
                runtime.ReplayBundle.Inputs.Length == 1,
                $"post-reset input {i + 1} should restart the replay window.");
        }

        Check(
            oldInputCount == 10,
            "the pre-reset diagnostic window should have retained ten processed inputs.");

        runtime.Reset();

        Check(
            runtime.ReplayBundle.IsEmpty,
            "presentation reset should clear replay integration state.");

        Check(
            runtime.ReplayBundleValidation.Count == 0,
            "presentation reset should leave a valid empty replay bundle.");

        var boundedReplay = new ViewportInputReplayRuntime(capacity: 3);

        for (var i = 0; i < 5; i++)
        {
            boundedReplay.Record(
                ViewportInputEventKind.PointerMove,
                new Vector2(i, i),
                sequence: i + 1);
        }

        Check(
            boundedReplay.Count == 3,
            "standalone input replay should retain only its configured capacity.");

        Check(
            boundedReplay.DroppedCount == 2,
            "standalone input replay should report dropped history.");

        Check(
            boundedReplay.Snapshot()[0].Sequence == 3,
            "bounded input replay should evict the oldest sequence.");

        boundedReplay.Reset();

        Check(
            boundedReplay.Count == 0 &&
            boundedReplay.DroppedCount == 0,
            "bounded input replay reset should clear entries and drop counters.");

        Check(
            runtime.ReplayBundle.Manifest.SessionId ==
                "continuous-viewport",
            "continuous replay bundle should use a stable diagnostic session identity.");

        assert(
            round == 100,
            $"Replay integration smoke should execute exactly 100 numbered rounds; actual {round}.");
    }

    private sealed class StableTileSource : ITileSource<string>
    {
        public ValueTask<string> LoadAsync(
            TileRequest request,
            System.Drawing.RectangleF imageRectangle,
            CancellationToken cancellationToken = default) =>
            ValueTask.FromResult($"tile:{request.Index.X},{request.Index.Y}");
    }
}
