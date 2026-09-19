using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportSceneVisibilitySmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var runtime = new RoiViewportRuntime(
            new Vector2(1000, 1000),
            new Vector2(400, 300));

        var visibleId = runtime.Document.Add(
            RoiGeometry.CreateRectangle(
                new Vector2(120, 100),
                new Vector2(80, 50)));

        var hiddenId = runtime.Document.Add(
            RoiGeometry.CreateRectangle(
                new Vector2(850, 850),
                new Vector2(80, 50)));

        runtime.Document.Select(visibleId);

        var firstSnapshot = runtime.CreateSnapshot();
        var firstScene = ViewportSceneRuntime.Build(firstSnapshot);
        var firstValidation = ViewportInvariantRuntime.ValidateSceneSnapshot(firstScene);

        assert(
            firstScene.Commands.Count > 0 &&
            firstValidation.Count == 0,
            "The first ROI viewport scene should produce valid framework-neutral scene commands.");

        assert(
            firstScene.Commands.Any(command =>
                command.RoiId == visibleId &&
                command.Kind == ViewportSceneCommandKind.RoiSelectionBounds),
            "Selecting a ROI should add a selection-bounds scene command for the same ROI.");

        var visibleRegion = ViewportVisibleRegionRuntime.Capture(
            firstSnapshot.Transform,
            new Vector2(100, 100));

        assert(
            ViewportInvariantRuntime.ValidateVisibleRegion(visibleRegion).Count == 0 &&
            visibleRegion.Tiles.Count > 0,
            "Visible-region capture should expose a non-empty deterministic tile set.");

        var visibleIds = firstSnapshot.Items
            .Where(item => item.Bounds.IntersectsWith(
                new System.Drawing.RectangleF(
                    0,
                    0,
                    firstSnapshot.Transform.ViewportSize.X,
                    firstSnapshot.Transform.ViewportSize.Y)))
            .Select(item => item.Id)
            .ToHashSet();

        assert(
            visibleIds.Contains(visibleId) &&
            !visibleIds.Contains(hiddenId),
            "Viewport ROI projection should distinguish visible and off-viewport document items.");

        runtime.Document.TranslateSelected(new Vector2(15, 10));

        var secondSnapshot = runtime.CreateSnapshot();
        var secondScene = ViewportSceneRuntime.Build(secondSnapshot);
        var moveDiff = ViewportSceneDiffRuntime.Diff(
            firstScene,
            secondScene);

        assert(
            moveDiff.Any(diff =>
                diff.RoiId == visibleId &&
                diff.Kind == ViewportSceneDiffKind.Changed),
            "Moving a ROI should produce a concrete Changed scene diff.");

        assert(
            moveDiff.All(diff =>
                diff.Kind != ViewportSceneDiffKind.TransformChanged),
            "A pure ROI move should not create a transform diff.");

        runtime.Document.Select(null);

        var thirdScene = ViewportSceneRuntime.Build(
            runtime.CreateSnapshot());

        var selectionDiff = ViewportSceneDiffRuntime.Diff(
            secondScene,
            thirdScene);

        assert(
            selectionDiff.Any(diff =>
                diff.RoiId == visibleId &&
                diff.Kind == ViewportSceneDiffKind.SelectionChanged),
            "Changing ROI selection should produce a SelectionChanged scene diff.");

        runtime.ResizeViewport(new Vector2(500, 320));

        var fourthScene = ViewportSceneRuntime.Build(
            runtime.CreateSnapshot());

        var transformDiff = ViewportSceneDiffRuntime.Diff(
            thirdScene,
            fourthScene);

        assert(
            transformDiff.Any(diff =>
                diff.Kind == ViewportSceneDiffKind.TransformChanged &&
                diff.RoiId == Guid.Empty),
            "Viewport resize should produce an explicit transform scene diff without inventing an ROI owner.");

        var sameSceneDiff = ViewportSceneDiffRuntime.Diff(
            fourthScene,
            fourthScene);

        assert(
            sameSceneDiff.Count == 0,
            "Diffing an identical scene against itself should produce no work.");

        var commandEvidence = ViewportRenderEvidenceRuntime.ComputeTextHash(
            string.Join(
                "
",
                fourthScene.Commands.Select(command => command.ToString())));

        assert(
            commandEvidence.Length == 64,
            "Scene command evidence should have a deterministic fixed-length digest.");

        var geometry = RoiGeometry.CreatePolygon(
            new[]
            {
                new Vector2(20, 20),
                new Vector2(90, 20),
                new Vector2(60, 80)
            });

        var polygonId = runtime.Document.Add(geometry);
        runtime.Document.Select(polygonId);

        var polygonScene = ViewportSceneRuntime.Build(
            runtime.CreateSnapshot());

        assert(
            polygonScene.Commands.Any(command =>
                command.RoiId == polygonId &&
                command.Handle == RoiHandleKind.Vertex),
            "Polygon ROI scene projection should expose vertex control-point commands.");

        assert(
            ViewportInvariantRuntime.ValidateSceneDiff(
                moveDiff).Count == 0 &&
            ViewportInvariantRuntime.ValidateSceneDiff(
                selectionDiff).Count == 0 &&
            ViewportInvariantRuntime.ValidateSceneDiff(
                transformDiff).Count == 0,
            "All generated scene diffs should satisfy structural invariants.");

        var syntheticRemoved = ViewportSceneDiffRuntime.Diff(
            polygonScene,
            fourthScene);

        assert(
            syntheticRemoved.Any(diff =>
                diff.Kind == ViewportSceneDiffKind.Removed &&
                diff.RoiId == polygonId),
            "Removing a ROI from the scene should expose a Removed scene diff.");
    }
}
