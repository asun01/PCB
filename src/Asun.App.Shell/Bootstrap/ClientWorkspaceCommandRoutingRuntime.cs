namespace Asun.App.Shell.Bootstrap;

public sealed record ClientWorkspaceCommandRouting(
    ClientWorkspaceKind Workspace,
    bool CanLoadProgram,
    bool CanRunInspection,
    bool CanCancelInspection,
    bool CanResetSession,
    bool CanEditRoi,
    bool CanReviewQuality,
    bool CanReviewResults);

public static class ClientWorkspaceCommandRoutingRuntime
{
    public static ClientWorkspaceCommandRouting Create(
        ClientWorkspaceSelection workspace,
        ClientCommandAvailability availability)
    {
        var canReviewResults=workspace.Workspace==ClientWorkspaceKind.Results;
        var canReviewQuality=workspace.Workspace==ClientWorkspaceKind.Quality;

        return workspace.Workspace switch
        {
            ClientWorkspaceKind.Home =>
                new(workspace.Workspace,
                    availability.CanLoad,
                    false,
                    false,
                    availability.CanReset,
                    false,
                    false,
                    false),

            ClientWorkspaceKind.Program =>
                new(workspace.Workspace,
                    availability.CanLoad,
                    false,
                    false,
                    availability.CanReset,
                    false,
                    false,
                    false),

            ClientWorkspaceKind.Inspection =>
                new(workspace.Workspace,
                    availability.CanLoad,
                    availability.CanRun,
                    availability.CanCancel,
                    availability.CanReset,
                    availability.CanSelectRoi || availability.CanCreateRoi,
                    false,
                    false),

            ClientWorkspaceKind.Quality =>
                new(workspace.Workspace,
                    false,
                    false,
                    false,
                    availability.CanReset,
                    false,
                    true,
                    false),

            ClientWorkspaceKind.Results =>
                new(workspace.Workspace,
                    false,
                    false,
                    false,
                    availability.CanReset,
                    false,
                    false,
                    canReviewResults),

            _ =>
                throw new ArgumentOutOfRangeException(nameof(workspace.Workspace))
        };
    }
}
