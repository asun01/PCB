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

var manyPercentiles = Percentiles.CalculateMany(
    new[] { 10d, 20d, 30d, 40d, 50d },
    new[] { 0d, 50d, 95d });

Assert(
    manyPercentiles.Count == 3 &&
    Math.Abs(manyPercentiles[0] - 10) < 1e-9 &&
    Math.Abs(manyPercentiles[1] - 30) < 1e-9 &&
    Math.Abs(manyPercentiles[2] - 48) < 1e-9,
    "Multiple percentile calculation should reuse one deterministic ordering.",
    failures);

Assert(
    Math.Abs(Percentiles.Calculate(new[] { 10d, 20d, 30d, 40d }, 25) - 17.5) < 1e-9,
    "Percentile interpolation should be deterministic.",
    failures);

var latency = new LatencyStatistics(new[] { 10d, 20d, 30d, 40d, 50d });
Assert(latency.Count == 5, "Latency count should be preserved.", failures);
Assert(
    latency.Median == latency.P50 &&
    latency.Samples.Count == 5 &&
    latency.Samples[0] == 10 &&
    latency.Samples[^1] == 50,
    "Latency statistics should expose deterministic median and ordered samples.",
    failures);
Assert(Math.Abs(latency.P50 - 30) < 1e-9, "Latency P50 should be deterministic.", failures);
Assert(Math.Abs(latency.P95 - 48) < 1e-9, "Latency P95 should use interpolation.", failures);
Assert(Math.Abs(latency.P99 - 49.6) < 1e-9, "Latency P99 should use interpolation.", failures);
Assert(
    Math.Abs(latency.Percentile(25) - 20) < 1e-9,
    "Arbitrary latency percentile should use the same interpolation rule.",
    failures);

var emptyRequestedPercentiles = Percentiles.CalculateMany(
    new[] { 0d, 10d, 20d },
    Array.Empty<double>());
Assert(
    emptyRequestedPercentiles.Count == 0,
    "An empty percentile request should return an empty result without changing the sample contract.",
    failures);

var duplicateRequestedPercentiles = Percentiles.CalculateMany(
    new[] { 0d, 10d, 20d },
    new[] { 50d, 50d, 100d });

Assert(
    duplicateRequestedPercentiles.Count == 3 &&
    Math.Abs(duplicateRequestedPercentiles[0] - duplicateRequestedPercentiles[1]) < 1e-9 &&
    Math.Abs(duplicateRequestedPercentiles[2] - 20) < 1e-9,
    "Multiple percentile calculation should preserve requested order and duplicates.",
    failures);

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
    pipeline.NodeCount == 3 &&
    pipeline.ContainsNode("Inspect") &&
    !pipeline.ContainsNode("Missing"),
    "Pipeline metadata should expose its node graph deterministically.",
    failures);

Assert(
    pipeline.TryGetNode("Report", out var reportNode) &&
    reportNode!.Id == "Report",
    "Pipeline node lookup should return the requested node.",
    failures);

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

var cancelledInteraction = interaction.BeginPan(new System.Numerics.Vector2(200, 200)).CancelPan();
Assert(!cancelledInteraction.IsPanning, "Viewport pan cancellation should end the active interaction.", failures);

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

var factorZoomInteraction = Asun.UI.Viewports.ViewportInteractionState.Create(viewport)
    .ApplyZoomFactor(
        zoomFactor: 10,
        minScale: 0.5,
        maxScale: 4,
        viewportAnchor: new System.Numerics.Vector2(600, 400));

Assert(
    Math.Abs(factorZoomInteraction.Transform.Scale - 4) < 1e-9,
    "Viewport interaction zoom factor should respect scale bounds.",
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

Assert(
    !viewport.ContainsViewportPoint(new System.Numerics.Vector2(float.NaN, 0)),
    "Viewport containment should reject non-finite points.",
    failures);

var renderedImageRectangle = viewport.RenderedImageRectangle;
Assert(
    renderedImageRectangle == new RectangleF(
        viewport.Translation.X,
        viewport.Translation.Y,
        viewport.RenderedImageSize.X,
        viewport.RenderedImageSize.Y),
    "Rendered image rectangle should match scale and translation.",
    failures);

var panClamped = viewport.PanByClamped(new System.Numerics.Vector2(-5000, 5000));
Assert(
    panClamped.RenderedImageRectangle.Right >= panClamped.ViewportSize.X &&
    panClamped.RenderedImageRectangle.Bottom >= panClamped.ViewportSize.Y,
    "PanByClamped should prevent blank viewport gaps.",
    failures);

var resetFit = viewport.PanBy(new System.Numerics.Vector2(-100, 25))
    .WithZoomFactor(2, new System.Numerics.Vector2(600, 400))
    .ResetToFit();

Assert(
    resetFit.ApproximatelyEquals(viewport),
    "ResetToFit should restore the canonical fit transform.",
    failures);

var panned = viewport.PanBy(new System.Numerics.Vector2(-100, 25));
Assert(
    panned.Translation == viewport.Translation + new System.Numerics.Vector2(-100, 25),
    "Viewport panning should apply the requested delta.",
    failures);

var resized = panned.WithViewportSize(new System.Numerics.Vector2(1600, 1000));

var fittedInteraction = Asun.UI.Viewports.ViewportInteractionState.Create(panned)
    .BeginPan(new System.Numerics.Vector2(50, 50))
    .FitToViewport();

Assert(
    !fittedInteraction.IsPanning &&
    fittedInteraction.Transform.IsImageFullyVisible,
    "FitToViewport should reset interaction state to a fully visible image.",
    failures);
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

Assert(
    Asun.UI.Viewports.ImageTileGeometry.ContainsTile(
        new System.Numerics.Vector2(1000, 500),
        new System.Numerics.Vector2(256, 256),
        new Asun.UI.Viewports.TileIndex(3, 1)) &&
    !Asun.UI.Viewports.ImageTileGeometry.ContainsTile(
        new System.Numerics.Vector2(1000, 500),
        new System.Numerics.Vector2(256, 256),
        new Asun.UI.Viewports.TileIndex(4, 1)),
    "Tile grid membership should reject indexes outside the image grid.",
    failures);

var outsidePointFound = Asun.UI.Viewports.ImageTileGeometry.TryGetTileIndexAtImagePoint(
    new System.Numerics.Vector2(1000, 500),
    new System.Numerics.Vector2(256, 256),
    new System.Numerics.Vector2(-1, 250),
    out _);

Assert(!outsidePointFound, "Points outside the image should not resolve to a tile.", failures);

var pointTileFound = Asun.UI.Viewports.ImageTileGeometry.TryGetTileIndexAtImagePoint(
    new System.Numerics.Vector2(1000, 500),
    new System.Numerics.Vector2(256, 256),
    new System.Numerics.Vector2(300, 300),
    out var pointTile);

Assert(
    pointTileFound && pointTile == new Asun.UI.Viewports.TileIndex(1, 1),
    "Image points should resolve to their containing tile.",
    failures);

var boundaryTile = Asun.UI.Viewports.ImageTileGeometry.GetTileIndexAtImagePoint(
    new System.Numerics.Vector2(1024, 512),
    new System.Numerics.Vector2(256, 256),
    new System.Numerics.Vector2(256, 256));

Assert(
    boundaryTile == new Asun.UI.Viewports.TileIndex(1, 1),
    "Tile boundaries should use the tile on the right and bottom for interior boundary points.",
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

var rangeRectangle = Asun.UI.Viewports.ImageTileGeometry.GetTileRangeRectangle(
    new System.Numerics.Vector2(1000, 500),
    new System.Numerics.Vector2(256, 256),
    new Asun.UI.Viewports.VisibleTileRange(
        new Asun.UI.Viewports.TileIndex(2, 0),
        new Asun.UI.Viewports.TileIndex(3, 1)));

Assert(
    rangeRectangle == new RectangleF(512, 0, 488, 500),
    "Tile range rectangle should cover every requested tile without exceeding image bounds.",
    failures);

var visibleTileRange = Asun.UI.Viewports.ImageTileGeometry.CalculateVisibleTiles(
    viewport.ImageSize,
    new System.Numerics.Vector2(256, 256),
    visible);

Assert(
    visibleTileRange.Minimum.X >= 0 &&
    tileRange.Minimum.Y >= 0 &&
    visibleTileRange.Maximum.X >= visibleTileRange.Minimum.X &&
    visibleTileRange.Maximum.Y >= visibleTileRange.Minimum.Y,
    "Visible tile range should stay inside the image tile grid.",
    failures);

Assert(
    visibleTileRange.Count == visibleTileRange.Enumerate().Count(),
    "Visible tile range count should match enumeration.",
    failures);

var asymmetricPrefetch = Asun.UI.Viewports.ImageTileGeometry.ExpandTileRange(
    new System.Numerics.Vector2(1000, 500),
    new System.Numerics.Vector2(256, 256),
    new Asun.UI.Viewports.VisibleTileRange(
        new Asun.UI.Viewports.TileIndex(1, 1),
        new Asun.UI.Viewports.TileIndex(2, 1)),
    marginX: 1,
    marginY: 0);

Assert(
    asymmetricPrefetch.Minimum == new Asun.UI.Viewports.TileIndex(0, 1) &&
    asymmetricPrefetch.Maximum == new Asun.UI.Viewports.TileIndex(3, 1),
    "Asymmetric tile margins should expand only the requested axes.",
    failures);

var prefetchTileRange = panned.GetPrefetchTileRange(
    new System.Numerics.Vector2(256, 256),
    marginTiles: 1);

var centerTile = Asun.UI.Viewports.ImageTileGeometry.GetCenterTileIndex(
    new System.Numerics.Vector2(1000, 500),
    new System.Numerics.Vector2(256, 256),
    new System.Numerics.Vector2(500, 250));

var centerPoint = Asun.UI.Viewports.ImageTileGeometry.GetTileCenter(
    new System.Numerics.Vector2(1000, 500),
    new System.Numerics.Vector2(256, 256),
    centerTile);

Assert(
    centerTile == new Asun.UI.Viewports.TileIndex(1, 0) &&
    centerPoint.X >= 256 && centerPoint.X < 512 &&
    centerPoint.Y >= 0 && centerPoint.Y < 256,
    "Tile center helpers should resolve a point and return a point inside that tile.",
    failures);

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

var clampedTile = Asun.UI.Viewports.ImageTileGeometry.ClampTileIndex(
    new System.Numerics.Vector2(1000, 500),
    new System.Numerics.Vector2(256, 256),
    new Asun.UI.Viewports.TileIndex(-4, 99));

Assert(
    clampedTile == new Asun.UI.Viewports.TileIndex(0, 1),
    "Tile index clamping should stay within the finite grid.",
    failures);

Assert(
    visible.X >= 0 &&
    visible.Y >= 0 &&
    visible.Right <= viewport.ImageSize.X &&
    visible.Bottom <= viewport.ImageSize.Y,
    "Visible image rectangle should remain inside image bounds.",
    failures);

var clampedInteraction = Asun.UI.Viewports.ViewportInteractionState.Create(
        viewport.WithScaleAround(2, new System.Numerics.Vector2(600, 400)))
    .ClampPan();

Assert(
    clampedInteraction.Transform.RenderedImageRectangle.Right >= clampedInteraction.Transform.ViewportSize.X &&
    clampedInteraction.Transform.RenderedImageRectangle.Bottom >= clampedInteraction.Transform.ViewportSize.Y,
    "Interaction pan clamping should prevent blank viewport gaps.",
    failures);

var resizeForwarded = Asun.UI.Viewports.ViewportInteractionState.Create(viewport)
    .WithViewportSize(new System.Numerics.Vector2(1400, 900));

Assert(
    resizeForwarded.Transform.ViewportSize == new System.Numerics.Vector2(1400, 900),
    "Interaction viewport resize should forward to the transform.",
    failures);

var zoomed = viewport.WithScaleAround(2, new System.Numerics.Vector2(600, 400));
var anchorBefore = viewport.ViewportToImage(new System.Numerics.Vector2(600, 400));
var anchorAfter = zoomed.ViewportToImage(new System.Numerics.Vector2(600, 400));

Assert(
    Math.Abs(anchorBefore.X - anchorAfter.X) < 1e-4f &&
    Math.Abs(anchorBefore.Y - anchorAfter.Y) < 1e-4f,
    "Zoom should preserve the viewport anchor.",
    failures);

var clampedFactor = viewport.WithZoomFactorClamped(
    zoomFactor: 100,
    minScale: 0.5,
    maxScale: 4,
    viewportAnchor: new System.Numerics.Vector2(600, 400));

Assert(
    Math.Abs(clampedFactor.Scale - 4) < 1e-9,
    "Clamped zoom factor should respect scale bounds.",
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
    Math.Abs(viewport.FitScale - viewport.Scale) < 1e-9 &&
    Math.Abs(viewport.ZoomRatioToFit - 1) < 1e-9,
    "Fit scale metadata should identify the current fit state.",
    failures);

Assert(
    viewport.IsImageFullyVisible,
    "Fit transform should keep the rendered image fully visible.",
    failures);

Assert(
    viewport.ImagePointAtViewportCenter == new System.Numerics.Vector2(500, 250),
    "Viewport center should map to the image center for a fit transform.",
    failures);

Assert(
    viewport.ViewportCenter == new System.Numerics.Vector2(600, 400) &&
    viewport.ImageCenter == new System.Numerics.Vector2(500, 250),
    "Viewport and image centers should be derived from their sizes.",
    failures);

var centeredPointTransform = viewport.CenterOnImagePoint(new System.Numerics.Vector2(250, 125));

var centeredInteraction = Asun.UI.Viewports.ViewportInteractionState.Create(viewport)
    .CenterOnImagePoint(new System.Numerics.Vector2(250, 125));

Assert(
    centeredInteraction.Transform.ViewportToImage(centeredInteraction.Transform.ViewportCenter) ==
        new System.Numerics.Vector2(250, 125),
    "Viewport interaction centering should forward to transform geometry.",
    failures);
Assert(
    centeredPointTransform.ViewportToImage(centeredPointTransform.ViewportCenter) ==
        new System.Numerics.Vector2(250, 125),
    "CenterOnImagePoint should place the requested image point at viewport center.",
    failures);

var centeredRectangleTransform = viewport.CenterOnImageRectangle(new RectangleF(100, 50, 200, 100));
Assert(
    centeredRectangleTransform.ViewportToImage(centeredRectangleTransform.ViewportCenter) ==
        new System.Numerics.Vector2(200, 100),
    "CenterOnImageRectangle should place the rectangle center at viewport center.",
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

Assert(
    transform.TranslationVector == new System.Numerics.Vector2(10, 20) &&
    Math.Abs(transform.LinearDeterminant - 4) < 1e-6,
    "Affine transform metadata should expose translation and linear determinant.",
    failures);

var expectedComposedPoint = new System.Numerics.Vector2(20, 34);

var transformedDirection = transform.TransformDirection(new System.Numerics.Vector2(1, 2));
Assert(
    transformedDirection == new System.Numerics.Vector2(2, 4),
    "Affine direction transforms should ignore translation.",
    failures);
var actualComposedPoint = transform.TransformPoint(new System.Numerics.Vector2(5, 7));
var transformedRectangle = transform.TransformRectangle(new RectangleF(0, 0, 10, 20));
Assert(
    transformedRectangle == new RectangleF(10, 20, 20, 40),
    "Affine rectangle transform should return the axis-aligned transformed bounds.",
    failures);

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

var clampedTranslation = viewport.WithTranslationClamped(
    new System.Numerics.Vector2(-5000, 5000));
Assert(
    clampedTranslation.Translation.X <= 0 &&
    clampedTranslation.Translation.Y <= 0 &&
    clampedTranslation.RenderedImageRectangle.Right >= clampedTranslation.ViewportSize.X &&
    clampedTranslation.RenderedImageRectangle.Bottom >= clampedTranslation.ViewportSize.Y,
    "Clamped translation should prevent blank viewport gaps.",
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

Assert(
    Math.Abs(transform.LinearScaleX - 2) < 1e-6 &&
    Math.Abs(transform.LinearScaleY - 2) < 1e-6 &&
    transform.ApproximatelyEquals(transform, 0),
    "Affine linear scale metadata and self-equality should be deterministic.",
    failures);

var singular = Asun.Vision.Contracts.AffineTransform2D.Scale(1, 0);
Assert(!singular.IsInvertible, "A singular affine transform should report non-invertible.", failures);

Assert(
    !singular.TryInvert(out _),
    "Singular affine transforms should fail the non-throwing inversion path.",
    failures);

Assert(
    transform.TryInvert(out var transformInverse) &&
    transformInverse.ApproximatelyEquals(transform.Inverse),
    "Affine TryInvert should match the throwing inverse path.",
    failures);

var imageSize = new Asun.Vision.Contracts.ImageSize(1000, 500);
Assert(
    imageSize.PixelCount == 500000 &&
    Math.Abs(imageSize.AspectRatio - 2) < 1e-12 &&
    imageSize.Vector == new System.Numerics.Vector2(1000, 500) &&
    imageSize.Contains(new Asun.Vision.Contracts.PixelPoint(500, 250)),
    "Image size metrics and point containment should be deterministic.",
    failures);

var imageBounds = new Asun.Vision.Contracts.PixelRect(0, 0, 1000, 500);
Assert(
    imageBounds.EnsureValid() == imageBounds,
    "Valid pixel rectangles should pass the explicit validity guard.",
    failures);
Assert(
    imageBounds.AreBoundsFinite &&
    imageBounds.Area == 500000,
    "Valid pixel rectangles should expose finite bounds and area.",
    failures);
var centeredRoi = Asun.Vision.Contracts.PixelRect.FromCenter(
    new Asun.Vision.Contracts.PixelPoint(200, 100),
    200,
    100);

Assert(
    centeredRoi == new Asun.Vision.Contracts.PixelRect(100, 50, 200, 100),
    "Pixel rectangle center factory should produce the requested center and size.",
    failures);

var ltrbRoi = Asun.Vision.Contracts.PixelRect.FromLTRB(300, 200, 100, 50);
Assert(
    ltrbRoi == centeredRoi,
    "Pixel rectangle LTRB factory should normalize direction.",
    failures);

var midpoint = new Asun.Vision.Contracts.PixelPoint(0, 0)
    .Midpoint(new Asun.Vision.Contracts.PixelPoint(10, 20));
Assert(
    midpoint == new Asun.Vision.Contracts.PixelPoint(5, 10),
    "Pixel point midpoint should be deterministic.",
    failures);

var interpolated = new Asun.Vision.Contracts.PixelPoint(0, 0)
    .Lerp(new Asun.Vision.Contracts.PixelPoint(10, 20), 0.25);
Assert(
    interpolated == new Asun.Vision.Contracts.PixelPoint(2.5, 5),
    "Pixel point interpolation should be deterministic.",
    failures);

var dragRoi = Asun.Vision.Contracts.PixelRect.FromPoints(
    new Asun.Vision.Contracts.PixelPoint(300, 200),
    new Asun.Vision.Contracts.PixelPoint(100, 50));

var hugeRejected = false;
try
{
    _ = Asun.Vision.Contracts.PixelRect.FromPoints(
        new Asun.Vision.Contracts.PixelPoint(double.MaxValue, 0),
        new Asun.Vision.Contracts.PixelPoint(-double.MaxValue, 0));
}
catch (ArgumentOutOfRangeException)
{
    hugeRejected = true;
}

Assert(hugeRejected, "Pixel rectangles with non-finite derived dimensions should be rejected.", failures);

Assert(
    dragRoi == new Asun.Vision.Contracts.PixelRect(100, 50, 200, 150),
    "ROI creation from drag endpoints should normalize axis direction.",
    failures);

var pointDistance = new Asun.Vision.Contracts.PixelPoint(3, 4);
Assert(
    Math.Abs(pointDistance.Length - 5) < 1e-12 &&
    Math.Abs(pointDistance.LengthSquared - 25) < 1e-12,
    "Pixel point magnitude should use Euclidean distance.",
    failures);

var translatedPoint = pointDistance.Translate(7, -4);
Assert(
    translatedPoint == new Asun.Vision.Contracts.PixelPoint(10, 0),
    "Pixel point translation should be deterministic.",
    failures);

var roi = new Asun.Vision.Contracts.PixelRect(100, 50, 200, 100);

Assert(
    roi.TopLeft == new Asun.Vision.Contracts.PixelPoint(100, 50) &&
    roi.TopRight == new Asun.Vision.Contracts.PixelPoint(300, 50) &&
    roi.BottomLeft == new Asun.Vision.Contracts.PixelPoint(100, 150) &&
    roi.BottomRight == new Asun.Vision.Contracts.PixelPoint(300, 150),
    "Pixel rectangle corner helpers should match its bounds.",
    failures);

Assert(
    imageBounds.Contains(roi) &&
    roi.Contains(roi.Center),
    "Pixel rectangle containment should be deterministic.",
    failures);

var normalized = new Asun.Vision.Contracts.PixelRect(300, 250, -200, -100)
    .Normalize();

Assert(
    normalized == new Asun.Vision.Contracts.PixelRect(100, 150, 200, 100),
    "Pixel rectangle normalization should recover positive bounds from reversed dimensions.",
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

var distanceSquared = roi.DistanceSquaredTo(new Asun.Vision.Contracts.PixelPoint(0, 50));
Assert(
    Math.Abs(distanceSquared - 10000) < 1e-9,
    "Pixel rectangle point distance should be zero on axes inside the rectangle and positive outside.",
    failures);

Assert(
    roi.Intersects(new Asun.Vision.Contracts.PixelRect(250, 100, 200, 100)) &&
    !roi.Intersects(new Asun.Vision.Contracts.PixelRect(500, 500, 20, 20)),
    "Pixel rectangle intersection predicate should agree with geometric overlap.",
    failures);

var disjointIntersection = roi.TryIntersect(
    new Asun.Vision.Contracts.PixelRect(500, 500, 20, 20),
    out _);
Assert(!disjointIntersection, "Disjoint pixel rectangles should report no intersection.", failures);

var overlap = roi.Intersect(new Asun.Vision.Contracts.PixelRect(250, 100, 200, 100));
Assert(
    overlap == new Asun.Vision.Contracts.PixelRect(250, 100, 50, 50),
    "Pixel rectangle intersection should return the overlapping region.",
    failures);

var scaledRoi = roi.ScaleAroundCenter(2, 0.5);
Assert(
    scaledRoi == new Asun.Vision.Contracts.PixelRect(0, 75, 400, 50),
    "Pixel rectangle center scaling should preserve the center.",
    failures);

var expandedRoi = roi.ExpandToInclude(new Asun.Vision.Contracts.PixelPoint(50, 300));
Assert(
    expandedRoi == new Asun.Vision.Contracts.PixelRect(50, 50, 250, 250),
    "Pixel rectangle expansion should include the requested point.",
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

var blankLookupRejected =
    !pipeline.ContainsNode(" ") &&
    !pipeline.TryGetNode(" ", out _);

Assert(blankLookupRejected, "Blank pipeline identifiers should not be treated as valid lookups.", failures);

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

var emptyExpandedRange = Asun.UI.Viewports.ImageTileGeometry.ExpandTileRange(
    viewport.ImageSize,
    new System.Numerics.Vector2(256, 256),
    emptyTileRange,
    marginTiles: 3);

Assert(
    emptyExpandedRange.IsEmpty && emptyExpandedRange.Count == 0,
    "Expanding an empty tile range should remain empty.",
    failures);

var invalidMarginRejected = false;
try
{
    _ = Asun.UI.Viewports.ImageTileGeometry.ExpandTileRange(
        new System.Numerics.Vector2(1000, 500),
        new System.Numerics.Vector2(256, 256),
        tileRange,
        marginX: -1,
        marginY: 0);
}
catch (ArgumentOutOfRangeException)
{
    invalidMarginRejected = true;
}

Assert(invalidMarginRejected, "Negative tile prefetch margins should be rejected.", failures);

var graphValidated = false;
try
{
    AsyncPipeline<object>.Validate(new[]
    {
        new AsyncPipeline<object>.Node("A", (_, _) => ValueTask.CompletedTask),
        new AsyncPipeline<object>.Node("B", new[] { "A" }, (_, _) => ValueTask.CompletedTask)
    });
    graphValidated = true;
}
catch
{
    graphValidated = false;
}

Assert(graphValidated, "Standalone pipeline graph validation should accept a valid graph.", failures);

var missingNodeValidationRejected = false;
try
{
    AsyncPipeline<object>.Validate(new[]
    {
        new AsyncPipeline<object>.Node("A", new[] { "Unknown" }, (_, _) => ValueTask.CompletedTask)
    });
}
catch (ArgumentException)
{
    missingNodeValidationRejected = true;
}

Assert(missingNodeValidationRejected, "Standalone validation should reject unknown dependencies.", failures);

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

var emptyPipelineRejected = false;
try
{
    AsyncPipeline<object>.Validate(Array.Empty<AsyncPipeline<object>.Node>());
}
catch (ArgumentException)
{
    emptyPipelineRejected = true;
}

Assert(emptyPipelineRejected, "Standalone validation should reject an empty graph.", failures);

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

var cancelledBatchObserved = false;
using (var batchCancellation = new CancellationTokenSource())
{
    batchCancellation.Cancel();

    try
    {
        await batchQueue.DequeueBatchAsync(new int[1], batchCancellation.Token);
    }
    catch (OperationCanceledException)
    {
        cancelledBatchObserved = true;
    }
}

Assert(cancelledBatchObserved, "Asynchronous batch dequeue should honor cancellation while waiting.", failures);

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

var zeroBatchRejected = false;
try
{
    _ = batchQueue.TryDequeueBatch(Span<int>.Empty);
}
catch (ArgumentException)
{
    zeroBatchRejected = true;
}

Assert(zeroBatchRejected, "Synchronous batch dequeue should reject an empty destination.", failures);

using var readinessQueue = new BoundedWorkQueue<int>(1);
Assert(readinessQueue.CanWrite, "A new queue should report writable readiness.", failures);
var canWrite = await readinessQueue.WaitToWriteAsync();
Assert(canWrite, "A non-full queue should report writable readiness.", failures);
Assert(readinessQueue.TryEnqueue(7), "Readiness queue should accept work.", failures);
var canRead = await readinessQueue.WaitToReadAsync();
Assert(canRead, "A queued item should report readable readiness.", failures);
Assert(readinessQueue.TryDequeue(out var readinessValue) && readinessValue == 7, "Readiness queue should preserve the queued value.", failures);
readinessQueue.Complete();
Assert(!readinessQueue.CanWrite, "A completed queue should not report writable readiness.", failures);

Assert(queue.TryComplete(), "The first queue completion should succeed.", failures);
Assert(!queue.TryComplete(), "Repeated queue completion should be idempotent.", failures);
Assert(!queue.TryEnqueue(3), "Completed queue should reject new work.", failures);
Assert(queue.IsCompleted, "Completed queue should report completion after draining.", failures);

var faultedQueue = new BoundedWorkQueue<int>(1);
var queueFailure = new InvalidOperationException("expected queue failure");
Assert(faultedQueue.TryComplete(queueFailure), "Faulted queue should accept its first completion.", failures);
var queueFaultObserved = false;
try
{
    await faultedQueue.Completion;
}
catch (InvalidOperationException)
{
    queueFaultObserved = true;
}

Assert(queueFaultObserved, "Faulted queue completion should propagate its completion error.", failures);
await queue.Completion;

var cancelledReadQueue = new BoundedWorkQueue<int>(1);
using (var readCancellation = new CancellationTokenSource())
{
    readCancellation.Cancel();
    var readCancelled = false;

    try
    {
        await cancelledReadQueue.DequeueAsync(readCancellation.Token);
    }
    catch (OperationCanceledException)
    {
        readCancelled = true;
    }

    Assert(readCancelled, "Queue dequeue should honor cancellation.", failures);
}

using var resources = new ResourceLeasePool<string>(new[]
{
    new KeyValuePair<string, int>("camera", 1),
    new KeyValuePair<string, int>("gpu", 2)
});

Assert(
    resources.ResourceCount == 2 &&
    resources.ResourceKeys.Contains("camera") &&
    resources.ResourceKeys.Contains("gpu"),
    "Resource metadata should expose the configured resource set.",
    failures);

Assert(resources.Capacity("camera") == 1, "Configured resource capacity should be exposed.", failures);
Assert(
    resources.TryGetCapacity("camera", out var knownCapacity) &&
    knownCapacity == 1 &&
    !resources.TryGetCapacity("missing", out _),
    "Resource capacity lookup should distinguish configured and unknown resources.",
    failures);

Assert(
    resources.TryGetAvailable("gpu", out var knownAvailable) &&
    knownAvailable == 2 &&
    !resources.TryGetAvailable("missing", out _),
    "Resource availability lookup should distinguish configured and unknown resources.",
    failures);

Assert(resources.Available("gpu") == 2, "Independent resource capacity should start available.", failures);

Assert(resources.TryAcquire("camera", out var cameraLease), "The first lease should be granted.", failures);
Assert(!resources.TryAcquire("camera", out _), "A saturated resource should reject a non-blocking lease.", failures);

var waitingLeaseTask = resources.AcquireAsync("camera").AsTask();
Assert(!waitingLeaseTask.IsCompleted, "A saturated resource should apply bounded waiting.", failures);

cameraLease!.Dispose();
Assert(cameraLease.IsDisposed, "Disposed resource leases should report released state.", failures);
using var secondCameraLease = await waitingLeaseTask;
Assert(!secondCameraLease.IsDisposed, "A newly acquired resource lease should be active.", failures);
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
    Assert(disposablePool.ResourceCount == 1, "Single-resource pool should expose its configured count.", failures);
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

resources.Dispose();
Assert(resources.IsDisposed, "Disposed resource pools should report their lifecycle state.", failures);
Assert(!resources.TryGetCapacity("camera", out _), "Disposed resource pools should reject metadata reads safely.", failures);

if (failures.Count > 0)
{
    foreach (var failure in failures)
        Console.Error.WriteLine($"FAIL: {failure}");

    return 1;
}

Console.WriteLine("Asun.Platform.Core smoke tests passed.");
return 0;
