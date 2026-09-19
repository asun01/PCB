using Asun.UI.Viewports;

public static class ViewportReplayBundleSerializationBoundaryHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        static ViewportReplaySessionBundle CreateBundle()
        {
            var inputs = new[]
            {
                new ViewportInputEvent(
                    1,
                    ViewportInputEventKind.PointerMove,
                    new System.Numerics.Vector2(10, 20),
                    0,
                    ViewportMouseButton.Left)
            };

            var manifest = ViewportReplaySessionBundleRuntime.CreateManifest(
                "json-boundary",
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

        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        for (var i = 0; i < 10; i++)
        {
            var bundle = CreateBundle();
            var json = ViewportReplaySessionBundleRuntime.ToJson(bundle);
            Check(
                !string.IsNullOrWhiteSpace(json),
                $"valid JSON {i + 1} should be emitted.");
        }

        for (var i = 0; i < 10; i++)
        {
            var bundle = CreateBundle();
            var restored = ViewportReplaySessionBundleRuntime.FromJson(
                ViewportReplaySessionBundleRuntime.ToJson(bundle));
            Check(
                ViewportReplaySessionBundleRuntime.AreEquivalent(bundle, restored),
                $"valid JSON roundtrip {i + 1} should be lossless.");
        }

        for (var i = 0; i < 10; i++)
        {
            var rejected = false;
            try
            {
                ViewportReplaySessionBundleRuntime.FromJson("{");
            }
            catch (InvalidOperationException exception)
            {
                rejected = exception.Message.Contains(
                    "malformed",
                    StringComparison.OrdinalIgnoreCase);
            }

            Check(
                rejected,
                $"malformed JSON {i + 1} should be normalized to a deterministic error.");
        }

        for (var i = 0; i < 10; i++)
        {
            var rejected = false;
            try
            {
                ViewportReplaySessionBundleRuntime.FromJson("null");
            }
            catch (InvalidOperationException)
            {
                rejected = true;
            }

            Check(
                rejected,
                $"null JSON {i + 1} should be rejected.");
        }

        for (var i = 0; i < 10; i++)
        {
            var bundle = CreateBundle() with
            {
                FormatVersion = 999
            };
            var rejected = false;
            try
            {
                ViewportReplaySessionBundleRuntime.ToJson(bundle);
            }
            catch (InvalidOperationException)
            {
                rejected = true;
            }

            Check(
                rejected,
                $"unsupported format version serialization {i + 1} should be blocked.");
        }

        for (var i = 0; i < 10; i++)
        {
            var bundle = CreateBundle();
            var json = ViewportReplaySessionBundleRuntime.ToJson(bundle);
            var unknownFieldJson =
                json.TrimEnd('}') + ",\"futureField\" : 123}";
            var restored =
                ViewportReplaySessionBundleRuntime.FromJson(
                    unknownFieldJson);

            Check(
                restored.FormatVersion ==
                ViewportReplaySessionBundleRuntime.CurrentFormatVersion,
                $"unknown field tolerance {i + 1} should preserve known schema.");
        }

        for (var i = 0; i < 10; i++)
        {
            var bundle = CreateBundle();
            var json = ViewportReplaySessionBundleRuntime.ToJson(bundle);
            Check(
                json.Contains(
                    "\"formatVersion\"",
                    StringComparison.Ordinal) &&
                json.Contains(
                    "\"manifest\"",
                    StringComparison.Ordinal),
                $"required JSON fields {i + 1} should be present.");
        }

        for (var i = 0; i < 10; i++)
        {
            var bundle = CreateBundle();
            var json = ViewportReplaySessionBundleRuntime.ToJson(bundle);
            var reformatted =
                json.Replace(",", ", ", StringComparison.Ordinal);
            var restored =
                ViewportReplaySessionBundleRuntime.FromJson(reformatted);

            Check(
                ViewportReplaySessionBundleRuntime.AreEquivalent(bundle, restored),
                $"formatting variation {i + 1} should not alter semantics.");
        }

        for (var i = 0; i < 10; i++)
        {
            var bundle = CreateBundle();
            var errors = ViewportReplaySessionBundleRuntime.Validate(bundle);
            Check(
                errors.Count == 0,
                $"pre-serialization validation {i + 1} should remain clean.");
        }

        assert(
            round == 100,
            $"Bundle serialization boundary smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
