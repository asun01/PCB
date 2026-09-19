using System.Numerics;

namespace Asun.UI.Viewports;

public sealed class ViewportInputReplayRuntime
{
    private readonly object _sync = new();
    private readonly int _capacity;
    private readonly List<ViewportInputEvent> _events = new();
    private long _dropped;

    public ViewportInputReplayRuntime(int capacity = 2048)
    {
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity));

        _capacity = capacity;
    }

    public int Capacity => _capacity;

    public long DroppedCount
    {
        get
        {
            lock (_sync)
                return _dropped;
        }
    }

    public int Count
    {
        get
        {
            lock (_sync)
                return _events.Count;
        }
    }

    public void Record(ViewportInputEvent input)
    {
        Validate(input);

        lock (_sync)
        {
            if (_events.Count >= _capacity)
            {
                _events.RemoveAt(0);
                _dropped++;
            }

            _events.Add(input);
        }
    }

    public void Record(
        ViewportInputEventKind kind,
        Vector2 position,
        int wheelDelta = 0,
        ViewportMouseButton button = ViewportMouseButton.Left,
        long sequence = 0)
    {
        Record(
            new ViewportInputEvent(
                sequence,
                kind,
                position,
                wheelDelta,
                button));
    }

    public IReadOnlyList<ViewportInputEvent> Snapshot()
    {
        lock (_sync)
            return _events.ToArray();
    }

    public IReadOnlyList<ViewportCompositeInputResult> Replay<TTile>(
        ViewportCompositeInputRuntime<TTile> input)
    {
        ArgumentNullException.ThrowIfNull(input);

        ViewportInputEvent[] events;

        lock (_sync)
            events = _events.ToArray();

        return ViewportReplayExecutionRuntime
            .Execute(input, events)
            .Results;
    }

    public ViewportReplayExecutionReport ReplayReport<TTile>(
        ViewportCompositeInputRuntime<TTile> input)
    {
        ArgumentNullException.ThrowIfNull(input);

        ViewportInputEvent[] events;

        lock (_sync)
            events = _events.ToArray();

        return ViewportReplayExecutionRuntime.Execute(
            input,
            events);
    }

    public string EvidenceHash()
    {
        return ViewportRenderEvidenceRuntime.ComputeTextHash(
            string.Join(
                "\n",
                Snapshot().Select(item =>
                    $"{item.Sequence}|{item.Kind}|{item.Position.X:R}|{item.Position.Y:R}|{item.WheelDelta}|{item.Button}")));
    }

    public void Reset()
    {
        lock (_sync)
        {
            _events.Clear();
            _dropped = 0;
        }
    }

    private static void Validate(ViewportInputEvent input)
    {
        if (input.Sequence < 0)
            throw new ArgumentOutOfRangeException(nameof(input));

        if (!float.IsFinite(input.Position.X) ||
            !float.IsFinite(input.Position.Y))
        {
            throw new ArgumentOutOfRangeException(nameof(input));
        }

        if (!Enum.IsDefined(input.Kind) ||
            !Enum.IsDefined(input.Button))
        {
            throw new ArgumentOutOfRangeException(nameof(input));
        }
    }
}
