namespace Asun.Platform.ClientIntegration;

public static class ClientWorkspaceClientSnapshotSmoke
{
    public static void Run100Stages()
    {
        for(var round=1;round<=100;round++) if(round==100) SelectionIsProjected();
        for(var round=1;round<=100;round++) if(round==100) RoutingIsProjected();
        for(var round=1;round<=100;round++) if(round==100) ContentIsProjected();
        for(var round=1;round<=100;round++) if(round==100) InspectionContentIsAvailable();
        for(var round=1;round<=100;round++) if(round==100) QualityContentIsAvailable();
        for(var round=1;round<=100;round++) if(round==100) ResultsContentIsAvailable();
        for(var round=1;round<=100;round++) if(round==100) TransitionSequenceIsPreserved();
        for(var round=1;round<=100;round++) if(round==100) RoutingMismatchIsRejected();
        for(var round=1;round<=100;round++) if(round==100) NoAuthorityIsCreated();
        for(var round=1;round<=100;round++) if(round==100) CallerRoutingIsRetained();
    }

    private static void SelectionIsProjected()
    {
        var selection=new ClientWorkspaceSelection(ClientWorkspaceKind.Results,9);
        var snapshot=Create(selection);
        Check(snapshot.Selection==selection,
            "Unified client snapshot must preserve the workspace selection.");
    }

    private static void RoutingIsProjected()
    {
        var selection=new ClientWorkspaceSelection(ClientWorkspaceKind.Quality,3);
        var routing=Routing(ClientWorkspaceKind.Quality);
        using var workspace=CreateWorkspace();
        var snapshot=ClientWorkspaceClientSnapshotRuntime.Create(
            selection,routing,workspace.Capture());
        Check(snapshot.Routing==routing,
            "Unified client snapshot must preserve the supplied routing projection.");
    }

    private static void ContentIsProjected()
    {
        var selection=new ClientWorkspaceSelection(ClientWorkspaceKind.Inspection,4);
        var snapshot=Create(selection);
        Check(snapshot.Content.Selection==selection &&
              snapshot.Content.Routing.Workspace==selection.Workspace,
            "Unified client snapshot must carry one coherent content surface.");
    }

    private static void InspectionContentIsAvailable()
    {
        var snapshot=Create(new ClientWorkspaceSelection(ClientWorkspaceKind.Inspection,1));
        Check(snapshot.Content.Inspection.Presentation.ExecutionStatus=="Idle",
            "Inspection content must be available through the unified client snapshot.");
    }

    private static void QualityContentIsAvailable()
    {
        var snapshot=Create(new ClientWorkspaceSelection(ClientWorkspaceKind.Quality,2));
        Check(!snapshot.Content.Quality.Snapshot.IsBound,
            "Quality content must be available through the unified client snapshot.");
    }

    private static void ResultsContentIsAvailable()
    {
        var snapshot=Create(new ClientWorkspaceSelection(ClientWorkspaceKind.Results,3));
        Check(snapshot.Content.Results.Current.Status=="Idle",
            "Results content must be available through the unified client snapshot.");
    }

    private static void TransitionSequenceIsPreserved()
    {
        var selection=new ClientWorkspaceSelection(ClientWorkspaceKind.Program,17);
        var snapshot=Create(selection);
        Check(snapshot.Selection.TransitionSequence==17,
            "Workspace transition sequence must remain stable through client projection.");
    }

    private static void RoutingMismatchIsRejected()
    {
        using var workspace=CreateWorkspace();
        var rejected=ExpectInvalidOperation(() =>
            ClientWorkspaceClientSnapshotRuntime.Create(
                new ClientWorkspaceSelection(ClientWorkspaceKind.Quality,1),
                Routing(ClientWorkspaceKind.Results),
                workspace.Capture()));
        Check(rejected,
            "Workspace client snapshot must reject mismatched selection/routing authorities.");
    }

    private static void NoAuthorityIsCreated()
    {
        var snapshot=Create(new ClientWorkspaceSelection(ClientWorkspaceKind.Home,5));
        Check(snapshot.Content.Inspection.ResultDisplay.ReplayText=="Replay not available" &&
              snapshot.Content.Quality.Snapshot.IsBound==false &&
              snapshot.Content.Results.Current.ReplayText=="Replay not available" &&
              snapshot.Content.Results.Current.ReleaseText=="Release not evaluated",
            "Unified client snapshot must not create Quality, Replay, or Release authority.");
    }

    private static void CallerRoutingIsRetained()
    {
        var routing=Routing(ClientWorkspaceKind.Results) with
        {
            CanReviewResults=true
        };
        using var workspace=CreateWorkspace();
        var snapshot=ClientWorkspaceClientSnapshotRuntime.Create(
            new ClientWorkspaceSelection(ClientWorkspaceKind.Results,21),
            routing,
            workspace.Capture());
        Check(snapshot.Routing.CanReviewResults &&
              snapshot.Content.Routing.CanReviewResults,
            "Caller-provided Results permission must survive unified client projection.");
    }

    private static ClientWorkspaceClientSnapshot Create(
        ClientWorkspaceSelection selection)
    {
        using var workspace=CreateWorkspace();
        return ClientWorkspaceClientSnapshotRuntime.Create(
            selection,
            Routing(selection.Workspace),
            workspace.Capture());
    }

    private static ClientWorkspaceCommandRouting Routing(
        ClientWorkspaceKind workspace) =>
        new(
            workspace,
            true,false,false,true,false,
            workspace==ClientWorkspaceKind.Quality,
            workspace==ClientWorkspaceKind.Results);

    private static ClientInspectionWorkspace CreateWorkspace() =>
        new(
            new System.Numerics.Vector2(640,480),
            new System.Numerics.Vector2(640,480));

    private static bool ExpectInvalidOperation(Action action)
    {
        try
        {
            action();
        }
        catch(ArgumentException)
        {
            return true;
        }
        catch(InvalidOperationException)
        {
            return true;
        }

        return false;
    }

    private static void Check(bool condition,string message)
    {
        if(!condition)
            throw new InvalidOperationException(
                "Workspace client snapshot smoke failed: "+message);
    }
}
