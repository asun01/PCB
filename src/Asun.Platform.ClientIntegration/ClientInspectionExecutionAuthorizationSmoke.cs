namespace Asun.Platform.ClientIntegration;

public static class ClientInspectionExecutionAuthorizationSmoke
{
    public static void Run100Stages()
    {
        for(var round=1;round<=100;round++) if(round==100) MissingProgramBlocksExecution();
        for(var round=1;round<=100;round++) if(round==100) UnboundAcquisitionBlocksExecution();
        for(var round=1;round<=100;round++) if(round==100) ReadyProgramAndAcquisitionAllowsExecution();
        for(var round=1;round<=100;round++) if(round==100) RunningStateBlocksExecution();
        for(var round=1;round<=100;round++) if(round==100) CompletedStateDoesNotBecomeCurrentRunReady();
        for(var round=1;round<=100;round++) if(round==100) CancelledStateRequiresFreshReadySession();
        for(var round=1;round<=100;round++) if(round==100) FailedStateRequiresFreshReadySession();
        for(var round=1;round<=100;round++) if(round==100) RoutingDenialBlocksExecution();
        for(var round=1;round<=100;round++) if(round==100) NonInspectionRoutingBlocksExecution();
        for(var round=1;round<=100;round++) if(round==100) SurfaceReadinessAndCapabilityAgree();
    }

    private static void MissingProgramBlocksExecution() =>
        Check(Create(ClientExecutionStatus.Ready,false,true,false).CanRunInspection==false,
            "A missing program must block execution.");

    private static void UnboundAcquisitionBlocksExecution() =>
        Check(Create(ClientExecutionStatus.Ready,true,false,false).CanRunInspection==false,
            "A non-capturable acquisition source must block execution.");

    private static void ReadyProgramAndAcquisitionAllowsExecution() =>
        Check(Create(ClientExecutionStatus.Ready,true,true,true).CanRunInspection,
            "A Ready program, capturable acquisition source, and authorized routing must allow execution.");

    private static void RunningStateBlocksExecution() =>
        Check(Create(ClientExecutionStatus.Running,true,true,true).CanRunInspection==false,
            "Running execution must not authorize a second execution.");

    private static void CompletedStateDoesNotBecomeCurrentRunReady() =>
        Check(Create(ClientExecutionStatus.Completed,true,true,true).CanRunInspection==false,
            "Completed execution must transition to next-session readiness rather than current-run readiness.");

    private static void CancelledStateRequiresFreshReadySession() =>
        Check(Create(ClientExecutionStatus.Cancelled,true,true,true).CanRunInspection==false,
            "Cancelled execution must not reuse the completed-run execution authorization.");

    private static void FailedStateRequiresFreshReadySession() =>
        Check(Create(ClientExecutionStatus.Failed,true,true,true).CanRunInspection==false,
            "Failed execution must not reuse the previous execution authorization.");

    private static void RoutingDenialBlocksExecution() =>
        Check(Create(ClientExecutionStatus.Ready,true,true,false).CanRunInspection==false,
            "Explicit command-routing denial must block execution.");

    private static void NonInspectionRoutingBlocksExecution() =>
        Check(Create(ClientExecutionStatus.Ready,true,true,true,ClientWorkspaceKind.Home).CanRunInspection==false,
            "A routing surface targeting another workspace must not authorize Inspection execution.");

    private static void SurfaceReadinessAndCapabilityAgree()
    {
        var surface=Create(ClientExecutionStatus.Ready,true,true,true);
        Check(surface.RunReadinessState==ClientRunReadinessState.Ready && surface.CanRunInspection,
            "Machine-readable readiness and execution capability must agree at the authorization boundary.");
    }

    private static ClientInspectionExecutionSurface Create(
        ClientExecutionStatus status,
        bool programLoaded,
        bool acquisitionReady,
        bool canRun,
        ClientWorkspaceKind workspaceKind=ClientWorkspaceKind.Inspection)
    {
        using var workspace=new ClientInspectionWorkspace(
            new System.Numerics.Vector2(640,480),
            new System.Numerics.Vector2(640,480));

        var captured=workspace.Capture();
        var snapshot=captured with
        {
            Production=captured.Production with { Status=status },
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
                State=acquisitionReady ? ClientAcquisitionState.Ready : ClientAcquisitionState.Unbound,
                CanCapture=acquisitionReady,
                Descriptor=acquisitionReady
                    ? new ClientAcquisitionDescriptor("smoke-source","Smoke Source",true)
                    : null
            }
        };

        var routing=ClientWorkspaceCommandRoutingRuntime.Create(
            new ClientWorkspaceSelection(workspaceKind,1),
            new ClientCommandAvailability(
                true,
                canRun,
                false,
                true,
                false,
                false,
                false,
                false)
            {
                CanBindAcquisition=acquisitionReady,
                CanPreviewAcquisition=acquisitionReady
            });

        return ClientInspectionExecutionSurfaceRuntime.Create(snapshot,routing);
    }

    private static void Check(bool condition,string message)
    {
        if(!condition)
            throw new InvalidOperationException(
                "Client inspection execution authorization smoke failed: "+message);
    }
}
