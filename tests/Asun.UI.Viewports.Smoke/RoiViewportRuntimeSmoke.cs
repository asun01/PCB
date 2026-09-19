using System.Numerics;
using Asun.UI.Viewports;

public static class RoiViewportRuntimeSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var runtime = new RoiViewportRuntime(
            imageSize: new Vector2(1000, 500),
            viewportSize: new Vector2(500, 500));

        runtime.Mode = RoiEditorMode.CreateRectangle;

        var fit = runtime.Transform;
        var createStart = new Vector2(100, 150);
        var createEnd = new Vector2(300, 250);

        var down = runtime.PointerDown(createStart);
        runtime.PointerMove(createEnd);
        var up = runtime.PointerUp(createEnd);

        var createdId = up.DocumentEvent.RoiId;
        assert(
            down.ImagePoint == fit.ViewportToImage(createStart) &&
            createdId is not null &&
            runtime.Document.Count == 1,
            "Viewport ROI bridge should convert pointer coordinates and create one ROI.");

        var imageGeometry =
            runtime.Document.Items.Single(item => item.Id == createdId!.Value).Geometry;

        var expectedCenter = (down.ImagePoint + up.ImagePoint) / 2f;
        assert(
            imageGeometry.Kind == RoiShapeKind.Rectangle &&
            imageGeometry.Center == expectedCenter,
            "Created ROI geometry should remain expressed in image coordinates.");

        runtime.Mode = RoiEditorMode.Select;

        var centerViewport = runtime.Transform.ImageToViewport(imageGeometry.Center);
        var hit = runtime.HitTestViewport(
            centerViewport,
            handleTolerancePixels: 6f);

        assert(
            hit.Id == createdId &&
            hit.Hit.Handle == RoiHandleKind.Body,
            "Viewport hit testing should identify the image-space ROI through screen coordinates.");

        runtime.PointerDown(centerViewport, handleTolerancePixels: 6f);
        var movedCenterViewport = centerViewport + new Vector2(50, 25);
        runtime.PointerMove(movedCenterViewport, handleTolerancePixels: 6f);
        runtime.PointerUp(movedCenterViewport);

        var movedGeometry =
            runtime.Document.Items.Single(item => item.Id == createdId!.Value).Geometry;

        assert(
            movedGeometry.Center == imageGeometry.Center +
                new Vector2(
                    50f / (float)runtime.Transform.Scale,
                    25f / (float)runtime.Transform.Scale),
            "Pointer movement in viewport pixels should translate ROI by the inverse zoom scale.");

        var beforeZoomImagePoint = runtime.Transform.ViewportToImage(centerViewport);
        runtime.ZoomAt(
            zoomFactor: 2,
            minScale: 0.1,
            maxScale: 20,
            viewportAnchor: centerViewport);

        var afterZoomImagePoint = runtime.Transform.ViewportToImage(centerViewport);

        assert(
            Vector2.Distance(beforeZoomImagePoint, afterZoomImagePoint) < 1e-4f,
            "Zoom should preserve the image point beneath the viewport anchor.");

        var zoomedHit = runtime.HitTestViewport(
            movedCenterViewport,
            handleTolerancePixels: 8f);

        assert(
            zoomedHit.Id == createdId &&
            zoomedHit.Hit.Hit,
            "ROI hit testing should remain stable after zoom.");

        var snapshot = runtime.CreateSnapshot();

        assert(
            snapshot.Items.Count == 1 &&
            snapshot.Items[0].Id == createdId &&
            snapshot.Items[0].Geometry.Center ==
                runtime.Transform.ImageToViewport(movedGeometry.Center),
            "Render snapshots should expose projected ROI geometry in viewport coordinates.");

        assert(
            snapshot.Items[0].ControlPoints.Count >= 8 &&
            snapshot.Items[0].Bounds.Width > 0 &&
            snapshot.Items[0].Bounds.Height > 0,
            "Projected ROI snapshots should contain UI-ready handles and bounds.");

        assert(
            runtime.Undo() &&
            runtime.Document.Items.Single(item => item.Id == createdId!.Value).Geometry
                .Equals(imageGeometry),
            "Viewport runtime should delegate undo to the document transaction layer.");

        assert(
            runtime.Redo() &&
            runtime.Document.Items.Single(item => item.Id == createdId!.Value).Geometry
                .Equals(movedGeometry),
            "Viewport runtime should delegate redo to the document transaction layer.");

        runtime.PanBy(new Vector2(-40, 30));
        var pannedSnapshot = runtime.CreateSnapshot();

        assert(
            pannedSnapshot.Items[0].Bounds.Width > 0 &&
            pannedSnapshot.Items[0].Bounds.Height > 0 &&
            pannedSnapshot.ViewportPointer is not null,
            "Panning should update projected render state without changing image-space ROI geometry.");
    }
}
