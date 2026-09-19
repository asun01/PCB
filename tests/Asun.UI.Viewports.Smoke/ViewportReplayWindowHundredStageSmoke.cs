using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportReplayWindowHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var inputs = Enumerable.Range(1, 12)
            .Select(sequence =>
                new ViewportInputEvent(
                    sequence,
                    ViewportInputEventKind.PointerMove,
                    new Vector2(sequence, sequence + 1),
                    0,
                    ViewportMouseButton.Left))
            .ToArray();

        var evidence = Enumerable.Range(1, 8)
            .Select(index =>
                new ViewportRenderEvidenceManifest(
                    index,
                    index,
                    ViewportDirtyFlags.Image,
                    1,
                    1,
                    1,
                    0,
                    0,
                    0,
                    0,
                    1,
                    1,
                    0,
                    new string((char)('a' + index), 64),
                    new string((char)('b' + index), 64),
                    new string((char)('c' + index), 64),
                    new string((char)('d' + index), 64)))
            .ToArray();

        var audit = Enumerable.Range(1, 8)
            .Select(index =>
                new ViewportPresentationAuditEvent(
                    index,
                    index % 2 == 0 ? "Presented" : "Completed",
                    index,
                    index,
                    ViewportRenderDeliveryStatus.Succeeded,
                    1,
                    0,
                    evidence[index - 1].StableKey))
            .ToArray();

        var manifest = ViewportReplaySessionBundleRuntime.CreateManifest(
            "window-session",
            DateTimeOffset.UnixEpoch,
            inputs,
            evidence,
            audit);

        var source = ViewportReplaySessionBundleRuntime.Capture(
            manifest,
            inputs,
            evidence,
            audit);

        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        for (var i = 0; i < 10; i++)
        {
            var window = ViewportReplaySessionBundleWindowRuntime.Tail(
                source,
                3 + i % 4,
                3 + i % 3,
                2 + i % 4);

            Check(
                ViewportReplaySessionBundleRuntime.Validate(window).Count == 0,
                $"tail {i + 1} should validate.");

            Check(
                ViewportReplaySessionBundleWindowRuntime.IsTailOf(
                    source,
                    window),
                $"tail {i + 1} should contain only source records.");

            var descriptor =
                ViewportReplaySessionBundleWindowRuntime.Describe(window);

            Check(
                descriptor.InputCount == window.Inputs.Length &&
                descriptor.EvidenceCount == window.Evidence.Length &&
                descriptor.AuditCount == window.Audit.Length,
                $"tail descriptor {i + 1} should match counts.");

            Check(
                descriptor.InputCount <= 3 + i % 4 &&
                descriptor.AuditCount <= 2 + i % 4,
                $"tail bounds {i + 1} should respect requested limits.");

            Check(
                window.Audit.All(a =>
                    string.IsNullOrEmpty(a.EvidenceKey) ||
                    window.Evidence.Any(e =>
                        e.StableKey == a.EvidenceKey)),
                $"tail {i + 1} should preserve audit evidence closure.");
        }

        for (var i = 0; i < 10; i++)
        {
            var window = ViewportReplaySessionBundleWindowRuntime.Tail(
                source,
                i,
                i,
                i);

            Check(
                ViewportReplaySessionBundleRuntime.Validate(window).Count == 0,
                $"progressive zero-to-tail window {i + 1} should remain valid.");

            Check(
                window.Inputs.Length == Math.Min(i, source.Inputs.Length),
                $"input tail size {i + 1} should be bounded.");

            Check(
                window.Audit.Length == Math.Min(i, source.Audit.Length),
                $"audit tail size {i + 1} should be bounded.");

            Check(
                ViewportReplaySessionBundleWindowRuntime.Describe(window)
                    .LastInputSequence ==
                (window.Inputs.Length == 0 ? 0 : window.Inputs[^1].Sequence),
                $"input tail descriptor {i + 1} should expose the latest sequence.");
        }

        for (var i = 0; i < 10; i++)
        {
            var window = ViewportReplaySessionBundleWindowRuntime.Tail(
                source,
                12,
                8,
                i + 1);

            var roundTrip =
                ViewportReplaySessionBundleRuntime.FromJson(
                    ViewportReplaySessionBundleRuntime.ToJson(window));

            Check(
                ViewportReplaySessionBundleRuntime.AreEquivalent(
                    window,
                    roundTrip),
                $"tail json roundtrip {i + 1} should preserve the window.");

            Check(
                ViewportReplaySessionBundleRuntime.Validate(
                    roundTrip).Count == 0,
                $"tail roundtrip validation {i + 1} should remain clean.");
        }

        var invalidLimitRejected = false;

        try
        {
            ViewportReplaySessionBundleWindowRuntime.Tail(
                source,
                -1,
                1,
                1);
        }
        catch (ArgumentOutOfRangeException)
        {
            invalidLimitRejected = true;
        }

        Check(
            invalidLimitRejected,
            "negative input tail limit should be rejected.");

        var invalidEvidenceLimitRejected = false;

        try
        {
            ViewportReplaySessionBundleWindowRuntime.Tail(
                source,
                1,
                -1,
                1);
        }
        catch (ArgumentOutOfRangeException)
        {
            invalidEvidenceLimitRejected = true;
        }

        Check(
            invalidEvidenceLimitRejected,
            "negative evidence tail limit should be rejected.");

        var invalidAuditLimitRejected = false;

        try
        {
            ViewportReplaySessionBundleWindowRuntime.Tail(
                source,
                1,
                1,
                -1);
        }
        catch (ArgumentOutOfRangeException)
        {
            invalidAuditLimitRejected = true;
        }

        Check(
            invalidAuditLimitRejected,
            "negative audit tail limit should be rejected.");

        var impossibleClosureRejected = false;

        try
        {
            ViewportReplaySessionBundleWindowRuntime.Tail(
                source,
                1,
                0,
                8);
        }
        catch (InvalidOperationException)
        {
            impossibleClosureRejected = true;
        }

        Check(
            impossibleClosureRejected,
            "audit windows that require unavailable evidence closure should be rejected.");

        Check(
            ViewportReplaySessionBundleWindowRuntime
                .Describe(
                    ViewportReplaySessionBundleRuntime.Capture(
                        ViewportReplaySessionBundleRuntime.CreateManifest(
                            "empty-window",
                            DateTimeOffset.UnixEpoch,
                            Array.Empty<ViewportInputEvent>(),
                            Array.Empty<ViewportRenderEvidenceManifest>(),
                            Array.Empty<ViewportPresentationAuditEvent>()),
                        Array.Empty<ViewportInputEvent>(),
                        Array.Empty<ViewportRenderEvidenceManifest>(),
                        Array.Empty<ViewportPresentationAuditEvent>()))
                == ViewportReplaySessionBundleWindow.Empty,
            "empty bundle should describe as the empty window.");

        for (var i = 0; i < 10; i++)
        {
            var window =
                ViewportReplaySessionBundleWindowRuntime.Tail(
                    source,
                    12,
                    8,
                    8);

            Check(
                ViewportReplaySessionBundleRuntime.AreEquivalent(
                    source,
                    window),
                $"full-size tail {i + 1} should reproduce the source bundle.");
        }

        assert(
            round == 100,
            $"Replay window smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
