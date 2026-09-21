namespace Asun.Platform.ClientIntegration;

public static class ClientResultsCommandRuntime
{
    public static bool SelectHistory(
        ClientInspectionWorkspace workspace,
        ClientWorkspaceCommandRouting routing,
        long ordinal)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        ArgumentNullException.ThrowIfNull(routing);

        if(routing.Workspace!=ClientWorkspaceKind.Results ||
           !routing.CanReviewResults)
            throw new InvalidOperationException(
                "Results history selection is not available in the current workspace.");

        return workspace.SelectHistory(ordinal);
    }
}
