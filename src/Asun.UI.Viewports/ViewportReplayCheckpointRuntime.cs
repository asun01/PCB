namespace Asun.UI.Viewports;

public sealed record ViewportReplayCheckpoint(
    int Ordinal,
    int InputCount,
    long LastInputSequence,
    string InputHash,
    string ResultHash,
    string StateHash,
    long Generation)
{
    public bool IsEmpty => InputCount == 0;
}

public readonly record struct ViewportReplayCheckpointComparison(
    bool IsEquivalent,
    IReadOnlyList<string> Differences)
{
    public static ViewportReplayCheckpointComparison Equivalent { get; } =
        new(true, Array.Empty<string>());
}

public static class ViewportReplayCheckpointRuntime
{
    public static ViewportReplayCheckpoint Capture(
        ViewportReplayExecutionStateReport report)
    {
        ArgumentNullException.ThrowIfNull(report);

        var lastSequence =
            report.Execution.Results.Count == 0
                ? 0
                : report.Execution.Results.Count;

        return new ViewportReplayCheckpoint(
            report.Execution.Results.Count,
            report.Execution.Results.Count,
            lastSequence,
            report.Execution.InputHash,
            report.Execution.ResultHash,
            report.FinalState.StateHash,
            report.FinalState.Generation);
    }

    public static ViewportReplayCheckpoint Capture(
        ViewportReplayExecutionStateReport report,
        IReadOnlyList<ViewportInputEvent> inputs)
    {
        ArgumentNullException.ThrowIfNull(report);
        ArgumentNullException.ThrowIfNull(inputs);

        if (inputs.Count != report.Execution.Count)
        {
            throw new ArgumentException(
                "Checkpoint input count must match the execution result count.",
                nameof(inputs));
        }

        var lastSequence =
            inputs.Count == 0
                ? 0
                : inputs[^1].Sequence;

        return new ViewportReplayCheckpoint(
            inputs.Count,
            inputs.Count,
            lastSequence,
            report.Execution.InputHash,
            report.Execution.ResultHash,
            report.FinalState.StateHash,
            report.FinalState.Generation);
    }

    public static IReadOnlyList<string> Validate(
        ViewportReplayCheckpoint checkpoint)
    {
        var errors = new List<string>();

        if (checkpoint.Ordinal < 0)
            errors.Add("Checkpoint ordinal must be non-negative.");

        if (checkpoint.InputCount < 0)
            errors.Add("Checkpoint input count must be non-negative.");

        if (checkpoint.LastInputSequence < 0)
            errors.Add("Checkpoint input sequence must be non-negative.");

        if (checkpoint.Generation < 0)
            errors.Add("Checkpoint generation must be non-negative.");

        if (checkpoint.InputHash.Length != 64 ||
            checkpoint.ResultHash.Length != 64 ||
            checkpoint.StateHash.Length != 64)
        {
            errors.Add("Checkpoint hashes must be SHA-256 length.");
        }

        if (checkpoint.InputCount == 0 &&
            checkpoint.LastInputSequence != 0)
        {
            errors.Add(
                "Empty checkpoint must have zero last input sequence.");
        }

        return errors;
    }

    public static ViewportReplayCheckpointComparison Compare(
        ViewportReplayCheckpoint expected,
        ViewportReplayCheckpoint actual)
    {
        var differences = new List<string>();

        CompareValue(differences, "Ordinal", expected.Ordinal, actual.Ordinal);
        CompareValue(differences, "InputCount", expected.InputCount, actual.InputCount);
        CompareValue(differences, "LastInputSequence", expected.LastInputSequence, actual.LastInputSequence);
        CompareValue(differences, "InputHash", expected.InputHash, actual.InputHash);
        CompareValue(differences, "ResultHash", expected.ResultHash, actual.ResultHash);
        CompareValue(differences, "StateHash", expected.StateHash, actual.StateHash);
        CompareValue(differences, "Generation", expected.Generation, actual.Generation);

        return differences.Count == 0
            ? ViewportReplayCheckpointComparison.Equivalent
            : new ViewportReplayCheckpointComparison(false, differences);
    }

    public static bool AreEquivalent(
        ViewportReplayCheckpoint expected,
        ViewportReplayCheckpoint actual) =>
        Compare(expected, actual).IsEquivalent;

    private static void CompareValue<T>(
        ICollection<string> differences,
        string name,
        T expected,
        T actual)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
        {
            differences.Add(
                $"{name}: expected '{expected}', actual '{actual}'.");
        }
    }
}

public sealed class ViewportReplayCheckpointStore
{
    private readonly object _sync = new();
    private readonly int _capacity;
    private readonly List<ViewportReplayCheckpoint> _entries = new();

    public ViewportReplayCheckpointStore(int capacity = 128)
    {
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity));

        _capacity = capacity;
    }

    public int Capacity => _capacity;

    public int Count
    {
        get
        {
            lock (_sync)
                return _entries.Count;
        }
    }

    public ViewportReplayCheckpoint? Latest
    {
        get
        {
            lock (_sync)
                return _entries.Count == 0 ? null : _entries[^1];
        }
    }

    public void Add(ViewportReplayCheckpoint checkpoint)
    {
        var errors = ViewportReplayCheckpointRuntime.Validate(checkpoint);

        if (errors.Count != 0)
            throw new InvalidOperationException(
                $"Invalid replay checkpoint: {errors[0]}");

        lock (_sync)
        {
            if (_entries.Count != 0)
            {
                var previous = _entries[^1];

                if (checkpoint.Ordinal <= previous.Ordinal ||
                    checkpoint.InputCount < previous.InputCount ||
                    checkpoint.LastInputSequence < previous.LastInputSequence)
                {
                    throw new InvalidOperationException(
                        "Replay checkpoints must increase monotonically.");
                }
            }

            if (_entries.Count >= _capacity)
                _entries.RemoveAt(0);

            _entries.Add(checkpoint);
        }
    }

    public IReadOnlyList<ViewportReplayCheckpoint> Snapshot()
    {
        lock (_sync)
            return _entries.ToArray();
    }

    public void Clear()
    {
        lock (_sync)
            _entries.Clear();
    }
}
