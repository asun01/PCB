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

    public ValueTask EnqueueAsync(T item, CancellationToken cancellationToken = default) =>
        _channel.Writer.WriteAsync(item, cancellationToken);

    public bool TryEnqueue(T item) =>
        _channel.Writer.TryWrite(item);

    public ValueTask<T> DequeueAsync(CancellationToken cancellationToken = default) =>
        _channel.Reader.ReadAsync(cancellationToken);

    public bool TryDequeue(out T? item) =>
        _channel.Reader.TryRead(out item);

    public void Complete(Exception? error = null) =>
        _channel.Writer.TryComplete(error);

    public IAsyncEnumerable<T> ReadAllAsync(CancellationToken cancellationToken = default) =>
        _channel.Reader.ReadAllAsync(cancellationToken);
}
