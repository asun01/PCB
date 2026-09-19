using System.Drawing;
using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportPresentationEndToEndSmoke
{
    public static async ValueTask RunAsync(Action<bool, string> assert)
    {
        await VerifyTileRoiVisibilityAsync(assert);
        await VerifyContinuousPresentationAsync(assert);
    }

    private static async ValueTask VerifyTileRoiVisibilityAsync(
        Action<bool, string> assert)
    {
        using var pipeline = CreatePipeline(new StableTileSource());

        var visibleRoi = pipeline.Composite.AddRoi(
            RoiGeometry.CreateRectangle(
                new Vector2(150, 100),
                new Vector2(80, 60)));

        var hiddenRoi = pipeline.Composite.AddRoi(
            RoiGeometry.CreateRectangle(
                new Vector2(850, 850),
                new Vector2(80, 60)));

        var frame = await pipeline.RefreshAsync(
            DateTimeOffset.UtcNow.AddSeconds(1));

        assert(
            frame is not null,
            "End-to-end visibility smoke should create an initial frame.");

        if (frame is null)
            return;

        var visibility = ViewportTileRoiVisibilityRuntime.Build(
            frame.Composite);

        assert(
            visibility.VisibleRoiIds.Contains(visibleRoi) &&
            !visibility.VisibleRoiIds.Contains(hiddenRoi),
            "Tile/ROI visibility should include only ROI geometry intersecting the current viewport.");

        assert(
            visibility.Tiles.Any(tile =>
                tile.IsLoaded &&
                tile.IntersectingRoiIds.Contains(visibleRoi)),
            "At least one loaded visible tile should report the visible ROI intersection.");

        assert(
            !visibility.Tiles.Any(tile =>
                tile.IntersectingRoiIds.Contains(hiddenRoi)),
            "An off-viewport ROI must not be projected into visible tile/ROI work.");

        var sink = new ViewportRenderReplaySink<string>();

        var delivery = await ViewportRenderDeliveryRuntime.TryDeliverAsync(
            frame,
            sink);

        var replay = sink.Snapshot;
        var commandHash = ViewportRenderEvidenceRuntime
            .ComputeCommandStreamHash(frame.CommandStream);
        var frameHash = ViewportRenderEvidenceRuntime
            .ComputePipelineFrameHash(frame);

        assert(
            delivery.Succeeded &&
            replay.TileCount > 0 &&
            replay.RoiCount > 0 &&
            replay.BeginCount == 1 &&
            replay.EndCount == 1 &&
            replay.CommitCount == 0,
            "A backend-neutral direct delivery should replay the visible Tile and ROI layers without requiring a presentation surface.");

        assert(
            replay.Operations
                .Where(operation =>
                    operation.Kind == ViewportRenderReplayOperationKind.DrawRoi)
                .All(operation => operation.RoiId == visibleRoi),
            "Replay delivery must never draw an off-viewport ROI.");

        assert(
            commandHash.Length == 64 &&
            frameHash.Length == 64 &&
            replay.EvidenceHash.Length == 64,
            "Render evidence fingerprints must use fixed-length SHA-256 representations.");

        var secondSink = new ViewportRenderReplaySink<string>();
        var secondDelivery = await ViewportRenderDeliveryRuntime.TryDeliverAsync(
            frame,
            secondSink);

        assert(
            secondDelivery.Succeeded &&
            secondSink.Snapshot.EvidenceHash == replay.EvidenceHash &&
            ViewportRenderEvidenceRuntime.ComputeCommandStreamHash(frame.CommandStream) == commandHash &&
            ViewportRenderEvidenceRuntime.ComputePipelineFrameHash(frame) == frameHash,
            "Replaying the same immutable frame must produce deterministic evidence.");

        secondSink.Reset();

        assert(
            secondSink.Snapshot.OperationCount == 0 &&
            secondSink.Snapshot.EvidenceHash ==
            ViewportRenderEvidenceRuntime.ComputeReplayHash(
                Array.Empty<ViewportRenderReplayOperation>()),
            "Replay sink reset must clear evidence operations deterministically.");
    }

    private static async ValueTask VerifyContinuousPresentationAsync(
        Action<bool, string> assert)
    {
        using var runtime = new ViewportPresentationRuntime<string>(
            new Vector2(1000, 1000),
            new Vector2(300, 200),
            new Vector2(100, 100),
            prefetchMarginTiles: 0,
            cacheCapacity: 32,
            maxConcurrency: 2,
            tileSource: new StableTileSource(),
            budget: new ViewportRenderBudget(16, 32, 8, 64),
            framesPerSecond: 120,
            idleDelay: TimeSpan.FromMilliseconds(1));

        var roiId = runtime.Composite.AddRoi(
            RoiGeometry.CreateRectangle(
                new Vector2(140, 90),
                new Vector2(60, 40)));

        var sink = new ViewportRenderReplaySink<string>();
        using var cancellation = new CancellationTokenSource();

        var worker = runtime
            .RunAsync(sink, cancellation.Token)
            .AsTask();

        var firstCommitted = await WaitUntilAsync(
            () => sink.Snapshot.CommitCount >= 1,
            TimeSpan.FromSeconds(2));

        assert(
            firstCommitted,
            "Continuous presentation should reach its first backend commit.");

        var firstGeneration = runtime.Composite.Generation;

        runtime.Submit(
            ViewportInputEventKind.PointerDown,
            new Vector2(140, 90),
            button: ViewportMouseButton.Middle);

        runtime.Submit(
            ViewportInputEventKind.PointerMove,
            new Vector2(170, 110),
            button: ViewportMouseButton.Middle);

        runtime.Submit(
            ViewportInputEventKind.PointerUp,
            new Vector2(170, 110),
            button: ViewportMouseButton.Middle);

        var generationAdvanced = await WaitUntilAsync(
            () => runtime.Composite.Generation > firstGeneration,
            TimeSpan.FromSeconds(2));

        assert(
            generationAdvanced,
            "Submitted pointer input should cross the input queue and advance the composite generation.");

        var secondCommitted = await WaitUntilAsync(
            () => sink.Snapshot.CommitCount >= 2,
            TimeSpan.FromSeconds(2));

        assert(
            secondCommitted,
            "Continuous presentation should commit a second frame after input-driven navigation.");

        runtime.Submit(
            ViewportInputEventKind.Wheel,
            new Vector2(150, 100),
            wheelDelta: 120);

        var zoomAdvanced = await WaitUntilAsync(
            () => runtime.Composite.Generation > firstGeneration + 1,
            TimeSpan.FromSeconds(2));

        assert(
            zoomAdvanced,
            "Wheel input should advance the composite generation through the same submission pipeline.");

        var thirdCommitted = await WaitUntilAsync(
            () => sink.Snapshot.CommitCount >= 3,
            TimeSpan.FromSeconds(2));

        assert(
            thirdCommitted,
            "Continuous presentation should commit the wheel-driven frame.");

        var snapshot = runtime.Snapshot;
        var replay = sink.Snapshot;

        assert(
            replay.BeginCount == replay.EndCount &&
            replay.BeginCount >= 3 &&
            replay.CommitCount >= 3 &&
            replay.DiscardCount == 0 &&
            replay.TileCount > 0 &&
            replay.RoiCount > 0,
            "Continuous replay should contain balanced frames, committed presentations, and visible Tile/ROI operations.");

        assert(
            snapshot.Execution.Presented >= 3 &&
            snapshot.Queue.PresentedSequence is not null &&
            snapshot.Buffer.PresentedSequence is not null &&
            snapshot.Surface.PresentationSequence >= 3 &&
            snapshot.Frames.RenderedFrames >= 3,
            "Queue, double buffer, surface, and frame runtime must converge on the committed presentation path.");

        assert(
            snapshot.IsPresentationStable &&
            snapshot.LastFrameState?.IsComplete == true &&
            snapshot.LastFrameState?.Status == ViewportRenderDeliveryStatus.Succeeded,
            "The continuous runtime should expose a stable completed presentation snapshot.");

        assert(
            runtime.Composite.Roi.Document.Items.Any(item => item.Id == roiId),
            "The ROI created before the continuous loop should remain part of the authoritative document.");

        cancellation.Cancel();

        try
        {
            await worker;
        }
        catch (OperationCanceledException)
        {
        }

        await runtime.StopAsync();

        assert(
            runtime.State == ViewportPresentationState.Stopped,
            "Stopping the presentation loop should leave the lifecycle in a deterministic stopped state.");
    }

    private static async ValueTask<bool> WaitUntilAsync(
        Func<bool> predicate,
        TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;

        while (!predicate())
        {
            if (DateTime.UtcNow >= deadline)
                return false;

            await Task.Delay(TimeSpan.FromMilliseconds(5));
        }

        return true;
    }

    private static ViewportRenderPipelineRuntime<string> CreatePipeline(
        ITileSource<string> source) =>
        new(
            new Vector2(1000, 1000),
            new Vector2(300, 200),
            new Vector2(100, 100),
            0,
            32,
            2,
            source,
            new ViewportRenderBudget(16, 32, 8, 64),
            120);

    private sealed class StableTileSource : ITileSource<string>
    {
        public ValueTask<string> LoadAsync(
            TileRequest request,
            RectangleF imageRectangle,
            CancellationToken cancellationToken = default) =>
            ValueTask.FromResult(
                $"tile:{request.Index.X},{request.Index.Y}");
    }
}
