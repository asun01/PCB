namespace Asun.Platform.ClientIntegration;

public static class ClientInspectionAcquisitionCommandRuntime
{
    public static bool BindSource(
        ClientInspectionWorkspace workspace,
        ClientWorkspaceCommandRouting routing,
        string sourceId)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        ArgumentNullException.ThrowIfNull(routing);

        if(routing.Workspace!=ClientWorkspaceKind.Inspection)
            throw new InvalidOperationException(
                "Acquisition source binding is not available in the current workspace.");

        if(!routing.CanBindAcquisition)
            throw new InvalidOperationException(
                "Acquisition source binding is not available under the current Inspection command routing.");

        if(string.IsNullOrWhiteSpace(sourceId))
            throw new ArgumentException(
                "Acquisition source id cannot be blank.",
                nameof(sourceId));

        return workspace.BindAcquisitionSource(sourceId);
    }

    public static ValueTask<ClientAcquisitionPreviewSnapshot> Preview(
        ClientInspectionWorkspace workspace,
        ClientInspectionExecutionSurface surface,
        CancellationToken cancellationToken=default)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        ArgumentNullException.ThrowIfNull(surface);

        return ClientInspectionExecutionCommandRuntime.PreviewAcquisitionAsync(
            workspace,
            surface,
            cancellationToken);
    }
}
