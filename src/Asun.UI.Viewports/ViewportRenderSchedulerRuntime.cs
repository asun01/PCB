namespace Asun.UI.Viewports;

public readonly record struct ViewportRenderSubmission(
    long Sequence,
    ViewportDirtyFlags DirtyFlags,
    long Generation);

public readonly record struct ViewportRenderSchedulerStatistics(
    long Submissions,
    long AcceptedFrames,
    long RateLimited,
    long StaleRejected);

public sealed class ViewportRenderSchedulerRuntime
{
    private readonly object _sync = new();
    private readonly ViewportFrameRateGate _rateGate;
    private readonly ViewportPointerCoalescer _pointerCoalescer;
    private readonly SemaphoreSlim _activitySignal = new(0);
    private ViewportDirtyFlags _pendingFlags;
    private long _sequence;
    private long _latestGeneration;
    private ViewportRenderSubmission? _latest;
    private long _submissions;
    private long _acceptedFrames;
    private long _rateLimited;
    private long _staleRejected;

    public ViewportRenderSchedulerRuntime(
        double framesPerSecond = 60)
    {
        _rateGate = new ViewportFrameRateGate(framesPerSecond);
        _pointerCoalescer = new ViewportPointerCoalescer();
    }

    public ViewportDirtyFlags PendingFlags
    {
        get
        {
            lock (_sync)
                return _pendingFlags;
        }
    }

    public ViewportRenderSchedulerStatistics Statistics =>
        new(
            Interlocked.Read(ref _submissions),
            Interlocked.Read(ref _acceptedFrames),
            Interlocked.Read(ref _rateLimited),
            Interlocked.Read(ref _staleRejected));

    public ViewportRenderSubmission? LatestSubmission
    {
        get
        {
            lock (_sync)
                return _latest;
        }
    }

    public void Submit(
        ViewportDirtyFlags flags,
        long generation)
    {
        if (flags == ViewportDirtyFlags.None)
            return;

        lock (_sync)
        {
            Interlocked.Increment(ref _submissions);

            var wasIdle = _pendingFlags == ViewportDirtyFlags.None;
            _pendingFlags |= flags;

            if (wasIdle)
                _activitySignal.Release();

            if (generation >= _latestGeneration)
                _latestGeneration = generation;

            _latest = new ViewportRenderSubmission(
                ++_sequence,
                _pendingFlags,
                _latestGeneration);
        }
    }

    public void SubmitPointer(
        System.Numerics.Vector2 position)
    {
        _pointerCoalescer.Submit(position);
    }

    public bool TryTakeFrame(
        DateTimeOffset now,
        out ViewportRenderSubmission submission,
        long minimumGeneration = 0)
    {
        lock (_sync)
        {
            if (_pendingFlags == ViewportDirtyFlags.None)
            {
                submission = default;
                return false;
            }

            if (_latestGeneration < minimumGeneration)
            {
                Interlocked.Increment(ref _staleRejected);
                submission = default;
                return false;
            }

            if (!_rateGate.TryEnter(now))
            {
                Interlocked.Increment(ref _rateLimited);
                submission = default;
                return false;
            }

            submission = _latest ??
                new ViewportRenderSubmission(
                    ++_sequence,
                    _pendingFlags,
                    0);

            _pendingFlags = ViewportDirtyFlags.None;
            _latest = null;
            _activitySignal.Wait(0);
            Interlocked.Increment(ref _acceptedFrames);
            return true;
        }
    }

    public TimeSpan GetNextFrameDelay(DateTimeOffset now) =>
        _rateGate.GetDelay(now);

    public async ValueTask WaitForActivityAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await _activitySignal
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public bool TryTakePointer(
        out CoalescedPointer pointer) =>
        _pointerCoalescer.TryTakeLatest(out pointer);

    public void Reset() {
        lock (_sync)
        {
            _pendingFlags = ViewportDirtyFlags.None;
            _latest = null;
            _latestGeneration = 0;
            _rateGate.Reset();
            _pointerCoalescer.Clear();

            while (_activitySignal.Wait(0))
            {
            }

            Interlocked.Exchange(ref _submissions, 0);
            Interlocked.Exchange(ref _acceptedFrames, 0);
            Interlocked.Exchange(ref _rateLimited, 0);
            Interlocked.Exchange(ref _staleRejected, 0);
        }
    }
}
