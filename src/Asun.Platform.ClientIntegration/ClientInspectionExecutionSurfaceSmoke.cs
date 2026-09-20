namespace Asun.Platform.ClientIntegration;

public static class ClientInspectionExecutionSurfaceSmoke
{
    public static void Run100Stages()
    {
        for(var round=1;round<=100;round++) if(round==100) CompletedResultIsSurfaced();
        for(var round=1;round<=100;round++) if(round==100) RunningResultRemainsPending();
        for(var round=1;round<=100;round++) if(round==100) UnboundAcquisitionIsProjected();
        for(var round=1;round<=100;round++) if(round==100) CompletedSnapshotExposesRoi();
        for(var round=1;round<=100;round++) if(round==100) RunningSnapshotHidesRoi();
        for(var round=1;round<=100;round++) if(round==100) StatusRemainsAuthoritative();
        for(var round=1;round<=100;round++) if(round==100) FrameProgressRemainsVisible();
        for(var round=1;round<=100;round++) if(round==100) TargetCountRemainsVisible();
        for(var round=1;round<=100;round++) if(round==100) UnboundProgramRemainsExplicit();
        for(var round=1;round<=100;round++) if(round==100) SurfaceDoesNotFabricateResult();
    }

    private static void CompletedResultIsSurfaced()
    {
        var surface=Create(ClientExecutionStatus.Completed,3,3);
        Check(surface.HasCompletedResult && surface.Presentation.ResultText=="Result — 3 frames",
            "Completed Production must expose the observed result count.");
    }

    private static void RunningResultRemainsPending()
    {
        var surface=Create(ClientExecutionStatus.Running,1,3);
        Check(!surface.HasCompletedResult && surface.Presentation.ResultText=="Result — pending",
            "Running Production must not expose a completed result.");
    }

    private static void UnboundAcquisitionIsProjected()
    {
        var surface=Create(ClientExecutionStatus.Idle,0,0);
        Check(!surface.HasAcquisition && surface.Presentation.AcquisitionText=="Acquisition — unbound",
            "Unbound Acquisition must remain explicit.");
    }

    private static void CompletedSnapshotExposesRoi()
    {
        var surface=Create(ClientExecutionStatus.Completed,3,3);
        Check(surface.CanInteractWithRoi && surface.Presentation.RoiText=="ROI — available",
            "Completed inspection must expose the captured ROI surface.");
    }

    private static void RunningSnapshotHidesRoi()
    {
        var surface=Create(ClientExecutionStatus.Running,1,3);
        Check(!surface.CanInteractWithRoi && surface.Presentation.RoiText.Contains("unavailable",StringComparison.Ordinal),
            "Running inspection must not expose ROI as available.");
    }

    private static void StatusRemainsAuthoritative()
    {
        var routing=new ClientWorkspaceCommandRouting(
            ClientWorkspaceKind.Inspection,
            false,
            true,
            true,
            true,
            true,
            false,
            false);
        var surface=Create(ClientExecutionStatus.Cancelled,2,3,routing);
        Check(surface.Presentation.ExecutionStatus=="Cancelled" &&
              surface.CommandRouting?.CanCancelInspection==true,
            "Surface must preserve authoritative status and supplied command routing.");
    }

    private static void FrameProgressRemainsVisible()
    {
        var surface=Create(ClientExecutionStatus.Running,2,5);
        Check(surface.Presentation.ProgressText.Contains("2/5 frames",StringComparison.Ordinal),
            "Surface must preserve observed progress.");
    }

    private static void TargetCountRemainsVisible()
    {
        var surface=Create(ClientExecutionStatus.Running,2,5);
        Check(surface.Presentation.ProgressText.Contains("5 frames",StringComparison.Ordinal),
            "Surface must preserve the Production target count.");
    }

    private static void UnboundProgramRemainsExplicit()
    {
        var surface=Create(ClientExecutionStatus.Idle,0,0);
        Check(surface.Presentation.ProgramSummary=="Program — unbound",
            "Unbound Program must remain explicit.");
    }

    private static void SurfaceDoesNotFabricateResult()
    {
        var surface=Create(ClientExecutionStatus.Failed,2,5);
        Check(!surface.HasCompletedResult && surface.Presentation.ResultText=="Result — pending",
            "Failed Production must not fabricate a completed result.");
    }

    private static ClientInspectionExecutionSurface Create(
        ClientExecutionStatus status,
        int processed,
        int target,
        ClientWorkspaceCommandRouting? commandRouting=null)
    {
        using var workspace=new ClientInspectionWorkspace(
            new System.Numerics.Vector2(640,480),
            new System.Numerics.Vector2(640,480));

        var captured=workspace.Capture();
        var snapshot=captured with
        {
            Production=captured.Production with
            {
                Status=status,
                FramesProcessed=processed,
                TargetFrameCount=target,
                LastFrameCount=processed,
                ActiveSessionId=Guid.NewGuid()
            }
        };

        return commandRouting is null
            ? ClientInspectionExecutionSurfaceRuntime.Create(snapshot)
            : ClientInspectionExecutionSurfaceRuntime.Create(snapshot,commandRouting);
    }

    private static void Check(bool condition,string message)
    {
        if(!condition)
            throw new InvalidOperationException("Inspection execution surface smoke failed: "+message);
    }
}
