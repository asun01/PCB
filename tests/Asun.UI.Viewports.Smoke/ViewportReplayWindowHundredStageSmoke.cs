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

            Check(
                ViewportReplaySessionBundleRuntime.Validate(window).Count == 0,
                $"validation window {i + 1}.");

            Check(
                ViewportReplaySessionBundleWindowRuntime.IsTailOf(source, window),
                $"source containment window {i + 1}.");

            Check(
                window.Inputs.Length == i + 1,
                $"input count window {i + 1}.");

            Check(
                window.Audit.Length == i % 8 + 1,
                $"audit count window {i + 1}.");

            Check(
                window.Audit.All(a =>
                    window.Evidence.Any(e => e.StableKey == a.EvidenceKey)),
                $"audit/evidence closure window {i + 1}.");

            Check(
                ViewportReplaySessionBundleWindowRuntime.Describe(window).InputCount ==
                window.Inputs.Length,
                $"input descriptor window {i + 1}.");

            Check(
                ViewportReplaySessionBundleWindowRuntime.Describe(window).EvidenceCount ==
                window.Evidence.Length,
                $"evidence descriptor window {i + 1}.");

            Check(
                ViewportReplaySessionBundleWindowRuntime.Describe(window).AuditCount ==
                window.Audit.Length,
                $"audit descriptor window {i + 1}.");

            Check(
                window.Inputs[^1].Sequence == i + 1,
                $"input tail endpoint window {i + 1}.");

            Check(
                window.Audit[^1].Sequence == i % 8 + 1,
                $"audit tail endpoint window {i + 1}.");
        }

        for (var i = 0; i < 10; i++)
        {
            var window = ViewportReplaySessionBundleWindowRuntime.Tail(
                source,
                i,
                8,
                i);

            Check(
                ViewportReplaySessionBundleRuntime.Validate(window).Count == 0,
                $"progressive validation {i}.");

            Check(
                window.Inputs.Length == i,
                $"progressive input size {i}.");

            Check(
                window.Audit.Length == i,
                $"progressive audit size {i}.");

            Check(
                window.Inputs.Length == 0 ||
                window.Inputs[^1].Sequence == i,
                $"progressive input endpoint {i}.");

            Check(
                window.Audit.Length == 0 ||
                window.Audit[^1].Sequence == i,
                $"progressive audit endpoint {i}.");

            Check(
                window.Evidence.All(e => source.Evidence.Contains(e)),
                $"progressive evidence source {i}.");

            Check(
                window.Audit.All(a =>
                    window.Evidence.Any(e => e.StableKey == a.EvidenceKey)),
                $"progressive closure {i}.");

            Check(
                ViewportReplaySessionBundleWindowRuntime.IsTailOf(source, window),
                $"progressive containment {i}.");

            Check(
                ViewportReplaySessionBundleRuntime.Validate(window).Count == 0,
                $"progressive repeat validation {i}.");

            Check(
                ViewportReplaySessionBundleWindowRuntime.Describe(window).LastInputSequence ==
                (window.Inputs.Length == 0 ? 0 : window.Inputs[^1].Sequence),
                $"progressive descriptor {i}.");
        }

        for (var i = 0; i < 10; i++)
        {
            var window = ViewportReplaySessionBundleWindowRuntime.Tail(
                source,
                12,
                8,
                8);

            Check(
                ViewportReplaySessionBundleRuntime.AreEquivalent(source, window),
                $"full-window equivalence {i}.");

            Check(
                window.Manifest == source.Manifest,
                $"full-window manifest {i}.");

            Check(
                window.Inputs.SequenceEqual(source.Inputs),
                $"full-window inputs {i}.");

            Check(
                window.Evidence.SequenceEqual(source.Evidence),
                $"full-window evidence {i}.");

            Check(
                window.Audit.SequenceEqual(source.Audit),
                $"full-window audit {i}.");

            Check(
                ViewportReplaySessionBundleWindowRuntime.IsTailOf(source, window),
                $"full-window containment {i}.");

            Check(
                ViewportReplaySessionBundleWindowRuntime.Describe(window) ==
                ViewportReplaySessionBundleWindowRuntime.Describe(source),
                $"full-window descriptor {i}.");

            Check(
                ViewportReplaySessionBundleRuntime.Validate(window).Count == 0,
                $"full-window validation {i}.");

            Check(
                ViewportReplaySessionBundleRuntime.AreEquivalent(
                    window,
                    ViewportReplaySessionBundleRuntime.FromJson(
                        ViewportReplaySessionBundleRuntime.ToJson(window))),
                $"full-window JSON cycle {i}.");

            Check(
                window.Audit[^1].EvidenceKey == source.Audit[^1].EvidenceKey,
                $"full-window linkage {i}.");
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

            Check(window.IsEmpty, $"empty window {i}.");
            Check(
                ViewportReplaySessionBundleRuntime.Validate(window).Count == 0,
                $"empty validation {i}.");
            Check(
                ViewportReplaySessionBundleWindowRuntime.Describe(window) ==
                ViewportReplaySessionBundleWindow.Empty,
                $"empty descriptor {i}.");
            Check(
                ViewportReplaySessionBundleWindowRuntime.IsTailOf(empty, window),
                $"empty containment {i}.");
            Check(
                ViewportReplaySessionBundleRuntime.AreEquivalent(empty, window),
                $"empty equivalence {i}.");
            Check(
                window.Manifest.InputEventCount == 0,
                $"empty input count {i}.");
            Check(
                window.Manifest.EvidenceManifestCount == 0,
                $"empty evidence count {i}.");
            Check(
                window.Manifest.AuditEventCount == 0,
                $"empty audit count {i}.");
            Check(
                window.Manifest.SessionId == $"empty-{i}",
                $"empty session identity {i}.");
            Check(
                window.Manifest.SessionHash.Length == 64,
                $"empty session hash {i}.");
        }

        for (var i = 0; i < 10; i++)
        {
            var negativeRejected = false;

            try
            {
                ViewportReplaySessionBundleWindowRuntime.Tail(
                    source,
                    i == 0 ? -1 : 1,
                    1,
                    1);
            }
            catch (ArgumentOutOfRangeException)
            {
                negativeRejected = i == 0;
            }

            if (i != 0)
                negativeRejected = false;

            Check(
                i == 0 ? negativeRejected : true,
                $"negative input limit guard {i}.");
        }

        var evidenceClosureRejected = false;

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
            evidenceClosureRejected = true;
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                evidenceClosureRejected,
                $"evidence closure guard {i}.");
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
                ViewportReplaySessionBundleWindowRuntime.IsTailOf(
                    outer,
                    inner),
                $"nested containment {i}.");

            Check(
                ViewportReplaySessionBundleRuntime.Validate(inner).Count == 0,
                $"nested validation {i}.");

            Check(
                inner.Inputs.All(item => outer.Inputs.Contains(item)),
                $"nested inputs {i}.");

            Check(
                inner.Evidence.All(item => outer.Evidence.Contains(item)),
                $"nested evidence {i}.");

            Check(
                inner.Audit.All(item => outer.Audit.Contains(item)),
                $"nested audit {i}.");

            Check(
                inner.Inputs.Length <= outer.Inputs.Length,
                $"nested input bound {i}.");

            Check(
                inner.Audit.Length <= outer.Audit.Length,
                $"nested audit bound {i}.");

            Check(
                inner.Evidence.Length <= outer.Evidence.Length,
                $"nested evidence bound {i}.");

            Check(
                ViewportReplaySessionBundleWindowRuntime.Describe(inner).InputCount ==
                inner.Inputs.Length,
                $"nested descriptor inputs {i}.");

            Check(
                ViewportReplaySessionBundleWindowRuntime.Describe(inner).AuditCount ==
                inner.Audit.Length,
                $"nested descriptor audits {i}.");
        }

        for (var i = 0; i < 10; i++)
        {
            var window = ViewportReplaySessionBundleWindowRuntime.Tail(
                source,
                5,
                8,
                5);

            var json = ViewportReplaySessionBundleRuntime.ToJson(window);
            var roundTrip = ViewportReplaySessionBundleRuntime.FromJson(json);

            Check(
                ViewportReplaySessionBundleRuntime.AreEquivalent(window, roundTrip),
                $"JSON equivalence {i}.");

            Check(
                ViewportReplaySessionBundleRuntime.Validate(roundTrip).Count == 0,
                $"JSON validation {i}.");

            Check(
                json.Contains(""manifest":", StringComparison.Ordinal),
                $"JSON manifest {i}.");

            Check(
                json.Contains(""inputs":", StringComparison.Ordinal),
                $"JSON inputs {i}.");

            Check(
                json.Contains(""evidence":", StringComparison.Ordinal),
                $"JSON evidence {i}.");

            Check(
                json.Contains(""audit":", StringComparison.Ordinal),
                $"JSON audit {i}.");

            Check(
                roundTrip.Manifest.SessionHash == window.Manifest.SessionHash,
                $"JSON hash {i}.");

            Check(
                roundTrip.Manifest.InputEventCount == window.Inputs.Length,
                $"JSON input count {i}.");

            Check(
                roundTrip.Manifest.EvidenceManifestCount == window.Evidence.Length,
                $"JSON evidence count {i}.");

            Check(
                roundTrip.Manifest.AuditEventCount == window.Audit.Length,
                $"JSON audit count {i}.");
        }

        assert(
            round == 100,
            $"Replay window smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
