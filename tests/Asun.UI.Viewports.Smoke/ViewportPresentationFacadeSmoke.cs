using System.Drawing;
using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportPresentationFacadeSmoke
{
    public static async ValueTask RunAsync(Action<bool, string> assert)
    {
        RunBackpressurePolicySmoke(assert);

        for (var i = 0; i < 500; i++)
        {
            using var presentation = new ViewportPresentationRuntime<string>(
                new Vector2(2000 + i % 7 * 31, 1500 + i % 9 * 23),
                new Vector2(640 + i % 5 * 20, 480 + i % 3 * 25),
                new Vector2(100, 100),
                1,
                64,
                4,
                new LocalTileSource(),
                new ViewportRenderBudget(8, 12, 2, 20),
                120);

            var center = new Vector2(
                320 + i % 11 * 10,
                240 + i % 7 * 8);

            var roiId = presentation.Composite.AddRoi(
                RoiGeometry.CreateRectangle(
                    center,
                    new Vector2(90, 60)));

            presentation.Composite.SelectRoi(roiId);

            var initialSnapshot = presentation.Snapshot;

            assert(
                initialSnapshot.IsGenerationStable &&
                initialSnapshot.IsSurfaceStable &&
                initialSnapshot.IsBufferStable &&
                initialSnapshot.IsQueueStable &&
                initialSnapshot.IsExecutionStable &&
                initialSnapshot.IsPresentationStable &&
                initialSnapshot.Surface.PresentationSequence == 0 &&
                initialSnapshot.IsPresentationStable &&
                initialSnapshot.Generation == presentation.Composite.Generation,
                $"Presentation facade {i + 1} should expose a stable diagnostic snapshot when the generation is idle.");

            presentation.Submit(
                ViewportInputEventKind.PointerMove,
                new Vector2(100, 100));

            presentation.Submit(
                ViewportInputEventKind.PointerMove,
                new Vector2(104, 103));

            presentation.Submit(
                ViewportInputEventKind.Wheel,
                center,
                wheelDelta: 120);

            var frame = await presentation.Pipeline.RefreshAsync(
                DateTimeOffset.UtcNow.AddSeconds(1));

            assert(
                frame is not null,
                $"Presentation facade {i + 1} should create an initial render frame.");

            if (frame is null)
                continue;

            var sink = new RecordingSink();
            var delivery = await ViewportRenderDeliveryRuntime.TryDeliverAsync(
                frame,
                sink);

            assert(
                delivery.Succeeded &&
                !delivery.Cancelled &&
                delivery.Error is null &&
                delivery.RenderedUnits > 0,
                $"Presentation facade {i + 1} should deliver an initial render frame.");

            presentation.Composite.TranslateSelected(new Vector2(7, 5));

            var incremental = await presentation.Pipeline.RefreshAsync(
                DateTimeOffset.UtcNow.AddSeconds(1));

            assert(
                incremental is not null &&
                incremental.WorkPlan.Items.Any(item => item.IsInvalidation) &&
                incremental.WorkPlan.Items.Any(item =>
                    item.Kind == ViewportRenderWorkKind.Roi &&
                    !item.IsInvalidation),
                $"Presentation facade {i + 1} should retain previous ROI bounds for incremental invalidation.");

            var before = presentation.Composite.Generation;
            var processed = presentation.Continuous.ProcessInputs();

            assert(
                processed == 2 &&
                presentation.Composite.Generation > before,
                $"Presentation facade {i + 1} should process coalesced input through the facade.");

            var cts = new CancellationTokenSource();
            var continuousSink = new CancellingSink(cts);

            try
            {
                await presentation.RunAsync(
                    continuousSink,
                    cts.Token);
            }
            catch (OperationCanceledException)
            {
            }

            assert(
                presentation.Statistics.RenderedFrames >= 1 &&
                presentation.Statistics.DeliveryFailures >= 1 &&
                presentation.PresentationExecution.Statistics.Executed >= 1 &&
                presentation.PresentationExecution.Statistics.LastGeneration is not null &&
                continuousSink.BeginCount >= 2 &&
                continuousSink.EndCount >= 1,
                $"Presentation facade {i + 1} should retry a failed delivery and complete the continuous render lifecycle.");

            presentation.CancelInput();

            assert(
                !presentation.TrySubmitWithBackpressure(
                    ViewportInputEventKind.PointerMove,
                    new Vector2(8, 9)) &&
                presentation.Backpressure.IsCancelled &&
                presentation.Input.IsCancelled &&
                presentation.Input.IsCompleted,
                $"Presentation facade {i + 1} should cancel both backpressure and submission lifecycles together.");

            presentation.Reset();

            assert(
                !presentation.Input.HasPending &&
                !presentation.Input.IsCancelled &&
                !presentation.Input.IsCompleted &&
                !presentation.Backpressure.IsCancelled &&
                !presentation.Backpressure.IsCompleted &&
                presentation.Pipeline.Scheduler.PendingFlags == ViewportDirtyFlags.None &&
                presentation.PresentationExecution.Statistics.Executed == 0 &&
                presentation.State == ViewportPresentationState.Created,
                $"Presentation facade {i + 1} reset should clear runtime state and restore input lifecycle.");

            assert(
                presentation.TrySubmitWithBackpressure(
                    ViewportInputEventKind.PointerMove,
                    new Vector2(12, 18)),
                $"Presentation facade {i + 1} should accept backpressure input again after reset.");
        }
    }


    private static void RunBackpressurePolicySmoke(Action<bool, string> assert)
    {
        using var dropNewestInput = new ViewportInputSubmissionRuntime();
        using var dropNewest = new ViewportInputBackpressureRuntime(2, ViewportInputDropPolicy.DropNewest);

        assert(dropNewest.TrySubmit(dropNewestInput, ViewportInputEventKind.PointerDown, new Vector2(1, 1)),
            "DropNewest should accept the first event.");
        assert(dropNewest.TrySubmit(dropNewestInput, ViewportInputEventKind.PointerDown, new Vector2(2, 2)),
            "DropNewest should accept events up to capacity.");
        assert(!dropNewest.TrySubmit(dropNewestInput, ViewportInputEventKind.PointerDown, new Vector2(3, 3)),
            "DropNewest should reject an event when capacity is full.");
        assert(dropNewest.Capture(dropNewestInput) is { Pending: 2, Dropped: 1 },
            "DropNewest should preserve the bounded queue and count the rejected event.");

        using var dropOldestInput = new ViewportInputSubmissionRuntime();
        using var dropOldest = new ViewportInputBackpressureRuntime(2, ViewportInputDropPolicy.DropOldest);
        dropOldest.TrySubmit(dropOldestInput, ViewportInputEventKind.PointerDown, new Vector2(1, 1));
        dropOldest.TrySubmit(dropOldestInput, ViewportInputEventKind.PointerDown, new Vector2(2, 2));
        assert(dropOldest.TrySubmit(dropOldestInput, ViewportInputEventKind.PointerDown, new Vector2(3, 3)),
            "DropOldest should accept a new event after evicting the oldest event.");
        var oldest = dropOldestInput.Drain(2);
        assert(oldest.Count == 2 &&
               oldest[0].Position == new Vector2(2, 2) &&
               oldest[1].Position == new Vector2(3, 3),
            "DropOldest should evict only the oldest pending event.");

        using var coalesceInput = new ViewportInputSubmissionRuntime();
        using var coalesce = new ViewportInputBackpressureRuntime(2, ViewportInputDropPolicy.CoalesceMoves);
        coalesce.TrySubmit(coalesceInput, ViewportInputEventKind.PointerDown, new Vector2(1, 1));
        coalesce.TrySubmit(coalesceInput, ViewportInputEventKind.PointerMove, new Vector2(2, 2));
        assert(coalesce.TrySubmit(coalesceInput, ViewportInputEventKind.PointerMove, new Vector2(8, 9)),
            "CoalesceMoves should replace the latest move when capacity is full.");
        var coalesced = coalesceInput.Drain(2);
        assert(coalesced.Count == 2 &&
               coalesced[1].Position == new Vector2(8, 9) &&
               coalesce.Capture(coalesceInput).Coalesced == 1,
            "CoalesceMoves should retain the queue and expose the replacement count.");

        using var completionInput = new ViewportInputSubmissionRuntime();
        using var completion = new ViewportInputBackpressureRuntime(4);
        completion.TrySubmit(completionInput, ViewportInputEventKind.PointerDown, new Vector2(1, 1));
        completion.TrySubmit(completionInput, ViewportInputEventKind.PointerDown, new Vector2(2, 2));
        completion.Complete(completionInput, cancelPending: true);
        assert(completion.IsCompleted &&
               completionInput.IsCompleted &&
               completionInput.PendingCount == 0 &&
               completion.Capture(completionInput).Dropped == 2,
            "Coupled completion should clear all pending input and count every discarded event.");

        using var cancellationInput = new ViewportInputSubmissionRuntime();
        using var cancellation = new ViewportInputBackpressureRuntime(4);
        cancellation.TrySubmit(cancellationInput, ViewportInputEventKind.PointerDown, new Vector2(4, 4));
        cancellation.TrySubmit(cancellationInput, ViewportInputEventKind.PointerDown, new Vector2(5, 5));
        cancellation.Cancel(cancellationInput);
        assert(cancellation.IsCancelled &&
               cancellationInput.IsCancelled &&
               cancellationInput.IsCompleted &&
               cancellationInput.PendingCount == 0 &&
               cancellation.Capture(cancellationInput).Dropped == 2,
            "Coupled cancellation should clear pending input and account for every discarded event.");
    }

    private sealed class LocalTileSource : ITileSource<string>
    {
        public ValueTask<string> LoadAsync(
            TileRequest request,
            RectangleF imageRectangle,
            CancellationToken cancellationToken = default) =>
            ValueTask.FromResult($"tile:{request.Index.X},{request.Index.Y}");
    }

    private sealed class RecordingSink : IViewportRenderSink<string>
    {
        public int BeginCount { get; private set; }
        public int EndCount { get; private set; }

        public ValueTask BeginFrameAsync(
            ViewportRenderFrameContext context,
            CancellationToken cancellationToken = default)
        {
            BeginCount++;
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
            CancellationToken cancellationToken = default)
        {
            EndCount++;
            return ValueTask.CompletedTask;
        }
    }

    private sealed class CancellingSink : IViewportRenderSink<string>
    {
        private readonly CancellationTokenSource _source;
        private bool _failed;

        public CancellingSink(CancellationTokenSource source)
        {
            _source = source;
        }

        public int BeginCount { get; private set; }
        public int EndCount { get; private set; }

        public ValueTask BeginFrameAsync(
            ViewportRenderFrameContext context,
            CancellationToken cancellationToken = default)
        {
            BeginCount++;

            if (!_failed)
            {
                _failed = true;
                throw new InvalidOperationException("synthetic retryable render failure");
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
            CancellationToken cancellationToken = default)
        {
            EndCount++;
            _source.Cancel();
            return ValueTask.CompletedTask;
        }
    }
}