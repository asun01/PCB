using System.Drawing;
using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportPresentationExecutionSmoke
{
    public static async ValueTask RunAsync(Action<bool, string> assert)
    {
        await VerifySuccessfulExecutionAsync(assert);
        await VerifySupersededExecutionAsync(assert);
    }

    private static async ValueTask VerifySuccessfulExecutionAsync(
        Action<bool, string> assert)
    {
        using var pipeline = new ViewportRenderPipelineRuntime<string>(
            new Vector2(200, 100),
            new Vector2(200, 100),
            new Vector2(100, 100),
            0,
            16,
            2,
            new StableTileSource(),
            new ViewportRenderBudget(8, 8, 4, 16),
            1000);

        var frame = await pipeline.RefreshAsync(
            DateTimeOffset.UtcNow.AddSeconds(20));

        assert(
            frame is not null,
            "Execution smoke should create a pipeline frame.");

        if (frame is null)
            return;

        using var queue = new ViewportPresentationQueueRuntime<string>();
        using var buffers = new ViewportPresentationBufferRuntime();
        using var surface = new ViewportRenderSurfaceRuntime();
        var delivery = new ViewportRenderDeliveryTracker();

        var execution = new ViewportPresentationExecutionRuntime<string>(
            queue,
            buffers,
            surface,
            delivery);

        var completion =
            new TaskCompletionSource<
                ViewportPresentationExecutionResult<string>>(
                TaskCreationOptions.RunContinuationsAsynchronously);

        using var cancellation = new CancellationTokenSource();

        var worker = execution
            .RunAsync(
                new TrackingSink(),
                result =>
                {
                    completion.TrySetResult(result);
                    return ValueTask.CompletedTask;
                },
                cancellation.Token)
            .AsTask();

        assert(
            queue.TryEnqueue(frame, out var packet),
            "Execution smoke should enqueue a frame before the worker consumes it.");

        var completed = await Task.WhenAny(
            completion.Task,
            Task.Delay(TimeSpan.FromSeconds(2)));

        assert(
            completed == completion.Task,
            "Presentation execution worker should wake from queue activity without polling.");

        if (completed != completion.Task)
        {
            cancellation.Cancel();
            try
            {
                await worker;
            }
            catch (OperationCanceledException)
            {
            }

            return;
        }

        var result = await completion.Task;
        var queueSnapshot = queue.Statistics;
        var bufferSnapshot = buffers.Snapshot;
        var surfaceSnapshot = surface.Snapshot;

        assert(
            result.Executed &&
            result.Presented &&
            result.Packet is not null &&
            result.Packet.Token == packet.Token &&
            result.Delivery.Succeeded &&
            execution.Statistics.Executed == 1 &&
            execution.Statistics.Presented == 1 &&
            execution.Statistics.LastSequence == packet.Token.Sequence &&
            queueSnapshot.Presented == 1 &&
            queueSnapshot.Pending == 0 &&
            queueSnapshot.InFlightGeneration is null &&
            queueSnapshot.PresentedGeneration == frame.Composite.Generation &&
            bufferSnapshot.PresentedGeneration == frame.Composite.Generation &&
            bufferSnapshot.PresentedSequence == packet.Token.Sequence &&
            surfaceSnapshot.PresentedGeneration == frame.Composite.Generation &&
            surfaceSnapshot.State == ViewportRenderSurfaceState.Presented,
            "Execution worker should complete Queue, Backbuffer, Surface, and Delivery into one presented fence.");

        cancellation.Cancel();

        try
        {
            await worker;
        }
        catch (OperationCanceledException)
        {
        }

        assert(
            worker.IsCompleted,
            "Presentation execution worker should stop cleanly after cancellation.");
    }

    private static async ValueTask VerifySupersededExecutionAsync(
        Action<bool, string> assert)
    {
        using var pipeline = new ViewportRenderPipelineRuntime<string>(
            new Vector2(300, 100),
            new Vector2(200, 100),
            new Vector2(100, 100),
            0,
            16,
            2,
            new StableTileSource(),
            new ViewportRenderBudget(8, 8, 4, 16),
            1000);

        var first = await pipeline.RefreshAsync(
            DateTimeOffset.UtcNow.AddSeconds(30));

        if (first is null)
        {
            assert(false, "Supersede smoke should create the first frame.");
            return;
        }

        var firstGeneration = first.Composite.Generation;
        pipeline.Composite.PanBy(new Vector2(20, 0));

        var second = await pipeline.RefreshAsync(
            DateTimeOffset.UtcNow.AddSeconds(31));

        assert(
            second is not null &&
            second.Composite.Generation > firstGeneration,
            "Supersede smoke should create a newer generation while the first frame is still eligible for rendering.");

        if (second is null)
            return;

        using var queue = new ViewportPresentationQueueRuntime<string>();
        using var buffers = new ViewportPresentationBufferRuntime();
        using var surface = new ViewportRenderSurfaceRuntime();
        var delivery = new ViewportRenderDeliveryTracker();

        var execution = new ViewportPresentationExecutionRuntime<string>(
            queue,
            buffers,
            surface,
            delivery);

        using var cancellation = new CancellationTokenSource();
        var firstStarted =
            new TaskCompletionSource<bool>(
                TaskCreationOptions.RunContinuationsAsynchronously);
        var results =
            new List<ViewportPresentationExecutionResult<string>>();
        var resultsLock = new object();
        var twoResults =
            new TaskCompletionSource<bool>(
                TaskCreationOptions.RunContinuationsAsynchronously);

        var sink = new SupersedingSink(firstStarted);

        var worker = execution
            .RunAsync(
                sink,
                result =>
                {
                    lock (resultsLock)
                    {
                        results.Add(result);

                        if (results.Count >= 2)
                            twoResults.TrySetResult(true);
                    }

                    return ValueTask.CompletedTask;
                },
                cancellation.Token)
            .AsTask();

        assert(
            queue.TryEnqueue(first, out _),
            "Supersede smoke should enqueue the older frame.");

        var started = await Task.WhenAny(
            firstStarted.Task,
            Task.Delay(TimeSpan.FromSeconds(2)));

        assert(
            started == firstStarted.Task,
            "Supersede smoke should block the first in-flight render before introducing a newer frame.");

        assert(
            queue.TryEnqueue(second, out _),
            "Supersede smoke should enqueue a newer frame while the first is rendering.");

        var completed = await Task.WhenAny(
            twoResults.Task,
            Task.Delay(TimeSpan.FromSeconds(2)));

        assert(
            completed == twoResults.Task,
            "Execution worker should finish the superseded frame and continue with the newer frame.");

        ViewportPresentationExecutionResult<string>[] snapshot;

        lock (resultsLock)
            snapshot = results.ToArray();

        assert(
            snapshot.Any(result =>
                result.Superseded &&
                result.Packet?.Frame.Composite.Generation == firstGeneration) &&
            snapshot.Any(result =>
                result.Presented &&
                result.Packet?.Frame.Composite.Generation == second.Composite.Generation) &&
            execution.Statistics.Executed == 2 &&
            execution.Statistics.Superseded == 1 &&
            execution.Statistics.Presented == 1 &&
            execution.Statistics.LastGeneration == second.Composite.Generation &&
            queue.Statistics.PresentedGeneration == second.Composite.Generation &&
            buffers.Snapshot.PresentedGeneration == second.Composite.Generation &&
            surface.Snapshot.PresentedGeneration == second.Composite.Generation,
            "A newer submission should cancel the stale in-flight render and leave Queue, Backbuffer, and Surface on the newer generation.");

        cancellation.Cancel();

        try
        {
            await worker;
        }
        catch (OperationCanceledException)
        {
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

    private sealed class SupersedingSink : IViewportRenderSink<string>
    {
        private readonly TaskCompletionSource<bool> _started;

        public SupersedingSink(TaskCompletionSource<bool> started)
        {
            _started = started;
        }

        public ValueTask BeginFrameAsync(
            ViewportRenderFrameContext context,
            CancellationToken cancellationToken = default)
        {
            _started.TrySetResult(true);

            return new ValueTask(
                Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken));
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
            CancellationToken cancellationToken = default) =>
            ValueTask.CompletedTask;
    }

    private sealed class TrackingSink : IViewportRenderSink<string>
    {
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
            CancellationToken cancellationToken = default) =>
            ValueTask.CompletedTask;
    }
}
