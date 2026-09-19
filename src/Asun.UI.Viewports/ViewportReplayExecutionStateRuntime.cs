namespace Asun.UI.Viewports;

public sealed record ViewportReplayExecutionStateReport(
    ViewportReplayExecutionReport Execution,
    ViewportReplayStateFingerprint InitialState,
    ViewportReplayStateFingerprint FinalState)
{
    public bool StateChanged =>
        !ViewportReplayStateFingerprintRuntime.AreEquivalent(
            InitialState,
            FinalState);

    public string FinalStateHash => FinalState.StateHash;
}

public static class ViewportReplayExecutionStateRuntime
{
    public static ViewportReplayExecutionStateReport Execute<TTile>(
        ViewportCompositeInputRuntime<TTile> input,
        IReadOnlyList<ViewportInputEvent> events)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(events);

        var initial = ViewportReplayStateFingerprintRuntime.Capture(
            input.Composite);

        var execution = ViewportReplayExecutionRuntime.Execute(
            input,
            events);

        var final = ViewportReplayStateFingerprintRuntime.Capture(
            input.Composite);

        return new ViewportReplayExecutionStateReport(
            execution,
            initial,
            final);
    }

    public static ViewportReplayExecutionStateReport Execute<TTile>(
        ViewportCompositeInputRuntime<TTile> input,
        ViewportReplaySessionBundle bundle)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(bundle);

        var initial = ViewportReplayStateFingerprintRuntime.Capture(
            input.Composite);

        var execution = ViewportReplayExecutionRuntime.Execute(
            input,
            bundle);

        var final = ViewportReplayStateFingerprintRuntime.Capture(
            input.Composite);

        return new ViewportReplayExecutionStateReport(
            execution,
            initial,
            final);
    }

    public static bool HasSameFinalState(
        ViewportReplayExecutionStateReport left,
        ViewportReplayExecutionStateReport right) =>
        ViewportReplayStateFingerprintRuntime.AreEquivalent(
            left.FinalState,
            right.FinalState);

    public static ViewportReplayStateComparison CompareFinalState(
        ViewportReplayExecutionStateReport expected,
        ViewportReplayExecutionStateReport actual) =>
        ViewportReplayStateFingerprintRuntime.Compare(
            expected.FinalState,
            actual.FinalState);
}
