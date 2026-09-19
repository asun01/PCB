using System.Drawing;
using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportTenChainSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var first = RoiGeometry.CreateRectangle(new Vector2(20, 20), new Vector2(10, 10));
        var second = RoiGeometry.CreateEllipse(new Vector2(180, 20), new Vector2(20, 16));
        var third = RoiGeometry.CreateRectangle(new Vector2(80, 100), new Vector2(40, 20));

        var items = new[]
        {
            new RoiDocumentItem(Guid.NewGuid(), first, 0),
            new RoiDocumentItem(Guid.NewGuid(), second, 1),
            new RoiDocumentItem(Guid.NewGuid(), third, 2)
        };

        // 1. Spatial index ----------------------------------------------------
        var index = new RoiSpatialIndex(32);
        index.Rebuild(items);

        assert(
            index.CellCount > 0 &&
            index.QueryPoint(new Vector2(180, 20)).Count == 1,
            "Spatial index should rebuild and find point-contained ROIs.");

        var rectangleCandidates = index.QueryRectangle(
            RectangleF.FromLTRB(0, 0, 220, 60));

        assert(
            rectangleCandidates.Count == 2 &&
            rectangleCandidates[0].ZIndex > rectangleCandidates[1].ZIndex,
            "Spatial rectangle queries should deduplicate candidates and preserve z-order.");

        var removedId = items[0].Id;
        assert(
            index.Remove(removedId) &&
            index.QueryPoint(new Vector2(20, 20)).Count == 0,
            "Spatial index removal should remove the ROI from all covered cells.");

        index.Add(items[0]);

        // 2. Multi-selection / marquee --------------------------------------
        var selection = new RoiSelectionRuntime();
        assert(
            selection.Select(new[] { items[0].Id }) &&
            selection.Select(new[] { items[1].Id }, RoiSelectionMode.Add) &&
            selection.SelectedIds.Count == 2,
            "Selection runtime should support replace and additive selection.");

        selection.Select(
            new[] { items[0].Id },
            RoiSelectionMode.Toggle);

        assert(
            !selection.SelectedIds.Contains(items[0].Id) &&
            selection.SelectedIds.Contains(items[1].Id),
            "Toggle selection should invert only the requested ROI.");

        selection.BeginMarquee(new Vector2(0, 0));
        selection.UpdateMarquee(new Vector2(120, 130));

        assert(
            selection.CompleteMarquee(items) &&
            selection.SelectedIds.Count == 2 &&
            selection.SelectedIds.Contains(items[0].Id) &&
            selection.SelectedIds.Contains(items[2].Id),
            "Marquee selection should select intersecting ROIs in one transaction.");

        // 3. Constraint / snap pipeline --------------------------------------
        var constrained = RoiConstraintRuntime.Apply(
            RoiGeometry.CreateRotatedRectangle(
                new Vector2(107, -4),
                new Vector2(100, 10),
                0.2f),
            new RoiConstraintProfile(
                new Vector2(20, 20),
                new Vector2(60, 60),
                true,
                new Vector2(10, 10),
                true,
                true),
            new Vector2(120, 80));

        assert(
            constrained.Size.X <= 60f &&
            constrained.Size.X >= 20f &&
            constrained.Size.Y <= 60f &&
            constrained.Size.Y >= 20f &&
            constrained.GetBounds().Left >= -1e-4f &&
            constrained.GetBounds().Top >= -1e-4f,
            "Constraint pipeline should clamp size, preserve ratio, snap, and keep the ROI in bounds.");

        var snapped = RoiConstraintRuntime.Snap(
            new Vector2(23, 27),
            new Vector2(10, 10));

        assert(
            snapped == new Vector2(20, 30),
            "Grid snapping should produce deterministic coordinates.");

        // 4. Polygon topology editing ----------------------------------------
        var polygon = RoiGeometry.CreatePolygon(new[]
        {
            new Vector2(0, 0),
            new Vector2(100, 0),
            new Vector2(100, 100),
            new Vector2(0, 100)
        });

        var edgeHit = RoiPolygonEditorRuntime.FindClosestEdge(
            polygon,
            new Vector2(50, 3));

        var inserted = RoiPolygonEditorRuntime.InsertVertex(
            polygon,
            edgeHit.ClosestPoint,
            maximumEdgeDistance: 5);

        var removed = RoiPolygonEditorRuntime.RemoveVertex(inserted, 1);
        var reversed = RoiPolygonEditorRuntime.Reverse(removed);

        assert(
            inserted.Vertices.Count == 5 &&
            removed.Vertices.Count == 4 &&
            reversed.IsValid &&
            reversed.Vertices.Count == 4,
            "Polygon topology runtime should support edge insertion, removal, and winding reversal.");

        // 5. Coordinate readout / grid overlay -------------------------------
        var transform = ViewportTransform.Create(
            new Vector2(1000, 500),
            new Vector2(500, 400),
            0.5,
            new Vector2(20, 30));

        var overlay = new ViewportCoordinateOverlayRuntime(transform);
        var readout = overlay.GetReadout(new Vector2(270, 180), 2);
        var crosshair = overlay.GetCrosshair(new Vector2(270, 180));
        var grid = overlay.BuildGrid(50);

        assert(
            readout.ImagePoint == new Vector2(500, 300) &&
            readout.XText == "500.00" &&
            readout.YText == "300.00" &&
            crosshair.HorizontalStart.X == 0 &&
            crosshair.HorizontalEnd.X == 500 &&
            grid.Count > 0,
            "Coordinate overlay should provide deterministic readouts, crosshair and visible grid lines.");

        var bar = ViewportCoordinateOverlayRuntime.ChooseScaleBarLength(
            visibleImageWidth: 500,
            targetScreenPixels: 100,
            scale: 0.5);

        assert(
            bar > 0 &&
            double.IsFinite(bar),
            "Scale-bar selection should return a finite human-readable length.");

        // 6. Mini-map navigation --------------------------------------------
        var miniMap = ViewportMiniMapRuntime.CreateSnapshot(
            transform,
            new Vector2(220, 140));

        var miniPoint = new Vector2(110, 70);
        var imageFromMini = ViewportMiniMapRuntime.MiniMapToImage(
            miniMap,
            miniPoint);

        var centered = ViewportMiniMapRuntime.CenterMainViewportOnMiniMapPoint(
            transform,
            miniMap,
            miniPoint);

        assert(
            miniMap.ViewportRectangle.Width > 0 &&
            miniMap.ViewportRectangle.Height > 0 &&
            Vector2.Distance(
                centered.ViewportToImage(centered.ViewportCenter),
                imageFromMini) < 1e-4f,
            "Mini-map navigation should project the viewport and center the main view on the clicked image point.");

        // 7. Navigation history ---------------------------------------------
        var history = new ViewportNavigationHistory(transform, capacity: 3);
        var moved = transform.PanBy(new Vector2(20, 10));
        var zoomed = moved.WithZoomFactor(2, new Vector2(250, 200));

        history.Record(moved);
        history.Record(zoomed);

        assert(
            history.CanBack &&
            history.Back(out var back) &&
            back == moved &&
            history.CanForward,
            "Viewport navigation history should support backward traversal.");

        assert(
            history.Forward(out var forward) &&
            forward == zoomed &&
            !history.CanForward,
            "Viewport navigation history should restore forward state.");

        history.Clear();

        assert(
            !history.CanBack && !history.CanForward &&
            history.Current == zoomed,
            "Clearing history should preserve the current viewport state.");

        // 8. Render command planning ----------------------------------------
        var roiViewport = new RoiViewportRuntime(
            new Vector2(1000, 500),
            new Vector2(500, 500));

        var renderId = roiViewport.Document.Add(
            RoiGeometry.CreateRectangle(
                new Vector2(500, 250),
                new Vector2(100, 60)));
        roiViewport.Document.Select(renderId);

        var renderPoint = roiViewport.Transform.ImageToViewport(
            new Vector2(500, 250));
        roiViewport.PointerMove(renderPoint);

        var renderSnapshot = roiViewport.CreateSnapshot();
        var commands = RoiRenderCommandBuilder.Build(renderSnapshot);

        assert(
            commands.Count >= 10 &&
            commands.Any(command =>
                command.Kind == RoiRenderCommandKind.Handle &&
                command.RoiId == renderId) &&
            commands.Any(command =>
                command.Kind == RoiRenderCommandKind.RotationHandle &&
                command.RoiId == renderId),
            "Render command builder should emit fill, outline, handles and rotation-handle commands for selected ROIs.");

        // 9. Pointer coalescing ----------------------------------------------
        var coalescer = new ViewportPointerCoalescer();
        coalescer.Submit(new Vector2(1, 1));
        coalescer.Submit(new Vector2(2, 2));
        coalescer.Submit(new Vector2(3, 3));

        assert(
            coalescer.SubmittedCount == 3 &&
            coalescer.CoalescedCount == 2 &&
            coalescer.HasPending,
            "Pointer coalescer should retain only the newest pending move.");

        assert(
            coalescer.TryTakeLatest(out var latest) &&
            latest.Sequence == 3 &&
            latest.Position == new Vector2(3, 3) &&
            !coalescer.HasPending,
            "Pointer coalescer should drain the latest sequence deterministically.");

        // 10. Multi-ROI group transformation -------------------------------
        var groupResult = RoiGroupTransformRuntime.Translate(
            new[] { items[0], items[2] },
            new Vector2(10, 20));

        assert(
            groupResult.Items.Count == 2 &&
            groupResult.Items[0].Geometry.Center ==
                items[0].Geometry.Center + new Vector2(10, 20) &&
            groupResult.Items[1].Geometry.Center ==
                items[2].Geometry.Center + new Vector2(10, 20),
            "Group translation should move every selected ROI by the same delta.");

        var scaled = RoiGroupTransformRuntime.Scale(
            new[] { items[0], items[2] },
            new Vector2(2, 2));

        assert(
            scaled.Items.Count == 2 &&
            scaled.Bounds.Width > 0 &&
            scaled.Bounds.Height > 0 &&
            scaled.Pivot != Vector2.Zero,
            "Group scaling should transform all items around a stable group pivot.");
    }
}
