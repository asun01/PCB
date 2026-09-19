namespace Asun.UI.Viewports;

public sealed record ViewportPresentationReplayDiagnosticSnapshot(
    ViewportPresentationSnapshot Presentation,
    ViewportReplaySessionBundle ReplayBundle,
    ViewportReplayStateFingerprint StateFingerprint,
    string DiagnosticHash)
{
    public bool IsStable =>
        Presentation.IsPresentationStable;
}

public static class ViewportPresentationReplayDiagnosticRuntime
{
    public static ViewportPresentationReplayDiagnosticSnapshot Capture<TTile>(
        ViewportPresentationRuntime<TTile> runtime)
    {
        ArgumentNullException.ThrowIfNull(runtime);

        var presentation =
            runtime.Snapshot;

        var bundle =
            runtime.ReplayBundle;

        var state =
            ViewportReplayStateFingerprintRuntime.Capture(
                runtime.Composite);

        var bundleErrors =
            ViewportReplaySessionBundleRuntime.Validate(
                bundle);

        if (bundleErrors.Count != 0)
        {
            throw new InvalidOperationException(
                $"Presentation replay Bundle is invalid: {bundleErrors[0]}");
        }

        var diagnosticHash =
            ViewportRenderEvidenceRuntime.ComputeTextHash(
                string.Join(
                    "|",
                    presentation.Generation,
                    presentation.State,
                    presentation.PendingInput,
                    presentation.PendingDirtyFlags,
                    presentation.Delivery.PresentedGeneration,
                    presentation.Delivery.PresentedSequence,
                    presentation.Frames.Presented,
                    bundle.Manifest.SessionHash,
                    state.StateHash));

        return new ViewportPresentationReplayDiagnosticSnapshot(
            presentation,
            bundle,
            state,
            diagnosticHash);
    }

    public static IReadOnlyList<string> Validate(
        ViewportPresentationReplayDiagnosticSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var errors =
            ViewportReplaySessionBundleRuntime.Validate(
                snapshot.ReplayBundle)
                .ToList();

        if (snapshot.DiagnosticHash.Length != 64)
            errors.Add(
                "Presentation diagnostic hash must be SHA-256 length.");

        if (snapshot.StateFingerprint.StateHash.Length != 64)
            errors.Add(
                "Presentation state fingerprint hash must be SHA-256 length.");

        return errors;
    }

    public static bool IsEquivalent(
        ViewportPresentationReplayDiagnosticSnapshot expected,
        ViewportPresentationReplayDiagnosticSnapshot actual)
    {
        ArgumentNullException.ThrowIfNull(expected);
        ArgumentNullException.ThrowIfNull(actual);

        return string.Equals(
                expected.DiagnosticHash,
                actual.DiagnosticHash,
                StringComparison.Ordinal) &&
            ViewportReplaySessionBundleRuntime.AreEquivalent(
                expected.ReplayBundle,
                actual.ReplayBundle) &&
            ViewportReplayStateFingerprintRuntime.AreEquivalent(
                expected.StateFingerprint,
                actual.StateFingerprint);
    }
}
