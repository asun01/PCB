namespace Asun.Platform.ClientIntegration;

public static class ClientInspectionExecutionCommandRuntime
{
    public static ClientProgramWorkspaceSnapshot LoadProgram(
        ClientInspectionWorkspace workspace,
        ClientInspectionExecutionSurface surface,
        Asun.Program.Core.InspectionProgram program,
        Asun.Platform.Pipeline.PipelineDefinition<Asun.Device.Contracts.CapturedFrame> pipeline,
        Guid sessionId,
        int frameCount)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        ArgumentNullException.ThrowIfNull(surface);

        if(surface.CommandRouting?.CanLoadProgram!=true)
            throw new InvalidOperationException("Program load command is not available in the current Inspection surface.");

        ArgumentNullException.ThrowIfNull(program);
        ArgumentNullException.ThrowIfNull(pipeline);

        return workspace.LoadProgram(program,pipeline,sessionId,frameCount);
    }

    public static ValueTask<ClientAcquisitionPreviewSnapshot> PreviewAcquisitionAsync(
        ClientInspectionWorkspace workspace,
        ClientInspectionExecutionSurface surface,
        CancellationToken cancellationToken=default)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        ArgumentNullException.ThrowIfNull(surface);

        if(surface.CommandRouting?.CanPreviewAcquisition!=true)
            throw new InvalidOperationException("Acquisition preview command is not available under the current Inspection command routing.");

        return workspace.PreviewAcquisitionAsync(cancellationToken);
    }

    public static ValueTask<Asun.Production.Runtime.ProductionSessionReport> ExecuteAsync(
        ClientInspectionWorkspace workspace,
        ClientInspectionExecutionSurface surface,
        Asun.Release.Core.ReleaseManifest releaseManifest,
        CancellationToken cancellationToken=default)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(releaseManifest);

        if(surface.RunReadinessState!=ClientRunReadinessState.Ready || !surface.CanRunInspection)
            throw new InvalidOperationException("Inspection execution requires an authoritative Ready Inspection surface.");

        return workspace.ExecuteAsync(releaseManifest,cancellationToken);
    }

    public static void Cancel(
        ClientInspectionWorkspace workspace,
        ClientInspectionExecutionSurface surface)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        ArgumentNullException.ThrowIfNull(surface);

        if(!surface.CanCancelInspection)
            throw new InvalidOperationException("Inspection cancellation command is not available in the current Inspection surface.");

        workspace.CancelExecution();
    }

    public static void Reset(
        ClientInspectionWorkspace workspace,
        ClientInspectionExecutionSurface surface)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        ArgumentNullException.ThrowIfNull(surface);

        if(surface.CommandRouting?.CanResetSession!=true)
            throw new InvalidOperationException("Inspection reset command is not available in the current Inspection surface.");

        workspace.ResetCurrentSession();
    }

    public static bool SetRoiMode(
        ClientInspectionWorkspace workspace,
        ClientInspectionExecutionSurface surface,
        RoiEditorMode mode)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        ArgumentNullException.ThrowIfNull(surface);

        if(!surface.CanEditRoi)
            return false;

        workspace.SetRoiMode(mode);
        return true;
    }

    public static bool UndoRoi(
        ClientInspectionWorkspace workspace,
        ClientInspectionExecutionSurface surface)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        ArgumentNullException.ThrowIfNull(surface);

        if(!surface.CanUndoRoiCommand)
            return false;

        return workspace.UndoRoi();
    }

    public static bool RedoRoi(
        ClientInspectionWorkspace workspace,
        ClientInspectionExecutionSurface surface)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        ArgumentNullException.ThrowIfNull(surface);

        if(!surface.CanRedoRoiCommand)
            return false;

        return workspace.RedoRoi();
    }

    public static bool SubmitRoiInput(
        ClientInspectionWorkspace workspace,
        ClientInspectionExecutionSurface surface,
        ViewportInputEventKind kind,
        System.Numerics.Vector2 viewportPoint,
        int wheelDelta=0,
        ViewportMouseButton button=ViewportMouseButton.Left)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        ArgumentNullException.ThrowIfNull(surface);

        if(!surface.CanEditRoi)
            return false;

        return workspace.SubmitRoiInput(kind,viewportPoint,wheelDelta,button);
    }
}
