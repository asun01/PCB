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

public sealed class ViewportInputSubmissionRuntime
{
    private readonly object _sync = new();
    private readonly LinkedList<ViewportInputEvent> _queue = new();
    private long _sequence;
    private long _submitted;
    private long _coalesced;

    public long SubmittedCount
    {
        get { lock (_sync) return _submitted; }
    }

    public long CoalescedCount
    {
        get { lock (_sync) return _coalesced; }
    }

    public bool HasPending
    {
        get
        {
            lock (_sync)
                return _queue.Count != 0;
        }
    }

    public long Submit(
        ViewportInputEventKind kind,
        Vector2 position,
        int wheelDelta = 0,
        ViewportMouseButton button = ViewportMouseButton.Left)
    {
        if (!float.IsFinite(position.X) ||
            !float.IsFinite(position.Y))
        {
            throw new ArgumentOutOfRangeException(nameof(position));
        }

        lock (_sync)
        {
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

            return sequence;
        }
    }

    public IReadOnlyList<ViewportInputEvent> Drain(
        int maxCount = 256)
    {
        if (maxCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxCount));

        lock (_sync)
        {
            var count = Math.Min(maxCount, _queue.Count);
            var events = new List<ViewportInputEvent>(count);

            for (var i = 0; i < count; i++)
                var node = _queue.First!;
                events.Add(node.Value);
                _queue.RemoveFirst();

            return events;
        }
    }

    public void Clear()
    {
        lock (_sync)
            _queue.Clear();
    }
}
