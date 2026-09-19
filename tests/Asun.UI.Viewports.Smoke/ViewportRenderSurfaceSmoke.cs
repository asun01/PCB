using System.Drawing;
using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportRenderSurfaceSmoke
{
    public static async ValueTask RunAsync(Action<bool, string> assert)
    {
        await VerifySuccessfulCommitAsync(assert);
        await VerifyIncrementalLayerCommitAsync(assert);
        await VerifyResetAsync(assert);
        await VerifyStaleGenerationFenceAsync(assert);
        await VerifyConcurrentDisposeAsync(assert);
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
            snapshot.LastRenderedUnits == result.RenderedUnits &&
            snapshot.PresentedRegionCount == frame.Batch.RegionCount &&
            sink.CommitContext is not null &&
            sink.CommitContext.Value.TileCount == frame.Batch.TileCount &&
            sink.CommitContext.Value.RoiCount == frame.Batch.RoiCount &&
            sink.CommitContext.Value.OverlayCount == frame.Batch.OverlayCount &&
            sink.CommitContext.Value.InvalidationCount == frame.Batch.InvalidationCount &&
            sink.CommitContext.Value.FullSurfaceCount == frame.Batch.FullSurfaceCount,
            "Successful delivery should commit exactly once and publish generation, regions, and layer metrics to the surface.");
    }

    private static async ValueTask VerifyIncrementalLayerCommitAsync(
        Action<bool, string> assert)
    {
        using var pipeline = CreatePipeline(new StableTileSource());

        var roiId = pipeline.Composite.AddRoi(
            RoiGeometry.CreateRectangle(
                new Vector2(50, 50),
                new Vector2(40, 30)));

        var first = await pipeline.RefreshAsync(
            DateTimeOffset.UtcNow.AddSeconds(4));

        assert(
            first is not null,
            "Surface smoke should create an initial frame before incremental presentation.");

        if (first is null)
            return;

        using var surface = new ViewportRenderSurfaceRuntime();

        var firstResult = await ViewportRenderDeliveryRuntime.TryDeliverAsync(
            first,
            new TrackingSink(),
            surface: surface);

        assert(
            firstResult.Succeeded &&
            surface.Snapshot.PresentationSequence == 1,
            "Surface smoke should establish a first presented generation before incremental updates.");

        pipeline.Composite.SelectRoi(roiId);
        pipeline.Composite.TranslateSelected(new Vector2(10, 6));

        var incremental = await pipeline.RefreshAsync(
            DateTimeOffset.UtcNow.AddSeconds(5));

        assert(
            incremental is not null &&
            incremental.Batch.FullSurfaceCount == 0 &&
            incremental.Batch.InvalidationCount > 0 &&
            incremental.Batch.RoiCount > 0,
            "ROI movement should create an incremental batch without a full-surface clear.");

        if (incremental is null)
            return;

        var incrementalSink = new TrackingSink();

        var incrementalResult = await ViewportRenderDeliveryRuntime.TryDeliverAsync(
            incremental,
            incrementalSink,
            surface: surface);

        var incrementalSnapshot = surface.Snapshot;

        assert(
            incrementalResult.Succeeded &&
            incrementalSnapshot.State == ViewportRenderSurfaceState.Presented &&
            incrementalSnapshot.PresentationSequence == 2 &&
            incrementalSnapshot.PresentedGeneration == incremental.Composite.Generation &&
            incrementalSnapshot.PresentedRegionCount == incremental.Batch.RegionCount &&
            incrementalSink.CommitContext is not null &&
            incrementalSink.CommitContext.Value.FullSurfaceCount == 0 &&
            incrementalSink.CommitContext.Value.InvalidationCount == incremental.Batch.InvalidationCount &&
            incrementalSink.CommitContext.Value.RoiCount == incremental.Batch.RoiCount,
            "Incremental ROI presentation should publish only the affected region set and layer metrics.");

        pipeline.Invalidate(
            ViewportDirtyFlags.Overlay,
            pipeline.Composite.Generation);

        var overlay = await pipeline.RefreshAsync(
            DateTimeOffset.UtcNow.AddSeconds(6));

        assert(
            overlay is not null &&
            overlay.Batch.FullSurfaceCount == 0 &&
            overlay.Batch.OverlayCount == 1 &&
            overlay.Batch.TileCount == 0 &&
            overlay.Batch.RoiCount == 0,
            "Overlay invalidation should create an overlay-only batch without replaying tiles or ROI.");

        if (overlay is null)
            return;

        var overlaySink = new TrackingSink();

        var overlayResult = await ViewportRenderDeliveryRuntime.TryDeliverAsync(
            overlay,
            overlaySink,
            surface: surface);

        var overlaySnapshot = surface.Snapshot;

        assert(
            overlayResult.Succeeded &&
            overlaySnapshot.State == ViewportRenderSurfaceState.Presented &&
            overlaySnapshot.PresentationSequence == 3 &&
            overlaySink.CommitContext is not null &&
            overlaySink.CommitContext.Value.OverlayCount == 1 &&
            overlaySink.CommitContext.Value.TileCount == 0 &&
            overlaySink.CommitContext.Value.RoiCount == 0 &&
            overlaySink.CommitContext.Value.FullSurfaceCount == 0 &&
            overlaySink.CommitContext.Value.InvalidationCount == 0,
            "Overlay-only presentation should advance the surface without replaying image or ROI layers.");
    }

    private static async ValueTask VerifyResetAsync(
        Action<bool, string> assert)
    {
        using var pipeline = CreatePipeline(new StableTileSource());

        var frame = await pipeline.RefreshAsync(
            DateTimeOffset.UtcNow.AddSeconds(7));

        assert(
            frame is not null,
            "Surface smoke should create a frame for reset verification.");

        if (frame is null)
            return;

        using var surface = new ViewportRenderSurfaceRuntime();
        var sink = new TrackingSink();

        var result = await ViewportRenderDeliveryRuntime.TryDeliverAsync(
            frame,
            sink,
            surface: surface);

        assert(
            result.Succeeded &&
            surface.Snapshot.PresentationSequence == 1,
            "Surface smoke should establish presentation history before reset.");

        surface.Reset();

        var snapshot = surface.Snapshot;

        assert(
            snapshot.State == ViewportRenderSurfaceState.Idle &&
            snapshot.RenderingGeneration is null &&
            snapshot.PresentedGeneration is null &&
            snapshot.DiscardedGeneration is null &&
            snapshot.LastDiscardStatus is null &&
            snapshot.PresentationSequence == 0 &&
            snapshot.LastRenderedUnits == 0 &&
            snapshot.LastPlannedUnits == 0 &&
            snapshot.PresentedRegionCount == 0,
            "Surface reset should clear the entire presentation session state.");
    }

    private static async ValueTask VerifyStaleGenerationFenceAsync(
        Action<bool, string> assert)
    {
        using var surface = new ViewportRenderSurfaceRuntime();

        var transaction = surface.Begin(10);
        surface.Commit(
            transaction,
            plannedUnits: 1,
            renderedUnits: 1,
            regions: Array.Empty<RectangleF>());

        assert(
            surface.Snapshot.PresentedGeneration == 10,
            "Surface stale-generation smoke should establish a newer presented generation first.");

        var rejected = false;

        try
        {
            surface.Begin(9);
        }
        catch (InvalidOperationException)
        {
            rejected = true;
        }

        assert(
            rejected &&
            surface.Snapshot.State == ViewportRenderSurfaceState.Presented &&
            surface.Snapshot.PresentedGeneration == 10,
            "Surface should reject an older generation without disturbing the newer presented surface.");

        var retryTransaction = surface.Begin(10);

        assert(
            retryTransaction.Sequence > transaction.Sequence,
            "A same-generation retry must receive a new transaction sequence after discard.");

        surface.Discard(
            retryTransaction,
            ViewportRenderDeliveryStatus.Cancelled);

        var staleCommitRejected = false;

        try
        {
            surface.Commit(
                transaction,
                plannedUnits: 1,
                renderedUnits: 1,
                regions: Array.Empty<RectangleF>());
        }
        catch (InvalidOperationException)
        {
            staleCommitRejected = true;
        }

        assert(
            staleCommitRejected &&
            surface.Snapshot.DiscardedGeneration == 10 &&
            surface.Snapshot.PresentedGeneration == 10,
            "An older discarded transaction token must not be able to commit over a later same-generation transaction.");
    }

    private static async ValueTask VerifyConcurrentDisposeAsync(
        Action<bool, string> assert)
    {
        using var pipeline = CreatePipeline(new StableTileSource());

        var frame = await pipeline.RefreshAsync(
            DateTimeOffset.UtcNow.AddSeconds(8));

        assert(
            frame is not null,
            "Surface smoke should create a frame for concurrent-dispose verification.");

        if (frame is null)
            return;

        using var surface = new ViewportRenderSurfaceRuntime();
        var sink = new DisposeDuringCommitSink(surface);

        var result = await ViewportRenderDeliveryRuntime.TryDeliverAsync(
            frame,
            sink,
            surface: surface);

        assert(
            !result.Succeeded &&
            result.Status == ViewportRenderDeliveryStatus.Failed &&
            result.Error is ObjectDisposedException &&
            sink.CommitCount == 1,
            "Concurrent surface disposal should preserve the original commit/dispose failure without a secondary rollback exception.");
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
            surface.Snapshot.DiscardedGeneration == frame.Composite.Generation &&
            surface.Snapshot.LastDiscardStatus == ViewportRenderDeliveryStatus.Failed &&
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
            surface.Snapshot.DiscardedGeneration == frame.Composite.Generation &&
            surface.Snapshot.LastDiscardStatus == ViewportRenderDeliveryStatus.Deferred &&
            sink.CommitCount == 0 &&
            sink.DiscardCount == 1,
            "Deferred delivery should discard the incomplete rendering transaction.");


        using var secondSurface = new ViewportRenderSurfaceRuntime();
        var discardFailureSink = new DiscardFailureSink();

        var secondResult = await ViewportRenderDeliveryRuntime.TryDeliverAsync(
            frame,
            discardFailureSink,
            surface: secondSurface);

        assert(
            secondResult.Status == ViewportRenderDeliveryStatus.Deferred &&
            secondSurface.Snapshot.State == ViewportRenderSurfaceState.Discarded &&
            discardFailureSink.DiscardCount == 1,
            "A failing discard callback must not replace the original deferred delivery result.");
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
        public ViewportRenderCommitContext? CommitContext { get; private set; }

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

        public virtual ValueTask CommitFrameAsync(
            ViewportRenderCommitContext context,
            CancellationToken cancellationToken = default)
        {
            CommitCount++;
            CommitContext = context;
            return ValueTask.CompletedTask;
        }

        public virtual ValueTask DiscardFrameAsync(
            ViewportRenderDiscardContext context,
            CancellationToken cancellationToken = default)
        {
            DiscardCount++;
            return ValueTask.CompletedTask;
        }
    }

    private sealed class DisposeDuringCommitSink : TrackingSink
    {
        private readonly ViewportRenderSurfaceRuntime _surface;

        public DisposeDuringCommitSink(ViewportRenderSurfaceRuntime surface)
        {
            _surface = surface;
        }

        public override ValueTask CommitFrameAsync(
            ViewportRenderCommitContext context,
            CancellationToken cancellationToken = default)
        {
            CommitCount++;
            _surface.Dispose();
            throw new ObjectDisposedException(nameof(ViewportRenderSurfaceRuntime));
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

        public override ValueTask DiscardFrameAsync(
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
