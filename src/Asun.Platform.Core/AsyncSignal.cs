namespace Asun.Platform.Core;

/// <summary>
/// One-shot asynchronous signal. Signaling is idempotent and waiting is cancellation-aware.
/// </summary>
public sealed class AsyncSignal
{
    private readonly TaskCompletionSource<bool> _completion =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    public bool IsSignaled =>
        _completion.Task.IsCompletedSuccessfully;

    public Task Completion =>
        _completion.Task;

    public bool TrySignal() =>
        _completion.TrySetResult(true);

    public void Signal() =>
        _ = TrySignal();

    public async ValueTask WaitAsync(CancellationToken cancellationToken = default)
    {
        if (!cancellationToken.CanBeCanceled)
        {
            await _completion.Task.ConfigureAwait(false);
            return;
        }

        await _completion.Task.WaitAsync(cancellationToken).ConfigureAwait(false);
    }
}
