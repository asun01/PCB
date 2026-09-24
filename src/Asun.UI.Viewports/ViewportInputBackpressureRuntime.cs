using System.Numerics;

namespace Asun.UI.Viewports;

public enum ViewportInputDropPolicy
{
    DropOldest,
    DropNewest,
    CoalesceMoves
}

public readonly record struct ViewportInputBackpressureSnapshot(
    int Capacity,
    int Pending,
    long Accepted,
    long Dropped,
    long Coalesced,
    bool IsCompleted,
    bool IsCancelled);

public sealed class ViewportInputBackpressureRuntime : IDisposable
{
    private readonly object _sync = new();
    private readonly int _capacity;
    private readonly ViewportInputDropPolicy _dropPolicy;
    private long _accepted;
    private long _dropped;
    private long _coalesced;
    private int _completed;
    private int _cancelled;
    private int _disposed;

    public ViewportInputBackpressureRuntime(
        int capacity = 512,
        ViewportInputDropPolicy dropPolicy = ViewportInputDropPolicy.CoalesceMoves)
    {
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity));

        _capacity = capacity;
        _dropPolicy = dropPolicy;
    }

    public bool IsCompleted => Volatile.Read(ref _completed) != 0;
    public bool IsCancelled => Volatile.Read(ref _cancelled) != 0;

    public ViewportInputBackpressureSnapshot Capture(ViewportInputSubmissionRuntime input)
    {
        ArgumentNullException.ThrowIfNull(input);
        lock (_sync)
        {
            return new ViewportInputBackpressureSnapshot(
                _capacity,
                input.PendingCount,
                _accepted,
                _dropped,
                _coalesced,
                IsCompleted,
                IsCancelled);
        }
    }

    public bool TrySubmit(
        ViewportInputSubmissionRuntime input,
        ViewportInputEventKind kind,
        Vector2 position,
        int wheelDelta = 0,
        ViewportMouseButton button = ViewportMouseButton.Left)
    {
        ArgumentNullException.ThrowIfNull(input);
        ValidatePosition(position);

        lock (_sync)
        {
            ThrowIfUnavailable();

            if (input.IsCompleted || input.IsCancelled)
                return false;

            if (input.PendingCount >= _capacity)
            {
                switch (_dropPolicy)
                {
                    case ViewportInputDropPolicy.DropNewest:
                        _dropped++;
                        return false;
                    case ViewportInputDropPolicy.DropOldest:
                        input.Drain(1);
                        _dropped++;
                        break;
                    case ViewportInputDropPolicy.CoalesceMoves:
                        if (kind == ViewportInputEventKind.PointerMove &&
                            input.TryReplaceLatestMove(position))
                        {
                            _accepted++;
                            _coalesced++;
                            return true;
                        }

                        input.Drain(1);
                        _dropped++;
                        break;
                }
            }

            try
            {
                input.Submit(kind, position, wheelDelta, button);
                _accepted++;
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
    }

    public void Complete(bool cancelPending = false)
    {
        if (Interlocked.Exchange(ref _completed, 1) != 0)
            return;

        if (cancelPending)
        {
            lock (_sync)
            {
                _dropped++;
            }
        }
    }

    public void Complete(
        ViewportInputSubmissionRuntime input,
        bool cancelPending = false)
    {
        ArgumentNullException.ThrowIfNull(input);

        var pending = cancelPending ? input.PendingCount : 0;
        Complete(false);

        if (pending != 0)
        {
            lock (_sync)
            {
                _dropped += pending;
            }
        }

        input.Complete(cancelPending);
    }

    public void Cancel()
    {
        if (Interlocked.Exchange(ref _cancelled, 1) == 0)
            Volatile.Write(ref _completed, 1);
    }

    public void Cancel(ViewportInputSubmissionRuntime input)
    {
        ArgumentNullException.ThrowIfNull(input);

        var pending = input.PendingCount;
        Cancel();

        if (pending != 0)
        {
            lock (_sync)
            {
                _dropped += pending;
            }
        }

        input.Cancel();
    }

    public void Reset()
    {
        lock (_sync)
        {
            ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
            Volatile.Write(ref _completed, 0);
            Volatile.Write(ref _cancelled, 0);
            _accepted = 0;
            _dropped = 0;
            _coalesced = 0;
        }
    }

    public void Reset(ViewportInputSubmissionRuntime input)
    {
        ArgumentNullException.ThrowIfNull(input);
        Reset();
        input.ResetLifecycle();
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 0)
            Cancel();
    }

    private void ThrowIfUnavailable()
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
        if (IsCompleted || IsCancelled)
            throw new InvalidOperationException("The input backpressure runtime is not accepting events.");
    }

    private static void ValidatePosition(Vector2 position)
    {
        if (!float.IsFinite(position.X) || !float.IsFinite(position.Y))
            throw new ArgumentOutOfRangeException(nameof(position));
    }
}
