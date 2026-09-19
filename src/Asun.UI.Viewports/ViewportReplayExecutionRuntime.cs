namespace Asun.UI.Viewports;

public sealed record ViewportReplayExecutionReport(
    IReadOnlyList<ViewportCompositeInputResult> Results,
    long InitialGeneration,
    long FinalGeneration,
    string InputHash,
    string ResultHash,
    int TransformChanges,
    int DocumentChanges,
    int SelectionChanges,
    int DirtyEvents)
{
    public long LastInputSequence { get; init; }

    public int Count => Results.Count;

    public bool IsEmpty => Results.Count == 0;
}

public static class ViewportReplayExecutionRuntime
{
    public static ViewportReplayExecutionReport Execute<TTile>(
        ViewportCompositeInputRuntime<TTile> input,
        IReadOnlyList<ViewportInputEvent> events)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(events);

        ValidateEvents(events);

        var initialGeneration = input.Composite.Generation;
        var results = new List<ViewportCompositeInputResult>(events.Count);

        foreach (var item in events)
            results.Add(input.Apply(item));

        var finalGeneration = input.Composite.Generation;

        var inputHash = ViewportRenderEvidenceRuntime.ComputeTextHash(
            string.Join(
                "\n",
                events.Select(item =>
                    $"{item.Sequence}|{item.Kind}|{item.Position.X:R}|{item.Position.Y:R}|{item.WheelDelta}|{item.Button}")));

        var resultHash = ViewportRenderEvidenceRuntime.ComputeTextHash(
            string.Join(
                "\n",
                results.Select(item =>
                    $"{item.Kind}|{item.ViewportPoint.X:R}|{item.ViewportPoint.Y:R}|{item.ImagePoint.X:R}|{item.ImagePoint.Y:R}|{item.TransformChanged}|{item.DocumentChanged}|{item.SelectionChanged}|{item.DirtyFlags}")));

        return new ViewportReplayExecutionReport(
            results.ToArray(),
            initialGeneration,
            finalGeneration,
            inputHash,
            resultHash,
            results.Count(item => item.TransformChanged),
            results.Count(item => item.DocumentChanged),
            results.Count(item => item.SelectionChanged),
            results.Count(item => item.DirtyFlags != ViewportDirtyFlags.None))
        {
            LastInputSequence =
                events.Count == 0
                    ? 0
                    : events[^1].Sequence
        };
    }

    public static ViewportReplayExecutionReport Execute<TTile>(
        ViewportCompositeInputRuntime<TTile> input,
        ViewportReplaySessionBundle bundle)
    {
        ArgumentNullException.ThrowIfNull(input);
        var errors = ViewportReplaySessionBundleRuntime.Validate(bundle);

        if (errors.Count != 0)
        {
            throw new InvalidOperationException(
                $"Cannot execute an invalid replay bundle: {errors[0]}");
        }

        return Execute(input, bundle.Inputs);
    }

    public static bool HasSameInput(
        ViewportReplayExecutionReport left,
        ViewportReplayExecutionReport right) =>
        string.Equals(
            left.InputHash,
            right.InputHash,
            StringComparison.Ordinal);

    public static bool HasSameResult(
        ViewportReplayExecutionReport left,
        ViewportReplayExecutionReport right) =>
        string.Equals(
            left.ResultHash,
            right.ResultHash,
            StringComparison.Ordinal);

    private static void ValidateEvents(
        IReadOnlyList<ViewportInputEvent> events)
    {
        long previousSequence = 0;

        foreach (var item in events)
        {
            if (item.Sequence <= previousSequence)
            {
                throw new InvalidOperationException(
                    "Replay input sequence must increase strictly.");
            }

            previousSequence = item.Sequence;

            if (!float.IsFinite(item.Position.X) ||
                !float.IsFinite(item.Position.Y))
            {
                throw new ArgumentOutOfRangeException(nameof(events));
            }

            if (!Enum.IsDefined(item.Kind) ||
                !Enum.IsDefined(item.Button))
            {
                throw new ArgumentOutOfRangeException(nameof(events));
            }
        }
    }
}
