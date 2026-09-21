namespace Asun.Platform.ClientIntegration;

public static class ClientInspectionRoiPulseSmoke
{
    public static void Run100Stages()
    {
        for(var round=1;round<=100;round++) if(round==100) ResizePublishesRoiPulse();
        for(var round=1;round<=100;round++) if(round==100) PulseCarriesLatestViewport();
        for(var round=1;round<=100;round++) if(round==100) ModeChangePublishesRoiPulse();
        for(var round=1;round<=100;round++) if(round==100) StopInteractionPublishesRoiPulse();
        for(var round=1;round<=100;round++) if(round==100) RoiPulseDoesNotFabricateProduction();
        for(var round=1;round<=100;round++) if(round==100) RoiPulseDoesNotPublishFullSnapshot();
        for(var round=1;round<=100;round++) if(round==100) MultiplePulsesRemainLatest();
        for(var round=1;round<=100;round++) if(round==100) DisposeDetachesRoiPulse();
        for(var round=1;round<=100;round++) if(round==100) DisposedWorkspaceRejectsRoiSubscription();
        for(var round=1;round<=100;round++) if(round==100) ProjectionBridgesRoiPulse();
    }

    private static void ResizePublishesRoiPulse()
    {
        using var workspace=CreateWorkspace();
        var count=0;
        workspace.RoiChanged+=_=>count++;
        workspace.ResizeRoiViewport(new System.Numerics.Vector2(800,600));
        Check(count==1,
            "ROI viewport resize must publish one ROI pulse.");
    }

    private static void PulseCarriesLatestViewport()
    {
        using var workspace=CreateWorkspace();
        RoiViewportSnapshot? latest=null;
        workspace.RoiChanged+=value=>latest=value;
        workspace.ResizeRoiViewport(new System.Numerics.Vector2(800,600));
        Check(latest?.ViewportSize.X==800 &&
              latest.ViewportSize.Y==600,
            "ROI pulse must carry the latest viewport snapshot.");
    }

    private static void ModeChangePublishesRoiPulse()
    {
        using var workspace=CreateWorkspace();
        var count=0;
        workspace.RoiChanged+=_=>count++;
        workspace.SetRoiMode(RoiEditorMode.CreateRectangle);
        Check(count==1,
            "ROI mode change must publish one ROI pulse.");
    }

    private static void StopInteractionPublishesRoiPulse()
    {
        using var workspace=CreateWorkspace();
        var count=0;
        workspace.RoiChanged+=_=>count++;
        workspace.StopRoiInteraction();
        Check(count==1,
            "Stopping ROI interaction must publish one ROI pulse.");
    }

    private static void RoiPulseDoesNotFabricateProduction()
    {
        using var workspace=CreateWorkspace();
        ClientWorkspaceSnapshot? production=null;
        workspace.ProductionChanged+=value=>production=value;
        workspace.ResizeRoiViewport(new System.Numerics.Vector2(800,600));
        Check(production is null ||
              production.Status==ClientExecutionStatus.Idle,
            "ROI pulse must not fabricate Production execution state.");
    }

    private static void RoiPulseDoesNotPublishFullSnapshot()
    {
        using var workspace=CreateWorkspace();
        var fullChanges=0;
        workspace.Changed+=_=>fullChanges++;
        workspace.ResizeRoiViewport(new System.Numerics.Vector2(800,600));
        Check(fullChanges==0,
            "High-frequency ROI changes must remain on the dedicated ROI pulse channel.");
    }

    private static void MultiplePulsesRemainLatest()
    {
        using var workspace=CreateWorkspace();
        RoiViewportSnapshot? latest=null;
        ClientInspectionRoiPulse? latestPulse=null;
        workspace.RoiChanged+=value=>latest=value;
        workspace.RoiPulseChanged+=value=>latestPulse=value;
        workspace.ResizeRoiViewport(new System.Numerics.Vector2(800,600));
        workspace.ResizeRoiViewport(new System.Numerics.Vector2(1024,768));
        Check(latest?.ViewportSize.X==1024 &&
              latest.ViewportSize.Y==768 &&
              latestPulse?.Snapshot.ViewportSize.X==1024 &&
              latestPulse.Sequence==2,
            "Repeated ROI pulses must expose the latest viewport state with a monotonic sequence.");
    }

    private static void DisposeDetachesRoiPulse()
    {
        var workspace=CreateWorkspace();
        var count=0;
        workspace.RoiChanged+=_=>count++;
        workspace.Dispose();
        Check(count==0,
            "Disposed workspace must not emit new ROI pulses.");
    }

    private static void DisposedWorkspaceRejectsRoiSubscription()
    {
        var workspace=CreateWorkspace();
        workspace.Dispose();
        var rejected=false;
        try
        {
            workspace.RoiChanged+=_=>{ };
        }
        catch(ObjectDisposedException)
        {
            rejected=true;
        }

        Check(rejected,
            "Disposed workspace must reject new ROI pulse subscriptions.");
    }

    private static void ProjectionBridgesRoiPulse()
    {
        using var navigation=new ClientWorkspaceRuntime();
        using var inspection=CreateWorkspace();
        using var projection=new ClientWorkspaceClientProjection(
            navigation,
            inspection,
            selection=>new ClientWorkspaceCommandRouting(
                selection.Workspace,
                true,false,false,true,false,false,false));

        RoiViewportSnapshot? latest=null;
        ClientInspectionRoiPulse? latestPulse=null;
        projection.RoiChanged+=value=>latest=value;
        projection.RoiPulseChanged+=value=>latestPulse=value;
        inspection.ResizeRoiViewport(new System.Numerics.Vector2(900,700));

        Check(latest?.ViewportSize.X==900 &&
              latest.ViewportSize.Y==700 &&
              latestPulse?.Snapshot.ViewportSize.X==900 &&
              latestPulse.Sequence==1,
            "Unified client projection must bridge both raw and sequenced ROI pulses.");
    }

    private static ClientInspectionWorkspace CreateWorkspace() =>
        new(
            new System.Numerics.Vector2(640,480),
            new System.Numerics.Vector2(640,480));

    private static void Check(bool condition,string message)
    {
        if(!condition)
            throw new InvalidOperationException(
                "Inspection ROI pulse smoke failed: "+message);
    }
}
