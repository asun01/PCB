using System.Drawing;
using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportRenderPrioritySmoke
{
    public static async ValueTask RunAsync(Action<bool, string> assert)
    {
        for (var i = 0; i < 500; i++)
        {
            using var runtime = new ViewportCompositeRuntime<string>(
                new Vector2(2400 + i % 7 * 37, 1800 + i % 9 * 29),
                new Vector2(640 + i % 5 * 20, 480 + i % 3 * 25),
                new Vector2(100, 100),
                1,
                64,
                4,
                new LocalTileSource());

            var first = runtime.AddRoi(
                RoiGeometry.CreateRectangle(
                    new Vector2(320 + i % 11 * 10, 240 + i % 7 * 9),
                    new Vector2(90, 70)));

            runtime.AddRoi(
                RoiGeometry.CreateEllipse(
                    new Vector2(540 + i % 8 * 12, 320 + i % 5 * 8),
                    new Vector2(100, 60)));

            runtime.SelectRoi(first);

            var frame = await runtime.RefreshAsync();

            var plan = ViewportRenderWorkRuntime.Plan(
                frame,
                ViewportDirtyFlags.All);

            var prioritized = ViewportRenderPriorityRuntime.Prioritize(
                plan,
                frame);

            assert(
                prioritized.Items.Count == plan.Items.Count &&
                prioritized.Generation == frame.Generation,
                $"Priority chain {i + 1} should preserve every work item and generation.");

            var firstRoi = prioritized.Items
                .FirstOrDefault(item => item.Kind == ViewportRenderWorkKind.Roi);

            if (firstRoi.RoiId != Guid.Empty)
            {
                assert(
                    firstRoi.RoiId == first,
                    $"Priority chain {i + 1} should promote selected ROI work.");
            }

            var batch = ViewportRenderBatchRuntime.Create(
                prioritized,
                frame.Tiles.Transform);

            var metrics = ViewportRenderPlanMetricsRuntime.Capture(
                prioritized,
                batch,
                frame.Roi.Items
                    .Where(item => item.IsSelected)
                    .Select(item => item.Id)
                    .ToHashSet());

            assert(
                metrics.Total == prioritized.Items.Count &&
                metrics.Tile == prioritized.TileWorkCount &&
                metrics.Roi == prioritized.RoiWorkCount &&
                metrics.Regions == batch.RegionCount,
                $"Priority chain {i + 1} should produce consistent plan metrics.");

            var budget = ViewportRenderBudgetRuntime.Apply(
                prioritized,
                new ViewportRenderBudget(4, 4, 1, 8));

            assert(
                budget.TileWorkCount <= 4 &&
                budget.RoiWorkCount <= 4 &&
                budget.Items.Count <= 8,
                $"Priority chain {i + 1} should apply budget after prioritization.");
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
