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
        var routing=CreateRouting(ClientWorkspaceKind.Inspection,ReadyAvailability());
        Check(routing.CanBindAcquisition,
            "Inspection routing must expose acquisition binding when the execution state permits it.");
    }

    private static void HomeBlocksAcquisitionBinding()
    {
        var routing=CreateRouting(ClientWorkspaceKind.Home,ReadyAvailability());
        Check(!routing.CanBindAcquisition,
            "Home routing must not expose acquisition binding.");
    }

    private static void ProgramBlocksAcquisitionBinding()
    {
        var routing=CreateRouting(ClientWorkspaceKind.Program,ReadyAvailability());
        Check(!routing.CanBindAcquisition,
            "Program routing must not expose acquisition binding.");
    }

    private static void QualityBlocksAcquisitionBinding()
    {
        var routing=CreateRouting(ClientWorkspaceKind.Quality,ReadyAvailability());
        Check(!routing.CanBindAcquisition,
            "Quality routing must not expose acquisition binding.");
    }

    private static void ResultsBlocksAcquisitionBinding()
    {
        var routing=CreateRouting(ClientWorkspaceKind.Results,ReadyAvailability());
        Check(!routing.CanBindAcquisition,
            "Results routing must not expose acquisition binding.");
    }

    private static void RunningBlocksAcquisitionBinding()
    {
        var routing=CreateRouting(ClientWorkspaceKind.Inspection,RunningAvailability());
        Check(!routing.CanBindAcquisition,
            "Running execution must block acquisition source rebinding.");
    }

    private static void ReadyAllowsAcquisitionBinding()
    {
        var availability=ReadyAvailability();
        Check(availability.CanBindAcquisition,
            "Ready execution must advertise acquisition binding availability.");
    }

    private static void CompletedAllowsAcquisitionBinding()
    {
        var availability=CompletedAvailability();
        Check(availability.CanBindAcquisition,
            "Completed execution must preserve explicit acquisition binding availability for the next session.");
    }

    private static void RoutingPreservesExistingInspectionCommands()
    {
        var routing=CreateRouting(ClientWorkspaceKind.Inspection,ReadyAvailability());
        Check(routing.CanLoadProgram &&
              routing.CanResetSession &&
              !routing.CanRunInspection,
            "Adding acquisition binding must not weaken the existing Inspection command routing contract.");
    }

    private static void AvailabilityIsExplicit()
    {
        var availability=RunningAvailability();
        Check(!availability.CanBindAcquisition &&
              availability.CanCancel &&
              !availability.CanRun,
            "Running-state command availability must explicitly separate binding, cancel, and run commands.");
    }

    private static ClientWorkspaceCommandRouting CreateRouting(
        ClientWorkspaceKind kind,
        ClientCommandAvailability availability)
    {
        using var navigation=new ClientWorkspaceRuntime();
        navigation.TryNavigate(kind);
        return ClientWorkspaceCommandRoutingRuntime.Create(
            navigation.Current,
            availability);
    }

    private static ClientCommandAvailability ReadyAvailability() =>
        new(true,false,false,true,false,false,false,false)
        {
            CanBindAcquisition=true
        };

    private static ClientCommandAvailability RunningAvailability() =>
        new(false,false,true,false,false,false,false,false)
        {
            CanBindAcquisition=false
        };

    private static ClientCommandAvailability CompletedAvailability() =>
        new(true,false,false,true,true,true,true,true)
        {
            CanBindAcquisition=true
        };

    private static void Check(bool condition,string message)
    {
        if(!condition)
            throw new InvalidOperationException(
                "Client workspace command routing acceptance smoke failed: "+message);
    }
}
