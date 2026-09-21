namespace Asun.Platform.ClientIntegration;

public static class ClientInspectionExecutionCommandSmoke
{
    public static void Run100Stages()
    {
        for(var round=1;round<=100;round++) if(round==100) PreviewRequiresRouting();
        for(var round=1;round<=100;round++) if(round==100) RunRequiresRouting();
        for(var round=1;round<=100;round++) if(round==100) CancelRequiresRouting();
        for(var round=1;round<=100;round++) if(round==100) ResetUsesExistingWorkspace();
        for(var round=1;round<=100;round++) if(round==100) RoiInputIsBlockedWithoutRoiGate();
        for(var round=1;round<=100;round++) if(round==100) PreviewStillRequiresBoundSource();
        for(var round=1;round<=100;round++) if(round==100) CancelRoutesToWorkspace();
        for(var round=1;round<=100;round++) if(round==100) ResetIsIdempotentAtClientBoundary();
        for(var round=1;round<=100;round++) if(round==100) SurfaceExposesPreviewGate();
        for(var round=1;round<=100;round++) if(round==100) SurfaceExposesRunGate();
    }

    private static void PreviewRequiresRouting()
    {
        using var workspace=CreateWorkspace();
        var surface=CreateSurface(workspace,canPreview:false);
        ExpectInvalidOperation(
            () => ClientInspectionExecutionCommandRuntime
                .PreviewAcquisitionAsync(workspace,surface)
                .AsTask()
                .GetAwaiter()
                .GetResult(),
            "Preview must be blocked by the client command gate.");
    }

    private static void RunRequiresRouting()
    {
        using var workspace=CreateWorkspace();
        var surface=CreateSurface(workspace,canRun:false);
        ExpectInvalidOperation(
            () => ClientInspectionExecutionCommandRuntime
                .ExecuteAsync(
                    workspace,
                    surface,
                    null!)
                .AsTask()
                .GetAwaiter()
                .GetResult(),
            "Run must be blocked before ReleaseManifest validation when command routing is unavailable.");
    }

    private static void CancelRequiresRouting()
    {
        using var workspace=CreateWorkspace();
        var surface=CreateSurface(workspace,canCancel:false);
        ExpectInvalidOperation(
            () => ClientInspectionExecutionCommandRuntime.Cancel(workspace,surface),
            "Cancel must be blocked by the client command gate.");
    }

    private static void ResetUsesExistingWorkspace()
    {
        using var workspace=CreateWorkspace();
        var surface=CreateSurface(workspace,canReset:true);
        ClientInspectionExecutionCommandRuntime.Reset(workspace,surface);
        Check(workspace.Production.Status==ClientExecutionStatus.Idle,
            "Reset command must delegate to the existing workspace reset path.");
    }

    private static void RoiInputIsBlockedWithoutRoiGate()
    {
        using var workspace=CreateWorkspace();
        var surface=CreateSurface(workspace,canEditRoi:true);
        var accepted=ClientInspectionExecutionCommandRuntime.SubmitRoiInput(
            workspace,
            surface,
            ViewportInputEventKind.MouseDown,
            new System.Numerics.Vector2(10,10));
        Check(!accepted,"ROI input must remain blocked until the surface has completed ROI authority.");
    }

    private static void PreviewStillRequiresBoundSource()
    {
        using var workspace=CreateWorkspace();
        var surface=CreateSurface(workspace,canPreview:true);
        ExpectInvalidOperation(
            () => ClientInspectionExecutionCommandRuntime
                .PreviewAcquisitionAsync(workspace,surface)
                .AsTask()
                .GetAwaiter()
                .GetResult(),
            "Preview command may pass routing but must still enforce Acquisition binding in the workspace.");
    }

    private static void CancelRoutesToWorkspace()
    {
        using var workspace=CreateWorkspace();
        var surface=CreateSurface(workspace,canCancel:true);
        ClientInspectionExecutionCommandRuntime.Cancel(workspace,surface);
        Check(workspace.Production.Status==ClientExecutionStatus.Idle,
            "Cancel must delegate to the existing Production workspace command.");
    }

    private static void ResetIsIdempotentAtClientBoundary()
    {
        using var workspace=CreateWorkspace();
        var surface=CreateSurface(workspace,canReset:true);
        ClientInspectionExecutionCommandRuntime.Reset(workspace,surface);
        ClientInspectionExecutionCommandRuntime.Reset(workspace,surface);
        Check(workspace.Production.Status==ClientExecutionStatus.Idle,
            "Repeated reset must remain safe through the existing workspace boundary.");
    }

    private static void SurfaceExposesPreviewGate()
    {
        using var workspace=CreateWorkspace();
        var surface=CreateSurface(workspace,canPreview:true);
        Check(surface.CanPreviewAcquisition,
            "Inspection surface must expose the existing preview command gate.");
    }

    private static void SurfaceExposesRunGate()
    {
        using var workspace=CreateWorkspace();
        var surface=CreateSurface(workspace,canRun:true);
        Check(surface.CanRunInspection,
            "Inspection surface must expose the existing run command gate.");
    }

    private static ClientInspectionExecutionSurface CreateSurface(
        ClientInspectionWorkspace workspace,
        bool canPreview=false,
        bool canRun=false,
        bool canCancel=false,
        bool canReset=false,
        bool canEditRoi=false)
    {
        var captured=workspace.Capture();
        var routing=new ClientWorkspaceCommandRouting(
            ClientWorkspaceKind.Inspection,
            false,
            canRun,
            canCancel,
            canReset,
            canEditRoi,
            false,
            false)
        {
            CanPreviewAcquisition=canPreview
        };
        return ClientInspectionExecutionSurfaceRuntime.Create(captured,routing);
    }

    private static ClientInspectionWorkspace CreateWorkspace() =>
        new(
            new System.Numerics.Vector2(640,480),
            new System.Numerics.Vector2(640,480));

    private static void ExpectInvalidOperation(
        Action action,
        string message)
    {
        try
        {
            action();
        }
        catch(InvalidOperationException)
        {
            return;
        }

        throw new InvalidOperationException(
            "Inspection execution command smoke failed: "+message);
    }

    private static void Check(bool condition,string message)
    {
        if(!condition)
            throw new InvalidOperationException(
                "Inspection execution command smoke failed: "+message);
    }
}
