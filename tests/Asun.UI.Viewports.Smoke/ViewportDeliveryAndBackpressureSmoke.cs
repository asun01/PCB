using System.Drawing;
using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportDeliveryAndBackpressureSmoke
{
    public static async ValueTask RunAsync(Action<bool, string> assert)
    {
        for (var i = 0; i < 500; i++)
        {
            using var pipeline = new ViewportRenderPipelineRuntime<string>(
                new Vector2(1600 + i % 7 * 29, 1200 + i % 9 * 23),
                new Vector2(600 + i % 5 * 20, 450 + i % 3 * 15),
                new Vector2(100, 100),
                1,
                64,
                4,
                new LocalTileSource());

            pipeline.Composite.AddRoi(
                RoiGeometry.CreateRectangle(
                    new Vector2(250 + i % 11 * 10, 200 + i % 7 * 8),
                    new Vector2(80, 60)));

            var frame = await pipeline.RefreshAsync(
                DateTimeOffset.UtcNow.AddSeconds(1));

            assert(
                frame is not null,
                $"Delivery chain {i + 1} should create a frame.");

            if (frame is null)
                continue;

            var successfulSink = new PassiveSink();
            var success = await ViewportRenderDeliveryRuntime.TryDeliverAsync(
                frame,
                successfulSink);

            assert(
                success.Succeeded &&
                success.Status == ViewportRenderDeliveryStatus.Succeeded &&
                !success.Cancelled &&
                success.Error is null &&
                success.RenderedUnits > 0,
                $"Delivery chain {i + 1} should report successful delivery.");

            var failingSink = new ThrowingSink();
            var failure = await ViewportRenderDeliveryRuntime.TryDeliverAsync(
                frame,
                failingSink);

            assert(
                !failure.Succeeded &&
                failure.Status == ViewportRenderDeliveryStatus.Failed &&
                !failure.Cancelled &&
                failure.Error is not null &&
                failingSink.EndCount == 1,
                $"Delivery chain {i + 1} should isolate sink exceptions and finalize the frame.");

            var cancelledSink = new CancelledSink();
            var cancellation = await ViewportRenderDeliveryRuntime.TryDeliverAsync(
                frame,
                cancelledSink);

            assert(
                !cancellation.Succeeded &&
                cancellation.Status == ViewportRenderDeliveryStatus.Cancelled &&
                cancellation.Cancelled &&
                cancellation.Error is null,
                $"Delivery chain {i + 1} should classify cancellation separately.");

            var input = new ViewportInputSubmissionRuntime();
            var backpressure = new ViewportInputBackpressureRuntime(
                4,
                ViewportInputDropPolicy.DropNewest);

            for (var n = 0; n < 10; n++)
            {
                backpressure.TrySubmit(
                    input,
                    n % 2 == 0
                        ? ViewportInputEventKind.PointerMove
                        : ViewportInputEventKind.Wheel,
                    new Vector2(n, n),
                    wheelDelta: n % 2 == 0 ? 0 : 120);
            }

            var snapshot = backpressure.Capture(input);

            assert(
                snapshot.Pending == 4 &&
                snapshot.Dropped == 6,
                $"Delivery chain {i + 1} should enforce bounded input capacity.");

            var drained = input.Drain();

            assert(
                drained.Count == 4,
                $"Delivery chain {i + 1} should retain exactly the bounded event count.");

            input.Cancel();

            assert(
                input.IsCompleted &&
                input.IsCancelled &&
                !input.TrySubmit(
                    ViewportInputEventKind.PointerMove,
                    new Vector2(99, 99)),
                $"Delivery chain {i + 1} should reject input after cancellation.");

            var tracked = new ViewportRenderDeliveryTracker();
            var trackedSuccess = await ViewportRenderDeliveryRuntime.TryDeliverAsync(
                frame,
                successfulSink,
                tracked);

            var trackedStats = tracked.Statistics;

            assert(
                trackedSuccess.Succeeded &&
                trackedStats.Attempts == 1 &&
                trackedStats.Succeeded == 1 &&
                trackedStats.Failed == 0 &&
                trackedStats.Deferred == 0 &&
                trackedStats.Cancelled == 0 &&
                trackedStats.RenderedUnits == trackedSuccess.RenderedUnits,
                $"Delivery chain {i + 1} should expose delivery statistics.");

            using var retryPipeline = new ViewportRenderPipelineRuntime<string>(
                new Vector2(100, 100),
                new Vector2(100, 100),
                new Vector2(100, 100),
                0,
                8,
                1,
                new FlakyTileSource(),
                new ViewportRenderBudget(4, 4, 1, 8),
                1000);

            var retryFrame = await retryPipeline.RefreshAsync(
                DateTimeOffset.UtcNow.AddSeconds(1));

            assert(
                retryFrame is not null,
                $"Delivery chain {i + 1} should create a frame for unavailable tile retry.");

            if (retryFrame is not null)
            {
                var retryFirst = await ViewportRenderDeliveryRuntime.TryDeliverAsync(
                    retryFrame,
                    new PassiveSink());

                assert(
                    retryFirst.Status == ViewportRenderDeliveryStatus.Deferred &&
                    retryFirst.Deferred &&
                    retryFirst.DeferredUnits == 1 &&
                    retryFirst.Error is ViewportRenderWorkUnavailableException,
                    $"Delivery chain {i + 1} should classify an unavailable tile as deferred.");

                retryPipeline.RequeueFrame(retryFrame);
                retryPipeline.Invalidate(
                    retryFrame.Submission.DirtyFlags,
                    retryFrame.Composite.Generation);

                var retrySecond = await retryPipeline.RefreshAsync(
                    DateTimeOffset.UtcNow.AddSeconds(2));

                assert(
                    retrySecond is not null,
                    $"Delivery chain {i + 1} should rebuild a frame after deferred tile delivery.");

                if (retrySecond is not null)
                {
                    var retryDelivered = await ViewportRenderDeliveryRuntime.TryDeliverAsync(
                        retrySecond,
                        new PassiveSink());

                    assert(
                        retryDelivered.Succeeded &&
                        retryDelivered.Status == ViewportRenderDeliveryStatus.Succeeded,
                        $"Delivery chain {i + 1} should succeed once the tile source recovers.");
                }

                var deferredStats = retryPipeline
                    .Scheduler
                    .Statistics;

                assert(
                    deferredStats.Submissions >= 2,
                    $"Delivery chain {i + 1} should resubmit deferred render work.");
            }

            using var coalescingInput = new ViewportInputSubmissionRuntime();
            var coalescing = new ViewportInputBackpressureRuntime(
                2,
                ViewportInputDropPolicy.CoalesceMoves);

            coalescing.TrySubmit(
                coalescingInput,
                ViewportInputEventKind.PointerMove,
                new Vector2(1, 1));

            coalescing.TrySubmit(
                coalescingInput,
                ViewportInputEventKind.Wheel,
                new Vector2(2, 2),
                wheelDelta: 120);

            assert(
                coalescing.TrySubmit(
                    coalescingInput,
                    ViewportInputEventKind.PointerMove,
                    new Vector2(3, 3)),
                $"Delivery chain {i + 1} should accept a saturated move by preserving capacity.");

            var coalescedFallback = coalescing.Capture(coalescingInput);

            assert(
                coalescedFallback.Pending == 2 &&
                coalescedFallback.Dropped == 1,
                $"Delivery chain {i + 1} should drop the oldest event when the saturated tail cannot be coalesced.");
        }
    }

    private sealed class FlakyTileSource : ITileSource<string>
    {
        private int _attempts;

        public ValueTask<string> LoadAsync(
            TileRequest request,
            RectangleF imageRectangle,
            CancellationToken cancellationToken = default)
        {
            if (Interlocked.Increment(ref _attempts) == 1)
                throw new InvalidOperationException("synthetic first-load failure");

            return ValueTask.FromResult("recovered-tile");
        }
    }

    private sealed class LocalTileSource : ITileSource<string>
    {
        public ValueTask<string> LoadAsync(
            TileRequest request,
            RectangleF imageRectangle,
            CancellationToken cancellationToken = default) =>
            ValueTask.FromResult($"tile:{request.Index.X},{request.Index.Y}");
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


    private sealed class CancelledSink : IViewportRenderSink<string>
    {
        public ValueTask BeginFrameAsync(
            ViewportRenderFrameContext context,
            CancellationToken cancellationToken = default) =>
            throw new OperationCanceledException();

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

    private sealed class ThrowingSink : IViewportRenderSink<string>
    {
        public int EndCount { get; private set; }

        public ValueTask BeginFrameAsync(
            ViewportRenderFrameContext context,
            CancellationToken cancellationToken = default) =>
            throw new InvalidOperationException("synthetic render failure");

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
            CancellationToken cancellationToken = default)
        {
            EndCount++;
            return ValueTask.CompletedTask;
        }
    }
}
