using System.Drawing;
using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportPresentationOutcomeEvidenceSmoke
{
    public static async ValueTask RunAsync(Action<bool, string> assert)
    {
        await VerifyFailureEvidenceAsync(assert);
        await VerifyDeferredRecoveryEvidenceAsync(assert);
        await VerifySupersededEvidenceAsync(assert);
    }

    private static async ValueTask VerifyFailureEvidenceAsync(
        Action<bool, string> assert)
    {
        using var runtime = CreateRuntime(new StableTileSource());
        using var cancellation = new CancellationTokenSource();

        var run = runtime
            .RunAsync(new FailingSink(), cancellation.Token)
            .AsTask();

        var observed = await WaitUntilAsync(
            () => runtime.AuditTrace.Snapshot().Any(
                item => item.Stage == "Failed"),
            TimeSpan.FromSeconds(2));

        var audit = runtime.AuditTrace.Snapshot();
        var evidence = runtime.EvidenceHistory.Snapshot();

        assert(
            observed &&
            audit.Any(item =>
                item.Stage == "Failed" &&
                item.DeliveryStatus == ViewportRenderDeliveryStatus.Failed) &&
            evidence.Count > 0 &&
            runtime.EvidenceHistory.Validate().Count == 0,
            "Continuous failure delivery should create valid evidence and a Failed audit event.");

        cancellation.Cancel();

        try
        {
            await run;
        }
        catch (OperationCanceledException)
        {
        }
    }

    private static async ValueTask VerifyDeferredRecoveryEvidenceAsync(
        Action<bool, string> assert)
    {
        using var runtime = CreateRuntime(new RecoveringTileSource());
        using var cancellation = new CancellationTokenSource();

        var run = runtime
            .RunAsync(new PassiveSink(), cancellation.Token)
            .AsTask();

        var observedDeferred = await WaitUntilAsync(
            () => runtime.AuditTrace.Snapshot().Any(
                item => item.Stage == "Deferred"),
            TimeSpan.FromSeconds(2));

        var observedPresented = await WaitUntilAsync(
            () => runtime.AuditTrace.Snapshot().Any(
                item => item.Stage == "Presented"),
            TimeSpan.FromSeconds(2));

        var deferred = runtime.AuditTrace.Snapshot()
            .FirstOrDefault(item => item.Stage == "Deferred");

        var presented = runtime.AuditTrace.Snapshot()
            .LastOrDefault(item => item.Stage == "Presented");

        assert(
            observedDeferred &&
            deferred.DeliveryStatus == ViewportRenderDeliveryStatus.Deferred &&
            deferred.DeferredUnits > 0,
            "Continuous deferred delivery should expose deferred units and Deferred status.");

        assert(
            observedPresented &&
            presented.DeliveryStatus == ViewportRenderDeliveryStatus.Succeeded &&
            presented.DeferredUnits == 0,
            "Deferred work should be retried and eventually produce a complete Presented audit event.");

        assert(
            runtime.EvidenceHistory.Count >= 2 &&
            runtime.EvidenceHistory.Validate().Count == 0 &&
            runtime.EvidenceHistory.Snapshot().Any(
                item => item.DeferredUnits > 0) &&
            runtime.EvidenceHistory.Snapshot().Any(
                item => item.DeferredUnits == 0),
            "Evidence history should preserve both partial-deferred and recovered-complete states.");

        cancellation.Cancel();

        try
        {
            await run;
        }
        catch (OperationCanceledException)
        {
        }
    }

    private static async ValueTask VerifySupersededEvidenceAsync(
        Action<bool, string> assert)
    {
        using var runtime = CreateRuntime(new StableTileSource());
        using var cancellation = new CancellationTokenSource();

        var sink = new SupersedingSink();

        var run = runtime
            .RunAsync(sink, cancellation.Token)
            .AsTask();

        var firstStarted = await Task.WhenAny(
            sink.FirstFrameStarted.Task,
            Task.Delay(TimeSpan.FromSeconds(2)));

        assert(
            firstStarted == sink.FirstFrameStarted.Task,
            "Supersession smoke should enter the first frame before submitting newer input.");

        var before = runtime.Composite.Generation;

        assert(
            runtime.TrySubmit(
                ViewportInputEventKind.PointerMove,
                new Vector2(40, 30)),
            "New input should be accepted while the first frame is in flight.");

        var supersededObserved = await WaitUntilAsync(
            () => runtime.AuditTrace.Snapshot().Any(
                item => item.Stage == "Superseded"),
            TimeSpan.FromSeconds(2));

        var presentedObserved = await WaitUntilAsync(
            () => runtime.AuditTrace.Snapshot().Any(
                item => item.Stage == "Presented" &&
                        item.Generation > before),
            TimeSpan.FromSeconds(2));

        var audit = runtime.AuditTrace.Snapshot();

        assert(
            supersededObserved &&
            audit.Any(item =>
                item.Stage == "Superseded" &&
                item.DeliveryStatus == ViewportRenderDeliveryStatus.Cancelled),
            "Newer input should supersede the in-flight frame and record a Cancelled/Superseded audit event.");

        assert(
            presentedObserved,
            "The newer generation should complete presentation after the superseded frame is cancelled.");

        assert(
            runtime.EvidenceHistory.Count >= 2 &&
            runtime.EvidenceHistory.Validate().Count == 0,
            "Superseded execution should still retain valid evidence for the in-flight and recovered generations.");

        cancellation.Cancel();

        try
        {
            await run;
        }
        catch (OperationCanceledException)
        {
        }
    }

    private static ViewportPresentationRuntime<string> CreateRuntime(
        ITileSource<string> source)
    {
        return new ViewportPresentationRuntime<string>(
            new Vector2(100, 100),
            new Vector2(100, 100),
            new Vector2(100, 100),
            0,
            8,
            1,
            source,
            new ViewportRenderBudget(4, 4, 4, 8),
            1000,
            TimeSpan.FromMilliseconds(2));
    }

    private static async ValueTask<bool> WaitUntilAsync(
        Func<bool> condition,
        TimeSpan timeout)
    {
        var start = DateTime.UtcNow;

        while (DateTime.UtcNow - start < timeout)
        {
            if (condition())
                return true;

            await Task.Delay(10);
        }

        return condition();
    }

    private sealed class StableTileSource : ITileSource<string>
    {
        public ValueTask<string> LoadAsync(
            TileRequest request,
            RectangleF imageRectangle,
            CancellationToken cancellationToken = default) =>
            ValueTask.FromResult("stable");
    }

    private sealed class RecoveringTileSource : ITileSource<string>
    {
        private int _attempts;

        public ValueTask<string> LoadAsync(
            TileRequest request,
            RectangleF imageRectangle,
            CancellationToken cancellationToken = default)
        {
            if (Interlocked.Increment(ref _attempts) == 1)
                throw new InvalidOperationException("synthetic deferred load");

            return ValueTask.FromResult("recovered");
        }
    }

    private sealed class PassiveSink : IViewportRenderSink<string>
    {
        public ValueTask BeginFrameAsync(
            ViewportRenderFrameContext context,
            CancellationToken cancellationToken = default) =>
            ValueTask.CompletedTask;

        public ValueTask DrawTileAsync(
            ViewportRenderTileContext<string> tile,
            CancellationToken cancellationToken = default) =>
            ValueTask.CompletedTask;

        public ValueTask DrawRoiAsync(
            ViewportRenderRoiContext roi,
            CancellationToken cancellationToken = default) =>
            ValueTask.CompletedTask;

        public ValueTask EndFrameAsync(
            ViewportRenderFrameContext context,
            CancellationToken cancellationToken = default) =>
            ValueTask.CompletedTask;
    }

    private sealed class FailingSink : IViewportRenderSink<string>
    {
        public ValueTask BeginFrameAsync(
            ViewportRenderFrameContext context,
            CancellationToken cancellationToken = default) =>
            throw new InvalidOperationException("synthetic presentation failure");

        public ValueTask DrawTileAsync(
            ViewportRenderTileContext<string> tile,
            CancellationToken cancellationToken = default) =>
            ValueTask.CompletedTask;

        public ValueTask DrawRoiAsync(
            ViewportRenderRoiContext roi,
            CancellationToken cancellationToken = default) =>
            ValueTask.CompletedTask;

        public ValueTask EndFrameAsync(
            ViewportRenderFrameContext context,
            CancellationToken cancellationToken = default) =>
            ValueTask.CompletedTask;
    }

    private sealed class SupersedingSink : IViewportRenderSink<string>
    {
        private int _frames;

        public TaskCompletionSource<bool> FirstFrameStarted { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public ValueTask BeginFrameAsync(
            ViewportRenderFrameContext context,
            CancellationToken cancellationToken = default)
        {
            if (Interlocked.Increment(ref _frames) == 1)
            {
                FirstFrameStarted.TrySetResult(true);

                return new ValueTask(
                    Task.Delay(
                        Timeout.InfiniteTimeSpan,
                        cancellationToken));
            }

            return ValueTask.CompletedTask;
        }

        public ValueTask DrawTileAsync(
            ViewportRenderTileContext<string> tile,
            CancellationToken cancellationToken = default) =>
            ValueTask.CompletedTask;

        public ValueTask DrawRoiAsync(
            ViewportRenderRoiContext roi,
            CancellationToken cancellationToken = default) =>
            ValueTask.CompletedTask;

        public ValueTask EndFrameAsync(
            ViewportRenderFrameContext context,
            CancellationToken cancellationToken = default) =>
            ValueTask.CompletedTask;
    }
}
