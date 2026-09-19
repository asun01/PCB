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

        var source = ViewportReplaySessionBundleRuntime.Capture(
            ViewportReplaySessionBundleRuntime.CreateManifest(
                "window-session",
                DateTimeOffset.UnixEpoch,
                inputs,
                evidence,
                audit),
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
                i + 1,
                8,
                i % 8 + 1);

            var descriptor =
                ViewportReplaySessionBundleWindowRuntime.Describe(window);

            Check(
                ViewportReplaySessionBundleRuntime.Validate(window).Count == 0 &&
                ViewportReplaySessionBundleWindowRuntime.IsTailOf(source, window) &&
                window.Inputs.Length == i + 1 &&
                window.Audit.Length == i % 8 + 1 &&
                window.Audit.All(a =>
                    window.Evidence.Any(e => e.StableKey == a.EvidenceKey)) &&
                descriptor.InputCount == window.Inputs.Length &&
                descriptor.EvidenceCount == window.Evidence.Length &&
                descriptor.AuditCount == window.Audit.Length,
                $"bounded diagnostic window {i + 1} should validate, stay within source, and preserve audit/evidence closure.");
        }

        for (var i = 0; i < 10; i++)
        {
            var window = ViewportReplaySessionBundleWindowRuntime.Tail(
                source,
                i,
                8,
                i);

            Check(
                ViewportReplaySessionBundleRuntime.Validate(window).Count == 0 &&
                window.Inputs.Length == i &&
                window.Audit.Length == i &&
                (window.Inputs.Length == 0 ||
                 window.Inputs[^1].Sequence == i) &&
                (window.Audit.Length == 0 ||
                 window.Audit[^1].Sequence == i) &&
                window.Evidence.All(e => source.Evidence.Contains(e)) &&
                ViewportReplaySessionBundleWindowRuntime.IsTailOf(source, window),
                $"progressive window {i} should remain valid and bounded.");
        }

        for (var i = 0; i < 10; i++)
        {
            var window = ViewportReplaySessionBundleWindowRuntime.Tail(
                source,
                12,
                8,
                8);

            var roundTrip = ViewportReplaySessionBundleRuntime.FromJson(
                ViewportReplaySessionBundleRuntime.ToJson(window));

            Check(
                ViewportReplaySessionBundleRuntime.AreEquivalent(source, window) &&
                window.Manifest == source.Manifest &&
                window.Inputs.SequenceEqual(source.Inputs) &&
                window.Evidence.SequenceEqual(source.Evidence) &&
                window.Audit.SequenceEqual(source.Audit) &&
                ViewportReplaySessionBundleRuntime.AreEquivalent(window, roundTrip),
                $"full-window JSON/equivalence cycle {i} should reproduce the source.");
        }

        for (var i = 0; i < 10; i++)
        {
            var empty = ViewportReplaySessionBundleRuntime.Capture(
                ViewportReplaySessionBundleRuntime.CreateManifest(
                    $"empty-{i}",
                    DateTimeOffset.UnixEpoch,
                    Array.Empty<ViewportInputEvent>(),
                    Array.Empty<ViewportRenderEvidenceManifest>(),
                    Array.Empty<ViewportPresentationAuditEvent>()),
                Array.Empty<ViewportInputEvent>(),
                Array.Empty<ViewportRenderEvidenceManifest>(),
                Array.Empty<ViewportPresentationAuditEvent>());

            var window = ViewportReplaySessionBundleWindowRuntime.Tail(
                empty,
                i,
                i,
                i);

            Check(
                window.IsEmpty &&
                ViewportReplaySessionBundleRuntime.Validate(window).Count == 0 &&
                ViewportReplaySessionBundleWindowRuntime.IsTailOf(empty, window) &&
                ViewportReplaySessionBundleWindowRuntime.Describe(window) ==
                ViewportReplaySessionBundleWindow.Empty,
                $"empty window {i} should remain valid and empty.");
        }

        for (var i = 0; i < 10; i++)
        {
            var inputGuardPassed = true;

            try
            {
                ViewportReplaySessionBundleWindowRuntime.Tail(
                    source,
                    i == 0 ? -1 : i,
                    8,
                    8);
            }
            catch (ArgumentOutOfRangeException)
            {
                inputGuardPassed = i == 0;
            }

            var evidenceGuardPassed = true;

            try
            {
                ViewportReplaySessionBundleWindowRuntime.Tail(
                    source,
                    1,
                    i == 0 ? -1 : 8,
                    8);
            }
            catch (ArgumentOutOfRangeException)
            {
                evidenceGuardPassed = i == 0;
            }

            var auditGuardPassed = true;

            try
            {
                ViewportReplaySessionBundleWindowRuntime.Tail(
                    source,
                    1,
                    8,
                    i == 0 ? -1 : 8);
            }
            catch (ArgumentOutOfRangeException)
            {
                auditGuardPassed = i == 0;
            }

            Check(
                inputGuardPassed &&
                evidenceGuardPassed &&
                auditGuardPassed,
                $"negative-limit guards at iteration {i} should behave deterministically.");
        }

        for (var i = 0; i < 10; i++)
        {
            var outer = ViewportReplaySessionBundleWindowRuntime.Tail(
                source,
                8,
                8,
                8);

            var inner = ViewportReplaySessionBundleWindowRuntime.Tail(
                source,
                i % 7,
                8,
                i % 7);

            Check(
                ViewportReplaySessionBundleWindowRuntime.IsTailOf(outer, inner) &&
                inner.Inputs.All(item => outer.Inputs.Contains(item)) &&
                inner.Evidence.All(item => outer.Evidence.Contains(item)) &&
                inner.Audit.All(item => outer.Audit.Contains(item)) &&
                inner.Inputs.Length <= outer.Inputs.Length &&
                inner.Audit.Length <= outer.Audit.Length,
                $"nested containment {i} should hold.");
        }

        for (var i = 0; i < 10; i++)
        {
            var window = ViewportReplaySessionBundleWindowRuntime.Tail(
                source,
                5 + i % 3,
                8,
                5 + i % 3);

            var json = ViewportReplaySessionBundleRuntime.ToJson(window);
            var restored = ViewportReplaySessionBundleRuntime.FromJson(json);

            Check(
                ViewportReplaySessionBundleRuntime.AreEquivalent(window, restored) &&
                ViewportReplaySessionBundleRuntime.Validate(restored).Count == 0 &&
                json.Contains(""manifest":", StringComparison.Ordinal) &&
                json.Contains(""inputs":", StringComparison.Ordinal) &&
                json.Contains(""evidence":", StringComparison.Ordinal) &&
                json.Contains(""audit":", StringComparison.Ordinal),
                $"repeated diagnostic JSON cycle {i} should be lossless.");
        }

        for (var i = 0; i < 10; i++)
        {
            var first = ViewportReplaySessionBundleWindowRuntime.Tail(
                source,
                4,
                8,
                4);

            var second = ViewportReplaySessionBundleWindowRuntime.Tail(
                source,
                4,
                8,
                4);

            Check(
                ViewportReplaySessionBundleRuntime.AreEquivalent(first, second) &&
                first.Manifest.SessionHash == second.Manifest.SessionHash &&
                ViewportReplaySessionBundleWindowRuntime.Describe(first) ==
                ViewportReplaySessionBundleWindowRuntime.Describe(second),
                $"deterministic repeated window {i} should be identical.");
        }

        for (var i = 0; i < 10; i++)
        {
            var window = ViewportReplaySessionBundleWindowRuntime.Tail(
                source,
                3,
                8,
                3);

            var mutated = window with
            {
                Manifest = window.Manifest with
                {
                    SessionHash = new string((char)('a' + i), 64)
                }
            };

            Check(
                !ViewportReplaySessionBundleRuntime.AreEquivalent(window, mutated) &&
                ViewportReplaySessionBundleRuntime.Validate(mutated).Count != 0,
                $"tampered diagnostic window {i} should be detected.");
        }

        for (var i = 0; i < 10; i++)
        {
            var full = ViewportReplaySessionBundleWindowRuntime.Tail(
                source,
                source.Inputs.Length,
                source.Evidence.Length,
                source.Audit.Length);

            Check(
                ViewportReplaySessionBundleRuntime.AreEquivalent(source, full) &&
                ViewportReplaySessionBundleRuntime.Validate(full).Count == 0 &&
                ViewportReplaySessionBundleWindowRuntime.IsTailOf(source, full),
                $"full-capacity window {i} should equal the source.");
        }

        assert(
            round == 100,
            $"Replay window smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
