namespace Asun.Platform.Core;

/// <summary>
/// A bounded asynchronous work queue. The queue applies backpressure instead of
/// creating an unbounded in-memory backlog.
/// </summary>
public sealed class BoundedWorkQueue<T>
{
    private readonly System.Threading.Channels.Channel<T> _channel;

    public BoundedWorkQueue(int capacity)
    {
        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), capacity, "Capacity must be greater than zero.");
        }

        Capacity = capacity;
        _channel = System.Threading.Channels.Channel.CreateBounded<T>(
            new System.Threading.Channels.BoundedChannelOptions(capacity)
            {
                FullMode = System.Threading.Channels.BoundedChannelFullMode.Wait,
                SingleReader = false,
                SingleWriter = false,
                AllowSynchronousContinuations = false
            });
    }

    public int Capacity { get; }

    public bool IsCompleted => _channel.Reader.Completion.IsCompleted;

    public Task Completion => _channel.Reader.Completion;

    public ValueTask EnqueueAsync(T item, CancellationToken cancellationToken = default) =>
        _channel.Writer.WriteAsync(item, cancellationToken);

    public bool TryEnqueue(T item) =>
        _channel.Writer.TryWrite(item);

    public ValueTask<T> DequeueAsync(CancellationToken cancellationToken = default) =>
        _channel.Reader.ReadAsync(cancellationToken);

    public bool TryDequeue(out T? item) =>
        _channel.Reader.TryRead(out item);

    /// <summary>
    /// Waits for the first available item, then drains additional items that are
    /// already available without waiting for the batch to fill.
    /// </summary>
    public async ValueTask<int> DequeueBatchAsync(
        Memory<T> destination,
        CancellationToken cancellationToken = default)
    {
        if (destination.Length == 0)
            throw new ArgumentException("Destination must contain at least one element.", nameof(destination));

        destination.Span[0] = await _channel.Reader
            .ReadAsync(cancellationToken)
            .ConfigureAwait(false);

        var count = 1;

        while (count < destination.Length &&
               _channel.Reader.TryRead(out var item))
        {
            destination.Span[count++] = item!;
        }

        return count;
    }

    /// <summary>
    /// Removes up to <paramref name="destination"/>.Length currently available
    /// items without waiting for additional work.
    /// </summary>
    public int TryDequeueBatch(Span<T> destination)
    {
        if (destination.Length == 0)
            throw new ArgumentException("Destination must contain at least one element.", nameof(destination));

        var count = 0;

        while (count < destination.Length &&
               _channel.Reader.TryRead(out var item))
        {
            destination[count++] = item!;
        }

        return count;
    }

    public bool TryComplete(Exception? error = null) =>
        _channel.Writer.TryComplete(error);

    public void Complete(Exception? error = null) =>
        _ = TryComplete(error);

    public IAsyncEnumerable<T> ReadAllAsync(CancellationToken cancellationToken = default) =>
        _channel.Reader.ReadAllAsync(cancellationToken);
}
