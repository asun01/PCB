using System.Numerics;

namespace Asun.UI.Viewports;

public enum ViewportInputEventKind
{
    PointerDown,
    PointerMove,
    PointerUp,
    Wheel,
    DoubleClick,
    Escape
}

public readonly record struct ViewportInputEvent(
    long Sequence,
    ViewportInputEventKind Kind,
    Vector2 Position,
    int WheelDelta,
    ViewportMouseButton Button);

public readonly record struct ViewportInputSubmissionSnapshot(
    bool IsCompleted,
    bool IsCancelled,
    int Pending,
    long Submitted,
    long Coalesced);

public sealed class ViewportInputSubmissionRuntime : IDisposable
{
    private readonly object _sync = new();
    private readonly LinkedList<ViewportInputEvent> _queue = new();
    private readonly CancellationTokenSource _lifetime = new();
    private long _sequence;
    private long _submitted;
    private long _coalesced;
    private int _completed;
    private int _disposed;

    public long SubmittedCount
    {
        get { lock (_sync) return _submitted; }
    }

    public long CoalescedCount
    {
        get { lock (_sync) return _coalesced; }
    }

    public bool IsCompleted => Volatile.Read(ref _completed) != 0;

    public bool IsCancelled => _lifetime.IsCancellationRequested;

    public bool HasPending
    {
        get
        {
            lock (_sync)
                return _queue.Count != 0;
        }
    }

    public int PendingCount
    {
        get
        {
            lock (_sync)
                return _queue.Count;
        }
    }

    public ViewportInputSubmissionSnapshot Snapshot()
    {
        lock (_sync)
        {
            return new ViewportInputSubmissionSnapshot(
                IsCompleted,
                IsCancelled,
                _queue.Count,
                _submitted,
                _coalesced);
        }
    }

    public bool TryReplaceLatestMove(Vector2 position)
    {
        ValidatePosition(position);

        lock (_sync)
        {
            ThrowIfDisposedOrCompleted();

            var node = _queue.Last;
            if (node is null ||
                node.Value.Kind != ViewportInputEventKind.PointerMove)
            {
                return false;
            }

            node.Value = node.Value with
            {
                Sequence = ++_sequence,
                Position = position
            };

            _coalesced++;
            return true;
        }
    }

    public long Submit(
        ViewportInputEventKind kind,
        Vector2 position,
        int wheelDelta = 0,
        ViewportMouseButton button = ViewportMouseButton.Left)
    {
        ValidatePosition(position);

        lock (_sync)
        {
            ThrowIfDisposedOrCompleted();

            _submitted++;
            var sequence = ++_sequence;

            if (kind == ViewportInputEventKind.PointerMove &&
                _queue.Last?.Value.Kind == ViewportInputEventKind.PointerMove)
            {
                _queue.RemoveLast();
                _coalesced++;
            }

            _queue.AddLast(
                new ViewportInputEvent(
                    sequence,
                    kind,
                    position,
                    wheelDelta,
                    button));

            Monitor.PulseAll(_sync);
            return sequence;
        }
    }

    public bool TrySubmit(
        ViewportInputEventKind kind,
        Vector2 position,
        int wheelDelta = 0,
        ViewportMouseButton button = ViewportMouseButton.Left)
    {
        if (IsCancelled || IsCompleted || Volatile.Read(ref _disposed) != 0)
            return false;

        try
        {
            Submit(kind, position, wheelDelta, button);
            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
        catch (ObjectDisposedException)
        {
            return false;
        }
    }

    public IReadOnlyList<ViewportInputEvent> Drain(int maxCount = 256)
    {
        if (maxCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxCount));

        lock (_sync)
        {
            var count = Math.Min(maxCount, _queue.Count);
            var events = new List<ViewportInputEvent>(count);

            for (var i = 0; i < count; i++)
            {
                var node = _queue.First!;
                events.Add(node.Value);
                _queue.RemoveFirst();
            }

            return events;
        }
    }

    public async ValueTask<IReadOnlyList<ViewportInputEvent>> WaitAndDrainAsync(
        int maxCount = 256,
        CancellationToken cancellationToken = default)
    {
        if (maxCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxCount));

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            lock (_sync)
            {
                if (_queue.Count != 0)
                {
                    var count = Math.Min(maxCount, _queue.Count);
                    var events = new List<ViewportInputEvent>(count);

                    for (var i = 0; i < count; i++)
                    {
                        var node = _queue.First!;
                        events.Add(node.Value);
                        _queue.RemoveFirst();
                    }

                    return events;
                }

                if (IsCompleted || IsCancelled)
                    return Array.Empty<ViewportInputEvent>();

                Monitor.Wait(_sync, TimeSpan.FromMilliseconds(8));
            }

            await Task.Yield();
        }
    }

    public void Clear()
    {
        lock (_sync)
            _queue.Clear();
    }

    public void Complete(bool cancelPending = false)
    {
        if (Interlocked.Exchange(ref _completed, 1) != 0)
            return;

        lock (_sync)
        {
            if (cancelPending)
                _queue.Clear();

            Monitor.PulseAll(_sync);
        }
    }

    public void Cancel()
    {
        if (!_lifetime.IsCancellationRequested)
            _lifetime.Cancel();

        lock (_sync)
        {
            _queue.Clear();
            Monitor.PulseAll(_sync);
        }

        Volatile.Write(ref _completed, 1);
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
            return;

        Cancel();
        _lifetime.Dispose();
    }

    private void ThrowIfDisposedOrCompleted()
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);

        if (Volatile.Read(ref _completed) != 0)
            throw new InvalidOperationException(
                "The viewport input submission runtime is already completed.");
    }

    private static void ValidatePosition(Vector2 position)
    {
        if (!float.IsFinite(position.X) ||
            !float.IsFinite(position.Y))
        {
            throw new ArgumentOutOfRangeException(nameof(position));
        }
    }
}
