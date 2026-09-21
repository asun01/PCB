namespace Asun.Platform.ClientIntegration;

public static class ClientProgramCommandRuntime
{
    public static bool SelectStep(
        ClientInspectionWorkspace workspace,
        ClientWorkspaceCommandRouting routing,
        Guid stepId)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        ArgumentNullException.ThrowIfNull(routing);

        if(routing.Workspace!=ClientWorkspaceKind.Program)
            throw new InvalidOperationException(
                "Program step selection is not available in the current workspace.");

        if(stepId==Guid.Empty)
            throw new ArgumentException("Program step id cannot be empty.",nameof(stepId));

        return workspace.SelectProgramStep(stepId);
    }
}
