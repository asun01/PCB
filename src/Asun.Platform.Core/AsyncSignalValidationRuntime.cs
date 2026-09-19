namespace Asun.Platform.Core;

public static class AsyncSignalValidationRuntime
{
    public static IReadOnlyList<string> Validate(AsyncSignal signal)
    {
        ArgumentNullException.ThrowIfNull(signal);

        var errors = new List<string>();

        if (signal.IsSignaled != signal.Completion.IsCompletedSuccessfully)
            errors.Add("Signal state and completion state must agree.");

        if (signal.IsSignaled && !signal.Completion.IsCompleted)
            errors.Add("A signaled async signal must have completed its completion task.");

        return errors;
    }

    public static bool IsValid(AsyncSignal signal) =>
        Validate(signal).Count == 0;
}
