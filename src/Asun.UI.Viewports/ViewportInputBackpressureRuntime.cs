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
    long Dropped,
    long Coalesced);

public sealed class ViewportInputBackpressureRuntime
{
    private readonly object _sync = new();
    private readonly int _capacity;
    private readonly ViewportInputDropPolicy _dropPolicy;
    private long _dropped;
    private long _coalesced;

    public ViewportInputBackpressureRuntime(
        int capacity = 512,
        ViewportInputDropPolicy dropPolicy = ViewportInputDropPolicy.CoalesceMoves)
    {
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity));

        _capacity = capacity;
        _dropPolicy = dropPolicy;
    }

    public ViewportInputBackpressureSnapshot Capture(
        ViewportInputSubmissionRuntime input)
    {
        ArgumentNullException.ThrowIfNull(input);

        lock (_sync)
        {
            return new ViewportInputBackpressureSnapshot(
                _capacity,
                input.PendingCount,
                _dropped,
                _coalesced);
        }
    }

    public bool TrySubmit(
        ViewportInputSubmissionRuntime input,
        ViewportInputEventKind kind,
        System.Numerics.Vector2 position,
        int wheelDelta = 0,
        ViewportMouseButton button = ViewportMouseButton.Left)
    {
        ArgumentNullException.ThrowIfNull(input);

        lock (_sync)
        {
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
                        if (kind == ViewportInputEventKind.PointerMove)
                        {
                            input.TryReplaceLatestMove(position);
                            _coalesced++;
                            return true;
                        }

                        input.Drain(1);
                        _dropped++;
                        break;
                }
            }

            input.Submit(
                kind,
                position,
                wheelDelta,
                button);

            return true;
        }
    }
}
