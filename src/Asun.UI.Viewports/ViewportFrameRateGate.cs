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

    public void Reset() => Interlocked.Exchange(ref _last, 0);
}
