namespace Asun.Platform.ClientIntegration;

public sealed record ClientWorkspaceContentSurface(
    ClientWorkspaceSelection Selection,
    ClientWorkspaceCommandRouting Routing,
    ClientHomePresentationSnapshot Home,
    ClientProgramSurface Program,
    ClientInspectionExecutionSurface Inspection,
    ClientQualitySurface Quality,
    ClientResultsSurface Results)
{
    public ClientInspectionWorkflowSnapshot Workflow { get; init; }=
        ClientInspectionWorkflowRuntime.Evaluate(
            new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(
                    ClientExecutionStatus.Ready,
                    null,
                    null,
                    null,
                    0,
                    0,
                    0,
                    null,
                    null),
                null,
                null,
                null,
                new ClientProductionRunHistorySnapshot(
                    20,
                    1,
                    0,
                    Array.Empty<ClientProductionRunHistoryEntry>())));
};

public static class ClientWorkspaceContentSurfaceRuntime
{
    public static ClientWorkspaceContentSurface CreateValidated(
        ClientWorkspaceSelection selection,
        ClientWorkspaceCommandRouting routing,
        ClientInspectionWorkspaceSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var errors=ClientInspectionWorkflowIntegrityRuntime.Validate(snapshot);
        if(errors.Count>0)
            throw new InvalidOperationException(string.Join(" ",errors));

        return Create(selection,routing,snapshot);
    }

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
            ClientHomePresentationRuntime.Create(snapshot),
            ClientProgramSurfaceRuntime.Create(snapshot),
            ClientInspectionExecutionSurfaceRuntime.Create(snapshot,routing),
            ClientQualitySurfaceRuntime.Create(snapshot),
            ClientResultsSurfaceRuntime.Create(snapshot))
        {
            Workflow=ClientInspectionWorkflowRuntime.Evaluate(snapshot)
        };
    }
}
