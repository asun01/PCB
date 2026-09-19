namespace Asun.UI.Viewports;

public sealed class ViewportFrameRateGate
{
    private readonly long _minimumTicks;
    private long _last;

    public ViewportFrameRateGate(double framesPerSecond)
    {
        if (!double.IsFinite(framesPerSecond) || framesPerSecond <= 0)
            throw new ArgumentOutOfRangeException(nameof(framesPerSecond));
        _minimumTicks = Math.Max(1, (long)(TimeSpan.TicksPerSecond / framesPerSecond));
    }

    public bool TryEnter(DateTimeOffset now)
    {
        var ticks = now.UtcTicks;
        while (true)
        {
            var previous = Volatile.Read(ref _last);
            if (ticks - previous < _minimumTicks) return false;
            if (Interlocked.CompareExchange(ref _last, ticks, previous) == previous) return true;
        }
    }

    public TimeSpan GetDelay(DateTimeOffset now)
    {
        var previous = Volatile.Read(ref _last);
        if (previous <= 0)
            return TimeSpan.Zero;

        var remaining = _minimumTicks - (now.UtcTicks - previous);
        return remaining <= 0
            ? TimeSpan.Zero
            : TimeSpan.FromTicks(remaining);
    }

    public void Reset() => Interlocked.Exchange(ref _last, 0);
}
