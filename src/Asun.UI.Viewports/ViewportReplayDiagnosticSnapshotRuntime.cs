namespace Asun.UI.Viewports;

public sealed record ViewportReplayDiagnosticSnapshot(
    ViewportReplayExecutionStateReport Execution,
    ViewportReplaySessionBundle Bundle,
    ViewportReplayCheckpoint Checkpoint,
    string DiagnosticHash)
{
    public string FinalStateHash =>
        Execution.FinalState.StateHash;

    public long Generation =>
        Execution.FinalState.Generation;
}

public readonly record struct ViewportReplayDiagnosticSnapshotComparison(
    bool IsEquivalent,
    IReadOnlyList<string> Differences)
{
    public static ViewportReplayDiagnosticSnapshotComparison Equivalent { get; } =
        new(true, Array.Empty<string>());
}

public static class ViewportReplayDiagnosticSnapshotRuntime
{
    public static ViewportReplayDiagnosticSnapshot Capture(
        ViewportReplayExecutionStateReport execution,
        ViewportReplaySessionBundle bundle,
        ViewportReplayCheckpoint checkpoint)
    {
        ArgumentNullException.ThrowIfNull(execution);
        ArgumentNullException.ThrowIfNull(bundle);

        ValidateCrossLayerConsistency(
            execution,
            bundle,
            checkpoint);

        var diagnosticHash =
            ViewportRenderEvidenceRuntime.ComputeTextHash(
                string.Join(
                    "|",
                    execution.Execution.InputHash,
                    execution.Execution.ResultHash,
                    execution.FinalState.StateHash,
                    bundle.Manifest.SessionHash,
                    checkpoint.InputHash,
                    checkpoint.ResultHash,
                    checkpoint.StateHash,
                    checkpoint.Generation));

        return new ViewportReplayDiagnosticSnapshot(
            execution,
            bundle,
            checkpoint,
            diagnosticHash);
    }

    public static IReadOnlyList<string> Validate(
        ViewportReplayDiagnosticSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var errors = new List<string>();

        errors.AddRange(
            ViewportReplaySessionBundleRuntime.Validate(
                snapshot.Bundle));

        errors.AddRange(
            ViewportReplayCheckpointRuntime.Validate(
                snapshot.Checkpoint));

        try
        {
            ValidateCrossLayerConsistency(
                snapshot.Execution,
                snapshot.Bundle,
                snapshot.Checkpoint);
        }
        catch (Exception exception) when (
            exception is InvalidOperationException or
            ArgumentException)
        {
            errors.Add(exception.Message);
        }

        var recomputed =
            ComputeDiagnosticHash(snapshot);

        if (!string.Equals(
                recomputed,
                snapshot.DiagnosticHash,
                StringComparison.Ordinal))
        {
            errors.Add(
                "Diagnostic hash does not match snapshot content.");
        }

        return errors;
    }

    public static ViewportReplayDiagnosticSnapshotComparison Compare(
        ViewportReplayDiagnosticSnapshot expected,
        ViewportReplayDiagnosticSnapshot actual)
    {
        var differences = new List<string>();

        var replay =
            ViewportReplayVerificationRuntime.Verify(
                expected.Execution,
                actual.Execution);

        foreach (var difference in replay.Differences)
            differences.Add($"Execution.{difference}");

        var bundle =
            ViewportReplayBundleVerificationRuntime.Verify(
                expected.Bundle,
                actual.Bundle);

        foreach (var difference in bundle.Differences)
            differences.Add($"Bundle.{difference}");

        var checkpoint =
            ViewportReplayCheckpointRuntime.Compare(
                expected.Checkpoint,
                actual.Checkpoint);

        foreach (var difference in checkpoint.Differences)
            differences.Add($"Checkpoint.{difference}");

        CompareValue(
            differences,
            "DiagnosticHash",
            expected.DiagnosticHash,
            actual.DiagnosticHash);

        return differences.Count == 0
            ? ViewportReplayDiagnosticSnapshotComparison.Equivalent
            : new ViewportReplayDiagnosticSnapshotComparison(
                false,
                differences);
    }

    public static bool AreEquivalent(
        ViewportReplayDiagnosticSnapshot expected,
        ViewportReplayDiagnosticSnapshot actual) =>
        Compare(expected, actual).IsEquivalent;

    public static string ComputeDiagnosticHash(
        ViewportReplayDiagnosticSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        return ComputeDiagnosticHashParts(
            snapshot.Execution,
            snapshot.Bundle,
            snapshot.Checkpoint);
    }

    private static string ComputeDiagnosticHash(
        ViewportReplayDiagnosticSnapshot snapshot,
        bool unused = false) =>
        ComputeDiagnosticHashParts(
            snapshot.Execution,
            snapshot.Bundle,
            snapshot.Checkpoint);

    private static string ComputeDiagnosticHashParts(
        ViewportReplayExecutionStateReport execution,
        ViewportReplaySessionBundle bundle,
        ViewportReplayCheckpoint checkpoint) =>
        ViewportRenderEvidenceRuntime.ComputeTextHash(
            string.Join(
                "|",
                execution.Execution.InputHash,
                execution.Execution.ResultHash,
                execution.FinalState.StateHash,
                bundle.Manifest.SessionHash,
                checkpoint.InputHash,
                checkpoint.ResultHash,
                checkpoint.StateHash,
                checkpoint.Generation));

    private static void ValidateCrossLayerConsistency(
        ViewportReplayExecutionStateReport execution,
        ViewportReplaySessionBundle bundle,
        ViewportReplayCheckpoint checkpoint)
    {
        var bundleErrors =
            ViewportReplaySessionBundleRuntime.Validate(bundle);

        if (bundleErrors.Count != 0)
            throw new InvalidOperationException(
                $"Replay Bundle is invalid: {bundleErrors[0]}");

        var checkpointErrors =
            ViewportReplayCheckpointRuntime.Validate(checkpoint);

        if (checkpointErrors.Count != 0)
            throw new InvalidOperationException(
                $"Replay Checkpoint is invalid: {checkpointErrors[0]}");

        if (!string.Equals(
                execution.Execution.InputHash,
                bundle.Manifest.InputHash,
                StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "Execution input hash does not match Bundle input hash.");
        }

        if (!string.Equals(
                execution.Execution.InputHash,
                checkpoint.InputHash,
                StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "Execution input hash does not match Checkpoint input hash.");
        }

        if (!string.Equals(
                execution.Execution.ResultHash,
                checkpoint.ResultHash,
                StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "Execution result hash does not match Checkpoint result hash.");
        }

        if (!string.Equals(
                execution.FinalState.StateHash,
                checkpoint.StateHash,
                StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "Execution final state hash does not match Checkpoint state hash.");
        }
    }

    private static void CompareValue<T>(
        ICollection<string> differences,
        string name,
        T expected,
        T actual)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
            differences.Add(
                $"{name}: expected '{expected}', actual '{actual}'.");
    }
}
