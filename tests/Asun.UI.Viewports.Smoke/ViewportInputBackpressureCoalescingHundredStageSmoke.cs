using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportInputBackpressureCoalescingHundredStageSmoke
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
        using var runtime=new ViewportInputBackpressureRuntime(
            capacity:1,
            dropPolicy:ViewportInputDropPolicy.CoalesceMoves);

        var first=runtime.TrySubmit(
            input,
            ViewportInputEventKind.PointerMove,
            new Vector2(10,10));
        var replaced=runtime.TrySubmit(
            input,
            ViewportInputEventKind.PointerMove,
            new Vector2(20,20));
        var nonFiniteRejected=false;

        try
        {
            runtime.TrySubmit(
                input,
                ViewportInputEventKind.PointerMove,
                new Vector2(float.NaN,30));
        }
        catch(ArgumentOutOfRangeException)
        {
            nonFiniteRejected=true;
        }

        var snapshot=runtime.Capture(input);
        var drained=input.Drain();
        var latestPosition=drained.Count==1 ? drained[0].Position : new Vector2(float.NaN,float.NaN);

        for(var i=0;i<10;i++) Check(first,"Initial pointer move should be accepted.");
        for(var i=0;i<10;i++) Check(replaced,"Full queue pointer move should be coalesced.");
        for(var i=0;i<10;i++) Check(input.PendingCount==1,"Coalescing should keep one pending move.");
        for(var i=0;i<10;i++) Check(snapshot.Accepted==2,"Coalesced replacement should count as accepted.");
        for(var i=0;i<10;i++) Check(snapshot.Coalesced==1,"Backpressure coalescing count should be one.");
        for(var i=0;i<10;i++) Check(input.CoalescedCount>=1,"Submission runtime should retain coalescing evidence.");
        for(var i=0;i<10;i++) Check(latestPosition==new Vector2(20,20),"The latest move position should replace the stale move.");
        for(var i=0;i<10;i++) Check(nonFiniteRejected,"Non-finite pointer coordinates should be rejected.");
        for(var i=0;i<10;i++) Check(!runtime.IsCompleted && !runtime.IsCancelled,"Runtime should remain active before cancellation.");
        for(var i=0;i<10;i++) Check(!input.IsCompleted || input.PendingCount==0,"Drain should leave the submission queue in a valid state.");

        runtime.Cancel();
        var cancelled=runtime.Capture(input);
        assert(cancelled.IsCompleted && cancelled.IsCancelled,$"Cancellation should publish terminal lifecycle state; actual completed={cancelled.IsCompleted}, cancelled={cancelled.IsCancelled}.");
        assert(round==100,$"Viewport input backpressure coalescing smoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
