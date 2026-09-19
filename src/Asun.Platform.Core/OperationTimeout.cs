namespace Asun.Platform.Core;

/// <summary>
/// Executes an operation with a caller-defined timeout while preserving
/// cancellation semantics. It does not retry or reinterpret failures.
/// </summary>
public static class OperationTimeout
{
    public static async ValueTask<T> ExecuteAsync<T>(
        Func<CancellationToken, ValueTask<T>> operation,
        TimeSpan timeout,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(operation);

        if (timeout <= TimeSpan.Zero && timeout != Timeout.InfiniteTimeSpan)
            throw new ArgumentOutOfRangeException(nameof(timeout));

        using var timeoutSource = timeout == Timeout.InfiniteTimeSpan
            ? null
            : new CancellationTokenSource(timeout);

        using var linkedSource = timeoutSource is null
            ? null
            : CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                timeoutSource.Token);

        var token = linkedSource?.Token ?? cancellationToken;

        try
        {
            return await operation(token);
        }
        catch (OperationCanceledException) when (
            timeoutSource is not null &&
            timeoutSource.IsCancellationRequested &&
            !cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException($"The operation exceeded the configured timeout of {timeout}.");
        }
    }

    public static async ValueTask ExecuteAsync(
        Func<CancellationToken, ValueTask> operation,
        TimeSpan timeout,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(operation);

        if (timeout <= TimeSpan.Zero && timeout != Timeout.InfiniteTimeSpan)
            throw new ArgumentOutOfRangeException(nameof(timeout));

        using var timeoutSource = timeout == Timeout.InfiniteTimeSpan
            ? null
            : new CancellationTokenSource(timeout);

        using var linkedSource = timeoutSource is null
            ? null
            : CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                timeoutSource.Token);

        var token = linkedSource?.Token ?? cancellationToken;

        try
        {
            await operation(token);
        }
        catch (OperationCanceledException) when (
            timeoutSource is not null &&
            timeoutSource.IsCancellationRequested &&
            !cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException($"The operation exceeded the configured timeout of {timeout}.");
        }
    }
}
