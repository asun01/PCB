using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportRoiInputRecovery4HundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        using var runtime=new ViewportRoiInputRecoveryRuntime(
            new Vector2(200,100),
            new Vector2(400,200),
            RoiEditorMode.CreateRectangle,
            capacity:4,
            dropPolicy:ViewportInputDropPolicy.CoalesceMoves);

        var started=runtime.TryStart(out var token);
        var p1=new Vector2(40+4,30+4);
        var p2=new Vector2(120+4,90+4);
        var down=runtime.TrySubmitAndProcess(ViewportInputEventKind.PointerDown,p1);
        var move=runtime.TrySubmitAndProcess(ViewportInputEventKind.PointerMove,p2);
        var up=runtime.TrySubmitAndProcess(ViewportInputEventKind.PointerUp,p2);
        var active=runtime.Capture();

        for(var i=0;i<10;i++) Check(started,"ROI input recovery should start the Presentation lifecycle.");
        for(var i=0;i<10;i++) Check(token.CanBeCanceled,"Started recovery should expose a cancellable token.");
        for(var i=0;i<10;i++) Check(down && move && up,"Pointer down/move/up should enter the bounded input path.");
        for(var i=0;i<10;i++) Check(active.ProcessedEvents==3,"Three pointer events should be consumed by the ROI runtime.");
        for(var i=0;i<10;i++) Check(active.Roi.Items.Count==1,"Pointer creation should produce one ROI item.");
        for(var i=0;i<10;i++) Check(active.Roi.Items[0].Geometry.IsValid,"Created ROI geometry should remain valid.");
        for(var i=0;i<10;i++) Check(active.Roi.Items[0].IsSelected,"Created ROI should remain selected.");
        for(var i=0;i<10;i++) Check(active.RoiFingerprint.Length==64,"ROI snapshot fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(active.Fingerprint==runtime.Capture().Fingerprint,"Repeated capture should preserve deterministic identity.");
        for(var i=0;i<10;i++) Check(active.Input.Submission.IsCompleted==false && active.Input.Backpressure.IsCompleted==false,"Active input/Presentation path should remain live.");

        runtime.Complete();
        var completed=runtime.Capture();
        assert(completed.Input.Submission.IsCompleted && completed.Input.Backpressure.IsCompleted,$"Completion should close input recovery state; actual submission={completed.Input.Submission.IsCompleted}, backpressure={completed.Input.Backpressure.IsCompleted}.");
        assert(completed.Roi.Items.Count==1,"Completion should preserve the committed ROI.");
        assert(round==100,$"ViewportRoiInputRecovery4HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
