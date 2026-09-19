using System.Numerics;
using Asun.UI.Viewports;

public static class RoiDocumentRuntimeSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var document = new RoiDocumentRuntime();

        var first = document.Add(
            RoiGeometry.CreateRectangle(
                new Vector2(50, 50),
                new Vector2(40, 40)));

        var second = document.Add(
            RoiGeometry.CreateEllipse(
                new Vector2(50, 50),
                new Vector2(20, 20)));

        assert(
            document.Count == 2 &&
            document.SelectedId == second,
            "Document should add independent ROIs and select the newest item.");

        var hit = document.HitTest(new Vector2(50, 50), handleTolerance: 2f);
        assert(
            hit.Id == second &&
            hit.Hit.Handle == RoiHandleKind.Body,
            "Hit testing should respect z-order and select the topmost ROI.");

        document.Select(first);
        assert(
            document.MoveSelectedToFront() &&
            document.Items[^1].Id == first,
            "Selected ROI should be reorderable to the front.");

        assert(
            document.Undo() &&
            document.Items[^1].Id == second,
            "Undo should restore the previous z-order.");

        assert(
            document.Redo() &&
            document.Items[^1].Id == first,
            "Redo should restore the reordered z-order.");

        document.Select(first);
        var beforeMove = document.Items.Single(item => item.Id == first).Geometry;

        var down = document.PointerDown(
            beforeMove.Center,
            handleTolerance: 2f);

        assert(
            down.RoiId == first &&
            down.EditorEvent.Interaction == RoiInteractionKind.Moving,
            "Pointer down should route to the selected ROI editor.");

        document.PointerMove(beforeMove.Center + new Vector2(25, 10));
        document.PointerUp(beforeMove.Center + new Vector2(25, 10));

        var afterMove = document.Items.Single(item => item.Id == first).Geometry;
        assert(
            afterMove.Center == beforeMove.Center + new Vector2(25, 10) &&
            document.CanUndo,
            "Completed pointer editing should become one undoable document transaction.");

        assert(
            document.Undo() &&
            document.Items.Single(item => item.Id == first).Geometry.Equals(beforeMove),
            "Undo should revert the entire pointer edit as one operation.");

        assert(
            document.Redo() &&
            document.Items.Single(item => item.Id == first).Geometry.Equals(afterMove),
            "Redo should replay the entire pointer edit.");

        var duplicate = document.DuplicateSelected(new Vector2(10, 5));
        assert(
            duplicate is not null &&
            document.Count == 3 &&
            document.SelectedId == duplicate,
            "Duplicate should create an independent translated ROI.");

        assert(
            document.DeleteSelected() &&
            document.Count == 2,
            "Delete should remove the selected ROI.");

        var blankDown = document.PointerDown(
            new Vector2(500, 500),
            handleTolerance: 2f);

        assert(
            blankDown.RoiId is null &&
            document.SelectedId is null,
            "Clicking empty canvas should clear the active selection.");

        document.Mode = RoiEditorMode.CreateRectangle;
        var createDown = document.PointerDown(new Vector2(200, 200));
        document.PointerMove(new Vector2(260, 240));
        var createUp = document.PointerUp(new Vector2(260, 240));

        assert(
            createDown.RoiId is not null &&
            createUp.RoiId == createDown.RoiId &&
            document.Count == 3,
            "Create mode should create and commit a new ROI through the same pointer pipeline.");

        document.Mode = RoiEditorMode.Select;
        var createdId = createUp.RoiId!.Value;
        var createdBeforeCancel = document.Items.Single(item => item.Id == createdId).Geometry;

        document.PointerDown(createdBeforeCancel.Center, handleTolerance: 2f);
        document.PointerMove(createdBeforeCancel.Center + new Vector2(100, 50));
        document.Cancel(createdBeforeCancel.Center + new Vector2(100, 50));

        assert(
            document.Items.Single(item => item.Id == createdId).Geometry.Equals(createdBeforeCancel),
            "Cancelling a document pointer transaction should restore the pre-edit geometry.");

        var snapshot = document.CreateSnapshot();
        assert(
            snapshot.Items.Count == document.Count &&
            snapshot.SelectedId == document.SelectedId,
            "Document snapshots should capture items and selection.");

        assert(
            document.Select(null) &&
            document.SelectedId is null,
            "Explicit deselection should clear the active ROI.");
        document.RestoreSnapshot(snapshot);

        assert(
            document.SelectedId == snapshot.SelectedId &&
            document.Count == snapshot.Items.Count,
            "Restoring a snapshot should restore document selection and item count.");
    }
}
