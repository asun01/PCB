namespace Asun.UI.Viewports;

public readonly record struct ViewportRenderDeliveryStatistics(
    long Attempts,
    long Succeeded,
    long Partial,
    long Failed,
    long Deferred,
    long Cancelled,
    long RenderedUnits,
    long LastGeneration,
    TimeSpan TotalDuration)
{
    public double SuccessRate =>
        Attempts == 0 ? 0d : (double)Succeeded / Attempts;
}

public sealed class ViewportRenderDeliveryTracker
{
    private long _attempts;
    private long _succeeded;
    private long _partial;
    private long _failed;
    private long _deferred;
    private long _cancelled;
    private long _renderedUnits;
    private long _lastGeneration;
    private long _totalDurationTicks;

    public ViewportRenderDeliveryStatistics Statistics =>
        new(
            Interlocked.Read(ref _attempts),
            Interlocked.Read(ref _succeeded),
            Interlocked.Read(ref _partial),
            Interlocked.Read(ref _failed),
            Interlocked.Read(ref _deferred),
            Interlocked.Read(ref _cancelled),
            Interlocked.Read(ref _renderedUnits),
            Interlocked.Read(ref _lastGeneration),
            TimeSpan.FromTicks(Interlocked.Read(ref _totalDurationTicks)));

    internal void Record(ViewportRenderDeliveryResult result, TimeSpan duration)
    {
        Interlocked.Increment(ref _attempts);
        Interlocked.Add(ref _totalDurationTicks, duration.Ticks);
        UpdateLastGeneration(result.Generation);
        Interlocked.Add(ref _renderedUnits, result.RenderedUnits);

        if (result.Cancelled)
            Interlocked.Increment(ref _cancelled);
        else if (result.Deferred)
        {
            Interlocked.Increment(ref _deferred);

            if (result.FrameState.IsPartial)
                Interlocked.Increment(ref _partial);
        }
        else if (result.Succeeded)
            Interlocked.Increment(ref _succeeded);
        else
            Interlocked.Increment(ref _failed);
    }

    private void UpdateLastGeneration(long generation)
    {
        while (true)
        {
            var current = Interlocked.Read(ref _lastGeneration);

            if (generation <= current)
                return;

            if (Interlocked.CompareExchange(
                    ref _lastGeneration,
                    generation,
                    current) == current)
                return;
        }
    }

    public void Reset()
    {
        Interlocked.Exchange(ref _attempts, 0);
        Interlocked.Exchange(ref _succeeded, 0);
        Interlocked.Exchange(ref _partial, 0);
        Interlocked.Exchange(ref _failed, 0);
        Interlocked.Exchange(ref _deferred, 0);
        Interlocked.Exchange(ref _cancelled, 0);
        Interlocked.Exchange(ref _renderedUnits, 0);
        Interlocked.Exchange(ref _lastGeneration, 0);
        Interlocked.Exchange(ref _totalDurationTicks, 0);
    }
}
