namespace Asun.UI.Viewports;

public readonly record struct ViewportRenderSubmission(
    long Sequence,
    ViewportDirtyFlags DirtyFlags,
    long Generation);

public sealed class ViewportRenderSchedulerRuntime
{
    private readonly object _sync = new();
    private readonly ViewportFrameRateGate _rateGate;
    private readonly ViewportPointerCoalescer _pointerCoalescer;
    private ViewportDirtyFlags _pendingFlags;
    private long _sequence;
    private long _latestGeneration;
    private ViewportRenderSubmission? _latest;

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
            _pendingFlags |= flags;
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
            if (_pendingFlags == ViewportDirtyFlags.None ||
                _latestGeneration < minimumGeneration ||
                !_rateGate.TryEnter(now))
            {
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
            return true;
        }
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
        }
    }
}
