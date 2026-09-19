using System.Numerics;
using Asun.UI.Viewports;

var failures = new List<string>();

void Assert(bool condition, string message)
{
    if (!condition)
        failures.Add(message);
}

static bool Near(float actual, float expected, float tolerance = 1e-4f) =>
    MathF.Abs(actual - expected) <= tolerance;

var editor = new RoiEditorRuntime
{
    Mode = RoiEditorMode.CreateRectangle
};

var down = editor.PointerDown(new Vector2(10, 10));
Assert(
    down.Interaction == RoiInteractionKind.Creating &&
    down.Kind == RoiEditorEventKind.PointerDown,
    "Rectangle creation should enter a creating interaction.");

editor.PointerMove(new Vector2(110, 60));
var created = editor.PointerUp(new Vector2(110, 60));
var rectangle = editor.Geometry;

Assert(
    created.HasCommittedGeometry &&
    rectangle is not null &&
    rectangle.Kind == RoiShapeKind.Rectangle &&
    Near(rectangle.Center.X, 60f) &&
    Near(rectangle.Center.Y, 35f) &&
    Near(rectangle.Size.X, 100f) &&
    Near(rectangle.Size.Y, 50f),
    "Rectangle creation should produce the expected committed geometry.");

var rectangleHit = RoiHitTester.HitTest(
    rectangle!,
    rectangle!.Center,
    handleTolerance: 4f);

Assert(
    rectangleHit.Hit &&
    rectangleHit.Handle == RoiHandleKind.Body,
    "Hit testing should recognize the ROI body when no control handle is closer.");

var topLeft = rectangle.GetControlPoints()
    .Single(control => control.Kind == RoiHandleKind.TopLeft);

var resizeDown = editor.PointerDown(topLeft.Position, handleTolerance: 2f);
Assert(
    resizeDown.Interaction == RoiInteractionKind.Resizing &&
    resizeDown.Handle == RoiHandleKind.TopLeft,
    "Corner handle should start a resize interaction.");

editor.PointerMove(new Vector2(0, 0));
editor.PointerUp(new Vector2(0, 0));
rectangle = editor.Geometry;

Assert(
    rectangle is not null &&
    Near(rectangle.Center.X, 55f) &&
    Near(rectangle.Center.Y, 30f) &&
    Near(rectangle.Size.X, 110f) &&
    Near(rectangle.Size.Y, 60f),
    "Corner resize should keep the opposite corner fixed.");

var committedBeforeMove = editor.CommittedGeometry!;
var moveDown = editor.PointerDown(committedBeforeMove.Center, handleTolerance: 2f);
Assert(
    moveDown.Interaction == RoiInteractionKind.Moving,
    "Body hit should start a move interaction.");

editor.PointerMove(committedBeforeMove.Center + new Vector2(20, 10));
editor.PointerUp(committedBeforeMove.Center + new Vector2(20, 10));

Assert(
    editor.Geometry is not null &&
    Near(editor.Geometry!.Center.X, committedBeforeMove.Center.X + 20f) &&
    Near(editor.Geometry.Center.Y, committedBeforeMove.Center.Y + 10f),
    "Move interaction should translate the ROI by the pointer delta.");

var rotationHandle = editor.Geometry!
    .GetControlPoints()
    .Single(control => control.Kind == RoiHandleKind.Rotation);

var rotationDown = editor.PointerDown(rotationHandle.Position, handleTolerance: 2f);
Assert(
    rotationDown.Interaction == RoiInteractionKind.Rotating &&
    rotationDown.Handle == RoiHandleKind.Rotation,
    "Rotation handle should start a rotate interaction.");

var rotationCenter = editor.Geometry!.Center;
editor.PointerMove(rotationCenter + new Vector2(50, 0));
editor.PointerUp(rotationCenter + new Vector2(50, 0));

Assert(
    editor.Geometry is not null &&
    Near(editor.Geometry!.RotationRadians, MathF.PI / 2f, 1e-3f),
    "Rotation should follow the rotation handle angle.");

var cancelledSource = editor.CommittedGeometry!;
editor.PointerDown(editor.Geometry!.Center, handleTolerance: 2f);
editor.PointerMove(editor.Geometry.Center + new Vector2(100, 100));
editor.Cancel(editor.Geometry.Center + new Vector2(100, 100));

Assert(
    editor.Geometry is not null &&
    editor.Geometry.Equals(cancelledSource),
    "Cancelling an active edit should restore the committed geometry.");

var ellipseEditor = new RoiEditorRuntime
{
    Mode = RoiEditorMode.CreateEllipse
};

ellipseEditor.PointerDown(new Vector2(20, 30));
ellipseEditor.PointerMove(new Vector2(120, 90));
ellipseEditor.PointerUp(new Vector2(120, 90));

Assert(
    ellipseEditor.Geometry is not null &&
    ellipseEditor.Geometry.Kind == RoiShapeKind.Ellipse &&
    ellipseEditor.Geometry.Contains(ellipseEditor.Geometry.Center),
    "Ellipse creation should produce a valid editable ellipse.");

var polygon = RoiGeometry.CreatePolygon(new[]
{
    new Vector2(0, 0),
    new Vector2(100, 0),
    new Vector2(50, 80)
});

Assert(
    polygon.IsValid &&
    polygon.Contains(new Vector2(50, 20)) &&
    !polygon.Contains(new Vector2(150, 20)),
    "Polygon ROI should validate and perform point containment.");

var polygonEditor = new RoiEditorRuntime();
polygonEditor.SetGeometry(polygon);
var vertex = polygon.GetControlPoints()
    .Single(control => control.Kind == RoiHandleKind.Vertex && control.Index == 1);

var polygonDown = polygonEditor.PointerDown(vertex.Position, handleTolerance: 2f);
Assert(
    polygonDown.Interaction == RoiInteractionKind.Resizing &&
    polygonDown.Handle == RoiHandleKind.Vertex &&
    polygonDown.Kind == RoiEditorEventKind.PointerDown,
    "Polygon vertex hit should enter vertex editing.");

polygonEditor.PointerMove(new Vector2(120, 0));
polygonEditor.PointerUp(new Vector2(120, 0));

Assert(
    polygonEditor.Geometry is not null &&
    polygonEditor.Geometry.Vertices[1] == new Vector2(120, 0),
    "Polygon vertex editing should replace the selected vertex.");

var teaching = new RoiTeachingGuide(new[]
{
    new RoiTeachingStep(
        "press",
        "Press to create the ROI.",
        RoiTeachingAction.PointerDown,
        RoiInteractionKind.Creating),
    new RoiTeachingStep(
        "drag",
        "Drag to size the ROI.",
        RoiTeachingAction.PointerMove,
        RoiInteractionKind.Creating),
    new RoiTeachingStep(
        "release",
        "Release to commit the ROI.",
        RoiTeachingAction.PointerUp,
        RoiInteractionKind.Creating)
});

teaching.Start(restart: true);

var teachingEditor = new RoiEditorRuntime
{
    Mode = RoiEditorMode.CreateRectangle
};

var teachingDown = teachingEditor.PointerDown(new Vector2(5, 5));
Assert(
    teaching.Observe(teachingDown) &&
    teaching.Snapshot.CurrentStepIndex == 1,
    "Teaching guide should advance after the expected pointer-down event.");

var teachingMove = teachingEditor.PointerMove(new Vector2(50, 40));
Assert(
    teaching.Observe(teachingMove) &&
    teaching.Snapshot.CurrentStepIndex == 2,
    "Teaching guide should advance after the expected drag event.");

var teachingUp = teachingEditor.PointerUp(new Vector2(50, 40));
Assert(
    teaching.Observe(teachingUp) &&
    teaching.Snapshot.IsActive &&
    teaching.Snapshot.CurrentStepIndex == 0,
    "Repeatable teaching sequences should restart automatically after completion.");

teaching.Stop();
Assert(
    !teaching.Snapshot.IsActive &&
    teaching.Snapshot.CurrentStepIndex == 0,
    "Stopping the teaching guide should preserve the current step.");

teaching.Reset();
Assert(
    !teaching.Snapshot.IsActive &&
    teaching.Snapshot.CurrentStepIndex == 0 &&
    teaching.Snapshot.CurrentStep is not null,
    "Reset should restore the first teaching step.");

RoiDocumentRuntimeSmoke.Run(Assert);

RoiViewportRuntimeSmoke.Run(Assert);

ViewportGestureRuntimeSmoke.Run(Assert);

ViewportTenChainSmoke.Run(Assert);

await ViewportFiftyChainSmoke.RunAsync(Assert);

AdvancedFiveHundredChainSmoke.Run(Assert);

ViewportFiveHundredChainsSmoke.Run(Assert);

ViewportFiveHundredWorkflowSmoke.Run(Assert);

await ViewportCompositeSmoke.RunAsync(Assert);

await ViewportRenderPipelineSmoke.RunAsync(Assert);

await ViewportRenderBudgetSmoke.RunAsync(Assert);

await ViewportRenderPrioritySmoke.RunAsync(Assert);

await ViewportRenderReuseSmoke.RunAsync(Assert);

await ViewportPresentationChainSmoke.RunAsync(Assert);

await ViewportPresentationFacadeSmoke.RunAsync(Assert);

await ViewportDeliveryAndBackpressureSmoke.RunAsync(Assert);
await ViewportContinuousAndDeferredSmoke.RunAsync(Assert);
await ViewportRenderSurfaceSmoke.RunAsync(Assert);
ViewportPresentationBufferSmoke.Run(Assert);
await ViewportPresentationQueueSmoke.RunAsync(Assert);
await ViewportPresentationExecutionSmoke.RunAsync(Assert);
await ViewportPresentationEndToEndSmoke.RunAsync(Assert);
await ViewportInvariantSmoke.RunAsync(Assert);
ViewportNavigationFeatureSmoke.Run(Assert);
ViewportWorkflowSmoke.Run(Assert);
ViewportSceneVisibilitySmoke.Run(Assert);
await ViewportEvidenceSmoke.RunAsync(Assert);
await ViewportInputEdgeSmoke.RunAsync(Assert);
await ViewportRenderPlanSmoke.RunAsync(Assert);
await ViewportDiagnosticsManifestSmoke.RunAsync(Assert);

await ViewportPresentationLifecycleSmoke.RunAsync(Assert);

await ViewportPresentationDisposeSmoke.RunAsync(Assert);

ViewportRenderSchedulerGenerationSmoke.Run(Assert);

await ViewportInputLifecycleSmoke.RunAsync(Assert);

if (failures.Count > 0)
{
    foreach (var failure in failures)
        Console.Error.WriteLine($"FAIL: {failure}");

    return 1;
}

Console.WriteLine("Asun.UI.Viewports ROI smoke tests passed.");
return 0;
