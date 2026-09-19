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

            var visibility = ViewportTileRoiVisibilityRuntime.Build(frame.Composite);

            assert(
                visibility.TileCount > 0 &&
                visibility.VisibleRoiIds.Contains(roiId),
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
            input.Submit(
                ViewportInputEventKind.PointerMove,
                new Vector2(100, 100));
            input.Submit(
                ViewportInputEventKind.PointerMove,
                new Vector2(105, 103));
            input.Submit(
                ViewportInputEventKind.Wheel,
                new Vector2(300, 220),
                wheelDelta: 120);

            var events = input.Drain();
            assert(
                events.Count == 2 &&
                events[0].Kind == ViewportInputEventKind.PointerMove &&
                events[0].Position == new Vector2(105, 103) &&
                events[1].Kind == ViewportInputEventKind.Wheel,
                $"Presentation chain {i + 1} should coalesce pointer move submissions.");

            var processed = new ViewportContinuousFrameRuntime<string>(pipeline, input);
            var processedCount = processed.ProcessInputs();

            assert(
                processedCount == 0,
                $"Presentation chain {i + 1} should observe an empty queue after manual input drain.");

            var stats = processed.Statistics;

            assert(
                stats.ProcessedInputs == 0 &&
                pipeline.Scheduler.PendingFlags != ViewportDirtyFlags.None,
                $"Presentation chain {i + 1} should preserve scheduler invalidation state.");

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
}
