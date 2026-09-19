using Asun.UI.Viewports;

public static class ViewportReplayBundleSchemaHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
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

        static ViewportReplaySessionBundle CreateBundle(
            IReadOnlyList<ViewportInputEvent> inputEvents)
        {
            var manifest =
                ViewportReplaySessionBundleRuntime.CreateManifest(
                    "schema-test",
                    DateTimeOffset.UnixEpoch,
                    inputEvents,
                    Array.Empty<ViewportRenderEvidenceManifest>(),
                    Array.Empty<ViewportPresentationAuditEvent>());

            return ViewportReplaySessionBundleRuntime.Capture(
                manifest,
                inputEvents,
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
            var bundle = CreateBundle(inputs);

            Check(
                bundle.FormatVersion ==
                ViewportReplaySessionBundleRuntime.CurrentFormatVersion &&
                ViewportReplaySessionBundleRuntime.Validate(bundle).Count == 0,
                $"current version {i + 1} should validate.");
        }

        for (var i = 0; i < 10; i++)
        {
            var bundle = CreateBundle(inputs);
            var json = ViewportReplaySessionBundleRuntime.ToJson(bundle);
            var restored =
                ViewportReplaySessionBundleRuntime.FromJson(json);

            Check(
                restored.FormatVersion == bundle.FormatVersion &&
                ViewportReplaySessionBundleRuntime.AreEquivalent(
                    bundle,
                    restored),
                $"versioned JSON roundtrip {i + 1} should be lossless.");
        }

        for (var i = 0; i < 10; i++)
        {
            var bundle = CreateBundle(inputs) with
            {
                FormatVersion = 999 + i
            };

            var errors =
                ViewportReplaySessionBundleRuntime.Validate(bundle);

            Check(
                errors.Count > 0 &&
                errors.Any(
                    item => item.Contains(
                        "format version",
                        StringComparison.OrdinalIgnoreCase)),
                $"unsupported version {i + 1} should be rejected.");
        }

        for (var i = 0; i < 10; i++)
        {
            var bundle = CreateBundle(inputs);
            var actual = bundle with
            {
                FormatVersion = bundle.FormatVersion + 1
            };

            var comparison =
                ViewportReplaySessionBundleRuntime.Compare(
                    bundle,
                    actual);

            Check(
                !comparison.IsEquivalent &&
                comparison.Differences.Any(
                    item => item.StartsWith(
                        "FormatVersion",
                        StringComparison.Ordinal)),
                $"format comparison {i + 1} should detect version differences.");
        }

        for (var i = 0; i < 10; i++)
        {
            var bundle = CreateBundle(inputs);
            var json =
                ViewportReplaySessionBundleRuntime.ToJson(bundle);

            Check(
                json.Contains(
                    "\"formatVersion\"",
                    StringComparison.Ordinal),
                $"JSON schema field {i + 1} should be explicit.");
        }

        for (var i = 0; i < 10; i++)
        {
            var bundle = CreateBundle(inputs);
            var malformed = bundle with
            {
                Inputs = null!
            };

            var errors =
                ViewportReplaySessionBundleRuntime.Validate(
                    malformed);

            Check(
                errors.Any(
                    item => item.Contains(
                        "inputs must not be null",
                        StringComparison.OrdinalIgnoreCase)),
                $"null input array {i + 1} should fail cleanly.");
        }

        for (var i = 0; i < 10; i++)
        {
            var bundle = CreateBundle(inputs);
            var malformed = bundle with
            {
                Evidence = null!
            };

            var errors =
                ViewportReplaySessionBundleRuntime.Validate(
                    malformed);

            Check(
                errors.Any(
                    item => item.Contains(
                        "evidence must not be null",
                        StringComparison.OrdinalIgnoreCase)),
                $"null evidence array {i + 1} should fail cleanly.");
        }

        for (var i = 0; i < 10; i++)
        {
            var bundle = CreateBundle(inputs);
            var malformed = bundle with
            {
                Audit = null!
            };

            var errors =
                ViewportReplaySessionBundleRuntime.Validate(
                    malformed);

            Check(
                errors.Any(
                    item => item.Contains(
                        "audit must not be null",
                        StringComparison.OrdinalIgnoreCase)),
                $"null audit array {i + 1} should fail cleanly.");
        }

        for (var i = 0; i < 10; i++)
        {
            var bundle = CreateBundle(inputs);
            var json =
                ViewportReplaySessionBundleRuntime.ToJson(bundle);

            var caseVariant = json.Replace(
                "\"formatVersion\"",
                "\"FORMATVERSION\",
                StringComparison.Ordinal);

            var restored =
                ViewportReplaySessionBundleRuntime.FromJson(
                    caseVariant);

            Check(
                restored.FormatVersion ==
                ViewportReplaySessionBundleRuntime.CurrentFormatVersion,
                $"case-insensitive schema {i + 1} should retain the version.");
        }

        for (var i = 0; i < 10; i++)
        {
            var bundle = CreateBundle(inputs);
            var json =
                ViewportReplaySessionBundleRuntime.ToJson(bundle);

            Check(
                !string.IsNullOrWhiteSpace(json) &&
                json.Length > 0,
                $"schema JSON {i + 1} should be non-empty.");
        }

        assert(
            round == 100,
            $"Replay bundle schema smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
