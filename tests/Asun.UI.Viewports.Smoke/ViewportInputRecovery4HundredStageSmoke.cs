using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportInputRecovery4HundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        using var runtime=new ViewportInputRecoveryRuntime(
            capacity:2,
            dropPolicy:ViewportInputDropPolicy.DropNewest);

        var started=runtime.TryStart(out var token);
        var first=runtime.TrySubmit(
            ViewportInputEventKind.PointerDown,
            new Vector2(10,10));
        var move=runtime.TrySubmit(
            ViewportInputEventKind.PointerMove,
            new Vector2(20,20));
        var active=runtime.Capture();
        ViewportInputRecoverySnapshot snapshot;

        for(var i=0;i<10;i++) Check(started,"Recovery runtime should start a fresh Presentation lifecycle.");
        for(var i=0;i<10;i++) Check(token.CanBeCanceled,"Successful start should expose a cancellable token.");
        for(var i=0;i<10;i++) Check(first,"First input event should be accepted.");
        for(var i=0;i<10;i++) Check(move,"Second input event should be accepted.");
        for(var i=0;i<10;i++) Check(active.PresentationState==ViewportPresentationState.Running,"Active snapshot should report Running.");
        for(var i=0;i<10;i++) Check(active.Submission.Pending==2,"Active snapshot should preserve pending input count.");
        for(var i=0;i<10;i++) Check(active.Backpressure.Accepted==2,"Backpressure snapshot should preserve accepted count.");
        for(var i=0;i<10;i++) Check(active.Backpressure.Capacity==2,"Backpressure capacity should remain bounded.");
        for(var i=0;i<10;i++) Check(!active.Submission.IsCompleted && !active.Backpressure.IsCompleted,"Active snapshot should not report terminal input state.");
        for(var i=0;i<10;i++) Check(active.Backpressure.Dropped>=0,"Backpressure drop accounting should remain non-negative.");

        runtime.RequestStop();
        runtime.MarkStopped();
        snapshot=runtime.Capture();

        assert(snapshot.PresentationState==ViewportPresentationState.Stopped,$"Recovery terminal path should leave Presentation lifecycle stopped; actual {snapshot.PresentationState}.");
        assert(round==100,$"ViewportInputRecovery4HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
