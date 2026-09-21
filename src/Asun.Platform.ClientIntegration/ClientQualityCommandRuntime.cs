namespace Asun.Platform.ClientIntegration;

public static class ClientQualityCommandRuntime
{
    public static bool SelectFinding(
        ClientInspectionWorkspace workspace,
        ClientWorkspaceCommandRouting routing,
        string findingId)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        ArgumentNullException.ThrowIfNull(routing);

        if(routing.Workspace!=ClientWorkspaceKind.Quality ||
           !routing.CanReviewQuality)
            throw new InvalidOperationException(
                "Quality finding selection is not available in the current workspace.");

        return workspace.SelectQualityFinding(findingId);
    }

    public static void Clear(
        ClientInspectionWorkspace workspace,
        ClientWorkspaceCommandRouting routing)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        ArgumentNullException.ThrowIfNull(routing);

        if(routing.Workspace!=ClientWorkspaceKind.Quality ||
           !routing.CanResetSession)
            throw new InvalidOperationException(
                "Quality clear command is not available in the current workspace.");

        workspace.ClearQualityRun();
    }
}
