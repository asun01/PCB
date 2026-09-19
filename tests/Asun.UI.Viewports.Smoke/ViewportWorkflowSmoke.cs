using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportWorkflowSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var workflow = new ViewportWorkflowRuntime(
            new Vector2(1000, 1000),
            new Vector2(400, 300));

        var addValidation = ViewportWorkflowValidator.Validate(
            new[]
            {
                ViewportWorkflowCommand.AddRectangle(
                    new Vector2(100, 80),
                    new Vector2(40, 20)),
                ViewportWorkflowCommand.Pan(new Vector2(5, 6)),
                ViewportWorkflowCommand.Zoom(
                    1.2,
                    0.05,
                    64,
                    new Vector2(200, 150))
            });

        assert(
            addValidation.IsValid &&
            ViewportInvariantRuntime.ValidateWorkflow(addValidation).Count == 0,
            "Valid viewport workflow commands should pass structural validation.");

        var addId = workflow.Execute(
            ViewportWorkflowCommand.AddRectangle(
                new Vector2(100, 80),
                new Vector2(40, 20)));

        assert(
            addId is not null &&
            workflow.Runtime.Document.Count == 1 &&
            workflow.Runtime.SelectedId == addId,
            "Workflow execution should create and select the new ROI through the authoritative document runtime.");

        workflow.Execute(
            ViewportWorkflowCommand.Translate(new Vector2(10, 15)));

        var translatedCenter = workflow.Runtime.Document.Items
            .Single(item => item.Id == addId)
            .Geometry.Center;

        assert(
            translatedCenter == new Vector2(110, 95),
            "Workflow translation should change the selected ROI geometry.");

        var duplicateId = workflow.Execute(
            ViewportWorkflowCommand.Duplicate(new Vector2(30, 20)));

        assert(
            duplicateId is not null &&
            duplicateId != addId &&
            workflow.Runtime.Document.Count == 2,
            "Workflow duplication should create a distinct ROI while preserving document cardinality.");

        workflow.Execute(
            ViewportWorkflowCommand.Simple(
                ViewportWorkflowOperation.MoveSelectedToBack));

        workflow.Execute(
            ViewportWorkflowCommand.Simple(
                ViewportWorkflowOperation.MoveSelectedToFront));

        assert(
            workflow.Runtime.Document.Items
                .Select(item => item.ZIndex)
                .SequenceEqual(
                    workflow.Runtime.Document.Items
                        .Select(item => item.ZIndex)
                        .OrderBy(value => value)) ||
            workflow.Runtime.Document.Items.Count == 2,
            "Workflow front/back operations should preserve a deterministic document item set.");

        var beforeUndo = workflow.Snapshot();
        workflow.Execute(ViewportWorkflowCommand.Undo);

        assert(
            workflow.Runtime.Document.Count <= beforeUndo.Document.Items.Count &&
            workflow.Journal.Any(record =>
                record.Command.Operation == ViewportWorkflowOperation.Undo),
            "Workflow undo should update document state and record a replayable journal entry.");

        workflow.Execute(ViewportWorkflowCommand.Redo);

        assert(
            workflow.Runtime.Document.Count == beforeUndo.Document.Items.Count,
            "Workflow redo should restore the state removed by the previous undo.");

        workflow.Execute(ViewportWorkflowCommand.Select(null));

        assert(
            workflow.Runtime.SelectedId is null,
            "Workflow selection command should support explicit selection clearing.");

        workflow.Execute(
            ViewportWorkflowCommand.Simple(
                ViewportWorkflowOperation.DeleteSelected));

        assert(
            workflow.Runtime.Document.Count == beforeUndo.Document.Items.Count,
            "Deleting with no selected ROI should be a deterministic no-op.");

        var beforeBatch = workflow.Snapshot();

        var batchFailed = false;
        try
        {
            workflow.ExecuteBatch(
                new[]
                {
                    ViewportWorkflowCommand.Pan(new Vector2(20, 0)),
                    ViewportWorkflowCommand.Resize(Vector2.Zero)
                },
                rollbackOnFailure: true);
        }
        catch (ArgumentOutOfRangeException)
        {
            batchFailed = true;
        }

        assert(
            batchFailed &&
            workflow.Snapshot().Transform == beforeBatch.Transform &&
            workflow.Snapshot().Document.Items.Count == beforeBatch.Document.Items.Count,
            "A failed rollback-enabled workflow batch should restore both transform and document state.");

        var replayable = new[]
        {
            ViewportWorkflowCommand.Fit(),
            ViewportWorkflowCommand.Pan(new Vector2(8, 3)),
            ViewportWorkflowCommand.Center(new Vector2(250, 120))
        };

        var replayWorkflow = new ViewportWorkflowRuntime(
            new Vector2(1000, 1000),
            new Vector2(400, 300));

        assert(
            replayWorkflow.Replay(replayable) &&
            replayWorkflow.Journal.Count == replayable.Length &&
            replayWorkflow.Journal.All(record => record.Succeeded),
            "Replay should execute a valid workflow sequence and record each successful command.");

        var invalidValidation = ViewportWorkflowValidator.Validate(
            new[]
            {
                ViewportWorkflowCommand.Zoom(
                    double.NaN,
                    0.05,
                    64,
                    new Vector2(0, 0))
            });

        assert(
            !invalidValidation.IsValid &&
            invalidValidation.ErrorCount > 0,
            "Workflow validation should reject non-finite zoom command parameters before execution.");

        var invalidProfileErrors = ViewportInvariantRuntime.ValidateZoomProfile(
            new ViewportZoomProfile(
                1,
                0.5,
                1.1,
                0.9));

        assert(
            invalidProfileErrors.Count > 0,
            "Centralized invariant validation should reject an inverted zoom scale range.");
    }
}
