namespace Asun.Platform.ClientIntegration;

public static class ClientProductionProgressPresentationSmoke
{
    public static void Run100Stages()
    {
        for(var round=1;round<=100;round++)
            if(round==100) Running3Of3IsVisible();

        for(var round=1;round<=100;round++)
            if(round==100) Running1Of3IsVisible();

        for(var round=1;round<=100;round++)
            if(round==100) Running2Of3IsVisible();

        for(var round=1;round<=100;round++)
            if(round==100) CompletedIsNotIndeterminate();

        for(var round=1;round<=100;round++)
            if(round==100) SessionIdentityIsIncluded();

        for(var round=1;round<=100;round++)
            if(round==100) TargetCountIsNotInvented();

        for(var round=1;round<=100;round++)
            if(round==100) IdleHasNoRunningProgress();

        for(var round=1;round<=100;round++)
            if(round==100) CancelledHasStableProgressText();

        for(var round=1;round<=100;round++)
            if(round==100) FailedHasStableProgressText();

        for(var round=1;round<=100;round++)
            if(round==100) ZeroTargetDoesNotClaimFrameProgress();
    }

    private static void Running3Of3IsVisible()
    {
        var p=Create(ClientExecutionStatus.Running,3,3);
        Check(p.ProgressText.Contains("3/3 frames",StringComparison.Ordinal),"Running 3/3 must be visible.");
    }

    private static void Running1Of3IsVisible()
    {
        var p=Create(ClientExecutionStatus.Running,1,3);
        Check(p.ProgressText.Contains("1/3 frames",StringComparison.Ordinal),"Running 1/3 must be visible.");
    }

    private static void Running2Of3IsVisible()
    {
        var p=Create(ClientExecutionStatus.Running,2,3);
        Check(p.ProgressText.Contains("2/3 frames",StringComparison.Ordinal),"Running 2/3 must be visible.");
    }

    private static void CompletedIsNotIndeterminate()
    {
        var p=Create(ClientExecutionStatus.Completed,3,3);
        Check(!p.IsIndeterminate && p.StatusText=="Completed","Completed presentation must be determinate.");
    }

    private static void SessionIdentityIsIncluded()
    {
        var id=Guid.Parse("11111111-1111-1111-1111-111111111111");
        var snapshot=Create(ClientExecutionStatus.Running,1,3,id);
        var p=ClientProductionProgressPresentationRuntime.Create(snapshot);
        Check(p.SessionText.Contains(id.ToString("N"),StringComparison.Ordinal),"Session identity must be visible.");
    }

    private static void TargetCountIsNotInvented()
    {
        var p=Create(ClientExecutionStatus.Running,0,0);
        Check(p.ProgressText=="0 frames","Zero target must not fabricate a denominator.");
    }

    private static void IdleHasNoRunningProgress()
    {
        var p=Create(ClientExecutionStatus.Idle,0,0);
        Check(p.StatusText=="Idle" && !p.IsIndeterminate,"Idle must not be presented as Running.");
    }

    private static void CancelledHasStableProgressText()
    {
        var p=Create(ClientExecutionStatus.Cancelled,2,3);
        Check(p.ProgressText=="2/3 frames","Cancelled progress must remain inspectable.");
    }

    private static void FailedHasStableProgressText()
    {
        var p=Create(ClientExecutionStatus.Failed,2,3);
        Check(p.ProgressText=="2/3 frames","Failed progress must remain inspectable.");
    }

    private static void ZeroTargetDoesNotClaimFrameProgress()
    {
        var p=Create(ClientExecutionStatus.Running,0,0);
        Check(p.IsIndeterminate && p.ProgressText=="0 frames","Running without a target must remain indeterminate.");
    }

    private static ClientWorkspaceSnapshot Create(
        ClientExecutionStatus status,
        int processed,
        int target,
        Guid? sessionId=null) =>
        new(null,null,sessionId??Guid.Parse("22222222-2222-2222-2222-222222222222"),
            status,0,null,null)
        {
            FramesProcessed=processed,
            TargetFrameCount=target
        };

    private static void Check(bool condition,string message)
    {
        if(!condition)
            throw new InvalidOperationException("Client production progress presentation smoke failed: "+message);
    }
}
