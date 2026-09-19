using System.Diagnostics;

namespace Asun.Platform.Core;

/// <summary>
/// Lightweight throughput meter based on a monotonic stopwatch.
/// </summary>
public sealed class RateMeter
{
    private long _count;
    private long _startTimestamp = Stopwatch.GetTimestamp();

    public long Count => Interlocked.Read(ref _count);

    public TimeSpan Elapsed =>
        Stopwatch.GetElapsedTime(Interlocked.Read(ref _startTimestamp));

    public double RatePerSecond
    {
        get
        {
            var elapsed = Elapsed.TotalSeconds;
            if (elapsed <= 0)
                return 0;

            return Count / elapsed;
        }
    }

    public void Increment(long amount = 1)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        Interlocked.Add(ref _count, amount);
    }

    public void Reset()
    {
        Interlocked.Exchange(ref _count, 0);
        Interlocked.Exchange(ref _startTimestamp, Stopwatch.GetTimestamp());
    }
}
