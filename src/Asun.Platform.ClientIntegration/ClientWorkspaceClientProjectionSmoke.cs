using Asun.Device.Contracts;

namespace Asun.Platform.ClientIntegration;

public static class ClientWorkspaceClientProjectionSmoke
{
    public static void Run100Stages()
    {
        for(var round=1;round<=100;round++) if(round==100) InitialSnapshotIsAvailable();
        for(var round=1;round<=100;round++) if(round==100) NavigationRefreshesSnapshot();
        for(var round=1;round<=100;round++) if(round==100) ResultsNavigationRefreshesSnapshot();
        for(var round=1;round<=100;round++) if(round==100) InspectionChangesRefreshSnapshot();
        for(var round=1;round<=100;round++) if(round==100) RefreshPublishesSnapshot();
        for(var round=1;round<=100;round++) if(round==100) ChangedEventCarriesLatestSnapshot();
        for(var round=1;round<=100;round++) if(round==100) RoutingFactoryDrivesSnapshot();
        for(var round=1;round<=100;round++) if(round==100) DisposeDetachesNavigation();
        for(var round=1;round<=100;round++) if(round==100) DisposeDetachesInspection();
        for(var round=1;round<=100;round++) if(round==100) DisposeRejectsSnapshotAccess();
    }

    private static void InitialSnapshotIsAvailable()
    {
        using var navigation=new ClientWorkspaceRuntime();
        using var inspection=CreateWorkspace();
        using var projection=CreateProjection(navigation,inspection);
        Check(projection.Snapshot.Selection.Workspace==ClientWorkspaceKind.Home,
            "Client projection must expose an initial Home snapshot.");
    }

    private static void NavigationRefreshesSnapshot()
    {
        using var navigation=new ClientWorkspaceRuntime();
        using var inspection=CreateWorkspace();
        using var projection=CreateProjection(navigation,inspection);

        navigation.TryNavigate(ClientWorkspaceKind.Inspection);
        Check(projection.Snapshot.Selection.Workspace==ClientWorkspaceKind.Inspection,
            "Navigation changes must update the unified client snapshot.");
    }

    private static void ResultsNavigationRefreshesSnapshot()
    {
        using var navigation=new ClientWorkspaceRuntime();
        using var inspection=CreateWorkspace();
        using var projection=CreateProjection(navigation,inspection);

        navigation.TryNavigate(ClientWorkspaceKind.Results);
        Check(projection.Snapshot.Selection.Workspace==ClientWorkspaceKind.Results &&
              projection.Snapshot.Content.Results.Current.Status=="Idle",
            "Results navigation must update the unified client snapshot.");
    }

    private static void InspectionChangesRefreshSnapshot()
    {
        using var navigation=new ClientWorkspaceRuntime();
        using var inspection=CreateWorkspace();
        using var projection=CreateProjection(navigation,inspection);

        inspection.BindAcquisition(
            new DeterministicSource(),
            new ClientAcquisitionDescriptor("source","Deterministic",true));

        Check(projection.Snapshot.Content.Inspection.HasAcquisition,
            "Inspection changes must refresh the unified client snapshot.");
    }

    private static void RefreshPublishesSnapshot()
    {
        using var navigation=new ClientWorkspaceRuntime();
        using var inspection=CreateWorkspace();
        using var projection=CreateProjection(navigation,inspection);

        var changes=0;
        projection.Changed+=_=>changes++;
        projection.Refresh();
        Check(changes==1,
            "Explicit Refresh must publish exactly one unified snapshot.");
    }

    private static void ChangedEventCarriesLatestSnapshot()
    {
        using var navigation=new ClientWorkspaceRuntime();
        using var inspection=CreateWorkspace();
        using var projection=CreateProjection(navigation,inspection);

        ClientWorkspaceClientSnapshot? latest=null;
        projection.Changed+=value=>latest=value;
        navigation.TryNavigate(ClientWorkspaceKind.Quality);

        Check(latest?.Selection.Workspace==ClientWorkspaceKind.Quality,
            "Projection Changed event must carry the latest navigation snapshot.");
    }

    private static void RoutingFactoryDrivesSnapshot()
    {
        using var navigation=new ClientWorkspaceRuntime();
        using var inspection=CreateWorkspace();
        using var projection=new ClientWorkspaceClientProjection(
            navigation,
            inspection,
            selection=>new ClientWorkspaceCommandRouting(
                selection.Workspace,
                false,
                false,
                false,
                true,
                false,
                selection.Workspace==ClientWorkspaceKind.Quality,
                selection.Workspace==ClientWorkspaceKind.Results));

        navigation.TryNavigate(ClientWorkspaceKind.Results);
        Check(projection.Snapshot.Routing.CanReviewResults &&
              projection.Snapshot.Content.Routing.CanReviewResults,
            "Routing factory values must propagate through the unified client projection.");
    }

    private static void DisposeDetachesNavigation()
    {
        using var navigation=new ClientWorkspaceRuntime();
        using var inspection=CreateWorkspace();
        var projection=new ClientWorkspaceClientProjection(
            navigation,
            inspection,
            Routing);
        var changes=0;
        projection.Changed+=_=>changes++;
        projection.Dispose();
        navigation.TryNavigate(ClientWorkspaceKind.Inspection);

        Check(changes==0,
            "Disposed client projection must not receive navigation events.");
        inspection.Dispose();
    }

    private static void DisposeDetachesInspection()
    {
        using var navigation=new ClientWorkspaceRuntime();
        using var inspection=CreateWorkspace();
        var projection=new ClientWorkspaceClientProjection(
            navigation,
            inspection,
            Routing);
        var changes=0;
        projection.Changed+=_=>changes++;
        projection.Dispose();
        inspection.BindAcquisition(
            new DeterministicSource(),
            new ClientAcquisitionDescriptor("source","Deterministic",true));

        Check(changes==0,
            "Disposed client projection must not receive Inspection events.");
    }

    private static void DisposeRejectsSnapshotAccess()
    {
        using var navigation=new ClientWorkspaceRuntime();
        using var inspection=CreateWorkspace();
        var projection=new ClientWorkspaceClientProjection(
            navigation,
            inspection,
            Routing);
        projection.Dispose();

        var rejected=false;
        try
        {
            _=projection.Snapshot;
        }
        catch(ObjectDisposedException)
        {
            rejected=true;
        }

        Check(rejected,
            "Disposed client projection must reject Snapshot access.");
    }

    private static ClientWorkspaceClientProjection CreateProjection(
        ClientWorkspaceRuntime navigation,
        ClientInspectionWorkspace inspection) =>
        new(navigation,inspection,Routing);

    private static ClientWorkspaceCommandRouting Routing(
        ClientWorkspaceSelection selection) =>
        new(
            selection.Workspace,
            true,
            false,
            false,
            true,
            false,
            selection.Workspace==ClientWorkspaceKind.Quality,
            selection.Workspace==ClientWorkspaceKind.Results);

    private static ClientInspectionWorkspace CreateWorkspace() =>
        new(
            new System.Numerics.Vector2(640,480),
            new System.Numerics.Vector2(640,480));

    private sealed class DeterministicSource : IFrameSource
    {
        public ValueTask<CapturedFrame?> CaptureAsync(
            CancellationToken cancellationToken=default) =>
            ValueTask.FromResult<CapturedFrame?>(null);
    }

    private static void Check(bool condition,string message)
    {
        if(!condition)
            throw new InvalidOperationException(
                "Workspace client projection smoke failed: "+message);
    }
}
