using System.Drawing;
using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportInvariantSmoke
{
    public static async ValueTask RunAsync(Action<bool, string> assert)
    {
        using var pipeline = new ViewportRenderPipelineRuntime<string>(
            new Vector2(1000, 1000),
            new Vector2(300, 200),
            new Vector2(100, 100),
            0,
            32,
            2,
            new StableTileSource(),
            new ViewportRenderBudget(16, 32, 8, 64),
            120);

        var roiId = pipeline.Composite.AddRoi(
            RoiGeometry.CreateRectangle(
                new Vector2(150, 100),
                new Vector2(60, 40)));

        var frame = await pipeline.RefreshAsync(
            DateTimeOffset.UtcNow.AddSeconds(1));

        assert(frame is not null, "Invariant smoke should create a render frame.");

        if (frame is null)
            return;

        var sink = new ViewportRenderReplaySink<string>();
        var delivery = await ViewportRenderDeliveryRuntime.TryDeliverAsync(
            frame,
            sink);

        var frameState = delivery.FrameState;

        var planMetrics = ViewportRenderPlanMetricsRuntime.Capture(
            frame.WorkPlan,
            frame.Batch,
            new HashSet<Guid> { roiId });

        var frameMetrics = ViewportRenderFrameRuntime.Capture(frame.Composite);
        var visibleRegion = ViewportVisibleRegionRuntime.Capture(
            frame.Composite.Tiles.Transform,
            frame.Composite.Tiles.TileSize);

        var scene = ViewportSceneRuntime.Build(frame.Composite.Roi);
        var sceneDiff = ViewportSceneDiffRuntime.Diff(scene, scene);

        var input = new ViewportInputSubmissionRuntime();
        input.Submit(
            ViewportInputEventKind.PointerMove,
            new Vector2(10, 20));
        var inputSnapshot = input.Snapshot();

        var backpressure = new ViewportInputBackpressureRuntime(
            4,
            ViewportInputDropPolicy.DropNewest);
        var backpressureSnapshot = backpressure.Capture(input);

        using var queue = new ViewportPresentationQueueRuntime<string>();
        queue.TryEnqueue(frame, out var packet);
        queue.TryTakeLatest(out var inFlight);
        var queueSnapshot = queue.Statistics;
        queue.TryCancel(inFlight.Token);

        using var buffers = new ViewportPresentationBufferRuntime();
        var bufferTransaction = buffers.Begin(
            packet.Token,
            frame.Batch.ItemCount,
            frame.Batch.Regions);
        buffers.Discard(bufferTransaction);
        var bufferSnapshot = buffers.Snapshot;

        using var surface = new ViewportRenderSurfaceRuntime();
        var surfaceResult = await ViewportRenderDeliveryRuntime.TryDeliverAsync(
            frame,
            new ViewportRenderReplaySink<string>(),
            surface: surface);
        var surfaceSnapshot = surface.Snapshot;

        var workflow = new ViewportWorkflowRuntime(
            new Vector2(1000, 1000),
            new Vector2(300, 200));

        workflow.Execute(ViewportWorkflowCommand.AddRectangle(
            new Vector2(100, 80),
            new Vector2(50, 30)));

        var workflowValidation = ViewportWorkflowValidator.Validate(
            new[]
            {
                ViewportWorkflowCommand.Fit(),
                ViewportWorkflowCommand.Pan(new Vector2(2, 3)),
                ViewportWorkflowCommand.Zoom(
                    1.1,
                    0.05,
                    64,
                    new Vector2(150, 100))
            });

        var errors = new List<string>();
        Add(errors, ViewportInvariantRuntime.ValidateWorkPlan(frame.WorkPlan));
        Add(errors, ViewportInvariantRuntime.ValidateBatch(frame.Batch));
        Add(errors, ViewportInvariantRuntime.ValidateCommandStream(frame.CommandStream));
        Add(errors, ViewportInvariantRuntime.ValidateFrameState(frameState));
        Add(errors, ViewportInvariantRuntime.ValidateDeliveryStatistics(
            new ViewportRenderDeliveryTracker().Statistics));
        Add(errors, ViewportInvariantRuntime.ValidateQueueStatistics(queueSnapshot));
        Add(errors, ViewportInvariantRuntime.ValidateBufferSnapshot(bufferSnapshot));
        Add(errors, ViewportInvariantRuntime.ValidateSurfaceSnapshot(surfaceSnapshot));
        Add(errors, ViewportInvariantRuntime.ValidateReplaySnapshot(sink.Snapshot));
        Add(errors, ViewportInvariantRuntime.ValidateInputSnapshot(inputSnapshot));
        Add(errors, ViewportInvariantRuntime.ValidateBackpressureSnapshot(backpressureSnapshot));
        Add(errors, ViewportInvariantRuntime.ValidateVisibleRegion(visibleRegion));
        Add(errors, ViewportInvariantRuntime.ValidateTileFrame(frame.Composite.Tiles));
        Add(errors, ViewportInvariantRuntime.ValidateSceneSnapshot(scene));
        Add(errors, ViewportInvariantRuntime.ValidateSceneDiff(sceneDiff));
        Add(errors, ViewportInvariantRuntime.ValidateWorkflow(workflowValidation));
        Add(errors, ViewportInvariantRuntime.ValidateWorkflowJournal(workflow.Journal));
        Add(errors, ViewportInvariantRuntime.ValidateZoomProfile(
            ViewportZoomProfileRuntime.Default));
        Add(errors, ViewportInvariantRuntime.ValidatePlanMetrics(planMetrics));
        Add(errors, ViewportInvariantRuntime.ValidateFrameMetrics(frameMetrics));

        assert(
            errors.Count == 0 &&
            delivery.Succeeded &&
            surfaceResult.Succeeded &&
            inputSnapshot.Pending == 1 &&
            queueSnapshot.LatestSubmissionSequence >= packet.Token.Sequence,
            "All live viewport runtime snapshots should satisfy the centralized structural invariants.");

        var invalidFrame = frameState with
        {
            PlannedUnits = 1,
            RenderedUnits = 2,
            DeferredUnits = 1
        };

        assert(
            ViewportInvariantRuntime.ValidateFrameState(invalidFrame).Count > 0,
            "Frame-state invariant validation must reject impossible rendered/deferred totals.");

        var invalidQueue = queueSnapshot with
        {
            CommitInProgress = true,
            InFlightSequence = null,
            CommittingSequence = null
        };

        assert(
            ViewportInvariantRuntime.ValidateQueueStatistics(invalidQueue).Count > 0,
            "Queue invariant validation must reject a commit window without an active token.");

        var invalidBuffer = bufferSnapshot with
        {
            RenderingSlot = 7
        };

        assert(
            ViewportInvariantRuntime.ValidateBufferSnapshot(invalidBuffer).Count > 0,
            "Backbuffer invariant validation must reject an out-of-range slot.");

        var invalidPlan = planMetrics with
        {
            SelectedRoi = planMetrics.Roi + 1
        };

        assert(
            ViewportInvariantRuntime.ValidatePlanMetrics(invalidPlan).Count > 0,
            "Render plan invariant validation must reject selected ROI counts above ROI totals.");

        var invalidWorkflow = workflowValidation with
        {
            IsValid = true,
            ErrorCount = 1,
            Errors = new[] { "synthetic" }
        };

        assert(
            ViewportInvariantRuntime.ValidateWorkflow(invalidWorkflow).Count > 0,
            "Workflow invariant validation must reject a valid flag paired with an error.");

        input.Dispose();
    }

    private static void Add(
        List<string> errors,
        IReadOnlyList<string> current)
    {
        errors.AddRange(current);
    }

    private sealed class StableTileSource : ITileSource<string>
    {
        public ValueTask<string> LoadAsync(
            TileRequest request,
            RectangleF imageRectangle,
            CancellationToken cancellationToken = default) =>
            ValueTask.FromResult($"tile:{request.Index.X},{request.Index.Y}");
    }
}
