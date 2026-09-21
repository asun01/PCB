namespace Asun.Platform.ClientIntegration;

public sealed record ClientWorkspaceClientSnapshot(
    ClientWorkspaceSelection Selection,
    ClientWorkspaceCommandRouting Routing,
    ClientWorkspaceContentSurface Content);

public static class ClientWorkspaceClientSnapshotRuntime
{
    public static ClientWorkspaceClientSnapshot Create(
        ClientWorkspaceSelection selection,
        ClientWorkspaceCommandRouting routing,
        ClientInspectionWorkspaceSnapshot inspection)
    {
        ArgumentNullException.ThrowIfNull(inspection);

        if(selection.Workspace!=routing.Workspace)
            throw new ArgumentException(
                "Workspace selection and command routing must target the same workspace.",
                nameof(routing));

        var content=ClientWorkspaceContentSurfaceRuntime.Create(
            selection,
            routing,
            inspection);

        return new ClientWorkspaceClientSnapshot(
            selection,
            routing,
            content);
    }

    public static ClientWorkspaceClientSnapshot Create(
        ClientWorkspaceSelection selection,
        ClientCommandAvailability availability,
        ClientInspectionWorkspaceSnapshot inspection)
    {
        ArgumentNullException.ThrowIfNull(inspection);

        var routing=ClientWorkspaceCommandRoutingRuntime.Create(
            selection,
            availability);

        var content=ClientWorkspaceContentSurfaceRuntime.Create(
            selection,
            routing,
            inspection);

        return new ClientWorkspaceClientSnapshot(
            selection,
            routing,
            content);
    }
}
