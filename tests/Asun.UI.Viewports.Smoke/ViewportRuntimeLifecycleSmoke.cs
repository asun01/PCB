using Asun.UI.Viewports;

public static class ViewportPresentationLifecycleSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var lifecycle = new ViewportPresentationLifecycleRuntime();

        assert(
            lifecycle.State == ViewportPresentationState.Created,
            "Presentation lifecycle should start in Created state.");

        assert(
            lifecycle.TryStart(out var token) &&
            !token.IsCancellationRequested &&
            lifecycle.State == ViewportPresentationState.Running,
            "Presentation lifecycle should transition to Running once.");

        assert(
            !lifecycle.TryStart(out _),
            "Presentation lifecycle should reject concurrent starts.");

        lifecycle.RequestStop();

        assert(
            lifecycle.State == ViewportPresentationState.Stopping &&
            token.IsCancellationRequested,
            "Stopping should cancel the active run token.");

        lifecycle.MarkStopped();

        assert(
            lifecycle.State == ViewportPresentationState.Stopped,
            "Presentation lifecycle should reach Stopped after the run exits.");

        lifecycle.Reset();

        assert(
            lifecycle.State == ViewportPresentationState.Created,
            "Stopped presentation should be reusable after reset.");

        lifecycle.Dispose();

        assert(
            lifecycle.State == ViewportPresentationState.Disposed,
            "Disposed presentation should expose the terminal lifecycle state.");
    }
}

public static class ViewportRenderSchedulerGenerationSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var scheduler = new ViewportRenderSchedulerRuntime(framesPerSecond: 1000);

        scheduler.Submit(ViewportDirtyFlags.Image, generation: 10);
        scheduler.Submit(ViewportDirtyFlags.Roi, generation: 11);

        assert(
            scheduler.TryTakeFrame(
                DateTimeOffset.UtcNow.AddSeconds(1),
                out var submission,
                minimumGeneration: 11),
            "Scheduler should accept a frame at the newest requested generation.");

        assert(
            submission.Generation == 11 &&
            submission.DirtyFlags.HasFlag(ViewportDirtyFlags.Image) &&
            submission.DirtyFlags.HasFlag(ViewportDirtyFlags.Roi),
            "Scheduler should retain accumulated dirty flags at the newest generation.");

        scheduler.Submit(ViewportDirtyFlags.Overlay, generation: 20);

        assert(
            !scheduler.TryTakeFrame(
                DateTimeOffset.UtcNow.AddSeconds(2),
                out _,
                minimumGeneration: 21),
            "Scheduler should reject a frame older than the requested generation.");

        assert(
            scheduler.PendingFlags.HasFlag(ViewportDirtyFlags.Overlay),
            "Rejected stale scheduling should preserve pending dirty work.");
    }
}
