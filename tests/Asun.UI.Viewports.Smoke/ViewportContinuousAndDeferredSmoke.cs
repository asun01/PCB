using System.Drawing;
using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportContinuousAndDeferredSmoke
{
    public static async ValueTask RunAsync(Action<bool, string> assert)
    {
        await VerifyPartialDeferredRetryAsync(assert);
        await VerifyIdleWakeAsync(assert);
        await VerifySurfaceTransactionAsync(assert);
    }

    private static async ValueTask VerifyPartialDeferredRetryAsync(
        Action<bool, string> assert)
    {
        var tileSource = new RecoveringTileSource(
            new TileIndex(0, 0));

        using var pipeline = new ViewportRenderPipelineRuntime<string>(
            new Vector2(200, 100),
            new Vector2(200, 100),
            new Vector2(100, 100),
            0,
            16,
            2,
            tileSource,
            new ViewportRenderBudget(8, 8, 4, 16),
            1000);

        var roiId = pipeline.Composite.AddRoi(
            RoiGeometry.CreateRectangle(
                new Vector2(100, 50),
                new Vector2(120, 60)));

        var frame = await pipeline.RefreshAsync(
            DateTimeOffset.UtcNow.AddSeconds(1));

        assert(
            frame is not null &&
            frame.WorkPlan.Items.Count >= 2 &&
            frame.Batch.TileCount >= 2 &&
            frame.Batch.FullSurfaceCount == 1 &&
            frame.Batch.InvalidationCount == 0,
            "Partial deferred delivery should create a frame with multiple work items.");

        if (frame is null)
            return;

        var visibility = ViewportTileRoiVisibilityRuntime.Build(
            frame.Composite);

        var visibleTilesForRoi = visibility.Tiles
            .Where(tile => tile.IntersectingRoiIds.Contains(roiId))
            .Select(tile => tile.Index)
            .ToArray();

        assert(
            visibility.VisibleRoiIds.Contains(roiId) &&
            visibleTilesForRoi.Length == 2 &&
            visibility.TryGetTile(
                new TileIndex(0, 0),
                out var firstVisibleTile) &&
            firstVisibleTile.IntersectingRoiIds.Contains(roiId) &&
            visibility.TryGetTile(
                new TileIndex(1, 0),
                out var secondVisibleTile) &&
            secondVisibleTile.IntersectingRoiIds.Contains(roiId),
            "Joint visibility should associate a cross-boundary ROI with every intersecting visible tile.");

        var firstSink = new RecordingSink();
        var first = await ViewportRenderDeliveryRuntime.TryDeliverAsync(
            frame,
            firstSink);

        assert(
            first.Status == ViewportRenderDeliveryStatus.Deferred &&
            first.DeferredWorkItems.Count == 1 &&
            first.RenderedUnits > 0 &&
            first.DeferredUnits == 1 &&
            first.FrameState.IsPartial &&
            first.FrameState.PlannedUnits == frame.Batch.ItemCount &&
            first.FrameState.RegionCount == frame.Batch.RegionCount,
            "A partially available tile set should report partial delivery plus an accurate frame state.");

        assert(
            firstSink.TileIndices.Count == 1 &&
            firstSink.TileIndices[0] != new TileIndex(0, 0),
            "The available tile should render even when another tile is deferred.");

        pipeline.RequeueFrame(
            frame,
            first.DeferredWorkItems);

        pipeline.Invalidate(
            frame.Submission.DirtyFlags,
            frame.Composite.Generation);

        var retryFrame = await pipeline.RefreshAsync(
            DateTimeOffset.UtcNow.AddSeconds(2));

        assert(
            retryFrame is not null &&
            retryFrame.WorkPlan.Items.Count == 1 &&
            retryFrame.WorkPlan.Items[0].Kind == ViewportRenderWorkKind.Tile &&
            retryFrame.WorkPlan.Items[0].Tile == new TileIndex(0, 0),
            "Retry planning should contain only the previously deferred tile.");

        if (retryFrame is null)
            return;

        var retrySink = new RecordingSink();

        var retry = await ViewportRenderDeliveryRuntime.TryDeliverAsync(
            retryFrame,
            retrySink);

        assert(
            retry.Succeeded &&
            retry.Status == ViewportRenderDeliveryStatus.Succeeded &&
            retry.DeferredWorkItems.Count == 0 &&
            retrySink.TileIndices.Count == 1 &&
            retrySink.TileIndices[0] == new TileIndex(0, 0),
            "Recovered delivery should render only the deferred tile without replaying successful work.");
    }

    private static async ValueTask VerifySurfaceTransactionAsync(
        Action<bool, string> assert)
    {
        using var pipeline = new ViewportRenderPipelineRuntime<string>(
            new Vector2(200, 100),
            new Vector2(200, 100),
            new Vector2(100, 100),
            0,
            8,
            1,
            new StableTileSource(),
            new ViewportRenderBudget(4, 4, 2, 8),
            1000);

        var roiId = pipeline.Composite.AddRoi(
            RoiGeometry.CreateRectangle(
                new Vector2(100, 50),
                new Vector2(80, 50)));

        var frame = await pipeline.RefreshAsync(
            DateTimeOffset.UtcNow.AddSeconds(1));

        assert(
            frame is not null,
            "Surface transaction smoke should create a render frame.");

        if (frame is null)
            return;

        using var surface = new ViewportRenderSurfaceRuntime();
        var successfulSink = new TransactionalSink();

        var delivered = await ViewportRenderDeliveryRuntime.TryDeliverAsync(
            frame,
            successfulSink,
            surface: surface);

        var presented = surface.Snapshot;

        assert(
            delivered.Succeeded &&
            presented.State == ViewportRenderSurfaceState.Presented &&
            presented.PresentedGeneration == frame.Composite.Generation &&
            presented.PresentationSequence == 1 &&
            presented.PresentedRegionCount == frame.Batch.RegionCount &&
            successfulSink.CommitContext is not null &&
            successfulSink.CommitContext.Value.TileCount == frame.Batch.TileCount &&
            successfulSink.CommitContext.Value.RoiCount == frame.Batch.RoiCount &&
            successfulSink.CommitContext.Value.OverlayCount == frame.Batch.OverlayCount &&
            successfulSink.CommitContext.Value.InvalidationCount == frame.Batch.InvalidationCount &&
            successfulSink.CommitContext.Value.FullSurfaceCount == frame.Batch.FullSurfaceCount &&
            successfulSink.Events.SequenceEqual(
                new[] { "Begin", "End", "Commit" }) &&
            successfulSink.DiscardCount == 0,
            "A successful render should atomically transition the surface to Presented after EndFrame and Commit.");

        pipeline.Composite.SelectRoi(roiId);
        pipeline.Composite.TranslateSelected(new Vector2(12, 7));

        var incrementalFrame = await pipeline.RefreshAsync(
            DateTimeOffset.UtcNow.AddSeconds(2));

        assert(
            incrementalFrame is not null &&
            incrementalFrame.Batch.FullSurfaceCount == 0 &&
            incrementalFrame.Batch.InvalidationCount > 0 &&
            incrementalFrame.Batch.RoiCount > 0,
            "ROI motion should produce an incremental render batch without a full-surface clear.");

        if (incrementalFrame is not null)
        {
            var incrementalSink = new TransactionalSink();

            var incrementalDelivery = await ViewportRenderDeliveryRuntime.TryDeliverAsync(
                incrementalFrame,
                incrementalSink,
                surface: surface);

            var incrementalPresented = surface.Snapshot;

            assert(
                incrementalDelivery.Succeeded &&
                incrementalPresented.State == ViewportRenderSurfaceState.Presented &&
                incrementalPresented.PresentationSequence == 2 &&
                incrementalPresented.PresentedGeneration == incrementalFrame.Composite.Generation &&
                incrementalPresented.PresentedRegionCount == incrementalFrame.Batch.RegionCount &&
                incrementalSink.CommitContext is not null &&
                incrementalSink.CommitContext.Value.FullSurfaceCount == 0 &&
                incrementalSink.CommitContext.Value.InvalidationCount == incrementalFrame.Batch.InvalidationCount &&
                incrementalSink.CommitContext.Value.RoiCount == incrementalFrame.Batch.RoiCount,
                "Incremental ROI presentation should commit only the affected regions and layer counts.");
        }

        pipeline.Invalidate(
            ViewportDirtyFlags.Overlay,
            pipeline.Composite.Generation);

        var overlayFrame = await pipeline.RefreshAsync(
            DateTimeOffset.UtcNow.AddSeconds(3));

        assert(
            overlayFrame is not null &&
            overlayFrame.Batch.FullSurfaceCount == 0 &&
            overlayFrame.Batch.OverlayCount == 1,
            "Overlay invalidation should produce an overlay-only render batch.");

        if (overlayFrame is not null)
        {
            var overlaySink = new TransactionalSink();

            var overlayDelivery = await ViewportRenderDeliveryRuntime.TryDeliverAsync(
                overlayFrame,
                overlaySink,
                surface: surface);

            var overlayPresented = surface.Snapshot;

            assert(
                overlayDelivery.Succeeded &&
                overlayPresented.State == ViewportRenderSurfaceState.Presented &&
                overlayPresented.PresentationSequence == 3 &&
                overlaySink.CommitContext is not null &&
                overlaySink.CommitContext.Value.OverlayCount == 1 &&
                overlaySink.CommitContext.Value.FullSurfaceCount == 0 &&
                overlaySink.CommitContext.Value.InvalidationCount == 0,
                "Overlay-only presentation should commit without replaying the image or ROI layers.");
        }

        using var failedSurface = new ViewportRenderSurfaceRuntime();
        var failingSink = new TransactionalSink
        {
            FailCommit = true
        };

        var failed = await ViewportRenderDeliveryRuntime.TryDeliverAsync(
            frame,
            failingSink,
            surface: failedSurface);

        var discarded = failedSurface.Snapshot;

        assert(
            !failed.Succeeded &&
            failed.Status == ViewportRenderDeliveryStatus.Failed &&
            discarded.State == ViewportRenderSurfaceState.Discarded &&
            discarded.PresentedGeneration is null &&
            failingSink.Events.SequenceEqual(
                new[] { "Begin", "End", "Commit", "Discard" }) &&
            failingSink.DiscardCount == 1,
            "A commit failure should discard the active surface transaction without publishing a new generation.");
    }

    private static async ValueTask VerifyIdleWakeAsync(
        Action<bool, string> assert)
    {
        using var presentation = new ViewportPresentationRuntime<string>(
            new Vector2(300, 200),
            new Vector2(200, 100),
            new Vector2(100, 100),
            0,
            16,
            2,
            new StableTileSource(),
            new ViewportRenderBudget(8, 8, 4, 16),
            1000,
            TimeSpan.FromSeconds(10));

        using var cancellation = new CancellationTokenSource();

        var sink = new WakeSink();
        var runTask = presentation
            .RunAsync(sink, cancellation.Token)
            .AsTask();

        var initialCompleted = await Task.WhenAny(
            sink.FirstFrame.Task,
            Task.Delay(TimeSpan.FromSeconds(2)));

        var initialRendered = initialCompleted == sink.FirstFrame.Task;

        var initialSnapshot = presentation.Snapshot;

        assert(
            initialRendered &&
            presentation.LastFrameState is { IsComplete: true } &&
            initialSnapshot.LastFrameState is { IsComplete: true } &&
            initialSnapshot.Surface.State == ViewportRenderSurfaceState.Presented &&
            initialSnapshot.Surface.PresentedGeneration == presentation.Composite.Generation,
            "Continuous runtime should render its initial frame and publish a complete presentation frame state.");

        var before = presentation.Composite.Generation;
        var started = System.Diagnostics.Stopwatch.StartNew();

        assert(
            presentation.TrySubmit(
                ViewportInputEventKind.PointerMove,
                new Vector2(40, 30)),
            "Presentation input should be accepted while the runtime is idle.");

        var secondCompleted = await Task.WhenAny(
            sink.SecondFrame.Task,
            Task.Delay(TimeSpan.FromMilliseconds(500)));

        var secondRendered = secondCompleted == sink.SecondFrame.Task;

        started.Stop();

        assert(
            secondRendered &&
            presentation.Composite.Generation > before &&
            started.Elapsed < TimeSpan.FromMilliseconds(500),
            "Input activity should wake the idle presentation runtime without waiting for the long idle delay.");

        cancellation.Cancel();

        try
        {
            await runTask;
        }
        catch (OperationCanceledException)
        {
        }

        assert(
            presentation.State == ViewportPresentationState.Stopped &&
            presentation.Surface.Snapshot.State == ViewportRenderSurfaceState.Presented,
            "Continuous runtime should return to Stopped while preserving the last presented surface state.");

        presentation.Reset();

        assert(
            presentation.Surface.Snapshot.State == ViewportRenderSurfaceState.Idle &&
            presentation.LastFrameState is null,
            "Presentation reset should clear the active surface transaction and last delivery state.");
    }

    private sealed class RecoveringTileSource : ITileSource<string>
    {
        private readonly TileIndex _deferred;
        private int _failed;

        public RecoveringTileSource(TileIndex deferred)
        {
            _deferred = deferred;
            _failed = 0;
        }

        public ValueTask<string> LoadAsync(
            TileRequest request,
            RectangleF imageRectangle,
            CancellationToken cancellationToken = default)
        {
            if (request.Index == _deferred &&
                Interlocked.Exchange(ref _failed, 1) == 0)
            {
                throw new InvalidOperationException("synthetic deferred tile");
            }

            return ValueTask.FromResult(
                $"tile:{request.Index.X},{request.Index.Y}");
        }
    }

    private sealed class StableTileSource : ITileSource<string>
    {
        public ValueTask<string> LoadAsync(
            TileRequest request,
            RectangleF imageRectangle,
            CancellationToken cancellationToken = default) =>
            ValueTask.FromResult(
                $"tile:{request.Index.X},{request.Index.Y}");
    }

    private sealed class RecordingSink : IViewportRenderSink<string>
    {
        public List<TileIndex> TileIndices { get; } = new();

        public ValueTask BeginFrameAsync(
            ViewportRenderFrameContext context,
            CancellationToken cancellationToken = default) =>
            ValueTask.CompletedTask;

        public ValueTask DrawTileAsync(
            ViewportRenderTileContext<string> tile,
            CancellationToken cancellationToken = default)
        {
            TileIndices.Add(tile.Index);
            return ValueTask.CompletedTask;
        }

        public ValueTask DrawRoiAsync(
            ViewportRenderRoiContext roi,
            CancellationToken cancellationToken = default) =>
            ValueTask.CompletedTask;

        public ValueTask EndFrameAsync(
            ViewportRenderFrameContext context,
            CancellationToken cancellationToken = default) =>
            ValueTask.CompletedTask;
    }

    private sealed class TransactionalSink : IViewportRenderSink<string>
    {
        public bool FailCommit { get; init; }
        public List<string> Events { get; } = new();
        public int DiscardCount { get; private set; }
        public ViewportRenderCommitContext? CommitContext { get; private set; }

        public ValueTask BeginFrameAsync(
            ViewportRenderFrameContext context,
            CancellationToken cancellationToken = default)
        {
            Events.Add("Begin");
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
            Events.Add("End");
            return ValueTask.CompletedTask;
        }

        public ValueTask CommitFrameAsync(
            ViewportRenderCommitContext context,
            CancellationToken cancellationToken = default)
        {
            Events.Add("Commit");
            CommitContext = context;

            if (FailCommit)
                throw new InvalidOperationException("synthetic commit failure");

            return ValueTask.CompletedTask;
        }

        public ValueTask DiscardFrameAsync(
            ViewportRenderDiscardContext context,
            CancellationToken cancellationToken = default)
        {
            Events.Add("Discard");
            DiscardCount++;
            return ValueTask.CompletedTask;
        }
    }

    private sealed class WakeSink : IViewportRenderSink<string>
    {
        private int _frames;

        public TaskCompletionSource<bool> FirstFrame { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public TaskCompletionSource<bool> SecondFrame { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

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
            CancellationToken cancellationToken = default)
        {
            switch (Interlocked.Increment(ref _frames))
            {
                case 1:
                    FirstFrame.TrySetResult(true);
                    break;
                case 2:
                    SecondFrame.TrySetResult(true);
                    break;
            }

            return ValueTask.CompletedTask;
        }
    }
}
