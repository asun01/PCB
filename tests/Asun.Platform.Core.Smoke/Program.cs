using Asun.Platform.Core;

var failures = new List<string>();

static void Assert(bool condition, string message, List<string> failures)
{
    if (!condition)
        failures.Add(message);
}

Assert(
    Math.Abs(Percentiles.Calculate(new[] { 10d, 20d, 30d, 40d, 50d }, 50) - 30) < 1e-9,
    "P50 should be the median.",
    failures);

Assert(
    Math.Abs(Percentiles.Calculate(new[] { 10d, 20d, 30d, 40d }, 25) - 17.5) < 1e-9,
    "Percentile interpolation should be deterministic.",
    failures);

var latency = new LatencyStatistics(new[] { 10d, 20d, 30d, 40d, 50d });
Assert(latency.Count == 5, "Latency count should be preserved.", failures);
Assert(Math.Abs(latency.P50 - 30) < 1e-9, "Latency P50 should be deterministic.", failures);
Assert(Math.Abs(latency.P95 - 48) < 1e-9, "Latency P95 should use interpolation.", failures);
Assert(Math.Abs(latency.P99 - 49.6) < 1e-9, "Latency P99 should use interpolation.", failures);

var execution = new List<string>();
var pipeline = new AsyncPipeline<List<string>>(new[]
{
    new AsyncPipeline<List<string>>.Node("Acquire", (_, _) =>
    {
        execution.Add("Acquire");
        return ValueTask.CompletedTask;
    }),
    new AsyncPipeline<List<string>>.Node("Inspect", new[] { "Acquire" }, (_, _) =>
    {
        execution.Add("Inspect");
        return ValueTask.CompletedTask;
    }),
    new AsyncPipeline<List<string>>.Node("Report", new[] { "Inspect" }, (_, _) =>
    {
        execution.Add("Report");
        return ValueTask.CompletedTask;
    })
});

await pipeline.ExecuteAsync(execution);
Assert(
    execution.SequenceEqual(new[] { "Acquire", "Inspect", "Report" }),
    "Pipeline dependencies should be honored.",
    failures);

var metricPipeline = new AsyncPipeline<object>(new[]
{
    new AsyncPipeline<object>.Node("Timed", async (_, token) =>
    {
        await Task.Delay(1, token);
    })
});

var pipelineMetrics = await metricPipeline.ExecuteWithMetricsAsync(new object());
Assert(
    pipelineMetrics.ContainsKey("Timed") &&
    pipelineMetrics["Timed"] > TimeSpan.Zero,
    "Pipeline metrics should record node execution duration.",
    failures);

var parallelMetricsPipeline = new AsyncPipeline<object>(new[]
{
    new AsyncPipeline<object>.Node("MetricA", async (_, token) =>
        await Task.Delay(1, token)),
    new AsyncPipeline<object>.Node("MetricB", async (_, token) =>
        await Task.Delay(1, token))
});

var parallelMetrics = await parallelMetricsPipeline.ExecuteWithMetricsAsync(new object());
Assert(
    parallelMetrics.Count == 2 &&
    parallelMetrics["MetricA"] > TimeSpan.Zero &&
    parallelMetrics["MetricB"] > TimeSpan.Zero,
    "Parallel pipeline metrics should record every node without concurrent dictionary writes.",
    failures);

using var cancelled = new CancellationTokenSource();
cancelled.Cancel();

var cancellationObserved = false;
try
{
    await pipeline.ExecuteAsync(execution, cancelled.Token);
}
catch (OperationCanceledException)
{
    cancellationObserved = true;
}

Assert(cancellationObserved, "Pipeline should honor cancellation before scheduling work.", failures);

var runningCancellationSource = new CancellationTokenSource();
var runningNodeStarted = new TaskCompletionSource<bool>(
    TaskCreationOptions.RunContinuationsAsynchronously);

var runningCancellationPipeline = new AsyncPipeline<object>(new[]
{
    new AsyncPipeline<object>.Node("Running", async (_, token) =>
    {
        runningNodeStarted.SetResult(true);
        await Task.Delay(Timeout.InfiniteTimeSpan, token);
    })
});

var runningCancellationTask = runningCancellationPipeline
    .ExecuteAsync(new object(), runningCancellationSource.Token)
    .AsTask();

await runningNodeStarted.Task;
runningCancellationSource.Cancel();

var runningCancellationObserved = false;
try
{
    await runningCancellationTask;
}
catch (OperationCanceledException)
{
    runningCancellationObserved = true;
}
finally
{
    runningCancellationSource.Dispose();
}

Assert(
    runningCancellationObserved,
    "Pipeline should propagate cancellation while a node is executing.",
    failures);

var timeoutObserved = false;
try
{
    await OperationTimeout.ExecuteAsync(
        async token => await Task.Delay(TimeSpan.FromSeconds(5), token),
        TimeSpan.FromMilliseconds(10));
}
catch (TimeoutException)
{
    timeoutObserved = true;
}

Assert(timeoutObserved, "Operation timeout should surface as TimeoutException.", failures);

var viewport = Asun.UI.Viewports.ViewportTransform.Fit(
    new System.Numerics.Vector2(1000, 500),
    new System.Numerics.Vector2(1200, 800));

var interaction = Asun.UI.Viewports.ViewportInteractionState.Create(viewport)
    .BeginPan(new System.Numerics.Vector2(100, 100))
    .UpdatePan(new System.Numerics.Vector2(120, 125));

Assert(
    interaction.IsPanning &&
    interaction.Transform.Translation == viewport.Translation + new System.Numerics.Vector2(20, 25),
    "Viewport interaction panning should follow pointer deltas.",
    failures);

interaction = interaction.EndPan();
Assert(!interaction.IsPanning, "Viewport pan should end explicitly.", failures);

var idleInteraction = Asun.UI.Viewports.ViewportInteractionState.Create(viewport);
var idleUpdated = idleInteraction.UpdatePan(new System.Numerics.Vector2(150, 175));
Assert(
    idleUpdated == idleInteraction,
    "Updating a non-panning viewport interaction should be a no-op.",
    failures);

var endedInteraction = interaction.EndPan().UpdatePan(new System.Numerics.Vector2(180, 200));
Assert(
    endedInteraction == interaction,
    "Updating after pan end should remain a no-op.",
    failures);

var roiZoom = viewport.ZoomToImageRectangle(
    new RectangleF(100, 100, 200, 100));

var roiScreenCenter = roiZoom.ImageToViewport(
    new System.Numerics.Vector2(200, 150));

Assert(
    Math.Abs(roiScreenCenter.X - viewport.ViewportCenter.X) < 1e-4f &&
    Math.Abs(roiScreenCenter.Y - viewport.ViewportCenter.Y) < 1e-4f,
    "Zoom-to-rectangle should center the selected image region.",
    failures);

var zoomInteraction = Asun.UI.Viewports.ViewportInteractionState.Create(viewport)
    .ApplyZoom(
        requestedScale: 10,
        minScale: 0.5,
        maxScale: 4,
        viewportAnchor: new System.Numerics.Vector2(600, 400));

Assert(
    Math.Abs(zoomInteraction.Transform.Scale - 4) < 1e-9,
    "Viewport interaction zoom should honor scale bounds.",
    failures);
var imagePoint = new System.Numerics.Vector2(250, 125);
var viewportPoint = viewport.ImageToViewport(imagePoint);
var roundTrip = viewport.ViewportToImage(viewportPoint);

Assert(
    Math.Abs(roundTrip.X - imagePoint.X) < 1e-4f &&
    Math.Abs(roundTrip.Y - imagePoint.Y) < 1e-4f,
    "Viewport coordinate conversion should round-trip.",
    failures);

Assert(
    viewport.ContainsViewportPoint(
        viewport.ImageToViewport(new System.Numerics.Vector2(0, 0))),
    "Viewport should contain the rendered image origin.",
    failures);

var panned = viewport.PanBy(new System.Numerics.Vector2(-100, 25));
Assert(
    panned.Translation == viewport.Translation + new System.Numerics.Vector2(-100, 25),
    "Viewport panning should apply the requested delta.",
    failures);

var resized = panned.WithViewportSize(new System.Numerics.Vector2(1600, 1000));
var centeredImagePointBeforeResize = panned.ViewportToImage(panned.ViewportCenter);
var centeredImagePointAfterResize = resized.ViewportToImage(resized.ViewportCenter);

Assert(
    Math.Abs(centeredImagePointBeforeResize.X - centeredImagePointAfterResize.X) < 1e-4f &&
    Math.Abs(centeredImagePointBeforeResize.Y - centeredImagePointAfterResize.Y) < 1e-4f &&
    resized.ViewportSize == new System.Numerics.Vector2(1600, 1000),
    "Viewport resize should preserve the current image center.",
    failures);

var visible = panned.GetVisibleImageRectangle();
var outsideTileRange = Asun.UI.Viewports.ImageTileGeometry.CalculateVisibleTiles(
    viewport.ImageSize,
    new System.Numerics.Vector2(256, 256),
    new RectangleF(2000, 2000, 100, 100));

Assert(
    outsideTileRange.Count == 0,
    "A viewport region outside the image should request no tiles.",
    failures);

var tileRange = panned.GetVisibleTileRange(
    new System.Numerics.Vector2(256, 256));

var edgeTile = Asun.UI.Viewports.ImageTileGeometry.GetTileRectangle(
    new System.Numerics.Vector2(1000, 500),
    new System.Numerics.Vector2(256, 256),
    new Asun.UI.Viewports.TileIndex(3, 1));

Assert(
    edgeTile == new RectangleF(768, 256, 232, 244),
    "Edge tile rectangles should be clipped to the image bounds.",
    failures);

var exactTile = Asun.UI.Viewports.ImageTileGeometry.CalculateVisibleTiles(
    new System.Numerics.Vector2(1024, 512),
    new System.Numerics.Vector2(256, 256),
    new RectangleF(256, 0, 256, 256));

Assert(
    exactTile.Minimum == new Asun.UI.Viewports.TileIndex(1, 0) &&
    exactTile.Maximum == new Asun.UI.Viewports.TileIndex(1, 0),
    "A tile-aligned visible rectangle should resolve to exactly one tile.",
    failures);

var tileRange = Asun.UI.Viewports.ImageTileGeometry.CalculateVisibleTiles(
    viewport.ImageSize,
    new System.Numerics.Vector2(256, 256),
    visible);

Assert(
    tileRange.Minimum.X >= 0 &&
    tileRange.Minimum.Y >= 0 &&
    tileRange.Maximum.X >= tileRange.Minimum.X &&
    tileRange.Maximum.Y >= tileRange.Minimum.Y,
    "Visible tile range should stay inside the image tile grid.",
    failures);

Assert(
    tileRange.Count == tileRange.Enumerate().Count(),
    "Visible tile range count should match enumeration.",
    failures);

var prefetchTileRange = panned.GetPrefetchTileRange(
    new System.Numerics.Vector2(256, 256),
    marginTiles: 1);

Assert(
    !prefetchTileRange.IsEmpty &&
    prefetchTileRange.Count >= tileRange.Count &&
    prefetchTileRange.Minimum.X <= tileRange.Minimum.X &&
    prefetchTileRange.Minimum.Y <= tileRange.Minimum.Y &&
    prefetchTileRange.Maximum.X >= tileRange.Maximum.X &&
    prefetchTileRange.Maximum.Y >= tileRange.Maximum.Y,
    "Tile prefetch range should contain the visible tile range.",
    failures);

var emptyTileRange = Asun.UI.Viewports.ImageTileGeometry.CalculateVisibleTiles(
    viewport.ImageSize,
    new System.Numerics.Vector2(256, 256),
    new RectangleF(-100, 0, 50, 50));

Assert(
    emptyTileRange.IsEmpty && emptyTileRange.Count == 0,
    "A fully outside tile window should remain an empty range.",
    failures);

Assert(
    visible.X >= 0 &&
    visible.Y >= 0 &&
    visible.Right <= viewport.ImageSize.X &&
    visible.Bottom <= viewport.ImageSize.Y,
    "Visible image rectangle should remain inside image bounds.",
    failures);

var zoomed = viewport.WithScaleAround(2, new System.Numerics.Vector2(600, 400));
var anchorBefore = viewport.ViewportToImage(new System.Numerics.Vector2(600, 400));
var anchorAfter = zoomed.ViewportToImage(new System.Numerics.Vector2(600, 400));

Assert(
    Math.Abs(anchorBefore.X - anchorAfter.X) < 1e-4f &&
    Math.Abs(anchorBefore.Y - anchorAfter.Y) < 1e-4f,
    "Zoom should preserve the viewport anchor.",
    failures);

var doubled = viewport.WithZoomFactor(2, new System.Numerics.Vector2(600, 400));
Assert(
    Math.Abs(doubled.Scale - zoomed.Scale) < 1e-9,
    "Zoom factor should compose with the current scale.",
    failures);

var outside = viewport.ClampViewportPointToImage(new System.Numerics.Vector2(-100, 1000));
Assert(
    Math.Abs(outside.X - viewport.Translation.X) < 1e-4f &&
    Math.Abs(outside.Y - (viewport.Translation.Y + viewport.RenderedImageSize.Y)) < 1e-4f,
    "Viewport point clamping should map to the nearest image boundary.",
    failures);

Assert(
    viewport.ViewportCenter == new System.Numerics.Vector2(600, 400) &&
    viewport.ImageCenter == new System.Numerics.Vector2(500, 250),
    "Viewport and image centers should be derived from their sizes.",
    failures);

var translation = Asun.Vision.Contracts.AffineTransform2D.Translation(10, 20);
var scale = Asun.Vision.Contracts.AffineTransform2D.UniformScale(2);
var transform = scale.Combine(translation);
var transformedPoint = transform.TransformPoint(new System.Numerics.Vector2(5, 7));
var restoredPoint = transform.Inverse.TransformPoint(transformedPoint);

Assert(
    Math.Abs(restoredPoint.X - 5) < 1e-4f &&
    Math.Abs(restoredPoint.Y - 7) < 1e-4f,
    "Affine transform inversion should restore the original point.",
    failures);

Assert(transform.IsInvertible, "A non-singular affine transform should be invertible.", failures);

var expectedComposedPoint = new System.Numerics.Vector2(20, 34);
var actualComposedPoint = transform.TransformPoint(new System.Numerics.Vector2(5, 7));
Assert(
    Math.Abs(actualComposedPoint.X - expectedComposedPoint.X) < 1e-4f &&
    Math.Abs(actualComposedPoint.Y - expectedComposedPoint.Y) < 1e-4f,
    "Affine transform composition should apply transforms in declared order.",
    failures);

var clampedZoom = viewport.WithScaleAroundClamped(
    requestedScale: 100,
    minScale: 0.5,
    maxScale: 4,
    viewportAnchor: new System.Numerics.Vector2(600, 400));

Assert(
    Math.Abs(clampedZoom.Scale - 4) < 1e-9,
    "Viewport zoom should respect the configured upper bound.",
    failures);

var clampedMinimumZoom = viewport.WithScaleAroundClamped(
    requestedScale: 0.01,
    minScale: 0.5,
    maxScale: 4,
    viewportAnchor: new System.Numerics.Vector2(600, 400));

Assert(
    Math.Abs(clampedMinimumZoom.Scale - 0.5) < 1e-9,
    "Viewport zoom should respect the configured lower bound.",
    failures);

var singular = Asun.Vision.Contracts.AffineTransform2D.Scale(1, 0);
Assert(!singular.IsInvertible, "A singular affine transform should report non-invertible.", failures);

var imageBounds = new Asun.Vision.Contracts.PixelRect(0, 0, 1000, 500);
var dragRoi = Asun.Vision.Contracts.PixelRect.FromPoints(
    new Asun.Vision.Contracts.PixelPoint(300, 200),
    new Asun.Vision.Contracts.PixelPoint(100, 50));

Assert(
    dragRoi == new Asun.Vision.Contracts.PixelRect(100, 50, 200, 150),
    "ROI creation from drag endpoints should normalize axis direction.",
    failures);

var roi = new Asun.Vision.Contracts.PixelRect(100, 50, 200, 100);

Assert(
    imageBounds.Contains(roi) &&
    roi.Contains(roi.Center),
    "Pixel rectangle containment should be deterministic.",
    failures);

var clipped = new Asun.Vision.Contracts.PixelRect(-50, 25, 200, 600)
    .ClampTo(imageBounds);
Assert(
    clipped == new Asun.Vision.Contracts.PixelRect(0, 25, 150, 475),
    "ROI clamping should keep the rectangle inside image bounds.",
    failures);

var union = roi.Union(new Asun.Vision.Contracts.PixelRect(250, 100, 200, 100));
Assert(
    union == new Asun.Vision.Contracts.PixelRect(100, 50, 350, 150),
    "Pixel rectangle union should contain both input rectangles.",
    failures);

var overlap = roi.Intersect(new Asun.Vision.Contracts.PixelRect(250, 100, 200, 100));
Assert(
    overlap == new Asun.Vision.Contracts.PixelRect(250, 100, 50, 50),
    "Pixel rectangle intersection should return the overlapping region.",
    failures);

var inflated = roi.Inflate(10, 20);
Assert(
    inflated == new Asun.Vision.Contracts.PixelRect(90, 30, 220, 140),
    "Pixel rectangle inflation should preserve the rectangle center.",
    failures);

var sourceRect = new RectangleF(100, 50, 200, 100);
var viewportRect = viewport.ImageToViewportRectangle(sourceRect);
var sourceRoundTripRect = viewport.ViewportToImageRectangle(viewportRect);

Assert(
    Math.Abs(sourceRoundTripRect.X - sourceRect.X) < 1e-4f &&
    Math.Abs(sourceRoundTripRect.Y - sourceRect.Y) < 1e-4f &&
    Math.Abs(sourceRoundTripRect.Width - sourceRect.Width) < 1e-4f &&
    Math.Abs(sourceRoundTripRect.Height - sourceRect.Height) < 1e-4f,
    "Viewport rectangle conversion should round-trip.",
    failures);

var duplicateDependencyRejected = false;
try
{
    _ = new AsyncPipeline<object>(new[]
    {
        new AsyncPipeline<object>.Node(
            "DuplicateDependency",
            new[] { "Root", "Root" },
            (_, _) => ValueTask.CompletedTask),
        new AsyncPipeline<object>.Node(
            "Root",
            (_, _) => ValueTask.CompletedTask)
    });
}
catch (ArgumentException)
{
    duplicateDependencyRejected = true;
}

Assert(
    duplicateDependencyRejected,
    "Pipeline nodes should reject duplicate dependency edges.",
    failures);

var nullNodeRejected = false;
try
{
    _ = new AsyncPipeline<object>(new AsyncPipeline<object>.Node[] { null! });
}
catch (ArgumentNullException)
{
    nullNodeRejected = true;
}

Assert(
    nullNodeRejected,
    "Pipeline should reject null nodes during construction.",
    failures);

var nullDependenciesRejected = false;
try
{
    _ = new AsyncPipeline<object>(new[]
    {
        new AsyncPipeline<object>.Node(
            "NullDependencies",
            null!,
            (_, _) => ValueTask.CompletedTask)
    });
}
catch (ArgumentNullException)
{
    nullDependenciesRejected = true;
}

Assert(
    nullDependenciesRejected,
    "Pipeline nodes should reject null dependency collections.",
    failures);

var mutableDependencies = new List<string>();
var snapshotExecution = new List<string>();
var snapshotPipeline = new AsyncPipeline<object>(new[]
{
    new AsyncPipeline<object>.Node("Root", (_, _) =>
    {
        snapshotExecution.Add("Root");
        return ValueTask.CompletedTask;
    }),
    new AsyncPipeline<object>.Node("Child", mutableDependencies, (_, _) =>
    {
        snapshotExecution.Add("Child");
        return ValueTask.CompletedTask;
    })
});

mutableDependencies.Add("Unknown");
await snapshotPipeline.ExecuteAsync(new object());
Assert(
    snapshotExecution.SequenceEqual(new[] { "Root", "Child" }),
    "Pipeline dependency collections should be snapshotted at construction.",
    failures);

var invalidGraphRejected = false;
try
{
    _ = new AsyncPipeline<object>(new[]
    {
        new AsyncPipeline<object>.Node("A", new[] { "B" }, (_, _) => ValueTask.CompletedTask),
        new AsyncPipeline<object>.Node("B", new[] { "A" }, (_, _) => ValueTask.CompletedTask)
    });
}
catch (ArgumentException)
{
    invalidGraphRejected = true;
}
Assert(invalidGraphRejected, "Cyclic pipeline graphs should be rejected.", failures);

var downstreamExecuted = false;
var failingPipeline = new AsyncPipeline<object>(new[]
{
    new AsyncPipeline<object>.Node("Fail", (_, _) =>
        ValueTask.FromException(new InvalidOperationException("expected"))),
    new AsyncPipeline<object>.Node("Downstream", new[] { "Fail" }, (_, _) =>
    {
        downstreamExecuted = true;
        return ValueTask.CompletedTask;
    })
});

var dependencyFailureObserved = false;
try
{
    await failingPipeline.ExecuteAsync(new object());
}
catch (InvalidOperationException)
{
    dependencyFailureObserved = true;
}

Assert(dependencyFailureObserved, "Pipeline dependency failures should propagate to the caller.", failures);
Assert(!downstreamExecuted, "A failed dependency must not execute downstream nodes.", failures);

using var queueCancellation = new CancellationTokenSource();
var blockedQueue = new BoundedWorkQueue<int>(1);
Assert(blockedQueue.TryEnqueue(1), "The queue should accept its first item.", failures);
var blockedEnqueue = blockedQueue.EnqueueAsync(2, queueCancellation.Token).AsTask();
queueCancellation.Cancel();

var enqueueCancelled = false;
try
{
    await blockedEnqueue;
}
catch (OperationCanceledException)
{
    enqueueCancelled = true;
}

Assert(enqueueCancelled, "A blocked enqueue should honor cancellation.", failures);

var queueCancellationDrain = new List<int>();
Assert(blockedQueue.TryDequeue(out var queuedValue) && queuedValue == 1, "The original queue item should remain intact after cancellation.", failures);
blockedQueue.Complete();
await foreach (var item in blockedQueue.ReadAllAsync())
    queueCancellationDrain.Add(item);

var parallelStarted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
var secondStarted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
var parallelPipeline = new AsyncPipeline<object>(new[]
{
    new AsyncPipeline<object>.Node("A", async (_, token) =>
    {
        parallelStarted.SetResult(true);
        await secondStarted.Task.WaitAsync(token);
    }),
    new AsyncPipeline<object>.Node("B", (_, _) =>
    {
        secondStarted.SetResult(true);
        return ValueTask.CompletedTask;
    })
});

await parallelPipeline.ExecuteAsync(new object());
Assert(
    parallelStarted.Task.IsCompletedSuccessfully &&
    secondStarted.Task.IsCompletedSuccessfully,
    "Independent pipeline nodes should be schedulable in parallel.",
    failures);

var queue = new BoundedWorkQueue<int>(2);
Assert(queue.TryEnqueue(1), "First enqueue should succeed.", failures);
Assert(queue.TryEnqueue(2), "Second enqueue should succeed.", failures);
Assert(!queue.TryEnqueue(3), "A full queue should apply backpressure.", failures);
Assert(queue.TryDequeue(out var first) && first == 1, "FIFO dequeue should preserve order.", failures);
Assert(queue.TryDequeue(out var second) && second == 2, "FIFO dequeue should preserve order.", failures);
Assert(!queue.TryDequeue(out _), "An empty queue should not produce a value.", failures);

var batchQueue = new BoundedWorkQueue<int>(4);
Assert(batchQueue.TryEnqueue(10), "Batch queue should accept the first item.", failures);
Assert(batchQueue.TryEnqueue(20), "Batch queue should accept the second item.", failures);
Assert(batchQueue.TryEnqueue(30), "Batch queue should accept the third item.", failures);

var batchBuffer = new int[2];
var batchCount = batchQueue.TryDequeueBatch(batchBuffer);
Assert(
    batchCount == 2 &&
    batchBuffer[0] == 10 &&
    batchBuffer[1] == 20,
    "Batch dequeue should preserve FIFO order and the requested batch size.",
    failures);

Assert(batchQueue.TryDequeue(out var batchTail) && batchTail == 30, "Batch dequeue should leave remaining work intact.", failures);

var asyncBatchQueue = new BoundedWorkQueue<int>(4);
Assert(asyncBatchQueue.TryEnqueue(40), "Async batch queue should accept the first item.", failures);
Assert(asyncBatchQueue.TryEnqueue(50), "Async batch queue should accept the second item.", failures);

var asyncBatchBuffer = new int[4];
var asyncBatchCount = await asyncBatchQueue.DequeueBatchAsync(asyncBatchBuffer);
Assert(
    asyncBatchCount == 2 &&
    asyncBatchBuffer[0] == 40 &&
    asyncBatchBuffer[1] == 50,
    "Async batch dequeue should wait for the first item and drain already available work.",
    failures);

Assert(queue.TryComplete(), "The first queue completion should succeed.", failures);
Assert(!queue.TryComplete(), "Repeated queue completion should be idempotent.", failures);
Assert(!queue.TryEnqueue(3), "Completed queue should reject new work.", failures);
Assert(queue.IsCompleted, "Completed queue should report completion after draining.", failures);
await queue.Completion;

using var resources = new ResourceLeasePool<string>(new[]
{
    new KeyValuePair<string, int>("camera", 1),
    new KeyValuePair<string, int>("gpu", 2)
});

Assert(resources.Capacity("camera") == 1, "Configured resource capacity should be exposed.", failures);
Assert(resources.Available("gpu") == 2, "Independent resource capacity should start available.", failures);

Assert(resources.TryAcquire("camera", out var cameraLease), "The first lease should be granted.", failures);
Assert(!resources.TryAcquire("camera", out _), "A saturated resource should reject a non-blocking lease.", failures);

var waitingLeaseTask = resources.AcquireAsync("camera").AsTask();
Assert(!waitingLeaseTask.IsCompleted, "A saturated resource should apply bounded waiting.", failures);

cameraLease!.Dispose();
using var secondCameraLease = await waitingLeaseTask;
Assert(resources.Available("camera") == 0, "Acquired lease should consume the available slot.", failures);

using var gpuLease1 = await resources.AcquireAsync("gpu");
using var gpuLease2 = await resources.AcquireAsync("gpu");
Assert(resources.Available("gpu") == 0, "Independent resource leases should be bounded by capacity.", failures);

var cancelledLease = false;
using (var leaseCancellation = new CancellationTokenSource())
{
    leaseCancellation.Cancel();

    try
    {
        _ = await resources.AcquireAsync("gpu", leaseCancellation.Token);
    }
    catch (OperationCanceledException)
    {
        cancelledLease = true;
    }
}

Assert(cancelledLease, "Resource acquisition should honor cancellation.", failures);

using (var disposablePool = new ResourceLeasePool<string>(
           new[] { new KeyValuePair<string, int>("single", 1) }))
{
    Assert(
        disposablePool.TryAcquire("single", out var activeLease),
        "A disposable pool should grant its active lease.",
        failures);

    disposablePool.Dispose();
    activeLease!.Dispose();
}

Assert(
    resources.Available("camera") == 0,
    "Active lease state should remain consistent before final disposal.",
    failures);

if (failures.Count > 0)
{
    foreach (var failure in failures)
        Console.Error.WriteLine($"FAIL: {failure}");

    return 1;
}

Console.WriteLine("Asun.Platform.Core smoke tests passed.");
return 0;
