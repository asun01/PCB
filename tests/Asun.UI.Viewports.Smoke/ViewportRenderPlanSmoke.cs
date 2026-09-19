using System.Drawing;
using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportRenderPlanSmoke
{
    public static async ValueTask RunAsync(Action<bool, string> assert)
    {
        using var pipeline = new ViewportRenderPipelineRuntime<string>(
            new Vector2(1000, 800),
            new Vector2(300, 200),
            new Vector2(100, 100),
            0,
            16,
            2,
            new StableTileSource(),
            new ViewportRenderBudget(2, 2, 1, 4),
            120);

        var roiId = pipeline.Composite.AddRoi(
            RoiGeometry.CreateRectangle(
                new Vector2(120, 90),
                new Vector2(40, 30)));

        var initial = await pipeline.RefreshAsync(
            DateTimeOffset.UtcNow.AddSeconds(1));

        assert(
            initial is not null &&
            initial.Batch.FullSurfaceCount == 1,
            "A complete initial invalidation should produce one full-surface clear command.");

        if (initial is null)
            return;

        var commandKinds = initial.CommandStream.Commands
            .Select(command => command.Kind)
            .ToArray();

        assert(
            commandKinds.Contains(ViewportRenderCommandKind.FullSurfaceClear) &&
            initial.CommandStream.FullSurfaceCount == 1,
            "Initial command stream should represent the full-surface boundary explicitly.");

        pipeline.Invalidate(
            ViewportDirtyFlags.Overlay,
            pipeline.Composite.Generation);

        var overlay = await pipeline.RefreshAsync(
            DateTimeOffset.UtcNow.AddSeconds(2));

        assert(
            overlay is not null &&
            overlay.Batch.OverlayCount == 1 &&
            overlay.Batch.TileCount == 0 &&
            overlay.Batch.RoiCount == 0 &&
            overlay.Batch.FullSurfaceCount == 0,
            "Overlay-only invalidation should not replay image or ROI layers.");

        pipeline.Composite.SelectRoi(roiId);
        pipeline.Composite.TranslateSelected(new Vector2(10, 5));

        var incremental = await pipeline.RefreshAsync(
            DateTimeOffset.UtcNow.AddSeconds(3));

        assert(
            incremental is not null &&
            incremental.Batch.InvalidationCount > 0 &&
            incremental.Batch.RoiCount > 0,
            "ROI movement should create an invalidation-plus-redraw plan for incremental presentation.");

        pipeline.Invalidate(
            ViewportDirtyFlags.Image,
            pipeline.Composite.Generation);

        var imageOnly = await pipeline.RefreshAsync(
            DateTimeOffset.UtcNow.AddSeconds(4));

        assert(
            imageOnly is not null &&
            imageOnly.Batch.TileCount > 0,
            "Image invalidation should create visible tile work.");

        var selectionFlags =
            ViewportDirtyFlags.Selection;

        pipeline.Invalidate(
            selectionFlags,
            pipeline.Composite.Generation);

        var selectionOnly = await pipeline.RefreshAsync(
            DateTimeOffset.UtcNow.AddSeconds(5));

        assert(
            selectionOnly is not null &&
            selectionOnly.Batch.RoiCount >= 1,
            "Selection invalidation should include the ROI scene required to present selection state.");

        var dedupPlan = ViewportRenderWorkRuntime.Plan(
            selectionOnly!.Composite,
            selectionFlags);

        assert(
            dedupPlan.Items.Count ==
            dedupPlan.Items.Distinct().Count(),
            "Render work planning should deduplicate identical WorkItems before batching.");

        var bounded = ViewportRenderBudgetRuntime.Apply(
            initial.WorkPlan,
            new ViewportRenderBudget(
                MaxTileWork: 1,
                MaxRoiWork: 1,
                MaxOverlayWork: 1,
                MaxTotalWork: 2));

        assert(
            bounded.Items.Count <= 2 &&
            bounded.Items.All(item =>
                item.Kind == ViewportRenderWorkKind.FullSurface ||
                item.IsInvalidation ||
                item.Kind == ViewportRenderWorkKind.Tile),
            "Render budgeting should respect the total budget and preserve high-priority clear/invalidation work.");

        var batch = ViewportRenderBatchRuntime.Create(
            bounded,
            initial.Composite.Tiles.Transform);

        assert(
            batch.Regions.All(region =>
                region.Left >= -1e-4f &&
                region.Top >= -1e-4f &&
                region.Right <= initial.Composite.Tiles.Transform.ViewportSize.X + 1e-4f &&
                region.Bottom <= initial.Composite.Tiles.Transform.ViewportSize.Y + 1e-4f),
            "Render batch region generation should clip all output to the viewport.");

        var commandStream = ViewportRenderCommandStreamRuntime.Build(batch);

        assert(
            commandStream.CommandCount == batch.ItemCount &&
            commandStream.Commands.Select(command => command.WorkItem)
                .SequenceEqual(batch.Items),
            "Command stream generation should preserve the exact batch item ordering and identity.");

        var empty = ViewportRenderBatchRuntime.Empty(
            initial.Composite.Generation);

        assert(
            empty.IsEmpty &&
            empty.Generation == initial.Composite.Generation &&
            ViewportRenderEvidenceRuntime.ComputeBatchHash(empty).Length == 64,
            "Empty render batches should remain generation-aware and evidence-fingerprintable.");
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
}
