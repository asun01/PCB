using Asun.Platform.Core;

var failures = new List<string>();

static void Assert(bool condition, string message, List<string> failures)
{
    if (!condition) failures.Add(message);
}

Assert(Math.Abs(Percentiles.Calculate(new[] { 10d, 20d, 30d, 40d, 50d }, 50) - 30) < 1e-9, "P50 should be the median.", failures);
Assert(Math.Abs(Percentiles.Calculate(new[] { 10d, 20d, 30d, 40d }, 25) - 17.5) < 1e-9, "Percentile interpolation should be deterministic.", failures);

var viewport = Asun.UI.Viewports.ViewportTransform.Fit(
    new System.Numerics.Vector2(1000, 500),
    new System.Numerics.Vector2(1200, 800));
var imagePoint = new System.Numerics.Vector2(250, 125);
var viewportPoint = viewport.ImageToViewport(imagePoint);
var roundTrip = viewport.ViewportToImage(viewportPoint);
Assert(Math.Abs(roundTrip.X - imagePoint.X) < 1e-4f && Math.Abs(roundTrip.Y - imagePoint.Y) < 1e-4f, "Viewport coordinate conversion should round-trip.", failures);
Assert(viewport.ContainsViewportPoint(viewport.ImageToViewport(new System.Numerics.Vector2(0, 0))), "Viewport should contain the rendered image origin.", failures);
var zoomed = viewport.WithScaleAround(2, new System.Numerics.Vector2(600, 400));
var anchorBefore = viewport.ViewportToImage(new System.Numerics.Vector2(600, 400));
var anchorAfter = zoomed.ViewportToImage(new System.Numerics.Vector2(600, 400));
Assert(Math.Abs(anchorBefore.X - anchorAfter.X) < 1e-4f && Math.Abs(anchorBefore.Y - anchorAfter.Y) < 1e-4f, "Zoom should preserve the viewport anchor.", failures);

var imageSize = new Asun.Vision.Contracts.ImageSize(1920, 1080);
Assert(imageSize.Center.X == 960 && imageSize.Center.Y == 540, "Image center should be deterministic.", failures);

var findingSet = new Asun.Domain.Quality.InspectionFindingSet(new[]
{
    new Asun.Domain.Quality.InspectionFinding("B", "sim", 0.8, 20, 30),
    new Asun.Domain.Quality.InspectionFinding("A", "sim", 0.9, 10, 20)
});
Assert(findingSet.Count == 2, "Finding set should retain all unique findings.", failures);
Assert(findingSet.Items[0].FindingId == "A", "Findings should have deterministic identifier ordering.", failures);

var boardRegion = new Asun.Domain.Pcb.BoardRegion("board", new Asun.Domain.Pcb.BoardPoint(0, 0), new Asun.Domain.Pcb.BoardSize(100, 50));
Assert(boardRegion.Center == new Asun.Domain.Pcb.BoardPoint(50, 25), "Board region center should be deterministic.", failures);

var latency = new LatencyStatistics(new[] { 10d, 20d, 30d, 40d, 50d });
Assert(latency.Count == 5, "Latency count should be preserved.", failures);
Assert(Math.Abs(latency.P50 - 30) < 1e-9, "Latency P50 should be deterministic.", failures);
Assert(Math.Abs(latency.P95 - 48) < 1e-9, "Latency P95 should use interpolation.", failures);
Assert(Math.Abs(latency.P99 - 49.6) < 1e-9, "Latency P99 should use interpolation.", failures);

var execution = new List<string>();
var pipeline = new AsyncPipeline<List<string>>(new[]
{
    new AsyncPipeline<List<string>>.Node("Acquire", (_, _) => { execution.Add("Acquire"); return ValueTask.CompletedTask; }),
    new AsyncPipeline<List<string>>.Node("Inspect", new[] { "Acquire" }, (_, _) => { execution.Add("Inspect"); return ValueTask.CompletedTask; }),
    new AsyncPipeline<List<string>>.Node("Report", new[] { "Inspect" }, (_, _) => { execution.Add("Report"); return ValueTask.CompletedTask; })
});
await pipeline.ExecuteAsync(execution);
Assert(execution.SequenceEqual(new[] { "Acquire", "Inspect", "Report" }), "Pipeline dependencies should be honored.", failures);

using var cancelled = new CancellationTokenSource();
cancelled.Cancel();
var cancellationObserved = false;
try { await pipeline.ExecuteAsync(execution, cancelled.Token); }
catch (OperationCanceledException) { cancellationObserved = true; }
Assert(cancellationObserved, "Pipeline should honor cancellation before scheduling work.", failures);

var timeoutObserved = false;
try
{
    await OperationTimeout.ExecuteAsync(async token => await Task.Delay(TimeSpan.FromSeconds(5), token), TimeSpan.FromMilliseconds(10));
}
catch (TimeoutException) { timeoutObserved = true; }
Assert(timeoutObserved, "Operation timeout should surface as TimeoutException.", failures);

var simulatedSource = new Asun.Device.Impl.SimulatedFrameSource(1920, 1080);
var request = new Asun.Device.Contracts.AcquisitionRequest("SIM", new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));
var frame1 = await simulatedSource.AcquireAsync(request);
var frame2 = await simulatedSource.AcquireAsync(request);
Assert(frame1.Sequence == 1 && frame2.Sequence == 2, "Simulated source should produce monotonic sequences.", failures);
Assert(frame1.Width == 1920 && frame1.Height == 1080, "Simulated source should preserve configured dimensions.", failures);

var queue = new BoundedWorkQueue<int>(2);
Assert(queue.TryEnqueue(1), "First enqueue should succeed.", failures);
Assert(queue.TryEnqueue(2), "Second enqueue should succeed.", failures);
Assert(!queue.TryEnqueue(3), "A full queue should apply backpressure.", failures);
Assert(queue.TryDequeue(out var first) && first == 1, "FIFO dequeue should preserve order.", failures);
Assert(queue.TryDequeue(out var second) && second == 2, "FIFO dequeue should preserve order.", failures);
Assert(!queue.TryDequeue(out _), "An empty queue should not produce a value.", failures);
queue.Complete();
Assert(queue.IsCompleted, "Completed queue should report completion after draining.", failures);

if (failures.Count > 0)
{
    foreach (var failure in failures) Console.Error.WriteLine($"FAIL: {failure}");
    return 1;
}

Console.WriteLine("Asun.Platform.Core smoke tests passed.");
return 0;
