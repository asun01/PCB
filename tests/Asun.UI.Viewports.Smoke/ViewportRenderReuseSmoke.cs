using System.Drawing;
using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportRenderReuseSmoke
{
    public static async ValueTask RunAsync(Action<bool, string> assert)
    {
        for (var i = 0; i < 500; i++)
        {
            using var runtime = new ViewportRenderPipelineRuntime<string>(
                new Vector2(1800 + i % 7 * 31, 1400 + i % 9 * 27),
                new Vector2(600 + i % 5 * 20, 450 + i % 3 * 25),
                new Vector2(100, 100),
                1,
                64,
                4,
                new LocalTileSource());

            runtime.Composite.AddRoi(
                RoiGeometry.CreateRectangle(
                    new Vector2(260 + i % 11 * 10, 210 + i % 7 * 8),
                    new Vector2(80, 60)));

            var first = await runtime.RefreshAsync(
                DateTimeOffset.UtcNow.AddSeconds(1));

            assert(
                first is not null,
                $"Reuse chain {i + 1} should produce an initial frame.");

            if (first is null)
                continue;

            var reuse = new ViewportRenderReuseRuntime<string>();
            reuse.Store(first);

            assert(
                reuse.TryReuse(first.Composite.Generation, out var reused) &&
                ReferenceEquals(reused, first),
                $"Reuse chain {i + 1} should return the same frame for an unchanged generation.");

            assert(
                reuse.LatestGeneration == first.Composite.Generation,
                $"Reuse chain {i + 1} should expose the cached generation.");

            runtime.Composite.PanBy(new Vector2(3, 1));

            var second = await runtime.RefreshAsync(
                DateTimeOffset.UtcNow.AddSeconds(2));

            assert(
                second is not null &&
                second.Composite.Generation != first.Composite.Generation &&
                !reuse.TryReuse(
                    second.Composite.Generation,
                    out _),
                $"Reuse chain {i + 1} should invalidate reuse after navigation.");

            reuse.Clear();

            assert(
                reuse.LatestGeneration is null &&
                !reuse.TryReuse(
                    second?.Composite.Generation ?? -1,
                    out _),
                $"Reuse chain {i + 1} should clear cached frames explicitly.");

            using var partialPipeline = new ViewportRenderPipelineRuntime<string>(
                new Vector2(1200, 900),
                new Vector2(400, 300),
                new Vector2(100, 100),
                1,
                32,
                2,
                new LocalTileSource(),
                new ViewportRenderBudget(1, 4, 1, 1),
                1000);

            partialPipeline.Invalidate(
                ViewportDirtyFlags.All,
                partialPipeline.Composite.Generation);

            var partial = await partialPipeline.RefreshAsync(
                DateTimeOffset.UtcNow.AddSeconds(1));

            assert(
                partial is not null &&
                partial.HasDeferredWork,
                $"Reuse chain {i + 1} should expose a partial frame when budget truncates work.");

            if (partial is not null)
            {
                var partialReuse = new ViewportRenderReuseRuntime<string>();
                partialReuse.Store(partial);

                assert(
                    partialReuse.LatestGeneration is null &&
                    !partialReuse.TryReuse(
                        partial.Composite.Generation,
                        out _),
                    $"Reuse chain {i + 1} should never cache a deferred partial frame.");
            }
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
}
