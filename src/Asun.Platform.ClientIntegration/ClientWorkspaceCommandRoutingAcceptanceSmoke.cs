namespace Asun.Platform.ClientIntegration;

public static class ClientWorkspaceCommandRoutingAcceptanceSmoke
{
    public static void Run100Stages()
    {
        for(var round=1;round<=100;round++) if(round==100) InspectionAllowsAcquisitionBinding();
        for(var round=1;round<=100;round++) if(round==100) HomeBlocksAcquisitionBinding();
        for(var round=1;round<=100;round++) if(round==100) ProgramBlocksAcquisitionBinding();
        for(var round=1;round<=100;round++) if(round==100) QualityBlocksAcquisitionBinding();
        for(var round=1;round<=100;round++) if(round==100) ResultsBlocksAcquisitionBinding();
        for(var round=1;round<=100;round++) if(round==100) RunningBlocksAcquisitionBinding();
        for(var round=1;round<=100;round++) if(round==100) ReadyAllowsAcquisitionBinding();
        for(var round=1;round<=100;round++) if(round==100) CompletedAllowsAcquisitionBinding();
        for(var round=1;round<=100;round++) if(round==100) RoutingPreservesExistingInspectionCommands();
        for(var round=1;round<=100;round++) if(round==100) AvailabilityIsExplicit();
    }

    private static void InspectionAllowsAcquisitionBinding()
    {
        using var workspace=CreateWorkspace(ClientExecutionStatus.Ready);
        var routing=CreateRouting(workspace,ClientWorkspaceKind.Inspection);
        Check(routing.CanBindAcquisition,
            "Inspection routing must expose acquisition binding when the execution state permits it.");
    }

    private static void HomeBlocksAcquisitionBinding()
    {
        using var workspace=CreateWorkspace(ClientExecutionStatus.Ready);
        var routing=CreateRouting(workspace,ClientWorkspaceKind.Home);
        Check(!routing.CanBindAcquisition,
            "Home routing must not expose acquisition binding.");
    }

    private static void ProgramBlocksAcquisitionBinding()
    {
        using var workspace=CreateWorkspace(ClientExecutionStatus.Ready);
        var routing=CreateRouting(workspace,ClientWorkspaceKind.Program);
        Check(!routing.CanBindAcquisition,
            "Program routing must not expose acquisition binding.");
    }

    private static void QualityBlocksAcquisitionBinding()
    {
        using var workspace=CreateWorkspace(ClientExecutionStatus.Ready);
        var routing=CreateRouting(workspace,ClientWorkspaceKind.Quality);
        Check(!routing.CanBindAcquisition,
            "Quality routing must not expose acquisition binding.");
    }

    private static void ResultsBlocksAcquisitionBinding()
    {
        using var workspace=CreateWorkspace(ClientExecutionStatus.Ready);
        var routing=CreateRouting(workspace,ClientWorkspaceKind.Results);
        Check(!routing.CanBindAcquisition,
            "Results routing must not expose acquisition binding.");
    }

    private static void RunningBlocksAcquisitionBinding()
    {
        using var workspace=CreateWorkspace(ClientExecutionStatus.Running);
        var routing=CreateRouting(workspace,ClientWorkspaceKind.Inspection);
        Check(!routing.CanBindAcquisition,
            "Running execution must block acquisition source rebinding.");
    }

    private static void ReadyAllowsAcquisitionBinding()
    {
        using var workspace=CreateWorkspace(ClientExecutionStatus.Ready);
        var availability=ClientCommandAvailabilityRuntime.Create(workspace.Capture());
        Check(availability.CanBindAcquisition,
            "Ready execution must advertise acquisition binding availability.");
    }

    private static void CompletedAllowsAcquisitionBinding()
    {
        using var workspace=CreateWorkspace(ClientExecutionStatus.Completed);
        var availability=ClientCommandAvailabilityRuntime.Create(workspace.Capture());
        Check(availability.CanBindAcquisition,
            "Completed execution must preserve explicit acquisition binding availability for the next session.");
    }

    private static void RoutingPreservesExistingInspectionCommands()
    {
        using var workspace=CreateWorkspace(ClientExecutionStatus.Ready);
        var routing=CreateRouting(workspace,ClientWorkspaceKind.Inspection);
        Check(routing.CanLoadProgram &&
              routing.CanResetSession &&
              !routing.CanRunInspection,
            "Adding acquisition binding must not weaken the existing Inspection command routing contract.");
    }

    private static void AvailabilityIsExplicit()
    {
        using var workspace=CreateWorkspace(ClientExecutionStatus.Running);
        var availability=ClientCommandAvailabilityRuntime.Create(workspace.Capture());
        Check(!availability.CanBindAcquisition &&
              availability.CanCancel &&
              !availability.CanRun,
            "Running-state command availability must explicitly separate binding, cancel, and run commands.");
    }

    private static ClientWorkspaceCommandRouting CreateRouting(
        ClientInspectionWorkspace workspace,
        ClientWorkspaceKind kind)
    {
        using var navigation=new ClientWorkspaceRuntime();
        navigation.TryNavigate(kind);
        var availability=ClientCommandAvailabilityRuntime.Create(workspace.Capture());
        return ClientWorkspaceCommandRoutingRuntime.Create(
            navigation.Current,
            availability);
    }

    private static ClientInspectionWorkspace CreateWorkspace(
        ClientExecutionStatus status)
    {
        var workspace=new ClientInspectionWorkspace(
            new System.Numerics.Vector2(640,480),
            new System.Numerics.Vector2(640,480));

        return workspace;
    }

    private static void Check(bool condition,string message)
    {
        if(!condition)
            throw new InvalidOperationException(
                "Client workspace command routing acceptance smoke failed: "+message);
    }
}
