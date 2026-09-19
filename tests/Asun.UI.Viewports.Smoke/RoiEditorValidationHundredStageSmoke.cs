using System.Numerics;
using Asun.UI.Viewports;

public static class RoiEditorValidationHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var editor = new RoiEditorRuntime();
        editor.SetGeometry(
            RoiGeometry.CreateRectangle(
                new Vector2(50, 50),
                new Vector2(20, 10)));

        var before = editor.CreateSnapshot();

        var round = 0;
        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        for (var i = 0; i < 10; i++)
            Check(
                RoiEditorValidationRuntime.IsValid(before),
                $"baseline ROI round {i + 1} should be valid.");

        editor.PointerDown(new Vector2(50, 50));
        editor.PointerMove(new Vector2(65, 70));
        var duringEdit = editor.CreateSnapshot();

        for (var i = 0; i < 10; i++)
            Check(
                duringEdit.Interaction.IsActive &&
                RoiEditorValidationRuntime.IsValid(duringEdit),
                $"active edit round {i + 1} should retain interaction state.");

        editor.Cancel(new Vector2(65, 70));
        var afterCancel = editor.CreateSnapshot();

        for (var i = 0; i < 10; i++)
            Check(
                RoiEditorValidationRuntime.IsCancelledToCommitted(
                    before,
                    afterCancel),
                $"cancel round {i + 1} should restore committed geometry.");

        editor.Mode = RoiEditorMode.CreateRectangle;
        editor.PointerDown(new Vector2(10, 10));
        editor.PointerMove(new Vector2(20, 30));
        var duringCreate = editor.CreateSnapshot();

        for (var i = 0; i < 10; i++)
            Check(
                duringCreate.Interaction.Kind == RoiInteractionKind.Creating &&
                duringCreate.Interaction.StartGeometry is null &&
                RoiEditorValidationRuntime.IsValid(duringCreate),
                $"create preview round {i + 1} should retain creation semantics.");

        editor.Cancel(new Vector2(20, 30));
        var afterCreateCancel = editor.CreateSnapshot();

        for (var i = 0; i < 10; i++)
            Check(
                RoiEditorValidationRuntime.IsValid(afterCreateCancel) &&
                afterCreateCancel.Geometry!.Equals(
                    afterCreateCancel.CommittedGeometry!),
                $"create cancellation round {i + 1} should restore committed ROI.");

        editor.Mode = RoiEditorMode.Select;
        editor.PointerDown(new Vector2(50, 50));
        editor.PointerMove(new Vector2(55, 55));
        editor.PointerUp(new Vector2(55, 55));
        var afterCommit = editor.CreateSnapshot();

        for (var i = 0; i < 10; i++)
            Check(
                !afterCommit.Interaction.IsActive &&
                afterCommit.Geometry!.Equals(
                    afterCommit.CommittedGeometry!),
                $"commit round {i + 1} should converge preview and committed geometry.");

        for (var i = 0; i < 10; i++)
            Check(
                RoiEditorValidationRuntime.IsValid(afterCommit),
                $"final ROI state round {i + 1} should remain valid.");

        for (var i = 0; i < 10; i++)
            Check(
                afterCommit.Interaction.Kind == RoiInteractionKind.Idle,
                $"final interaction round {i + 1} should be idle.");

        for (var i = 0; i < 10; i++)
            Check(
                afterCommit.Geometry is not null &&
                afterCommit.CommittedGeometry is not null,
                $"final geometry round {i + 1} should remain committed.");

        for (var i = 0; i < 10; i++)
            Check(
                afterCommit.Mode == RoiEditorMode.Select,
                $"final mode round {i + 1} should return to Select.");

        assert(
            round == 100,
            $"ROI editor validation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
