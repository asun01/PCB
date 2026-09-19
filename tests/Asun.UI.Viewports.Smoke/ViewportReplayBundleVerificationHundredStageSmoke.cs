using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportReplayBundleVerificationHundredStageSmoke
{
    private static readonly Guid StableRoiId =
        Guid.Parse("11111111-2222-3333-4444-555555555555");

    public static void Run(Action<bool, string> assert)
    {
        var inputs = new[]
        {
            new ViewportInputEvent(1, ViewportInputEventKind.PointerMove, new Vector2(10, 20), 0, ViewportMouseButton.Left),
            new ViewportInputEvent(2, ViewportInputEventKind.PointerDown, new Vector2(20, 30), 0, ViewportMouseButton.Left),
            new ViewportInputEvent(3, ViewportInputEventKind.PointerMove, new Vector2(40, 50), 0, ViewportMouseButton.Left),
            new ViewportInputEvent(4, ViewportInputEventKind.PointerUp, new Vector2(40, 50), 0, ViewportMouseButton.Left)
        };

        static ViewportReplaySessionBundle CreateBundle(
            IReadOnlyList<ViewportInputEvent> inputEvents,
            int variant = 0)
        {
            var evidence = new[]
            {
                new ViewportRenderEvidenceManifest(
                    1,
                    1,
                    ViewportDirtyFlags.Image,
                    2 + variant,
                    1,
                    1,
                    1,
                    0,
                    0,
                    0,
                    2 + variant,
                    2 + variant,
                    0,
                    new string('a', 64),
                    new string('b', 64),
                    new string('c', 64),
                    new string('d', 64))
            };

            var audit = new[]
            {
                new ViewportPresentationAuditEvent(
                    1,
                    "Presented",
                    1,
                    1,
                    ViewportRenderDeliveryStatus.Succeeded,
                    2 + variant,
                    0,
                    evidence[0].StableKey)
            };

            var manifest =
                ViewportReplaySessionBundleRuntime.CreateManifest(
                    "bundle-verification",
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

        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        for (var i = 0; i < 10; i++)
        {
            var expected = CreateBundle(inputs);
            var actual = CreateBundle(inputs);
            var result =
                ViewportReplayBundleVerificationRuntime.Verify(
                    expected,
                    actual);

            Check(
                result.IsEquivalent &&
                ViewportReplayBundleVerificationRuntime.IsEquivalent(result),
                $"identical bundles {i + 1} should verify.");
        }

        for (var i = 0; i < 10; i++)
        {
            var expected = CreateBundle(inputs);
            var actual = CreateBundle(
                inputs.Select(item =>
                    item.Sequence == 2
                        ? item with
                        {
                            Position =
                                item.Position +
                                new Vector2(i + 1, 0)
                        }
                        : item).ToArray());

            var result =
                ViewportReplayBundleVerificationRuntime.Verify(
                    expected,
                    actual);

            Check(
                !result.IsEquivalent &&
                !result.InputMatches &&
                result.EvidenceMatches &&
                result.AuditMatches &&
                result.Differences.Any(
                    item => item.StartsWith(
                        "Inputs[",
                        StringComparison.Ordinal)),
                $"input differences {i + 1} should be isolated.");
        }

        for (var i = 0; i < 10; i++)
        {
            var expected = CreateBundle(inputs);
            var actual = CreateBundle(inputs, i + 1);
            var result =
                ViewportReplayBundleVerificationRuntime.Verify(
                    expected,
                    actual);

            Check(
                !result.IsEquivalent &&
                result.InputMatches &&
                !result.EvidenceMatches &&
                !result.AuditMatches &&
                result.Differences.Any(
                    item => item.StartsWith(
                        "Evidence[",
                        StringComparison.Ordinal)),
                $"evidence differences {i + 1} should be detected.");
        }

        for (var i = 0; i < 10; i++)
        {
            var expected = CreateBundle(inputs);
            var actual = CreateBundle(inputs);

            var changedAudit =
                actual.Audit[0] with
                {
                    RenderedUnits = 3 + i
                };

            actual = actual with
            {
                Audit = new[] { changedAudit },
                Manifest =
                    ViewportReplaySessionBundleRuntime.CreateManifest(
                        actual.Manifest.SessionId,
                        actual.Manifest.CreatedAtUtc,
                        actual.Inputs,
                        actual.Evidence,
                        new[] { changedAudit })
            };

            var result =
                ViewportReplayBundleVerificationRuntime.Verify(
                    expected,
                    actual);

            Check(
                !result.IsEquivalent &&
                result.InputMatches &&
                result.EvidenceMatches &&
                !result.AuditMatches &&
                result.Differences.Any(
                    item => item.StartsWith(
                        "Audit[",
                        StringComparison.Ordinal)),
                $"audit differences {i + 1} should be detected.");
        }

        for (var i = 0; i < 10; i++)
        {
            var expected = CreateBundle(inputs);
            var actual = expected with
            {
                Manifest =
                    expected.Manifest with
                    {
                        SessionHash =
                            new string((char)('a' + i), 64)
                    }
            };

            var result =
                ViewportReplayBundleVerificationRuntime.Verify(
                    expected,
                    actual);

            Check(
                !result.IsEquivalent &&
                !result.ManifestMatches &&
                result.InputMatches &&
                result.EvidenceMatches &&
                result.AuditMatches &&
                result.Differences.Any(
                    item => item.StartsWith(
                        "Manifest.",
                        StringComparison.Ordinal)),
                $"manifest differences {i + 1} should be detected.");
        }

        for (var i = 0; i < 10; i++)
        {
            var expected = CreateBundle(inputs);
            var actual = ViewportReplaySessionBundleRuntime.FromJson(
                ViewportReplaySessionBundleRuntime.ToJson(expected));

            var result =
                ViewportReplayBundleVerificationRuntime.Verify(
                    expected,
                    actual);

            Check(
                result.IsEquivalent &&
                result.Differences.Count == 0,
                $"JSON roundtrip {i + 1} should preserve bundle equivalence.");
        }

        for (var i = 0; i < 10; i++)
        {
            var expected = CreateBundle(inputs);

            var changedInput = inputs
                .Select(item =>
                    item.Sequence == 3
                        ? item with
                        {
                            WheelDelta = i + 1
                        }
                        : item)
                .ToArray();

            var actual = CreateBundle(changedInput);
            var result =
                ViewportReplayBundleVerificationRuntime.Verify(
                    expected,
                    actual);

            Check(
                !result.IsEquivalent &&
                !result.InputMatches &&
                result.Differences.Count > 0,
                $"secondary input mutation {i + 1} should remain visible.");
        }

        for (var i = 0; i < 10; i++)
        {
            var expected = CreateBundle(inputs);
            var actual = CreateBundle(inputs);

            var evidence = actual.Evidence[0] with
            {
                StableKey = actual.Evidence[0].StableKey
            };

            actual = actual with
            {
                Evidence = new[] { evidence }
            };

            var result =
                ViewportReplayBundleVerificationRuntime.Verify(
                    expected,
                    actual);

            Check(
                result.IsEquivalent,
                $"stable evidence identity {i + 1} should not create a difference.");
        }

        for (var i = 0; i < 10; i++)
        {
            var expected = CreateBundle(inputs);
            var actual = CreateBundle(inputs);

            var result =
                ViewportReplayBundleVerificationRuntime.Verify(
                    expected,
                    actual);

            Check(
                result.ManifestMatches &&
                result.InputMatches &&
                result.EvidenceMatches &&
                result.AuditMatches &&
                result.Differences.Count == 0,
                $"component flags {i + 1} should all be true on clean verification.");
        }

        assert(
            round == 100,
            $"Bundle verification smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
