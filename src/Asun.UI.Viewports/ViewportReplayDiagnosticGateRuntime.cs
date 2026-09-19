namespace Asun.UI.Viewports;

public readonly record struct ViewportReplayDiagnosticGateResult(
    bool IsValid,
    bool ExecutionMatches,
    bool StateMatches,
    bool BundleMatches,
    bool CheckpointMatches,
    bool PresentationEvidenceMatches,
    IReadOnlyList<string> Differences)
{
    public static ViewportReplayDiagnosticGateResult Passed { get; } =
        new(
            true,
            true,
            true,
            true,
            true,
            true,
            Array.Empty<string>());
}

public static class ViewportReplayDiagnosticGateRuntime
{
    public static ViewportReplayDiagnosticGateResult Verify(
        ViewportReplayExecutionStateReport expectedExecution,
        ViewportReplayExecutionStateReport actualExecution,
        ViewportReplaySessionBundle expectedBundle,
        ViewportReplaySessionBundle actualBundle,
        ViewportReplayCheckpoint expectedCheckpoint,
        ViewportReplayCheckpoint actualCheckpoint)
    {
        ArgumentNullException.ThrowIfNull(expectedExecution);
        ArgumentNullException.ThrowIfNull(actualExecution);
        ArgumentNullException.ThrowIfNull(expectedBundle);
        ArgumentNullException.ThrowIfNull(actualBundle);

        var differences = new List<string>();

        var executionMatches =
            ViewportReplayExecutionRuntime.HasSameInput(
                expectedExecution.Execution,
                actualExecution.Execution) &&
            ViewportReplayExecutionRuntime.HasSameResult(
                expectedExecution.Execution,
                actualExecution.Execution);

        if (!executionMatches)
            differences.Add("Execution mismatch.");

        var stateComparison =
            ViewportReplayStateFingerprintRuntime.Compare(
                expectedExecution.FinalState,
                actualExecution.FinalState);

        var stateMatches = stateComparison.IsEquivalent;

        foreach (var difference in stateComparison.Differences)
            differences.Add($"State.{difference}");

        var bundleComparison =
            ViewportReplayBundleVerificationRuntime.Verify(
                expectedBundle,
                actualBundle);

        var bundleMatches = bundleComparison.IsEquivalent;

        foreach (var difference in bundleComparison.Differences)
            differences.Add($"Bundle.{difference}");

        var checkpointComparison =
            ViewportReplayCheckpointRuntime.Compare(
                expectedCheckpoint,
                actualCheckpoint);

        var checkpointMatches =
            checkpointComparison.IsEquivalent;

        foreach (var difference in checkpointComparison.Differences)
            differences.Add($"Checkpoint.{difference}");

        var presentationGate =
            ViewportReplayPresentationEvidenceGateRuntime.Verify(
                expectedBundle.Evidence,
                actualBundle.Evidence,
                expectedBundle.Audit,
                actualBundle.Audit);

        var presentationMatches =
            presentationGate.IsEquivalent;

        foreach (var difference in presentationGate.Differences)
            differences.Add($"Presentation.{difference}");

        var valid =
            executionMatches &&
            stateMatches &&
            bundleMatches &&
            checkpointMatches &&
            presentationMatches;

        return valid
            ? ViewportReplayDiagnosticGateResult.Passed
            : new ViewportReplayDiagnosticGateResult(
                false,
                executionMatches,
                stateMatches,
                bundleMatches,
                checkpointMatches,
                presentationMatches,
                differences);
    }

    public static bool IsValid(
        ViewportReplayDiagnosticGateResult result) =>
        result.IsValid &&
        result.ExecutionMatches &&
        result.StateMatches &&
        result.BundleMatches &&
        result.CheckpointMatches &&
        result.PresentationEvidenceMatches;
}
