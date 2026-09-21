namespace Asun.Platform.ClientIntegration;

public static class ClientInspectionExecutionCommandRuntime
{
    public static ValueTask<ClientAcquisitionPreviewSnapshot> PreviewAcquisitionAsync(
        ClientInspectionWorkspace workspace,
        ClientInspectionExecutionSurface surface,
        CancellationToken cancellationToken=default)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        ArgumentNullException.ThrowIfNull(surface);

        if(!surface.CanPreviewAcquisition)
            throw new InvalidOperationException("Acquisition preview command is not available in the current Inspection surface.");

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

        if(!surface.CanRunInspection)
            throw new InvalidOperationException("Inspection execution command is not available in the current Inspection surface.");

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
