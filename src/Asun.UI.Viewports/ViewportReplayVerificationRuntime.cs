namespace Asun.UI.Viewports;

public readonly record struct ViewportReplayBundleInputVerificationResult(
    bool IsValid,
    bool InputMatches,
    IReadOnlyList<string> Differences)
{
    public static ViewportReplayBundleInputVerificationResult Passed { get; } =
        new(true, true, Array.Empty<string>());
}

public readonly record struct ViewportReplayVerificationResult(
    bool IsValid,
    bool InputMatches,
    bool ResultMatches,
    bool FinalStateMatches,
    IReadOnlyList<string> Differences)
{
    public static ViewportReplayVerificationResult Passed { get; } =
        new(
            true,
            true,
            true,
            true,
            Array.Empty<string>());
}

public static class ViewportReplayVerificationRuntime
{
    public static ViewportReplayVerificationResult Verify(
        ViewportReplayExecutionStateReport expected,
        ViewportReplayExecutionStateReport actual)
    {
        ArgumentNullException.ThrowIfNull(expected);
        ArgumentNullException.ThrowIfNull(actual);

        var differences = new List<string>();

        var inputMatches =
            ViewportReplayExecutionRuntime.HasSameInput(
                expected.Execution,
                actual.Execution);

        if (!inputMatches)
        {
            differences.Add(
                $"Execution.InputHash: expected '{expected.Execution.InputHash}', actual '{actual.Execution.InputHash}'.");
        }

        var resultMatches =
            ViewportReplayExecutionRuntime.HasSameResult(
                expected.Execution,
                actual.Execution);

        if (!resultMatches)
        {
            differences.Add(
                $"Execution.ResultHash: expected '{expected.Execution.ResultHash}', actual '{actual.Execution.ResultHash}'.");
        }

        var finalStateComparison =
            ViewportReplayStateFingerprintRuntime.Compare(
                expected.FinalState,
                actual.FinalState);

        foreach (var difference in finalStateComparison.Differences)
        {
            differences.Add(
                $"FinalState.{difference}");
        }

        var finalStateMatches = finalStateComparison.IsEquivalent;
        var valid =
            inputMatches &&
            resultMatches &&
            finalStateMatches;

        return valid
            ? ViewportReplayVerificationResult.Passed
            : new ViewportReplayVerificationResult(
                false,
                inputMatches,
                resultMatches,
                finalStateMatches,
                differences);
    }

    public static ViewportReplayBundleInputVerificationResult VerifyInputAgainstBundle(
        ViewportReplaySessionBundle expectedBundle,
        ViewportReplayExecutionStateReport actual)
    {
        ArgumentNullException.ThrowIfNull(expectedBundle);
        ArgumentNullException.ThrowIfNull(actual);

        var errors =
            ViewportReplaySessionBundleRuntime.Validate(
                expectedBundle);

        if (errors.Count != 0)
        {
            throw new InvalidOperationException(
                $"Cannot verify against an invalid replay bundle: {errors[0]}");
        }

        var inputMatches =
            string.Equals(
                expectedBundle.Manifest.InputHash,
                actual.Execution.InputHash,
                StringComparison.Ordinal);

        return inputMatches
            ? ViewportReplayBundleInputVerificationResult.Passed
            : new ViewportReplayBundleInputVerificationResult(
                false,
                false,
                new[]
                {
                    $"Bundle.InputHash: expected '{expectedBundle.Manifest.InputHash}', actual '{actual.Execution.InputHash}'."
                });
    }

    public static bool IsValid(
        ViewportReplayVerificationResult result) =>
        result.IsValid &&
        result.InputMatches &&
        result.ResultMatches &&
        result.FinalStateMatches;
}
