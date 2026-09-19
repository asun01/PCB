using System.Drawing;
using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportPresentationQueueSmoke
{
    public static async ValueTask RunAsync(Action<bool, string> assert)
    {
        using var pipeline = CreatePipeline(new StableTileSource());

        var first = await pipeline.RefreshAsync(
            DateTimeOffset.UtcNow.AddSeconds(10));

        assert(
            first is not null &&
            first.CommandStream.CommandCount == first.Batch.ItemCount &&
            first.CommandStream.RegionCount == first.Batch.RegionCount &&
            first.CommandStream.Generation == first.Batch.Generation &&
            first.CommandStream.Commands.Select(command => command.Sequence)
                .SequenceEqual(Enumerable.Range(1, first.CommandStream.CommandCount)),
            "Every pipeline frame should expose an immutable command stream with contiguous sequence numbers and batch-aligned regions.");

        if (first is null)
            return;

        using var queue = new ViewportPresentationQueueRuntime<string>(capacity: 2);

        assert(
            queue.TryEnqueue(first, out var firstPacket) &&
            firstPacket.CommandStream.Generation == first.Composite.Generation,
            "Presentation queue should enqueue the pipeline frame and expose its command stream.");

        var initialGeneration = pipeline.Composite.Generation;
        pipeline.Composite.PanBy(new Vector2(5, 0));

        assert(
            pipeline.Composite.Generation > initialGeneration,
            "Queue smoke should create a newer generation through real navigation.");

        var second = await pipeline.RefreshAsync(
            DateTimeOffset.UtcNow.AddSeconds(11));

        assert(
            second is not null &&
            second.Composite.Generation > first.Composite.Generation,
            "Queue smoke should obtain a newer generation after navigation.");

        if (second is null)
            return;

        assert(
            queue.TryEnqueue(second, out var secondPacket) &&
            secondPacket.Token.Sequence > firstPacket.Token.Sequence,
            "Newer presentation generations should receive monotonic submission tokens.");

        assert(
            queue.TryTakeLatest(out var latest) &&
            latest.Token == secondPacket.Token &&
            queue.Statistics.Dropped == 1 &&
            !queue.TryTakeLatest(out _),
            "Latest-take should coalesce an older pending frame and keep at most one in-flight presentation.");

        assert(
            queue.TryAcknowledgePresented(latest.Token) &&
            queue.Statistics.Presented == 1 &&
            queue.Statistics.PresentedGeneration == second.Composite.Generation,
            "The render boundary should acknowledge only the current in-flight token.");

        assert(
            !queue.TryEnqueue(first, out _) &&
            queue.Statistics.StaleRejected == 1,
            "A stale generation must be rejected after a newer generation has entered the presentation queue.");

        assert(
            queue.TryEnqueue(second, out var retryPacket) &&
            queue.TryTakeLatest(out var retryLatest) &&
            queue.TryCancel(retryLatest.Token) &&
            queue.Statistics.Cancelled == 1 &&
            retryLatest.Token == retryPacket.Token,
            "An in-flight presentation may be cancelled only by its exact transaction token.");

        queue.Reset();

        assert(
            queue.Statistics.Pending == 0 &&
            queue.Statistics.LatestGeneration is null &&
            queue.Statistics.PresentedGeneration is null &&
            queue.Statistics.PresentedSequence is null,
            "Queue reset should clear presentation state without invalidating the monotonic token domain.");

        assert(
            queue.TryEnqueue(first, out var postResetPacket) &&
            queue.TryTakeLatest(out var postResetLatest) &&
            postResetLatest.Token.Sequence > retryPacket.Token.Sequence &&
            queue.TryAcknowledgePresented(postResetLatest.Token),
            "Reset must not allow an old presentation token to alias a new post-reset submission.");

        pipeline.Invalidate(
            ViewportDirtyFlags.Overlay,
            pipeline.Composite.Generation);

        var overlay = await pipeline.RefreshAsync(
            DateTimeOffset.UtcNow.AddSeconds(12));

        assert(
            overlay is not null &&
            overlay.CommandStream.Commands.Count == 1 &&
            overlay.CommandStream.Commands[0].Kind == ViewportRenderCommandKind.DrawOverlay &&
            overlay.CommandStream.OverlayCount == 1 &&
            overlay.Batch.OverlayCount == 1,
            "Overlay-only work should be represented by exactly one overlay command.");

        if (overlay is null)
            return;

        var sink = new CommandTrackingSink();

        var result = await ViewportRenderDeliveryRuntime.TryDeliverAsync(
            overlay,
            sink);

        assert(
            result.Succeeded &&
            sink.OverlayCount == 1 &&
            sink.TileCount == 0 &&
            sink.RoiCount == 0 &&
            sink.ClearCount == 0,
            "Delivery should execute the command stream at the sink boundary without replaying unrelated layers.");
    }

    private static ViewportRenderPipelineRuntime<string> CreatePipeline(
        ITileSource<string> source) =>
        new(
            new Vector2(100, 100),
            new Vector2(100, 100),
            new Vector2(100, 100),
            0,
            8,
            1,
            source,
            new ViewportRenderBudget(4, 4, 1, 8),
            1000);

    private sealed class StableTileSource : ITileSource<string>
    {
        public ValueTask<string> LoadAsync(
            TileRequest request,
            RectangleF imageRectangle,
            CancellationToken cancellationToken = default) =>
            ValueTask.FromResult("stable");
    }

    private sealed class CommandTrackingSink : IViewportRenderSink<string>
    {
        public int TileCount { get; private set; }
        public int RoiCount { get; private set; }
        public int OverlayCount { get; private set; }
        public int ClearCount { get; private set; }

        public ValueTask BeginFrameAsync(
            ViewportRenderFrameContext context,
            CancellationToken cancellationToken = default) =>
            ValueTask.CompletedTask;

        public ValueTask DrawTileAsync(
            ViewportRenderTileContext<string> tile,
            CancellationToken cancellationToken = default)
        {
            TileCount++;
            return ValueTask.CompletedTask;
        }

        public ValueTask DrawRoiAsync(
            ViewportRenderRoiContext roi,
            CancellationToken cancellationToken = default)
        {
            RoiCount++;
            return ValueTask.CompletedTask;
        }

        public ValueTask DrawOverlayAsync(
            ViewportRenderOverlayContext overlay,
            CancellationToken cancellationToken = default)
        {
            OverlayCount++;
            return ValueTask.CompletedTask;
        }

        public ValueTask ClearInvalidatedRegionAsync(
            ViewportRenderInvalidationContext invalidation,
            CancellationToken cancellationToken = default)
        {
            ClearCount++;
            return ValueTask.CompletedTask;
        }

        public ValueTask EndFrameAsync(
            ViewportRenderFrameContext context,
            CancellationToken cancellationToken = default) =>
            ValueTask.CompletedTask;
    }
}
