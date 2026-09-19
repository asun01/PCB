using System.Drawing;
using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportRenderReuseValidationHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        static ViewportRenderPipelineRuntime<string> CreatePipeline()
        {
            return new ViewportRenderPipelineRuntime<string>(
                new Vector2(1600, 1200),
                new Vector2(500, 400),
                new Vector2(100, 100),
                1,
                64,
                4,
                new LocalTileSource());
        }

        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        for (var i = 0; i < 10; i++)
        {
            using var pipeline = CreatePipeline();
            pipeline.Invalidate(
                ViewportDirtyFlags.All,
                pipeline.Composite.Generation);

            var frame =
                pipeline.RefreshAsync(
                    DateTimeOffset.UtcNow.AddSeconds(1))
                    .GetAwaiter()
                    .GetResult();

            pipeline.MarkPresented(frame!);

            Check(
                ViewportRenderReuseValidationRuntime.IsValid(
                    pipeline.Reuse,
                    frame!.Composite.Generation),
                $"presented reuse {i + 1} should validate.");
        }

        for (var i = 0; i < 10; i++)
        {
            using var pipeline = CreatePipeline();

            pipeline.Invalidate(
                ViewportDirtyFlags.All,
                pipeline.Composite.Generation);

            var first =
                pipeline.RefreshAsync(
                    DateTimeOffset.UtcNow.AddSeconds(1))
                    .GetAwaiter()
                    .GetResult();

            pipeline.MarkPresented(first!);
            var oldGeneration = first!.Composite.Generation;

            pipeline.Composite.PanBy(new Vector2(5, 2));

            pipeline.Invalidate(
                ViewportDirtyFlags.All,
                pipeline.Composite.Generation);

            var second =
                pipeline.RefreshAsync(
                    DateTimeOffset.UtcNow.AddSeconds(2))
                    .GetAwaiter()
                    .GetResult();

            Check(
                second!.Composite.Generation > oldGeneration &&
                pipeline.Reuse.LatestGeneration == null,
                $"new generation invalidation {i + 1} should clear reuse.");
        }

        for (var i = 0; i < 10; i++)
        {
            using var pipeline = CreatePipeline();

            var reuse = new ViewportRenderReuseRuntime<string>();
            pipeline.Invalidate(
                ViewportDirtyFlags.All,
                pipeline.Composite.Generation);

            var frame =
                pipeline.RefreshAsync(
                    DateTimeOffset.UtcNow.AddSeconds(1))
                    .GetAwaiter()
                    .GetResult();

            Check(
                ViewportRenderReuseValidationRuntime.IsValid(
                    reuse,
                    frame!.Composite.Generation),
                $"empty reuse {i + 1} should validate.");
        }

        for (var i = 0; i < 10; i++)
        {
            var reuse = new ViewportRenderReuseRuntime<string>();

            Check(
                reuse.LatestGeneration is null &&
                ViewportRenderReuseValidationRuntime.IsValid(
                    reuse,
                    i),
                $"empty reuse generation {i + 1} should validate.");
        }

        for (var i = 0; i < 10; i++)
        {
            using var pipeline = CreatePipeline();
            pipeline.Invalidate(
                ViewportDirtyFlags.All,
                pipeline.Composite.Generation);

            var frame =
                pipeline.RefreshAsync(
                    DateTimeOffset.UtcNow.AddSeconds(1))
                    .GetAwaiter()
                    .GetResult();

            pipeline.MarkPresented(frame!);
            pipeline.Reuse.Clear();

            Check(
                pipeline.Reuse.LatestGeneration is null,
                $"reuse clear {i + 1} should remove the cached frame.");
        }

        for (var i = 0; i < 10; i++)
        {
            using var pipeline = CreatePipeline();

            pipeline.Invalidate(
                ViewportDirtyFlags.All,
                pipeline.Composite.Generation);

            var frame =
                pipeline.RefreshAsync(
                    DateTimeOffset.UtcNow.AddSeconds(1))
                    .GetAwaiter()
                    .GetResult();

            pipeline.MarkPresented(frame!);
            var partial =
                new ViewportRenderReuseRuntime<string>();
            partial.Store(frame!);

            Check(
                ViewportRenderReuseValidationRuntime.IsValid(
                    partial,
                    frame!.Composite.Generation),
                $"stored full frame {i + 1} should validate.");
        }

        for (var i = 0; i < 10; i++)
        {
            using var pipeline = CreatePipeline();
            pipeline.Invalidate(
                ViewportDirtyFlags.All,
                pipeline.Composite.Generation);

            var frame =
                pipeline.RefreshAsync(
                    DateTimeOffset.UtcNow.AddSeconds(1))
                    .GetAwaiter()
                    .GetResult();

            Check(
                frame is not null &&
                frame.WorkPlan.Generation ==
                frame.Composite.Generation,
                $"pipeline generation {i + 1} should support reuse validation.");
        }

        for (var i = 0; i < 10; i++)
        {
            using var pipeline = CreatePipeline();
            pipeline.Invalidate(
                ViewportDirtyFlags.All,
                pipeline.Composite.Generation);

            var frame =
                pipeline.RefreshAsync(
                    DateTimeOffset.UtcNow.AddSeconds(1))
                    .GetAwaiter()
                    .GetResult();

            pipeline.MarkPresented(frame!);
            var reuse = pipeline.Reuse;
            var old = frame!.Composite.Generation;

            pipeline.Composite.PanBy(new Vector2(4, 3));

            Check(
                !reuse.TryReuse(old + 1, out _) &&
                reuse.LatestGeneration == old,
                $"future generation lookup {i + 1} should not falsely reuse.");

        }

        for (var i = 0; i < 10; i++)
        {
            using var pipeline = CreatePipeline();
            pipeline.Invalidate(
                ViewportDirtyFlags.All,
                pipeline.Composite.Generation);

            var frame =
                pipeline.RefreshAsync(
                    DateTimeOffset.UtcNow.AddSeconds(1))
                    .GetAwaiter()
                    .GetResult();

            pipeline.MarkPresented(frame!);
            pipeline.Composite.PanBy(new Vector2(2, 1));
            pipeline.Invalidate(
                ViewportDirtyFlags.All,
                pipeline.Composite.Generation);

            Check(
                ViewportRenderReuseValidationRuntime.IsValid(
                    pipeline.Reuse,
                    pipeline.Composite.Generation),
                $"reuse validator after invalidation {i + 1} should remain clean.");

        }

        for (var i = 0; i < 10; i++)
        {
            using var pipeline = CreatePipeline();
            pipeline.Invalidate(
                ViewportDirtyFlags.All,
                pipeline.Composite.Generation);

            var frame =
                pipeline.RefreshAsync(
                    DateTimeOffset.UtcNow.AddSeconds(1))
                    .GetAwaiter()
                    .GetResult();

            pipeline.MarkPresented(frame!);
            pipeline.Reuse.Clear();

            Check(
                ViewportRenderReuseValidationRuntime.IsValid(
                    pipeline.Reuse,
                    frame!.Composite.Generation) &&
                pipeline.Reuse.LatestGeneration is null,
                $"reuse post-clear validation {i + 1} should remain clean.");

        }

        assert(
            round == 100,
            $"Render reuse validation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }

    private sealed class LocalTileSource : ITileSource<string>
    {
        public ValueTask<string> LoadAsync(
            TileRequest request,
            RectangleF imageRectangle,
            CancellationToken cancellationToken = default) =>
            ValueTask.FromResult($"tile:{request.Index.X},{request.Index.Y}");
    }
}
