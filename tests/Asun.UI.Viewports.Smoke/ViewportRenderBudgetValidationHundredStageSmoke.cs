using System.Drawing;
using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportRenderBudgetValidationHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        static (
            ViewportCompositeRuntime<string> Runtime,
            ViewportCompositeFrame<string> Frame,
            ViewportRenderWorkPlan Plan) CreateScenario()
        {
            var runtime = new ViewportCompositeRuntime<string>(
                new Vector2(1800, 1400),
                new Vector2(600, 450),
                new Vector2(100, 100),
                1,
                64,
                4,
                new LocalTileSource());

            runtime.AddRoi(
                RoiGeometry.CreateRectangle(
                    new Vector2(250, 220),
                    new Vector2(80, 60)));

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
            var scenario = CreateScenario();
            var prioritized =
                ViewportRenderPriorityRuntime.Prioritize(
                    scenario.Plan,
                    scenario.Frame);
            var budget =
                new ViewportRenderBudget(2, 2, 1, 5);
            var limited =
                ViewportRenderBudgetRuntime.Apply(
                    prioritized,
                    budget);

            Check(
                ViewportRenderBudgetValidationRuntime.IsValid(
                    prioritized,
                    limited,
                    budget),
                $"budget validator {i + 1} should accept valid limits.");

            scenario.Runtime.Dispose();
        }

        for (var i = 0; i < 10; i++)
        {
            var scenario = CreateScenario();
            var limited =
                ViewportRenderBudgetRuntime.Apply(
                    scenario.Plan,
                    new ViewportRenderBudget(1, 1, 1, 3));

            Check(
                limited.Items.Count <= 3 &&
                limited.TileWorkCount <= 1 &&
                limited.RoiWorkCount <= 1,
                $"budget ceilings {i + 1} should remain bounded.");

            scenario.Runtime.Dispose();
        }

        for (var i = 0; i < 10; i++)
        {
            var scenario = CreateScenario();
            var budgeted =
                ViewportRenderBudgetRuntime.Apply(
                    scenario.Plan,
                    new ViewportRenderBudget(1, 1, 1, 1));

            var sourceSet = scenario.Plan.Items.ToHashSet();

            Check(
                budgeted.Items.All(sourceSet.Contains),
                $"budget subset {i + 1} should contain only source items.");

            scenario.Runtime.Dispose();
        }

        for (var i = 0; i < 10; i++)
        {
            var scenario = CreateScenario();
            var budgeted =
                ViewportRenderBudgetRuntime.Apply(
                    scenario.Plan,
                    new ViewportRenderBudget(0, 0, 0, 1));

            Check(
                ViewportRenderBudgetValidationRuntime.IsValid(
                    scenario.Plan,
                    budgeted,
                    new ViewportRenderBudget(0, 0, 0, 1)),
                $"zero category budget {i + 1} should remain internally coherent.");

            scenario.Runtime.Dispose();
        }

        for (var i = 0; i < 10; i++)
        {
            var scenario = CreateScenario();

            var invalid = new ViewportRenderBudget(
                -1, 1, 1, 1);

            var rejected = false;

            try
            {
                invalid.Validate();
            }
            catch (ArgumentOutOfRangeException)
            {
                rejected = true;
            }

            Check(
                rejected,
                $"negative budget {i + 1} should be rejected.");

            scenario.Runtime.Dispose();
        }

        for (var i = 0; i < 10; i++)
        {
            var scenario = CreateScenario();
            var budget =
                new ViewportRenderBudget(4, 4, 2, 12);
            var budgeted =
                ViewportRenderBudgetRuntime.Apply(
                    scenario.Plan,
                    budget);

            Check(
                budgeted.Generation == scenario.Plan.Generation,
                $"budget generation {i + 1} should be preserved.");

            scenario.Runtime.Dispose();
        }

        for (var i = 0; i < 10; i++)
        {
            var scenario = CreateScenario();
            var budgeted =
                ViewportRenderBudgetRuntime.Apply(
                    scenario.Plan,
                    new ViewportRenderBudget(64, 256, 8, 320));

            Check(
                budgeted.Items.Count == scenario.Plan.Items.Count,
                $"unconstrained budget {i + 1} should retain all work.");

            scenario.Runtime.Dispose();
        }

        for (var i = 0; i < 10; i++)
        {
            var scenario = CreateScenario();
            var prioritized =
                ViewportRenderPriorityRuntime.Prioritize(
                    scenario.Plan,
                    scenario.Frame);
            var budget =
                new ViewportRenderBudget(1, 4, 1, 2);
            var limited =
                ViewportRenderBudgetRuntime.Apply(
                    prioritized,
                    budget);

            Check(
                limited.Items.Count <= 2 &&
                ViewportRenderBudgetValidationRuntime.Validate(
                    prioritized,
                    limited,
                    budget).Count == 0,
                $"priority-to-budget chain {i + 1} should validate.");

            scenario.Runtime.Dispose();
        }

        for (var i = 0; i < 10; i++)
        {
            var scenario = CreateScenario();
            var budget =
                new ViewportRenderBudget(1, 1, 0, 1);

            var limited =
                ViewportRenderBudgetRuntime.Apply(
                    scenario.Plan,
                    budget);

            Check(
                ViewportRenderBudgetValidationRuntime.IsValid(
                    scenario.Plan,
                    limited,
                    budget),
                $"minimal budget {i + 1} should validate.");

            scenario.Runtime.Dispose();
        }

        for (var i = 0; i < 10; i++)
        {
            var scenario = CreateScenario();
            var budget =
                new ViewportRenderBudget(1, 1, 1, 1);
            var limited =
                ViewportRenderBudgetRuntime.Apply(
                    scenario.Plan,
                    budget);

            Check(
                limited.Items.Count <= 1 &&
                ViewportRenderBudgetValidationRuntime.IsValid(
                    scenario.Plan,
                    limited,
                    budget),
                $"strict total budget {i + 1} should remain valid.");

            scenario.Runtime.Dispose();
        }

        assert(
            round == 100,
            $"Render budget validation smoke should execute exactly 100 numbered rounds; actual {round}.");
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
