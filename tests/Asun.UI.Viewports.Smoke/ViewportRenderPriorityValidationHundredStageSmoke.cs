using System.Drawing;
using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportRenderPriorityValidationHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        static (
            ViewportCompositeRuntime<string> Runtime,
            ViewportCompositeFrame<string> Frame,
            ViewportRenderWorkPlan Plan) CreateScenario()
        {
            var runtime = new ViewportCompositeRuntime<string>(
                new Vector2(1600, 1200),
                new Vector2(500, 400),
                new Vector2(100, 100),
                1,
                64,
                4,
                new LocalTileSource());

            var selected =
                runtime.AddRoi(
                    RoiGeometry.CreateRectangle(
                        new Vector2(220, 180),
                        new Vector2(80, 60)));

            runtime.AddRoi(
                RoiGeometry.CreateEllipse(
                    new Vector2(500, 260),
                    new Vector2(70, 50)));

            runtime.SelectRoi(selected);

            var frame = runtime.RefreshAsync(true)
                .GetAwaiter()
                .GetResult();

            var plan = ViewportRenderWorkRuntime.Plan(
                frame,
                ViewportDirtyFlags.All);

            return (runtime, frame, plan);
        }

        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        for (var i = 0; i < 10; i++)
        {
            var s = CreateScenario();
            var prioritized =
                ViewportRenderPriorityRuntime.Prioritize(
                    s.Plan,
                    s.Frame);

            Check(
                ViewportRenderPriorityValidationRuntime.IsValid(
                    s.Plan,
                    prioritized,
                    s.Frame),
                $"priority invariant {i + 1} should validate.");

            s.Runtime.Dispose();
        }

        for (var i = 0; i < 10; i++)
        {
            var s = CreateScenario();
            var prioritized =
                ViewportRenderPriorityRuntime.Prioritize(
                    s.Plan,
                    s.Frame);

            Check(
                prioritized.Items.Count == s.Plan.Items.Count,
                $"priority count {i + 1} should remain unchanged.");

            s.Runtime.Dispose();
        }

        for (var i = 0; i < 10; i++)
        {
            var s = CreateScenario();
            var prioritized =
                ViewportRenderPriorityRuntime.Prioritize(
                    s.Plan,
                    s.Frame);

            var priorityKeys =
                prioritized.Items
                    .Select(item =>
                        item.IsInvalidation
                            ? -2
                            : item.Kind switch
                            {
                                ViewportRenderWorkKind.FullSurface => -1,
                                ViewportRenderWorkKind.Tile => 0,
                                ViewportRenderWorkKind.Roi => 3,
                                ViewportRenderWorkKind.Selection => 4,
                                ViewportRenderWorkKind.Overlay => 5,
                                _ => 6
                            })
                    .ToArray();

            Check(
                priorityKeys
                    .Zip(
                        priorityKeys.Skip(1),
                        (left, right) => left <= right)
                    .All(item => item),
                $"priority ordering {i + 1} should be non-decreasing.");

            s.Runtime.Dispose();
        }

        for (var i = 0; i < 10; i++)
        {
            var s = CreateScenario();
            var prioritized =
                ViewportRenderPriorityRuntime.Prioritize(
                    s.Plan,
                    s.Frame);

            var selectedId = s.Frame.Roi.Items
                .Single(item => item.IsSelected)
                .Id;

            Check(
                prioritized.Items
                    .Where(item =>
                        item.Kind == ViewportRenderWorkKind.Roi &&
                        !item.IsInvalidation)
                    .Select(item => item.RoiId)
                    .Contains(selectedId),
                $"selected ROI {i + 1} should remain in prioritized output.");

            s.Runtime.Dispose();
        }

        for (var i = 0; i < 10; i++)
        {
            var s = CreateScenario();
            var prioritized =
                ViewportRenderPriorityRuntime.Prioritize(
                    s.Plan,
                    s.Frame);

            var budgeted =
                ViewportRenderBudgetRuntime.Apply(
                    prioritized,
                    new ViewportRenderBudget(4, 2, 1, 8));

            Check(
                ViewportRenderBudgetValidationRuntime.IsValid(
                    prioritized,
                    budgeted,
                    new ViewportRenderBudget(4, 2, 1, 8)),
                $"priority-budget combination {i + 1} should validate.");

            s.Runtime.Dispose();
        }

        for (var i = 0; i < 10; i++)
        {
            var s = CreateScenario();
            var first =
                ViewportRenderPriorityRuntime.Prioritize(
                    s.Plan,
                    s.Frame);
            var second =
                ViewportRenderPriorityRuntime.Prioritize(
                    s.Plan,
                    s.Frame);

            Check(
                first.Items.SequenceEqual(second.Items),
                $"priority determinism {i + 1} should hold.");

            s.Runtime.Dispose();
        }

        for (var i = 0; i < 10; i++)
        {
            var s = CreateScenario();
            var prioritized =
                ViewportRenderPriorityRuntime.Prioritize(
                    s.Plan,
                    s.Frame);

            Check(
                prioritized.Generation == s.Frame.Generation,
                $"priority generation {i + 1} should match frame.");

            s.Runtime.Dispose();
        }

        for (var i = 0; i < 10; i++)
        {
            var s = CreateScenario();
            var prioritized =
                ViewportRenderPriorityRuntime.Prioritize(
                    s.Plan,
                    s.Frame);

            Check(
                prioritized.TileWorkCount >= 0 &&
                prioritized.RoiWorkCount >= 0 &&
                !prioritized.IsEmpty,
                $"priority counts {i + 1} should remain coherent.");

            s.Runtime.Dispose();
        }

        for (var i = 0; i < 10; i++)
        {
            var s = CreateScenario();
            var prioritized =
                ViewportRenderPriorityRuntime.Prioritize(
                    s.Plan,
                    s.Frame);
            Check(
                ViewportRenderPriorityValidationRuntime.Validate(
                    s.Plan,
                    prioritized,
                    s.Frame).Count == 0,
                $"priority final invariant {i + 1} should remain clean.");

            s.Runtime.Dispose();
        }

        for (var i = 0; i < 10; i++)
        {
            var s = CreateScenario();
            var prioritized =
                ViewportRenderPriorityRuntime.Prioritize(
                    s.Plan,
                    s.Frame);

            var invalidationCount =
                prioritized.Items.TakeWhile(
                    item => item.IsInvalidation).Count();

            Check(
                prioritized.Items
                    .Skip(invalidationCount)
                    .All(item => !item.IsInvalidation),
                $"invalidation prefix {i + 1} should be contiguous.");

            s.Runtime.Dispose();
        }

        assert(
            round == 100,
            $"Render priority validation smoke should execute exactly 100 numbered rounds; actual {round}.");
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
