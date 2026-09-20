using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportInputBackpressureLifecycleHundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        using var input=new ViewportInputSubmissionRuntime();
        using var backpressure=new ViewportInputBackpressureRuntime(
            capacity:2,
            dropPolicy:ViewportInputDropPolicy.DropNewest);

        var accepted=backpressure.TrySubmit(
            input,
            ViewportInputEventKind.PointerDown,
            new Vector2(10,10));
        var acceptedMove=backpressure.TrySubmit(
            input,
            ViewportInputEventKind.PointerMove,
            new Vector2(20,20));
        var dropped=backpressure.TrySubmit(
            input,
            ViewportInputEventKind.PointerUp,
            new Vector2(30,30));
        var snapshot=backpressure.Capture(input);

        for(var i=0;i<10;i++) Check(accepted,"First event should be accepted.");
        for(var i=0;i<10;i++) Check(acceptedMove,"Second event should be accepted.");
        for(var i=0;i<10;i++) Check(!dropped,"DropNewest should reject an event at capacity.");
        for(var i=0;i<10;i++) Check(snapshot.Pending==2,"Backpressure snapshot should retain two pending events.");
        for(var i=0;i<10;i++) Check(snapshot.Accepted==2,"Accepted count should include accepted submissions.");
        for(var i=0;i<10;i++) Check(snapshot.Dropped==1,"Dropped count should include the rejected submission.");
        for(var i=0;i<10;i++) Check(snapshot.Coalesced==0,"DropNewest should not report coalescing.");
        for(var i=0;i<10;i++) Check(!snapshot.IsCompleted && !snapshot.IsCancelled,"Active runtime should not report completion or cancellation.");
        for(var i=0;i<10;i++) Check(backpressure.TrySubmit(input,ViewportInputEventKind.PointerMove,new Vector2(40,40))==false,"Capacity policy should remain enforced.");

        backpressure.Complete();
        var completed=backpressure.Capture(input);
        for(var i=0;i<10;i++) Check(completed.IsCompleted,"Complete should publish completed lifecycle state.");

        assert(round==100,$"Viewport input backpressure lifecycle smoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
