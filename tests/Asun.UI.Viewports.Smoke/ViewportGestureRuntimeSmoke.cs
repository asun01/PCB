using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportGestureRuntimeSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var viewport = new RoiViewportRuntime(
            imageSize: new Vector2(1000, 500),
            viewportSize: new Vector2(500, 500));

        var gestures = new ViewportGestureRuntime(
            viewport,
            handleTolerancePixels: 6f,
            minScale: 0.05,
            maxScale: 16,
            wheelStep: 1.25);

        var roiId = viewport.Document.Add(
            RoiGeometry.CreateRectangle(
                new Vector2(500, 250),
                new Vector2(200, 100)));

        viewport.Document.Select(roiId);

        var centerViewport = viewport.Transform.ImageToViewport(
            new Vector2(500, 250));

        var hoverEvent = gestures.PointerMove(centerViewport);
        assert(
            hoverEvent.Kind == ViewportGestureKind.Idle &&
            viewport.Hover.Id == roiId &&
            viewport.Hover.Hit.Hit,
            "Idle pointer movement should update ROI hover without starting a gesture.");

        var roiDown = gestures.PointerDown(centerViewport);
        assert(
            roiDown.Kind == ViewportGestureKind.RoiEditing &&
            gestures.IsPointerDown,
            "Pointer down on an ROI should enter the ROI editing gesture path.");

        var roiMove = gestures.PointerMove(
            centerViewport + new Vector2(20, 10));

        var roiUp = gestures.PointerUp(
            centerViewport + new Vector2(20, 10));

        assert(
            roiMove.Kind == ViewportGestureKind.RoiEditing &&
            roiUp.Kind == ViewportGestureKind.RoiEditing &&
            !gestures.IsPointerDown,
            "ROI drag should stay in the editing path until pointer up.");

        var movedImageGeometry = viewport.Document.Items
            .Single(item => item.Id == roiId)
            .Geometry;

        assert(
            movedImageGeometry.Center.X > 500f &&
            movedImageGeometry.Center.Y > 250f,
            "ROI drag should update image-space geometry.");

        var beforeWheel = viewport.Transform;
        var wheelAnchor = centerViewport;
        var imageAtAnchorBefore = beforeWheel.ViewportToImage(wheelAnchor);

        var wheel = gestures.Wheel(wheelAnchor, 120);

        var imageAtAnchorAfter =
            viewport.Transform.ViewportToImage(wheelAnchor);

        assert(
            wheel.TransformChanged &&
            Vector2.Distance(imageAtAnchorBefore, imageAtAnchorAfter) < 1e-4f,
            "Wheel zoom should preserve the image point under the pointer.");

        var roiBoundsBeforeFocus = viewport.Document.Items
            .Single(item => item.Id == roiId)
            .Geometry
            .GetBounds();

        var focus = gestures.DoubleClick(
            viewport.Transform.ImageToViewport(
                roiBoundsBeforeFocus.Center));

        assert(
            focus.TransformChanged &&
            viewport.SelectedId == roiId,
            "Double click on an ROI should select it and focus the viewport.");

        var focusedTransform = viewport.Transform;
        assert(
            focusedTransform.GetVisibleImageRectangle().IntersectsWith(roiBoundsBeforeFocus),
            "ROI focus should leave the target ROI inside the visible image region.");

        gestures.ResetGestureState();

        var transformBeforePan = viewport.Transform;
        var blankPoint = new Vector2(
            5,
            5);

        var panDown = gestures.PointerDown(
            blankPoint,
            ViewportMouseButton.Left);

        var panMove = gestures.PointerMove(
            blankPoint + new Vector2(30, 15));

        var panUp = gestures.PointerUp(
            blankPoint + new Vector2(30, 15));

        assert(
            panDown.Kind == ViewportGestureKind.Panning &&
            panMove.Kind == ViewportGestureKind.Panning &&
            panUp.Kind == ViewportGestureKind.Panning &&
            transformBeforePan != viewport.Transform,
            "Dragging empty canvas should use the panning gesture path.");

        var beforeCancelGeometry = viewport.Document.Items
            .Single(item => item.Id == roiId)
            .Geometry;

        var roiCenterAfterPan =
            viewport.Transform.ImageToViewport(beforeCancelGeometry.Center);

        gestures.PointerDown(roiCenterAfterPan);
        gestures.PointerMove(
            roiCenterAfterPan + new Vector2(40, 40));

        var cancelResult = gestures.KeyDown(
            ViewportKey.Escape,
            roiCenterAfterPan + new Vector2(40, 40));

        var afterCancelGeometry = viewport.Document.Items
            .Single(item => item.Id == roiId)
            .Geometry;

        assert(
            cancelResult &&
            !gestures.IsPointerDown &&
            gestures.Kind == ViewportGestureKind.Idle &&
            afterCancelGeometry.Equals(beforeCancelGeometry),
            "Escape should cancel an active ROI edit and restore the committed geometry.");

        viewport.Mode = RoiEditorMode.CreateRectangle;

        var createdBefore = viewport.Document.Count;
        var createPoint = new Vector2(150, 180);

        var createDown = gestures.PointerDown(createPoint);
        gestures.PointerMove(createPoint + new Vector2(80, 50));

        var createCancelled = gestures.KeyDown(
            ViewportKey.Escape,
            createPoint + new Vector2(80, 50));

        assert(
            createDown.Kind == ViewportGestureKind.CreatingRoi &&
            createCancelled &&
            viewport.Document.Count == createdBefore &&
            gestures.Kind == ViewportGestureKind.Idle,
            "Escape during ROI creation should remove the uncommitted ROI.");

        viewport.Mode = RoiEditorMode.Select;

        var middleDown = gestures.PointerDown(
            new Vector2(250, 250),
            ViewportMouseButton.Middle);

        gestures.PointerMove(new Vector2(270, 260));
        gestures.PointerUp(new Vector2(270, 260));

        assert(
            middleDown.Kind == ViewportGestureKind.Panning &&
            !gestures.IsPointerDown,
            "Middle mouse dragging should always route to panning.");

        var snapshot = gestures.Snapshot;

        assert(
            snapshot.Kind == ViewportGestureKind.Idle &&
            !snapshot.IsPointerDown,
            "Completed gestures should return to an idle snapshot.");

        _ = beforeWheel;
    }
}
