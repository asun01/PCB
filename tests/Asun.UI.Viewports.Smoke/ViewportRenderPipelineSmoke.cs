using System.Drawing;
using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportRenderPipelineSmoke
{
    public static async ValueTask RunAsync(Action<bool, string> assert)
    {
        for (var i = 0; i < 500; i++)
        {
            using var pipeline = new ViewportRenderPipelineRuntime<string>(
                new Vector2(3000 + i % 13 * 29, 2200 + i % 17 * 23),
                new Vector2(700 + i % 5 * 20, 500 + i % 7 * 15),
                new Vector2(100, 100),
                1,
                96,
                4,
                new LocalTileSource(),
                new ViewportRenderBudget(
                    8,
                    16,
                    2,
                    24),
                120);

            var center = new Vector2(
                320 + i % 13 * 14,
                260 + i % 11 * 12);

            pipeline.Composite.AddRoi(
                RoiGeometry.CreateRectangle(
                    center,
                    new Vector2(110, 80)));

            pipeline.Composite.DuplicateSelected(
                new Vector2(130, 90));

            pipeline.Composite.PanBy(
                new Vector2(
                    4 - i % 3,
                    2 + i % 2));

            pipeline.SubmitPointer(
                new Vector2(
                    100 + i % 20,
                    80 + i % 15));

            pipeline.SubmitPointer(
                new Vector2(
                    101 + i % 20,
                    82 + i % 15));

            var result = await pipeline.RefreshAsync(
                DateTimeOffset.UtcNow.AddSeconds(1),
                includePrefetch: i % 2 == 0);

            assert(
                result is not null &&
                result.Accepted,
                $"Pipeline chain {i + 1} should accept its first frame.");

            if (result is null)
                continue;

            assert(
                result.Composite.IsReady &&
                result.Composite.Roi.Document.Items.Count == 2,
                $"Pipeline chain {i + 1} should produce a ready composite frame.");

            assert(
                result.WorkPlan.Generation == result.Composite.Generation &&
                result.Batch.Generation == result.Composite.Generation &&
                result.Batch.ItemCount == result.WorkPlan.Items.Count,
                $"Pipeline chain {i + 1} should preserve generation across all layers.");

            assert(
                result.Batch.RegionCount > 0 &&
                result.Batch.Regions.All(region =>
                    region.Left >= 0 &&
                    region.Top >= 0 &&
                    region.Right <= result.Composite.Tiles.Transform.ViewportSize.X + 1e-4f &&
                    region.Bottom <= result.Composite.Tiles.Transform.ViewportSize.Y + 1e-4f),
                $"Pipeline chain {i + 1} should return viewport-clipped render regions.");

            assert(
                pipeline.TryTakePointer(out var pointer) &&
                pointer.Position == new Vector2(
                    101 + i % 20,
                    82 + i % 15),
                $"Pipeline chain {i + 1} should preserve the latest coalesced pointer.");

            pipeline.Composite.PanBy(new Vector2(5, 1));

            var second = await pipeline.RefreshAsync(
                DateTimeOffset.UtcNow.AddSeconds(2));

            assert(
                second is not null &&
                second.Composite.Tiles.Transform == second.Composite.Roi.Transform &&
                second.Batch.Generation == second.Composite.Generation,
                $"Pipeline chain {i + 1} should continue after navigation.");

            pipeline.Reset();

            assert(
                pipeline.Scheduler.PendingFlags == ViewportDirtyFlags.None,
                $"Pipeline chain {i + 1} reset should clear pending scheduler work.");
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
