namespace Asun.Platform.ClientIntegration;

public sealed record ClientWorkspaceContentSurface(
    ClientWorkspaceSelection Selection,
    ClientWorkspaceCommandRouting Routing,
    ClientProgramSurface Program,
    ClientInspectionExecutionSurface Inspection,
    ClientQualitySurface Quality,
    ClientResultsSurface Results);

public static class ClientWorkspaceContentSurfaceRuntime
{
    public static ClientWorkspaceContentSurface Create(
        ClientWorkspaceSelection selection,
        ClientWorkspaceCommandRouting routing,
        ClientInspectionWorkspaceSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        if(selection.Workspace!=routing.Workspace)
            throw new ArgumentException(
                "Workspace selection and command routing must target the same workspace.",
                nameof(routing));

        return new ClientWorkspaceContentSurface(
            selection,
            routing,
            ClientProgramSurfaceRuntime.Create(snapshot),
            ClientInspectionExecutionSurfaceRuntime.Create(snapshot,routing),
            ClientQualitySurfaceRuntime.Create(snapshot),
            ClientResultsSurfaceRuntime.Create(snapshot));
    }
}
