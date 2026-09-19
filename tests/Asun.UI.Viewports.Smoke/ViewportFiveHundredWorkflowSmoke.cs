using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportFiveHundredWorkflowSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var successfulChains = 0;
        var totalSceneCommands = 0;
        var totalJournalRecords = 0;

        for (var i = 0; i < 500; i++)
        {
            var imageSize = new Vector2(
                4000f + (i % 17) * 37f,
                3000f + (i % 13) * 29f);

            var viewportSize = new Vector2(
                800f + (i % 5) * 40f,
                600f + (i % 7) * 30f);

            var workflow = new ViewportWorkflowRuntime(
                imageSize,
                viewportSize);

            var rectangleCenter = new Vector2(
                250f + (i % 19) * 17f,
                200f + (i % 23) * 13f);

            var rectangleSize = new Vector2(
                80f + (i % 11) * 5f,
                60f + (i % 9) * 4f);

            var ellipseCenter = rectangleCenter + new Vector2(
                180f + (i % 7) * 9f,
                120f + (i % 5) * 7f);

            var commands = new[]
            {
                ViewportWorkflowCommand.Fit(),
                ViewportWorkflowCommand.Resize(viewportSize),
                ViewportWorkflowCommand.AddRectangle(
                    rectangleCenter,
                    rectangleSize),
                ViewportWorkflowCommand.Translate(
                    new Vector2(
                        5f + (i % 7),
                        -3f - (i % 5))),
                ViewportWorkflowCommand.Duplicate(
                    new Vector2(
                        45f + (i % 11),
                        30f + (i % 13))),
                ViewportWorkflowCommand.AddEllipse(
                    ellipseCenter,
                    new Vector2(
                        70f + (i % 8) * 4f,
                        50f + (i % 6) * 3f)),
                ViewportWorkflowCommand.Pan(
                    new Vector2(
                        12f - (i % 9),
                        -8f + (i % 7))),
                ViewportWorkflowCommand.Zoom(
                    1.02 + (i % 4) * 0.01,
                    0.01,
                    20,
                    viewportSize * 0.5f),
                ViewportWorkflowCommand.Center(
                    rectangleCenter),
                ViewportWorkflowCommand.MoveSelectedToFront,
                ViewportWorkflowCommand.Fit()
            };

            var validation = ViewportWorkflowValidator.Validate(commands);

            assert(
                validation.IsValid &&
                validation.CommandCount == commands.Length &&
                validation.ErrorCount == 0,
                $"Workflow chain {i + 1} should validate before execution.");

            if (!validation.IsValid)
                continue;

            var before = workflow.Snapshot();
            workflow.ExecuteBatch(commands);

            var after = workflow.Snapshot();
            var sceneBefore = ViewportSceneRuntime.Build(
                workflow.Runtime.CreateSnapshot());

            workflow.Execute(
                ViewportWorkflowCommand.Translate(
                    new Vector2(
                        2f + i % 3,
                        1f + i % 2)));

            var sceneAfter = ViewportSceneRuntime.Build(
                workflow.Runtime.CreateSnapshot());

            var diff = ViewportSceneDiffRuntime.Diff(
                sceneBefore,
                sceneAfter);

            var replayed = workflow.Replay(
                new[]
                {
                    ViewportWorkflowCommand.Undo(),
                    ViewportWorkflowCommand.Redo()
                });

            var restored = workflow.Snapshot();

            var sceneCommandCount =
                sceneAfter.Commands.Count;

            var journalCount =
                workflow.Journal.Count;

            assert(
                after.Document.Items.Count >= 2,
                $"Workflow chain {i + 1} should contain multiple ROIs.");

            assert(
                sceneCommandCount > 0 &&
                diff.Count > 0,
                $"Workflow chain {i + 1} should produce incremental scene changes.");

            assert(
                replayed &&
                restored.Document.Items.Count == after.Document.Items.Count &&
                Math.Abs(
                    restored.Transform.Scale -
                    workflow.Runtime.Transform.Scale) < 1e-9,
                $"Workflow chain {i + 1} should support undo/redo replay.");

            assert(
                before.Document.Items.Count == 0 &&
                after.Document.Items.Count >= 2,
                $"Workflow chain {i + 1} should transition from empty to populated state.");

            totalSceneCommands += sceneCommandCount;
            totalJournalRecords += journalCount;
            successfulChains++;
        }

        assert(
            successfulChains == 500,
            $"Expected 500 executable workflow chains, got {successfulChains}.");

        assert(
            totalSceneCommands > 500 &&
            totalJournalRecords >= successfulChains * 10,
            "The 500 workflow chains should exercise non-trivial scene and journal activity.");
    }
}
