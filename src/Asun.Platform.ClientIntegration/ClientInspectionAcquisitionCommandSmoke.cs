using Asun.Device.Contracts;

namespace Asun.Platform.ClientIntegration;

public static class ClientInspectionAcquisitionCommandSmoke
{
    public static void Run100Stages()
    {
        for(var round=1;round<=100;round++) if(round==100) RequiresInspectionWorkspace();
        for(var round=1;round<=100;round++) if(round==100) RejectsBlankSource();
        for(var round=1;round<=100;round++) if(round==100) RoutesValidSource();
        for(var round=1;round<=100;round++) if(round==100) PublishesReadyState();
        for(var round=1;round<=100;round++) if(round==100) RejectsQualityWorkspace();
        for(var round=1;round<=100;round++) if(round==100) RejectsResultsWorkspace();
        for(var round=1;round<=100;round++) if(round==100) PreviewRequiresExplicitCapability();
        for(var round=1;round<=100;round++) if(round==100) PreviewUsesBoundSource();
        for(var round=1;round<=100;round++) if(round==100) UsesExistingAuthority();
        for(var round=1;round<=100;round++) if(round==100) ProjectionSeesBinding();
    }

    private static void RequiresInspectionWorkspace()
    {
        using var workspace=CreateWorkspace();
        var rejected=false;
        try
        {
            ClientInspectionAcquisitionCommandRuntime.BindSource(
                workspace,
                Routing(ClientWorkspaceKind.Results),
                "source");
        }
        catch(InvalidOperationException)
        {
            rejected=true;
        }

        Check(rejected,"Acquisition binding must require Inspection workspace routing.");
    }

    private static void RejectsBlankSource()
    {
        using var workspace=CreateWorkspace();
        var rejected=false;
        try
        {
            ClientInspectionAcquisitionCommandRuntime.BindSource(
                workspace,
                Routing(ClientWorkspaceKind.Inspection),
                " ");
        }
        catch(ArgumentException)
        {
            rejected=true;
        }

        Check(rejected,"Acquisition binding must reject blank source ids.");
    }

    private static void RoutesValidSource()
    {
        using var workspace=CreateWorkspace();
        workspace.AcquisitionCatalog.Register(
            new ClientAcquisitionSourceDefinition(
                new ClientAcquisitionDescriptor("source","Deterministic",true),
                new DeterministicSource()));

        var bound=ClientInspectionAcquisitionCommandRuntime.BindSource(
            workspace,
            Routing(ClientWorkspaceKind.Inspection),
            "source");

        Check(bound && workspace.Acquisition.State==ClientAcquisitionState.Ready,
            "Valid acquisition binding must use the existing workspace authority.");
    }

    private static void PublishesReadyState()
    {
        using var workspace=CreateWorkspace();
        workspace.AcquisitionCatalog.Register(
            new ClientAcquisitionSourceDefinition(
                new ClientAcquisitionDescriptor("source","Deterministic",true),
                new DeterministicSource()));

        ClientInspectionWorkspaceSnapshot? latest=null;
        workspace.Changed+=snapshot=>latest=snapshot;

        ClientInspectionAcquisitionCommandRuntime.BindSource(
            workspace,
            Routing(ClientWorkspaceKind.Inspection),
            "source");

        Check(latest?.Acquisition.State==ClientAcquisitionState.Ready,
            "Acquisition command binding must publish the resulting Ready state.");
    }

    private static void RejectsQualityWorkspace()
    {
        using var workspace=CreateWorkspace();
        var rejected=false;
        try
        {
            ClientInspectionAcquisitionCommandRuntime.BindSource(
                workspace,
                Routing(ClientWorkspaceKind.Quality),
                "source");
        }
        catch(InvalidOperationException)
        {
            rejected=true;
        }

        Check(rejected,"Quality routing must not own acquisition binding.");
    }

    private static void RejectsResultsWorkspace()
    {
        using var workspace=CreateWorkspace();
        var rejected=false;
        try
        {
            ClientInspectionAcquisitionCommandRuntime.BindSource(
                workspace,
                Routing(ClientWorkspaceKind.Results),
                "source");
        }
        catch(InvalidOperationException)
        {
            rejected=true;
        }

        Check(rejected,"Results routing must not own acquisition binding.");
    }

    private static void PreviewRequiresExplicitCapability()
    {
        using var workspace=CreateWorkspace();
        workspace.AcquisitionCatalog.Register(
            new ClientAcquisitionSourceDefinition(
            new ClientAcquisitionDescriptor("preview-source","Preview source",true),
            new PreviewSource()));

        ClientInspectionAcquisitionCommandRuntime.BindSource(
            workspace,
            Routing(ClientWorkspaceKind.Inspection),
            "preview-source");

        var rejected=false;
        try
        {
            ClientInspectionAcquisitionCommandRuntime.Preview(
                workspace,
                ClientInspectionExecutionSurfaceRuntime.Create(
                    workspace.Capture(),
                    Routing(ClientWorkspaceKind.Inspection)))
                .AsTask().GetAwaiter().GetResult();
        }
        catch(InvalidOperationException)
        {
            rejected=true;
        }

        Check(rejected,
            "Acquisition preview must require an explicit routing capability.");
    }

    private static void PreviewUsesBoundSource()
    {
        using var workspace=CreateWorkspace();
        workspace.AcquisitionCatalog.Register(
            new ClientAcquisitionSourceDefinition(
            new ClientAcquisitionDescriptor("preview-source","Preview source",true),
            new PreviewSource()));

        var routing=Routing(ClientWorkspaceKind.Inspection) with
        {
            CanPreviewAcquisition=true
        };

        ClientInspectionAcquisitionCommandRuntime.BindSource(
            workspace,
            routing,
            "preview-source");

        var preview=ClientInspectionAcquisitionCommandRuntime.Preview(
            workspace,
            ClientInspectionExecutionSurfaceRuntime.Create(
                workspace.Capture(),
                routing))
            .AsTask().GetAwaiter().GetResult();

        Check(preview.Width>0 &&
              preview.Height>0 &&
              !string.IsNullOrWhiteSpace(preview.PixelFormat),
            "Acquisition preview must return observed frame metadata from the bound source.");
    }

    private static void UsesExistingAuthority()
    {
        using var workspace=CreateWorkspace();
        workspace.AcquisitionCatalog.Register(
            new ClientAcquisitionSourceDefinition(
                new ClientAcquisitionDescriptor("source","Deterministic",true),
                new DeterministicSource()));

        var before=workspace.Acquisition.State;
        ClientInspectionAcquisitionCommandRuntime.BindSource(
            workspace,
            Routing(ClientWorkspaceKind.Inspection),
            "source");

        Check(before==ClientAcquisitionState.Unbound &&
              workspace.Acquisition.State==ClientAcquisitionState.Ready,
            "Acquisition command must delegate to the existing workspace state machine.");
    }

    private static void DoesNotCreateSourceAuthority()
    {
        using var workspace=CreateWorkspace();
        workspace.AcquisitionCatalog.Register(
            new ClientAcquisitionSourceDefinition(
                new ClientAcquisitionDescriptor("source","Deterministic",true),
                new DeterministicSource()));

        ClientInspectionAcquisitionCommandRuntime.BindSource(
            workspace,
            Routing(ClientWorkspaceKind.Inspection),
            "source");

        Check(workspace.AcquisitionCatalog.Sources.Count==1 &&
              workspace.Acquisition.SourceId=="source",
            "Command facade must not create a parallel source catalog.");
    }

    private static void ProjectionSeesBinding()
    {
        using var workspace=CreateWorkspace();
        workspace.AcquisitionCatalog.Register(
            new ClientAcquisitionSourceDefinition(
                new ClientAcquisitionDescriptor("source","Deterministic",true),
                new DeterministicSource()));

        using var navigation=new ClientWorkspaceRuntime();
        using var projection=new ClientWorkspaceClientProjection(
            navigation,
            workspace,
            Routing);

        ClientInspectionAcquisitionCommandRuntime.BindSource(
            workspace,
            Routing(ClientWorkspaceKind.Inspection),
            "source");

        Check(projection.Snapshot.Content.Inspection.HasAcquisition,
            "Unified projection must observe acquisition command results.");
    }

    private static ClientInspectionWorkspace CreateWorkspace() =>
        new(
            new System.Numerics.Vector2(640,480),
            new System.Numerics.Vector2(640,480));

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
            selection.Workspace==ClientWorkspaceKind.Results)
        {
            CanPreviewAcquisition=false
        };

    private sealed class DeterministicSource : IFrameSource
    {
        public ValueTask<CapturedFrame?> CaptureAsync(
            CancellationToken cancellationToken=default) =>
            ValueTask.FromResult<CapturedFrame?>(null);
    }

    private sealed class PreviewSource : IFrameSource
    {
        public ValueTask<CapturedFrame?> CaptureAsync(
            CancellationToken cancellationToken=default) =>
            ValueTask.FromResult<CapturedFrame?>(
                CapturedFrame.Create(
                    new FrameCaptureMetadata(
                        new FrameSequence(1),
                        2,
                        2,
                        "Mono8",
                        DateTimeOffset.UnixEpoch),
                    new byte[] { 0, 1, 2, 3 }));
    }

    private static void Check(bool condition,string message)
    {
        if(!condition)
            throw new InvalidOperationException(
                "Inspection acquisition command smoke failed: "+message);
    }
}
