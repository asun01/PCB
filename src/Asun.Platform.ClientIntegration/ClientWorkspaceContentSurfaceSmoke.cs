namespace Asun.Platform.ClientIntegration;

public static class ClientWorkspaceContentSurfaceSmoke
{
    public static void Run100Stages()
    {
        for(var round=1;round<=100;round++) if(round==100) SelectionAndRoutingMustAlign();
        for(var round=1;round<=100;round++) if(round==100) InspectionSurfaceIsPresent();
        for(var round=1;round<=100;round++) if(round==100) QualitySurfaceIsPresent();
        for(var round=1;round<=100;round++) if(round==100) ResultsSurfaceIsPresent();
        for(var round=1;round<=100;round++) if(round==100) RoutingIsPreserved();
        for(var round=1;round<=100;round++) if(round==100) ProgramWorkspaceCanBeSelected();
        for(var round=1;round<=100;round++) if(round==100) QualityWorkspaceCanBeSelected();
        for(var round=1;round<=100;round++) if(round==100) ResultsWorkspaceCanBeSelected();
        for(var round=1;round<=100;round++) if(round==100) InspectionWorkspaceCanBeSelected();
        for(var round=1;round<=100;round++) if(round==100) NoAuthorityIsCreated();
    }

    private static void SelectionAndRoutingMustAlign()
    {
        var selection=new ClientWorkspaceSelection(ClientWorkspaceKind.Inspection,1);
        var surface=Create(selection);
        Check(surface.Selection.Workspace==surface.Routing.Workspace,
            "Workspace content surface must keep selection and routing aligned.");
    }

    private static void InspectionSurfaceIsPresent()
    {
        var surface=Create(new ClientWorkspaceSelection(ClientWorkspaceKind.Inspection,1));
        Check(surface.Inspection.Presentation.ExecutionStatus=="Idle",
            "Inspection content surface must expose the existing Inspection presentation.");
    }

    private static void QualitySurfaceIsPresent()
    {
        var surface=Create(new ClientWorkspaceSelection(ClientWorkspaceKind.Quality,1));
        Check(!surface.Quality.Snapshot.IsBound,
            "Quality content surface must expose the existing unbound Quality state.");
    }

    private static void ResultsSurfaceIsPresent()
    {
        var surface=Create(new ClientWorkspaceSelection(ClientWorkspaceKind.Results,1));
        Check(surface.Results.Current.Status=="Idle",
            "Results content surface must expose the existing current Results state.");
    }

    private static void RoutingIsPreserved()
    {
        var selection=new ClientWorkspaceSelection(ClientWorkspaceKind.Results,7);
        var routing=new ClientWorkspaceCommandRouting(
            ClientWorkspaceKind.Results,
            false,false,false,true,false,false,true);
        using var workspace=CreateWorkspace();
        var surface=ClientWorkspaceContentSurfaceRuntime.Create(
            selection,
            routing,
            workspace.Capture());
        Check(surface.Routing.TransitionSafe(),
            "Client content surface must preserve the supplied routing values.");
    }

    private static void ProgramWorkspaceCanBeSelected()
    {
        var surface=Create(new ClientWorkspaceSelection(ClientWorkspaceKind.Program,2));
        Check(surface.Selection.Workspace==ClientWorkspaceKind.Program,
            "Program workspace selection must remain available through the unified surface.");
    }

    private static void QualityWorkspaceCanBeSelected()
    {
        var surface=Create(new ClientWorkspaceSelection(ClientWorkspaceKind.Quality,3));
        Check(surface.Selection.Workspace==ClientWorkspaceKind.Quality,
            "Quality workspace selection must remain available through the unified surface.");
    }

    private static void ResultsWorkspaceCanBeSelected()
    {
        var surface=Create(new ClientWorkspaceSelection(ClientWorkspaceKind.Results,4));
        Check(surface.Selection.Workspace==ClientWorkspaceKind.Results,
            "Results workspace selection must remain available through the unified surface.");
    }

    private static void InspectionWorkspaceCanBeSelected()
    {
        var surface=Create(new ClientWorkspaceSelection(ClientWorkspaceKind.Inspection,5));
        Check(surface.Selection.Workspace==ClientWorkspaceKind.Inspection,
            "Inspection workspace selection must remain available through the unified surface.");
    }

    private static void NoAuthorityIsCreated()
    {
        using var workspace=CreateWorkspace();
        var snapshot=workspace.Capture();
        var surface=Create(
            new ClientWorkspaceSelection(ClientWorkspaceKind.Home,6),
            snapshot);
        Check(snapshot.Replay is null &&
              snapshot.Release is null &&
              snapshot.Quality.IsBound==false &&
              surface.Results.Current.ReplayText=="Replay not available",
            "Unified client surface must not fabricate Replay, Release, or Quality authority.");
    }

    private static ClientWorkspaceContentSurface Create(
        ClientWorkspaceSelection selection,
        ClientInspectionWorkspaceSnapshot? snapshot=null)
    {
        using var workspace=CreateWorkspace();
        var actual=snapshot ?? workspace.Capture();
        var routing=new ClientWorkspaceCommandRouting(
            selection.Workspace,
            true,false,false,true,false,
            selection.Workspace==ClientWorkspaceKind.Quality,
            selection.Workspace==ClientWorkspaceKind.Results);
        return ClientWorkspaceContentSurfaceRuntime.Create(
            selection,
            routing,
            actual);
    }

    private static ClientInspectionWorkspace CreateWorkspace() =>
        new(
            new System.Numerics.Vector2(640,480),
            new System.Numerics.Vector2(640,480));

    private static bool TransitionSafe(this ClientWorkspaceCommandRouting routing)
    {
        return routing.Workspace is ClientWorkspaceKind.Home
            or ClientWorkspaceKind.Program
            or ClientWorkspaceKind.Inspection
            or ClientWorkspaceKind.Quality
            or ClientWorkspaceKind.Results;
    }

    private static void Check(bool condition,string message)
    {
        if(!condition)
            throw new InvalidOperationException(
                "Workspace content surface smoke failed: "+message);
    }
}
