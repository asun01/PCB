using Asun.UI.Viewports;

public static class ViewportRenderCommandStreamValidationHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        static ViewportRenderPipelineFrame<string>? CreateFrame()
        {
            using var pipeline = new ViewportRenderPipelineRuntime<string>(
                new System.Numerics.Vector2(800, 600),
                new System.Numerics.Vector2(400, 300),
                new System.Numerics.Vector2(100, 100),
                1, 16, 2, new LocalTileSource());

            pipeline.Invalidate(
                ViewportDirtyFlags.All,
                pipeline.Composite.Generation);

            return pipeline.RefreshAsync(
                DateTimeOffset.UtcNow.AddSeconds(1))
                .GetAwaiter()
                .GetResult();
        }

        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        for (var i = 0; i < 10; i++)
        {
            var frame = CreateFrame();
            Check(
                frame is not null &&
                ViewportRenderCommandStreamValidationRuntime.IsValid(
                    frame.CommandStream),
                $"real pipeline command stream {i + 1} should validate.");
        }

        for (var i = 0; i < 10; i++)
        {
            var frame = CreateFrame();
            Check(
                frame is not null &&
                frame.CommandStream.Generation >= 0 &&
                frame.CommandStream.CommandCount ==
                frame.CommandStream.Commands.Count,
                $"command count {i + 1} should be coherent.");
        }

        for (var i = 0; i < 10; i++)
        {
            var frame = CreateFrame();
            Check(
                frame is not null &&
                frame.CommandStream.Commands
                    .Select((command, index) => command.Sequence == index + 1)
                    .All(item => item),
                $"command sequencing {i + 1} should be contiguous.");
        }

        for (var i = 0; i < 10; i++)
        {
            var frame = CreateFrame();
            Check(
                frame is not null &&
                frame.CommandStream.Commands.All(
                    command => command.WorkItem.Generation ==
                        frame.CommandStream.Generation),
                $"command generations {i + 1} should match the stream.");
        }

        for (var i = 0; i < 10; i++)
        {
            var frame = CreateFrame();
            Check(
                frame is not null &&
                frame.CommandStream.Commands.All(
                    command => float.IsFinite(command.Bounds.X) &&
                        float.IsFinite(command.Bounds.Y) &&
                        float.IsFinite(command.Bounds.Width) &&
                        float.IsFinite(command.Bounds.Height)),
                $"command bounds {i + 1} should be finite.");
        }

        for (var i = 0; i < 10; i++)
        {
            var frame = CreateFrame();
            Check(
                frame is not null &&
                frame.CommandStream.Regions.All(
                    region => float.IsFinite(region.X) &&
                        float.IsFinite(region.Y) &&
                        float.IsFinite(region.Width) &&
                        float.IsFinite(region.Height)),
                $"command regions {i + 1} should be finite.");
        }

        for (var i = 0; i < 10; i++)
        {
            var first = CreateFrame();
            var second = CreateFrame();
            Check(
                first is not null &&
                second is not null &&
                first.CommandStream.CommandCount ==
                second.CommandStream.CommandCount,
                $"command stream shape {i + 1} should remain deterministic.");
        }

        for (var i = 0; i < 10; i++)
        {
            var frame = CreateFrame();
            Check(
                frame is not null &&
                frame.CommandStream.TileCount >= 0 &&
                frame.CommandStream.RoiCount >= 0 &&
                frame.CommandStream.OverlayCount >= 0,
                $"command counters {i + 1} should remain non-negative.");
        }

        for (var i = 0; i < 10; i++)
        {
            var frame = CreateFrame();
            Check(
                frame is not null &&
                frame.CommandStream.InvalidationCount >= 0 &&
                frame.CommandStream.FullSurfaceCount >= 0,
                $"clear command counters {i + 1} should remain non-negative.");
        }

        assert(
            round == 100,
            $"Render command stream validation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }

    private sealed class LocalTileSource : ITileSource<string>
    {
        public ValueTask<string> LoadAsync(
            TileRequest request,
            System.Drawing.RectangleF imageRectangle,
            CancellationToken cancellationToken = default) =>
            ValueTask.FromResult(
                $"tile:{request.Index.X},{request.Index.Y}");
    }
}
