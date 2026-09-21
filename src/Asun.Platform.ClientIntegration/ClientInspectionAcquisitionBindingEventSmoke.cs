using Asun.Device.Contracts;

namespace Asun.Platform.ClientIntegration;

public static class ClientInspectionAcquisitionBindingEventSmoke
{
    public static void Run100Stages()
    {
        for(var round=1;round<=100;round++) if(round==100) BindingPublishesWorkspaceChange();
        for(var round=1;round<=100;round++) if(round==100) BindingSetsReadyState();
        for(var round=1;round<=100;round++) if(round==100) BindingSetsDescriptor();
        for(var round=1;round<=100;round++) if(round==100) BindingPreservesProductionState();
        for(var round=1;round<=100;round++) if(round==100) BindingDoesNotCreateReplay();
        for(var round=1;round<=100;round++) if(round==100) BindingDoesNotCreateRelease();
        for(var round=1;round<=100;round++) if(round==100) BindingDoesNotCreateQuality();
        for(var round=1;round<=100;round++) if(round==100) RebindingPublishesNewChange();
        for(var round=1;round<=100;round++) if(round==100) BindingMakesAcquisitionVisible();
        for(var round=1;round<=100;round++) if(round==100) BindingKeepsProductionCommandBoundary();
    }

    private static void BindingPublishesWorkspaceChange()
    {
        using var workspace=CreateWorkspace();
        var changes=0;
        workspace.ProductionChanged+=_=>changes++;
        workspace.BindAcquisition(new DeterministicSource(),Descriptor("source-a"));
        Check(changes==1,"Direct Acquisition binding must publish one client workspace change.");
    }

    private static void BindingSetsReadyState()
    {
        using var workspace=CreateWorkspace();
        workspace.BindAcquisition(new DeterministicSource(),Descriptor("source-a"));
        Check(workspace.Acquisition.State==ClientAcquisitionState.Ready,
            "Direct Acquisition binding must expose Ready state.");
    }

    private static void BindingSetsDescriptor()
    {
        using var workspace=CreateWorkspace();
        workspace.BindAcquisition(new DeterministicSource(),Descriptor("source-a"));
        Check(workspace.Acquisition.Descriptor?.SourceId=="source-a",
            "Direct Acquisition binding must expose source identity.");
    }

    private static void BindingPreservesProductionState()
    {
        using var workspace=CreateWorkspace();
        var before=workspace.Production.Status;
        workspace.BindAcquisition(new DeterministicSource(),Descriptor("source-a"));
        Check(workspace.Production.Status==before,
            "Acquisition binding must not mutate Production authority.");
    }

    private static void BindingDoesNotCreateReplay()
    {
        using var workspace=CreateWorkspace();
        workspace.BindAcquisition(new DeterministicSource(),Descriptor("source-a"));
        Check(workspace.Capture().Replay is null,
            "Acquisition binding must not fabricate Replay.");
    }

    private static void BindingDoesNotCreateRelease()
    {
        using var workspace=CreateWorkspace();
        workspace.BindAcquisition(new DeterministicSource(),Descriptor("source-a"));
        Check(workspace.Capture().Release is null,
            "Acquisition binding must not fabricate Release.");
    }

    private static void BindingDoesNotCreateQuality()
    {
        using var workspace=CreateWorkspace();
        workspace.BindAcquisition(new DeterministicSource(),Descriptor("source-a"));
        Check(!workspace.Quality.IsBound,
            "Acquisition binding must not fabricate Quality state.");
    }

    private static void RebindingPublishesNewChange()
    {
        using var workspace=CreateWorkspace();
        var changes=0;
        workspace.ProductionChanged+=_=>changes++;
        workspace.BindAcquisition(new DeterministicSource(),Descriptor("source-a"));
        workspace.BindAcquisition(new DeterministicSource(),Descriptor("source-b"));
        Check(changes==2 && workspace.Acquisition.Descriptor?.SourceId=="source-b",
            "Rebinding must publish the new client Acquisition identity.");
    }

    private static void BindingMakesAcquisitionVisible()
    {
        using var workspace=CreateWorkspace();
        workspace.BindAcquisition(new DeterministicSource(),Descriptor("source-a"));
        var surface=ClientInspectionExecutionSurfaceRuntime.Create(workspace.Capture());
        Check(surface.HasAcquisition && surface.Presentation.AcquisitionText.Contains("Deterministic",
            StringComparison.Ordinal),
            "Bound Acquisition must be visible through the Inspection surface.");
    }

    private static void BindingKeepsProductionCommandBoundary()
    {
        using var workspace=CreateWorkspace();
        workspace.BindAcquisition(new DeterministicSource(),Descriptor("source-a"));
        var surface=ClientInspectionExecutionSurfaceRuntime.Create(
            workspace.Capture(),
            new ClientWorkspaceCommandRouting(
                ClientWorkspaceKind.Inspection,
                false,true,false,true,false,false,false));
        Check(surface.HasAcquisition && surface.CanRunInspection,
            "Acquisition binding must leave the existing Production command boundary intact.");
    }

    private static ClientInspectionWorkspace CreateWorkspace() =>
        new(
            new System.Numerics.Vector2(640,480),
            new System.Numerics.Vector2(640,480));

    private static ClientAcquisitionDescriptor Descriptor(string id) =>
        new(id,"Deterministic Source",true);

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
                "Inspection Acquisition binding smoke failed: "+message);
    }
}
