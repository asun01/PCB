using System.Drawing;
using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportPresentationExecutionSmoke
{
    public static async ValueTask RunAsync(Action<bool, string> assert)
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

    private sealed class StableTileSource : ITileSource<string>
    {
        public ValueTask<string> LoadAsync(
            TileRequest request,
            RectangleF imageRectangle,
            CancellationToken cancellationToken = default) =>
            ValueTask.FromResult(
                $"tile:{request.Index.X},{request.Index.Y}");
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
