using System.Drawing;
using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportRenderSurfaceSmoke
{
    public static async ValueTask RunAsync(Action<bool, string> assert)
    {
        await VerifySuccessfulCommitAsync(assert);
        await VerifyFailureDiscardAsync(assert);
        await VerifyDeferredDiscardAsync(assert);
    }

    private static async ValueTask VerifySuccessfulCommitAsync(
        Action<bool, string> assert)
    {
        using var pipeline = CreatePipeline(new StableTileSource());

        var frame = await pipeline.RefreshAsync(
            DateTimeOffset.UtcNow.AddSeconds(1));

        assert(
            frame is not null,
            "Surface smoke should create a frame for commit verification.");

        if (frame is null)
            return;

        using var surface = new ViewportRenderSurfaceRuntime();
        var sink = new TrackingSink();

        var result = await ViewportRenderDeliveryRuntime.TryDeliverAsync(
            frame,
            sink,
            surface: surface);

        var snapshot = surface.Snapshot;

        assert(
            result.Succeeded &&
            result.FrameState.IsComplete &&
            sink.BeginCount == 1 &&
            sink.EndCount == 1 &&
            sink.CommitCount == 1 &&
            sink.DiscardCount == 0 &&
            snapshot.State == ViewportRenderSurfaceState.Presented &&
            snapshot.PresentedGeneration == frame.Composite.Generation &&
            snapshot.PresentationSequence == 1 &&
            snapshot.LastPlannedUnits == frame.Batch.ItemCount &&
            snapshot.LastRenderedUnits == result.RenderedUnits,
            "Successful delivery should commit exactly once and publish the generation to the surface.");
    }

    private static async ValueTask VerifyFailureDiscardAsync(
        Action<bool, string> assert)
    {
        using var pipeline = CreatePipeline(new StableTileSource());

        var frame = await pipeline.RefreshAsync(
            DateTimeOffset.UtcNow.AddSeconds(2));

        assert(
            frame is not null,
            "Surface smoke should create a frame for failure verification.");

        if (frame is null)
            return;

        using var surface = new ViewportRenderSurfaceRuntime();
        var sink = new FailingSink();

        var result = await ViewportRenderDeliveryRuntime.TryDeliverAsync(
            frame,
            sink,
            surface: surface);

        assert(
            !result.Succeeded &&
            result.Status == ViewportRenderDeliveryStatus.Failed &&
            surface.Snapshot.State == ViewportRenderSurfaceState.Discarded &&
            sink.CommitCount == 0 &&
            sink.DiscardCount == 1,
            "Failed delivery should discard the rendering transaction and never commit the surface.");
    }

    private static async ValueTask VerifyDeferredDiscardAsync(
        Action<bool, string> assert)
    {
        using var pipeline = CreatePipeline(new FailingFirstTileSource());

        var frame = await pipeline.RefreshAsync(
            DateTimeOffset.UtcNow.AddSeconds(3));

        assert(
            frame is not null,
            "Surface smoke should create a frame for deferred verification.");

        if (frame is null)
            return;

        using var surface = new ViewportRenderSurfaceRuntime();
        var sink = new TrackingSink();

        var result = await ViewportRenderDeliveryRuntime.TryDeliverAsync(
            frame,
            sink,
            surface: surface);

        assert(
            result.Status == ViewportRenderDeliveryStatus.Deferred &&
            (result.FrameState.IsPartial ||
             result.FrameState.HasDeferredWork),
            "Deferred delivery should preserve deferred frame state.");

        assert(
            surface.Snapshot.State == ViewportRenderSurfaceState.Discarded &&
            sink.CommitCount == 0 &&
            sink.DiscardCount == 1,
            "Deferred delivery should discard the incomplete rendering transaction.");


        var secondSurface = new ViewportRenderSurfaceRuntime();
        var discardFailureSink = new DiscardFailureSink();

        var secondResult = await ViewportRenderDeliveryRuntime.TryDeliverAsync(
            frame,
            discardFailureSink,
            surface: secondSurface);

        assert(
            secondResult.Status == ViewportRenderDeliveryStatus.Deferred &&
            secondSurface.Snapshot.State == ViewportRenderSurfaceState.Discarded &&
            discardFailureSink.DiscardCount == 1,
            "A failing discard callback must not replace the original deferred delivery result.");    }

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

    private sealed class FailingFirstTileSource : ITileSource<string>
    {
        private int _failed;

        public ValueTask<string> LoadAsync(
            TileRequest request,
            RectangleF imageRectangle,
            CancellationToken cancellationToken = default)
        {
            if (Interlocked.Exchange(ref _failed, 1) == 0)
                throw new InvalidOperationException("synthetic deferred tile");

            return ValueTask.FromResult("recovered");
        }
    }

    private class TrackingSink : IViewportRenderSink<string>
    {
        public int BeginCount { get; protected set; }
        public int EndCount { get; protected set; }
        public int CommitCount { get; protected set; }
        public int DiscardCount { get; protected set; }

        public virtual ValueTask BeginFrameAsync(
            ViewportRenderFrameContext context,
            CancellationToken cancellationToken = default)
        {
            BeginCount++;
            return ValueTask.CompletedTask;
        }

        public virtual ValueTask DrawTileAsync(
            ViewportRenderTileContext<string> tile,
            CancellationToken cancellationToken = default) =>
            ValueTask.CompletedTask;

        public virtual ValueTask DrawRoiAsync(
            ViewportRenderRoiContext roi,
            CancellationToken cancellationToken = default) =>
            ValueTask.CompletedTask;

        public virtual ValueTask EndFrameAsync(
            ViewportRenderFrameContext context,
            CancellationToken cancellationToken = default)
        {
            EndCount++;
            return ValueTask.CompletedTask;
        }

        public ValueTask CommitFrameAsync(
            ViewportRenderCommitContext context,
            CancellationToken cancellationToken = default)
        {
            CommitCount++;
            return ValueTask.CompletedTask;
        }

        public ValueTask DiscardFrameAsync(
            ViewportRenderDiscardContext context,
            CancellationToken cancellationToken = default)
        {
            DiscardCount++;
            return ValueTask.CompletedTask;
        }
    }

    private sealed class DiscardFailureSink : TrackingSink
    {
        public override ValueTask BeginFrameAsync(
            ViewportRenderFrameContext context,
            CancellationToken cancellationToken = default)
        {
            BeginCount++;
            return ValueTask.CompletedTask;
        }

        public override ValueTask DrawTileAsync(
            ViewportRenderTileContext<string> tile,
            CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("synthetic deferred work");
        }

        public override ValueTask EndFrameAsync(
            ViewportRenderFrameContext context,
            CancellationToken cancellationToken = default)
        {
            EndCount++;
            return ValueTask.CompletedTask;
        }

        public new ValueTask DiscardFrameAsync(
            ViewportRenderDiscardContext context,
            CancellationToken cancellationToken = default)
        {
            DiscardCount++;
            throw new InvalidOperationException("synthetic discard cleanup failure");
        }
    }

    private sealed class FailingSink : TrackingSink
    {
        public override ValueTask BeginFrameAsync(
            ViewportRenderFrameContext context,
            CancellationToken cancellationToken = default)
        {
            BeginCount++;
            throw new InvalidOperationException("synthetic render failure");
        }
    }
}
