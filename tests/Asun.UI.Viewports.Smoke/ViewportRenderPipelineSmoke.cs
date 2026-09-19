using System.Drawing;
using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportRenderPipelineSmoke
{
    public static async ValueTask RunAsync(Action<bool, string> assert)
    {
        for (var i = 0; i < 500; i++)
        {
            using var runtime = new ViewportCompositeRuntime<string>(
                new Vector2(1600 + i % 11 * 37, 1200 + i % 13 * 31),
                new Vector2(500 + i % 7 * 20, 400 + i % 5 * 15),
                new Vector2(100, 100),
                1,
                64,
                4,
                new LocalTileSource());

            runtime.AddRoi(
                RoiGeometry.CreateRectangle(
                    new Vector2(300 + i % 9 * 11, 240 + i % 7 * 9),
                    new Vector2(90, 60)));

            runtime.PanBy(
                new Vector2(5 - i % 3, 3 - i % 2));

            var frame = await runtime.RefreshAsync(
                includePrefetch: i % 2 == 0);

            var scheduler = new ViewportRenderSchedulerRuntime(120);
            scheduler.Submit(
                ViewportDirtyFlags.Image |
                ViewportDirtyFlags.Roi |
                ViewportDirtyFlags.Selection,
                frame.Generation);

            scheduler.SubmitPointer(
                new Vector2(i % 100, i % 80));
            scheduler.SubmitPointer(
                new Vector2(i % 100 + 1, i % 80 + 2));

            var now = DateTimeOffset.UtcNow.AddSeconds(1);
            var accepted = scheduler.TryTakeFrame(
                now,
                out var submission);

            assert(
                accepted &&
                submission.DirtyFlags.HasFlag(ViewportDirtyFlags.Image) &&
                submission.DirtyFlags.HasFlag(ViewportDirtyFlags.Roi) &&
                submission.Generation == frame.Generation,
                $"Render pipeline {i + 1} should schedule one coalesced frame.");

            assert(
                scheduler.TryTakePointer(out var pointer) &&
                pointer.Position == new Vector2(i % 100 + 1, i % 80 + 2),
                $"Render pipeline {i + 1} should coalesce pointer input to the latest event.");

            var plan = ViewportRenderWorkRuntime.Plan(
                frame,
                submission.DirtyFlags);

            var regions = ViewportRenderRegionRuntime.ClipAndMerge(
                plan.Items.Select(item => item.Bounds),
                frame.Tiles.Transform);

            assert(
                plan.Items.Count > 0 &&
                plan.Generation == frame.Generation &&
                regions.Count > 0,
                $"Render pipeline {i + 1} should produce clipped render work.");

            var tileWork = plan.Items.Count(
                item => item.Kind == ViewportRenderWorkKind.Tile);

            var roiWork = plan.Items.Count(
                item => item.Kind == ViewportRenderWorkKind.Roi);

            assert(
                tileWork > 0 &&
                roiWork > 0,
                $"Render pipeline {i + 1} should combine tile and ROI render work.");

            runtime.PanBy(new Vector2(4, 1));
            var nextFrame = await runtime.RefreshAsync();

            var nextPlan = ViewportRenderWorkRuntime.Plan(
                nextFrame,
                ViewportDirtyFlags.Image |
                ViewportDirtyFlags.Transform |
                ViewportDirtyFlags.Roi);

            assert(
                nextPlan.Generation == nextFrame.Generation &&
                nextPlan.Items.Any(item => item.Kind == ViewportRenderWorkKind.Tile),
                $"Render pipeline {i + 1} should rebuild tile work after navigation.");
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
