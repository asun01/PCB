namespace Asun.Platform.ClientIntegration;

public static class ClientInspectionRunReadinessSmoke
{
    public static void Run100Stages()
    {
        for(var round=1;round<=100;round++) if(round==100) IdleWithoutProgramIsExplicit();
        for(var round=1;round<=100;round++) if(round==100) IdleWithProgramStillNeedsAcquisition();
        for(var round=1;round<=100;round++) if(round==100) ReadyWithoutAcquisitionIsExplicit();
        for(var round=1;round<=100;round++) if(round==100) ReadyWithAcquisitionIsReady();
        for(var round=1;round<=100;round++) if(round==100) RunningStateWinsOverReadiness();
        for(var round=1;round<=100;round++) if(round==100) CompletedWithAcquisitionIsReadyForNextSession();
        for(var round=1;round<=100;round++) if(round==100) CompletedWithoutAcquisitionStatesNextPrerequisite();
        for(var round=1;round<=100;round++) if(round==100) CancelledWithAcquisitionIsRecoverable();
        for(var round=1;round<=100;round++) if(round==100) FailedWithAcquisitionIsRecoverable();
        for(var round=1;round<=100;round++) if(round==100) SurfaceExposesPresentationReadiness();
    }

    private static void IdleWithoutProgramIsExplicit()
    {
        var snapshot=Create(ClientExecutionStatus.Idle,false,false);
        var presentation=ClientInspectionExecutionPresentationRuntime.Create(snapshot);
        Check(presentation.RunReadinessText=="Run — load a program",
            "Idle without a loaded program must report the program prerequisite.");
    }

    private static void IdleWithProgramStillNeedsAcquisition()
    {
        var snapshot=Create(ClientExecutionStatus.Idle,true,false);
        var presentation=ClientInspectionExecutionPresentationRuntime.Create(snapshot);
        Check(presentation.RunReadinessText=="Run — load a program and bind an acquisition source",
            "An idle client must not claim run readiness merely because a program projection exists.");
    }

    private static void ReadyWithoutAcquisitionIsExplicit()
    {
        var snapshot=Create(ClientExecutionStatus.Ready,true,false);
        var presentation=ClientInspectionExecutionPresentationRuntime.Create(snapshot);
        Check(presentation.RunReadinessText=="Run — bind an acquisition source",
            "Ready execution without a capturable source must expose the acquisition prerequisite.");
    }

    private static void ReadyWithAcquisitionIsReady()
    {
        var snapshot=Create(ClientExecutionStatus.Ready,true,true);
        var presentation=ClientInspectionExecutionPresentationRuntime.Create(snapshot);
        Check(presentation.RunReadinessText=="Run — ready",
            "Ready execution with a capturable source must expose run readiness.");
    }

    private static void RunningStateWinsOverReadiness()
    {
        var snapshot=Create(ClientExecutionStatus.Running,true,true);
        var presentation=ClientInspectionExecutionPresentationRuntime.Create(snapshot);
        Check(presentation.RunReadinessText=="Run — execution in progress",
            "Running execution must not expose a new-run readiness state.");
    }

    private static void CompletedWithAcquisitionIsReadyForNextSession()
    {
        var snapshot=Create(ClientExecutionStatus.Completed,true,true);
        var presentation=ClientInspectionExecutionPresentationRuntime.Create(snapshot);
        Check(presentation.RunReadinessText=="Run — ready for next session",
            "Completed execution with a capturable source must expose next-session readiness.");
    }

    private static void CompletedWithoutAcquisitionStatesNextPrerequisite()
    {
        var snapshot=Create(ClientExecutionStatus.Completed,true,false);
        var presentation=ClientInspectionExecutionPresentationRuntime.Create(snapshot);
        Check(presentation.RunReadinessText=="Run — bind an acquisition source for the next session",
            "Completed execution without a source must state the next-session prerequisite.");
    }

    private static void CancelledWithAcquisitionIsRecoverable()
    {
        var snapshot=Create(ClientExecutionStatus.Cancelled,true,true);
        var presentation=ClientInspectionExecutionPresentationRuntime.Create(snapshot);
        Check(presentation.RunReadinessText=="Run — ready after cancellation",
            "Cancelled execution with a capturable source must expose recovery readiness.");
    }

    private static void FailedWithAcquisitionIsRecoverable()
    {
        var snapshot=Create(ClientExecutionStatus.Failed,true,true);
        var presentation=ClientInspectionExecutionPresentationRuntime.Create(snapshot);
        Check(presentation.RunReadinessText=="Run — ready after failure",
            "Failed execution with a capturable source must expose recovery readiness.");
    }

    private static void SurfaceExposesPresentationReadiness()
    {
        var snapshot=Create(ClientExecutionStatus.Ready,true,true);
        var routing=ClientWorkspaceCommandRoutingRuntime.Create(
            new ClientWorkspaceSelection(ClientWorkspaceKind.Inspection,1),
            new ClientCommandAvailability(true,true,false,true,false,false,false,false)
            {
                CanBindAcquisition=true,
                CanPreviewAcquisition=true
            });
        var surface=ClientInspectionExecutionSurfaceRuntime.Create(snapshot,routing);
        Check(surface.RunReadinessText==surface.Presentation.RunReadinessText &&
              surface.CanRunInspection,
            "Inspection Surface must preserve the authoritative presentation readiness and run capability.");
    }

    private static ClientInspectionWorkspaceSnapshot Create(
        ClientExecutionStatus status,
        bool programLoaded,
        bool acquisitionReady)
    {
        using var workspace=new ClientInspectionWorkspace(
            new System.Numerics.Vector2(640,480),
            new System.Numerics.Vector2(640,480));

        var captured=workspace.Capture();
        return captured with
        {
            Production=captured.Production with
            {
                Status=status,
                FramesProcessed=status==ClientExecutionStatus.Running ? 1 : 0,
                TargetFrameCount=status==ClientExecutionStatus.Running ? 3 : 0,
                LastFrameCount=status==ClientExecutionStatus.Completed ? 3 : 0
            },
            Program=programLoaded
                ? new ClientProgramWorkspaceSnapshot(
                    ClientProgramLoadStatus.Ready,
                    Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    "Smoke Program",
                    new Version(1,0),
                    1,
                    "smoke-plan",
                    Array.Empty<string>())
                : null,
            Acquisition=captured.Acquisition with
            {
                State=acquisitionReady
                    ? ClientAcquisitionState.Ready
                    : ClientAcquisitionState.Unbound,
                CanCapture=acquisitionReady,
                Descriptor=acquisitionReady
                    ? new ClientAcquisitionDescriptor(
                        "smoke-source",
                        "Smoke Source",
                        true)
                    : null
            }
        };
    }

    private static void Check(bool condition,string message)
    {
        if(!condition)
            throw new InvalidOperationException(
                "Client inspection run readiness smoke failed: "+message);
    }
}
