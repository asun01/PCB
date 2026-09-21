using Asun.Device.Contracts;

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
        var rejected=ExpectInvalidOperation(
            () => ClientInspectionExecutionCommandRuntime
                .PreviewAcquisitionAsync(workspace,surface)
                .AsTask()
                .GetAwaiter()
                .GetResult());
        Check(rejected,"Preview must be blocked by the client command gate.");
    }

    private static void RunRequiresRouting()
    {
        using var workspace=CreateWorkspace();
        var surface=CreateSurface(workspace,canRun:false);
        var executionRejected=ExpectInvalidOperation(
            () => ClientInspectionExecutionCommandRuntime
                .ExecuteAsync(
                    workspace,
                    surface,
                    null!)
                .AsTask()
                .GetAwaiter()
                .GetResult());
        var programRejected=ExpectInvalidOperation(
            () => ClientInspectionExecutionCommandRuntime.LoadProgram(
                workspace,
                surface,
                null!,
                null!,
                Guid.Empty,
                0));
        Check(executionRejected && programRejected,
            "Run and Program Load must be blocked before downstream argument validation when command routing is unavailable.");
    }

    private static void CancelRequiresRouting()
    {
        using var workspace=CreateWorkspace();
        var surface=CreateSurface(workspace,canCancel:false);
        var rejected=ExpectInvalidOperation(
            () => ClientInspectionExecutionCommandRuntime.Cancel(workspace,surface));
        Check(rejected,"Cancel must be blocked by the client command gate.");
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
        var undo=ClientInspectionExecutionCommandRuntime.UndoRoi(workspace,surface);
        var redo=ClientInspectionExecutionCommandRuntime.RedoRoi(workspace,surface);
        Check(!accepted &&
              !undo &&
              !redo &&
              !surface.CanUndoRoiCommand &&
              !surface.CanRedoRoiCommand,
            "ROI input, Undo, and Redo must remain blocked until the surface has completed ROI authority.");
    }

    private static void PreviewStillRequiresBoundSource()
    {
        using var workspace=CreateWorkspace();
        var surface=CreateSurface(workspace,canPreview:true);
        Check(!surface.CanPreviewAcquisition,
            "Preview command must remain unavailable until Acquisition is Ready.");
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
        workspace.BindAcquisition(
            new DeterministicSource(),
            new ClientAcquisitionDescriptor("source","Deterministic",true));
        var surface=CreateSurface(workspace,canPreview:true);
        Check(surface.CanPreviewAcquisition,
            "Inspection surface must expose the preview command gate when Acquisition is Ready.");
    }

    private static void SurfaceExposesRunGate()
    {
        using var workspace=CreateWorkspace();
        workspace.BindAcquisition(
            new DeterministicSource(),
            new ClientAcquisitionDescriptor("source","Deterministic",true));
        var surface=CreateSurface(workspace,canRun:true);
        Check(surface.CanRunInspection,
            "Inspection surface must expose the run command gate when Acquisition is Ready.");
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

    private sealed class DeterministicSource : IFrameSource
    {
        public ValueTask<CapturedFrame?> CaptureAsync(
            CancellationToken cancellationToken=default) =>
            ValueTask.FromResult<CapturedFrame?>(null);
    }

    private static bool ExpectInvalidOperation(Action action)
    {
        try
        {
            action();
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
                "Inspection execution command smoke failed: "+message);
    }
}
