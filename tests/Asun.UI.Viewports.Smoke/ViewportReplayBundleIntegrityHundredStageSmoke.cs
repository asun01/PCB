using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportReplayBundleIntegrityHundredStageSmoke
{
    private static ViewportRenderEvidenceManifest CreateEvidence(int generation)
    {
        return new ViewportRenderEvidenceManifest(
            generation,
            generation,
            ViewportDirtyFlags.Image,
            1,
            1,
            1,
            1,
            0,
            0,
            0,
            1,
            1,
            0,
            new string('a', 64),
            new string('b', 64),
            new string('c', 64),
            new string('d', 64));
    }

    private static ViewportReplaySessionBundle CreateBundle(
        IReadOnlyList<ViewportInputEvent> inputs,
        IReadOnlyList<ViewportRenderEvidenceManifest> evidence)
    {
        var audit = evidence.Select(item =>
            new ViewportPresentationAuditEvent(
                item.SubmissionSequence,
                "Presented",
                item.Generation,
                item.SubmissionSequence,
                ViewportRenderDeliveryStatus.Succeeded,
                item.RenderedUnits,
                0,
                item.StableKey)).ToArray();

        var manifest = ViewportReplaySessionBundleRuntime.CreateManifest(
            "integrity-test",
            DateTimeOffset.UnixEpoch,
            inputs,
            evidence,
            audit);

        return ViewportReplaySessionBundleRuntime.Capture(
            manifest,
            inputs,
            evidence,
            audit);
    }

    public static void Run(Action<bool, string> assert)
    {
        var validInputs = new[]
        {
            new ViewportInputEvent(1, ViewportInputEventKind.PointerMove, new Vector2(10, 20), 0, ViewportMouseButton.Left)
        };

        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        for (var i = 0; i < 10; i++)
        {
            var evidence = new[] { CreateEvidence(i + 1) };
            var bundle = CreateBundle(validInputs, evidence);
            Check(
                ViewportReplaySessionBundleRuntime.Validate(bundle).Count == 0,
                $"clean bundle {i + 1} should validate.");
        }

        for (var i = 0; i < 10; i++)
        {
            var evidence = CreateEvidence(i + 1);
            var duplicate = new[] { evidence, evidence };
            var bundle = CreateBundle(validInputs, duplicate);
            var errors = ViewportReplaySessionBundleRuntime.Validate(bundle);
            Check(
                errors.Any(item => item.Contains("stable keys", StringComparison.OrdinalIgnoreCase)),
                $"duplicate evidence key {i + 1} should be rejected.");
        }

        for (var i = 0; i < 10; i++)
        {
            var evidence = CreateEvidence(i + 1) with { StableKey = string.Empty };
            var bundle = CreateBundle(validInputs, new[] { evidence });
            var errors = ViewportReplaySessionBundleRuntime.Validate(bundle);
            Check(
                errors.Any(item => item.Contains("stable keys", StringComparison.OrdinalIgnoreCase)),
                $"empty evidence key {i + 1} should be rejected.");
        }

        for (var i = 0; i < 10; i++)
        {
            var invalidInput =
                new ViewportInputEvent(
                    1,
                    ViewportInputEventKind.PointerMove,
                    new Vector2(float.NaN, 20),
                    0,
                    ViewportMouseButton.Left);
            var bundle = CreateBundle(new[] { invalidInput }, Array.Empty<ViewportRenderEvidenceManifest>());
            var errors = ViewportReplaySessionBundleRuntime.Validate(bundle);
            Check(
                errors.Any(item => item.Contains("finite", StringComparison.OrdinalIgnoreCase)),
                $"non-finite input {i + 1} should be rejected.");
        }

        for (var i = 0; i < 10; i++)
        {
            var invalidInput =
                new ViewportInputEvent(
                    1,
                    (ViewportInputEventKind)int.MaxValue,
                    new Vector2(10, 20),
                    0,
                    ViewportMouseButton.Left);
            var bundle = CreateBundle(new[] { invalidInput }, Array.Empty<ViewportRenderEvidenceManifest>());
            var errors = ViewportReplaySessionBundleRuntime.Validate(bundle);
            Check(
                errors.Any(item => item.Contains("kind and button", StringComparison.OrdinalIgnoreCase)),
                $"undefined input kind {i + 1} should be rejected.");
        }

        for (var i = 0; i < 10; i++)
        {
            var bundle = CreateBundle(validInputs, Array.Empty<ViewportRenderEvidenceManifest>());
            var mutated = bundle with
            {
                Inputs = new[]
                {
                    validInputs[0] with
                    {
                        Sequence = 2
                    }
                }
            };
            var errors = ViewportReplaySessionBundleRuntime.Validate(mutated);
            Check(
                errors.Any(item => item.Contains("input hash", StringComparison.OrdinalIgnoreCase)),
                $"manifest input tamper {i + 1} should be visible.");
        }

        for (var i = 0; i < 10; i++)
        {
            var bundle = CreateBundle(validInputs, Array.Empty<ViewportRenderEvidenceManifest>());
            var malformed = bundle with
            {
                Inputs = null!
            };
            Check(
                ViewportReplaySessionBundleRuntime.Validate(malformed).Count > 0,
                $"null input array {i + 1} should not crash.");
        }

        for (var i = 0; i < 10; i++)
        {
            var bundle = CreateBundle(validInputs, Array.Empty<ViewportRenderEvidenceManifest>());
            var json = ViewportReplaySessionBundleRuntime.ToJson(bundle);
            var restored = ViewportReplaySessionBundleRuntime.FromJson(json);
            Check(
                ViewportReplaySessionBundleRuntime.AreEquivalent(bundle, restored),
                $"bundle roundtrip {i + 1} should remain equivalent.");
        }

        for (var i = 0; i < 10; i++)
        {
            var evidence = new[] { CreateEvidence(i + 1) };
            var bundle = CreateBundle(validInputs, evidence);
            var audit = bundle.Audit[0] with { EvidenceKey = "missing-key" };
            var malformed = bundle with
            {
                Audit = new[] { audit },
                Manifest = ViewportReplaySessionBundleRuntime.CreateManifest(
                    bundle.Manifest.SessionId,
                    bundle.Manifest.CreatedAtUtc,
                    bundle.Inputs,
                    bundle.Evidence,
                    new[] { audit })
            };
            var errors = ViewportReplaySessionBundleRuntime.Validate(malformed);
            Check(
                errors.Any(item => item.Contains("does not reference retained evidence", StringComparison.OrdinalIgnoreCase)),
                $"dangling evidence reference {i + 1} should be rejected.");
        }

        assert(
            round == 100,
            $"Bundle integrity smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
