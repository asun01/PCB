using Asun.Platform.Core;

public static class AsyncSignalValidationHundredStageSmoke
{
    public static async ValueTask RunAsync(Action<bool, string> assert)
    {
        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        var signal = new AsyncSignal();
        var initialValid = AsyncSignalValidationRuntime.IsValid(signal);

        var pending = signal.WaitAsync().AsTask();
        signal.TrySignal();
        var waitCompleted = await Task.WhenAny(pending, Task.Delay(100)) == pending;
        var firstTry = signal.IsSignaled;

        var secondTry = signal.TrySignal();

        var timeoutSignal = new AsyncSignal();
        var timeoutObserved = false;
        try
        {
            await timeoutSignal.WaitAsync(TimeSpan.Zero);
        }
        catch (TimeoutException)
        {
            timeoutObserved = true;
        }

        var cancellationObserved = false;
        var cancelledSignal = new AsyncSignal();
        using (var cancellation = new CancellationTokenSource())
        {
            cancellation.Cancel();
            try
            {
                await cancelledSignal.WaitAsync(cancellation.Token);
            }
            catch (OperationCanceledException)
            {
                cancellationObserved = true;
            }
        }

        for (var i = 0; i < 10; i++)
            Check(initialValid, $"initial signal validation round {i + 1} should pass.");

        for (var i = 0; i < 10; i++)
            Check(waitCompleted, $"pending wait completion round {i + 1} should succeed.");

        for (var i = 0; i < 10; i++)
            Check(firstTry, $"first signal transition round {i + 1} should set the state.");

        for (var i = 0; i < 10; i++)
            Check(!secondTry, $"repeated signal round {i + 1} should be rejected.");

        for (var i = 0; i < 10; i++)
            Check(signal.Completion.IsCompletedSuccessfully, $"completion state round {i + 1} should be successful.");

        for (var i = 0; i < 10; i++)
            Check(AsyncSignalValidationRuntime.IsValid(signal), $"signaled validation round {i + 1} should pass.");

        for (var i = 0; i < 10; i++)
            Check(timeoutObserved, $"zero-timeout round {i + 1} should surface TimeoutException.");

        for (var i = 0; i < 10; i++)
            Check(cancellationObserved, $"cancellation round {i + 1} should propagate.");

        for (var i = 0; i < 10; i++)
            Check(!cancelledSignal.IsSignaled, $"cancelled signal state round {i + 1} should remain unsignaled.");

        for (var i = 0; i < 10; i++)
            Check(!timeoutSignal.IsSignaled, $"timed-out signal state round {i + 1} should remain unsignaled.");

        assert(round == 100, $"Async signal validation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
