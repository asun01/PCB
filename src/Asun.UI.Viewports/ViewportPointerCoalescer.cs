using System.Numerics;

namespace Asun.UI.Viewports;

public readonly record struct CoalescedPointer(
    long Sequence,
    Vector2 Position);

public sealed class ViewportPointerCoalescer
{
    private readonly object _sync = new();
    private long _nextSequence;
    private CoalescedPointer? _latest;

    public long SubmittedCount { get; private set; }

    public long CoalescedCount { get; private set; }

    public void Submit(Vector2 position)
    {
        if (!float.IsFinite(position.X) || !float.IsFinite(position.Y))
            throw new ArgumentOutOfRangeException(nameof(position));

        lock (_sync)
        {
            SubmittedCount++;

            if (_latest is not null)
                CoalescedCount++;

            _latest = new CoalescedPointer(
                ++_nextSequence,
                position);
        }
    }

    public bool TryTakeLatest(out CoalescedPointer pointer)
    {
        lock (_sync)
        {
            if (_latest is null)
            {
                pointer = default;
                return false;
            }

            pointer = _latest.Value;
            _latest = null;
            return true;
        }
    }

    public bool HasPending
    {
        get
        {
            lock (_sync)
                return _latest is not null;
        }
    }

    public void Clear()
    {
        lock (_sync)
            _latest = null;
    }
}
