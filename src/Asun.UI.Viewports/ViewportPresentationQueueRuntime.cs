namespace Asun.UI.Viewports;

public readonly record struct ViewportPresentationSubmissionToken(
    long Generation,
    long Sequence);

public sealed class ViewportPresentationPacket<TTile>
{
    internal ViewportPresentationPacket(
        ViewportPresentationSubmissionToken token,
        ViewportRenderPipelineFrame<TTile> frame)
    {
        Token = token;
        Frame = frame;
    }

    public ViewportPresentationSubmissionToken Token { get; }

    public ViewportRenderPipelineFrame<TTile> Frame { get; }

    public ViewportRenderCommandStream CommandStream =>
        Frame.CommandStream;
}

public readonly record struct ViewportPresentationQueueStatistics(
    long Enqueued,
    long Dequeued,
    long Presented,
    long Dropped,
    long StaleRejected,
    long Cancelled,
    int Pending,
    long? LatestGeneration,
    long? PresentedGeneration,
    long? PresentedSequence,
    long? InFlightGeneration,
    long? InFlightSequence);

public sealed class ViewportPresentationQueueRuntime<TTile> : IDisposable
{
    private readonly object _sync = new();
    private readonly Queue<ViewportPresentationPacket<TTile>> _pending = new();
    private readonly SemaphoreSlim _activitySignal = new(0, 1);
    private readonly int _capacity;
    private long _submissionSequence;
    private long _latestSubmissionSequence;
    private CancellationTokenSource? _inFlightCancellation;
    private long _enqueued;
    private long _dequeued;
    private long _presented;
    private long _dropped;
    private long _staleRejected;
    private long _cancelled;
    private long? _latestGeneration;
    private ViewportPresentationPacket<TTile>? _inFlight;
    private long? _presentedGeneration;
    private long? _presentedSequence;
    private int _disposed;

    public ViewportPresentationQueueRuntime(int capacity = 2)
    {
        if (capacity < 1)
            throw new ArgumentOutOfRangeException(nameof(capacity));

        _capacity = capacity;
    }

    public int Capacity => _capacity;

    public ViewportPresentationQueueStatistics Statistics
    {
        get
        {
            lock (_sync)
            {
                return new(
                    _enqueued,
                    _dequeued,
                    _presented,
                    _dropped,
                    _staleRejected,
                    _cancelled,
                    _pending.Count,
                    _latestGeneration,
                    _presentedGeneration,
                    _presentedSequence,
                    _inFlight?.Token.Generation,
                    _inFlight?.Token.Sequence);
            }
        }
    }

    public bool TryEnqueue(
        ViewportRenderPipelineFrame<TTile> frame,
        out ViewportPresentationPacket<TTile> packet)
    {
        ArgumentNullException.ThrowIfNull(frame);

        lock (_sync)
        {
            ThrowIfDisposed();

            var generation = frame.Composite.Generation;

            if (_latestGeneration is long latest &&
                generation < latest)
            {
                _staleRejected++;
                packet = default!;
                return false;
            }

            _latestGeneration = generation;

            while (_pending.Count != 0 &&
                   _pending.Peek().Frame.Composite.Generation < generation)
            {
                _pending.Dequeue();
                _dropped++;
            }

            while (_pending.Count >= _capacity)
            {
                _pending.Dequeue();
                _dropped++;
            }

            packet = new ViewportPresentationPacket<TTile>(
                new ViewportPresentationSubmissionToken(
                    generation,
                    ++_submissionSequence),
                frame);

            _latestSubmissionSequence = packet.Token.Sequence;

            if (_inFlight is not null &&
                packet.Token.Sequence > _inFlight.Token.Sequence)
            {
                _inFlightCancellation?.Cancel();
            }

            var wasEmpty = _pending.Count == 0;
            _pending.Enqueue(packet);
            _enqueued++;

            if (wasEmpty)
                _activitySignal.Release();

            return true;
        }
    }

    public bool TryTakeLatest(
        out ViewportPresentationPacket<TTile> packet)
    {
        lock (_sync)
        {
            ThrowIfDisposed();

            if (_inFlight is not null ||
                _pending.Count == 0)
            {
                packet = default!;
                return false;
            }

            packet = _pending.Dequeue();

            while (_pending.Count != 0)
            {
                packet = _pending.Dequeue();
                _dropped++;
            }

            _activitySignal.Wait(0);
            _inFlight = packet;
            _inFlightCancellation = new CancellationTokenSource();
            _dequeued++;
            return true;
        }
    }

    public CancellationToken GetInFlightCancellationToken(
        ViewportPresentationSubmissionToken token)
    {
        lock (_sync)
        {
            ThrowIfDisposed();

            if (_inFlight is null ||
                _inFlight.Token != token ||
                _inFlightCancellation is null)
                throw new InvalidOperationException(
                    "The requested presentation token is not in flight.");

            return _inFlightCancellation.Token;
        }
    }

    public bool IsCurrent(ViewportPresentationSubmissionToken token)
    {
        lock (_sync)
        {
            ThrowIfDisposed();

            return _inFlight is not null &&
                _inFlight.Token == token &&
                _latestSubmissionSequence == token.Sequence;
        }
    }

    public async ValueTask WaitForActivityAsync(
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        await _activitySignal
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public bool TryAcknowledgePresented(
        ViewportPresentationSubmissionToken token)
    {
        lock (_sync)
        {
            ThrowIfDisposed();

            if (_inFlight is null ||
                _inFlight.Token != token ||
                _latestSubmissionSequence != token.Sequence)
                return false;

            if (_presentedSequence is long presented &&
                token.Sequence <= presented)
                return false;

            if (_presentedGeneration is long presentedGeneration &&
                token.Generation < presentedGeneration)
                return false;

            _presentedGeneration = token.Generation;
            _presentedSequence = token.Sequence;
            _inFlight = null;
            _inFlightCancellation?.Dispose();
            _inFlightCancellation = null;
            _presented++;
            return true;
        }
    }

    public bool TryCancel(
        ViewportPresentationSubmissionToken token)
    {
        lock (_sync)
        {
            ThrowIfDisposed();

            if (_inFlight is null ||
                _inFlight.Token != token)
                return false;

            _inFlight = null;
            _inFlightCancellation?.Dispose();
            _inFlightCancellation = null;
            _cancelled++;
            return true;
        }
    }

    public void Reset()
    {
        lock (_sync)
        {
            ThrowIfDisposed();

            _pending.Clear();
            _inFlight = null;
            _inFlightCancellation?.Cancel();
            _inFlightCancellation?.Dispose();
            _inFlightCancellation = null;
            _activitySignal.Wait(0);
            _enqueued = 0;
            _dequeued = 0;
            _presented = 0;
            _dropped = 0;
            _staleRejected = 0;
            _cancelled = 0;
            _latestGeneration = null;
            _presentedGeneration = null;
            _presentedSequence = null;
        }
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
            return;

        lock (_sync)
        {
            _pending.Clear();
            _inFlight = null;
            _inFlightCancellation?.Cancel();
            _inFlightCancellation?.Dispose();
            _inFlightCancellation = null;
            _activitySignal.Dispose();
        }
    }

    private void ThrowIfDisposed() =>
        ObjectDisposedException.ThrowIf(_disposed != 0, this);
}
