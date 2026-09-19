using System.Drawing;
using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportRenderBudgetSmoke
{
    public static async ValueTask RunAsync(Action<bool, string> assert)
    {
        for (var i = 0; i < 500; i++)
        {
            using var runtime = new ViewportCompositeRuntime<string>(
                new Vector2(2200 + i % 9 * 41, 1700 + i % 11 * 37),
                new Vector2(600 + i % 7 * 15, 450 + i % 5 * 20),
                new Vector2(80, 80),
                1,
                100,
                4,
                new LocalTileSource());

            var roiId = runtime.AddRoi(
                RoiGeometry.CreateRectangle(
                    new Vector2(300 + i % 13 * 17, 250 + i % 7 * 13),
                    new Vector2(100, 70)));

            runtime.SelectRoi(roiId);
            runtime.DuplicateSelected(new Vector2(140, 100));

            var frame = await runtime.RefreshAsync(
                includePrefetch: i % 2 == 0);

            var plan = ViewportRenderWorkRuntime.Plan(
                frame,
                ViewportDirtyFlags.All);

            var prioritized = ViewportRenderPriorityRuntime.Prioritize(
                plan,
                frame);

            assert(
                prioritized.Items.Count > 0 &&
                prioritized.Items[0].Kind == ViewportRenderWorkKind.FullSurface &&
                prioritized.Items.Skip(1).Any(item =>
                    item.Kind == ViewportRenderWorkKind.Tile),
                $"Budget chain {i + 1} should clear the full surface before drawing tiles.");

            var budget = new ViewportRenderBudget(
                MaxTileWork: 4 + i % 5,
                MaxRoiWork: 4 + i % 7,
                MaxOverlayWork: 1,
                MaxTotalWork: 12 + i % 9);

            var limited = ViewportRenderBudgetRuntime.Apply(
                plan,
                budget);

            var batch = ViewportRenderBatchRuntime.Create(
                limited,
                frame.Tiles.Transform);

            assert(
                limited.Items.Count <= budget.MaxTotalWork &&
                limited.TileWorkCount <= budget.MaxTileWork &&
                limited.RoiWorkCount <= budget.MaxRoiWork,
                $"Budget chain {i + 1} should enforce deterministic work ceilings.");

            using var pipeline = new ViewportRenderPipelineRuntime<string>(
                new Vector2(1600, 1200),
                new Vector2(400, 300),
                new Vector2(100, 100),
                1,
                64,
                2,
                new LocalTileSource(),
                new ViewportRenderBudget(1, 4, 1, 1),
                1000);

            pipeline.Invalidate(
                ViewportDirtyFlags.All,
                pipeline.Composite.Generation);

            var fullFrame = await pipeline.RefreshAsync(
                DateTimeOffset.UtcNow.AddSeconds(1));

            assert(
                fullFrame is not null &&
                fullFrame.HasDeferredWork &&
                fullFrame.WorkPlan.Items.Any(item =>
                    item.Kind == ViewportRenderWorkKind.FullSurface),
                $"Budget chain {i + 1} should retain a deferred full-frame continuation.");

            if (fullFrame is not null)
            {
                var continuation = await pipeline.RefreshAsync(
                    DateTimeOffset.UtcNow.AddSeconds(2));

                assert(
                    continuation is not null &&
                    continuation.WorkPlan.Items.All(item =>
                        item.Kind != ViewportRenderWorkKind.FullSurface) &&
                    continuation.WorkPlan.Items.Any(item =>
                        item.Kind == ViewportRenderWorkKind.Tile),
                    $"Budget chain {i + 1} should continue with tile work after the one-time full clear.");
            }

            assert(
                batch.Generation == frame.Generation &&
                batch.ItemCount == limited.Items.Count &&
                batch.RegionCount > 0,
                $"Budget chain {i + 1} should consolidate render work into regions.");

            assert(
                batch.Regions.All(region =>
                    region.Left >= 0 &&
                    region.Top >= 0 &&
                    region.Right <= frame.Tiles.Transform.ViewportSize.X + 1e-4f &&
                    region.Bottom <= frame.Tiles.Transform.ViewportSize.Y + 1e-4f),
                $"Budget chain {i + 1} should clip all regions to the viewport.");

            var empty = ViewportRenderBatchRuntime.Empty(frame.Generation);
            assert(
                empty.IsEmpty &&
                empty.Generation == frame.Generation,
                $"Budget chain {i + 1} should expose an explicit empty batch.");

            runtime.TranslateSelected(new Vector2(9, 6));

            var incremental = await runtime.RefreshAsync();

            var incrementalPlan = ViewportRenderWorkRuntime.Plan(
                incremental,
                incremental.DirtyFlags);

            var incrementalPrioritized = ViewportRenderPriorityRuntime.Prioritize(
                incrementalPlan,
                incremental);

            assert(
                incrementalPrioritized.Items.Count > 0 &&
                incrementalPrioritized.Items[0].IsInvalidation,
                $"Budget chain {i + 1} should invalidate stale ROI regions before redraw.");

            assert(
                incrementalPlan.Items.Any(item => item.IsInvalidation) &&
                incrementalPlan.Items.Any(item =>
                    item.Kind == ViewportRenderWorkKind.Roi &&
                    !item.IsInvalidation),
                $"Budget chain {i + 1} should produce both old-region invalidation and current ROI work.");

            var constrained = ViewportRenderBudgetRuntime.Apply(
                incrementalPlan,
                new ViewportRenderBudget(
                    MaxTileWork: 0,
                    MaxRoiWork: 1,
                    MaxOverlayWork: 0,
                    MaxTotalWork: 2));

            assert(
                constrained.Items.Any(item => item.IsInvalidation) &&
                constrained.Items.Any(item =>
                    item.Kind == ViewportRenderWorkKind.Roi &&
                    !item.IsInvalidation) &&
                constrained.Items.Count <= 2 &&
                constrained.RoiWorkCount <= 1 &&
                constrained.InvalidationWorkCount >= 1,
                $"Budget chain {i + 1} should preserve invalidation before normal ROI work.");

            var invalidationOnly = ViewportRenderBudgetRuntime.Apply(
                incrementalPlan,
                new ViewportRenderBudget(
                    MaxTileWork: 0,
                    MaxRoiWork: 0,
                    MaxOverlayWork: 0,
                    MaxTotalWork: 1));

            assert(
                invalidationOnly.InvalidationWorkCount == 1 &&
                invalidationOnly.RoiWorkCount == 0,
                $"Budget chain {i + 1} should preserve one invalidation even when ROI draw budget is zero.");
        }
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
