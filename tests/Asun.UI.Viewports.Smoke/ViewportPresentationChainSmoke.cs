using System.Drawing;
using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportPresentationChainSmoke
{
    public static async ValueTask RunAsync(Action<bool, string> assert)
    {
        for (var i = 0; i < 500; i++)
        {
            using var pipeline = new ViewportRenderPipelineRuntime<string>(
                new Vector2(1800 + i % 7 * 31, 1400 + i % 9 * 23),
                new Vector2(600 + i % 5 * 20, 450 + i % 3 * 25),
                new Vector2(100, 100),
                1,
                64,
                4,
                new LocalTileSource());

            var roiId = pipeline.Composite.AddRoi(
                RoiGeometry.CreateRectangle(
                    new Vector2(300 + i % 11 * 12, 230 + i % 7 * 10),
                    new Vector2(90, 70)));

            var frame = await pipeline.RefreshAsync(
                DateTimeOffset.UtcNow.AddSeconds(1));

            assert(
                frame is not null,
                $"Presentation chain {i + 1} should create an initial frame.");

            if (frame is null)
                continue;

            var visibility = ViewportTileRoiVisibilityRuntime.Build(
                frame.Composite);

            assert(
                visibility.TileCount > 0 &&
                visibility.VisibleRoiIds.Contains(roiId) &&
                visibility.Tiles.Any(tile =>
                    tile.IntersectingRoiIds.Contains(roiId)),
                $"Presentation chain {i + 1} should join visible tiles and ROI state.");

            var sink = new RecordingSink();

            var rendered = await ViewportRenderAdapterRuntime.RenderAsync(
                frame,
                sink);

            assert(
                rendered > 0 &&
                sink.BeginCount == 1 &&
                sink.EndCount == 1 &&
                sink.TileCount > 0 &&
                sink.RoiCount > 0,
                $"Presentation chain {i + 1} should adapt the framework-neutral frame to a sink.");

            var input = new ViewportInputSubmissionRuntime();
            var roiViewportPoint = pipeline.Composite.Transform.ImageToViewport(
                new Vector2(300 + i % 11 * 12, 230 + i % 7 * 10));

            input.Submit(
                ViewportInputEventKind.PointerMove,
                new Vector2(100, 100));

            input.Submit(
                ViewportInputEventKind.PointerMove,
                new Vector2(105, 103));

            input.Submit(
                ViewportInputEventKind.PointerDown,
                roiViewportPoint);

            input.Submit(
                ViewportInputEventKind.PointerMove,
                roiViewportPoint + new Vector2(12, 8));

            input.Submit(
                ViewportInputEventKind.PointerUp,
                roiViewportPoint + new Vector2(12, 8));

            input.Submit(
                ViewportInputEventKind.Wheel,
                roiViewportPoint,
                wheelDelta: 120);

            var events = input.Drain();

            assert(
                events.Count == 6 &&
                events[0].Kind == ViewportInputEventKind.PointerMove &&
                events[0].Position == new Vector2(105, 103) &&
                events[^1].Kind == ViewportInputEventKind.Wheel,
                $"Presentation chain {i + 1} should coalesce pointer move submissions while preserving ordering.");

            var continuous = new ViewportContinuousFrameRuntime<string>(
                pipeline,
                input);

            var generationBeforeInput = pipeline.Composite.Generation;
            var processedCount = continuous.ProcessInputs();

            assert(
                processedCount == 5 &&
                pipeline.Composite.Generation > generationBeforeInput,
                $"Presentation chain {i + 1} should apply submitted interaction events to the composite runtime.");

            var pendingAfterInput = pipeline.Scheduler.PendingFlags;

            assert(
                pendingAfterInput != ViewportDirtyFlags.None,
                $"Presentation chain {i + 1} should schedule rendering after input changes.");

            var cts = new CancellationTokenSource();
            var continuousSink = new CancellingSink(cts);

            try
            {
                await continuous.RunAsync(
                    continuousSink,
                    cts.Token);
            }
            catch (OperationCanceledException)
            {
                // External cancellation is the normal completion path of this smoke.
            }

            var continuousStats = continuous.Statistics;

            assert(
                continuousStats.RenderedFrames >= 1 &&
                continuousStats.ProcessedInputs == 0 &&
                continuousSink.BeginCount >= 1 &&
                continuousSink.EndCount >= 1,
                $"Presentation chain {i + 1} should execute a continuous frame and deliver it to the sink.");

            assert(
                continuousStats.LoopCount >= continuousStats.RenderedFrames,
                $"Presentation chain {i + 1} should maintain valid continuous-loop accounting.");

            pipeline.Reset();

            assert(
                pipeline.Scheduler.PendingFlags == ViewportDirtyFlags.None,
                $"Presentation chain {i + 1} should reset presentation scheduling.");
        }
    }

    private sealed class LocalTileSource : ITileSource<string>
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
        public int BeginCount { get; private set; }
        public int EndCount { get; private set; }
        public int TileCount { get; private set; }
        public int RoiCount { get; private set; }

        public ValueTask BeginFrameAsync(
            ViewportRenderFrameContext context,
            CancellationToken cancellationToken = default)
        {
            BeginCount++;
            return ValueTask.CompletedTask;
        }

        public ValueTask DrawTileAsync(
            ViewportRenderTileContext<string> tile,
            CancellationToken cancellationToken = default)
        {
            TileCount++;
            return ValueTask.CompletedTask;
        }

        public ValueTask DrawRoiAsync(
            ViewportRenderRoiContext roi,
            CancellationToken cancellationToken = default)
        {
            RoiCount++;
            return ValueTask.CompletedTask;
        }

        public ValueTask EndFrameAsync(
            ViewportRenderFrameContext context,
            CancellationToken cancellationToken = default)
        {
            EndCount++;
            return ValueTask.CompletedTask;
        }
    }

    private sealed class CancellingSink : IViewportRenderSink<string>
    {
        private readonly CancellationTokenSource _source;

        public CancellingSink(CancellationTokenSource source)
        {
            _source = source;
        }

        public int BeginCount { get; private set; }
        public int EndCount { get; private set; }

        public ValueTask BeginFrameAsync(
            ViewportRenderFrameContext context,
            CancellationToken cancellationToken = default)
        {
            BeginCount++;
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
            EndCount++;
            _source.Cancel();
            return ValueTask.CompletedTask;
        }
    }
}
