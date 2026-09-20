namespace Asun.Platform.ClientIntegration;

public sealed record ClientInspectionExecutionSurface(
    ClientInspectionExecutionPresentation Presentation,
    bool HasAcquisition,
    bool HasCompletedResult,
    bool CanInteractWithRoi)
{
    public ClientWorkspaceCommandRouting? CommandRouting { get; init; }
}

public static class ClientInspectionExecutionSurfaceRuntime
{
    public static ClientInspectionExecutionSurface Create(
        ClientInspectionWorkspaceSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var presentation=ClientInspectionExecutionPresentationRuntime.Create(snapshot);

        return new ClientInspectionExecutionSurface(
            presentation,
            snapshot.Acquisition.State==ClientAcquisitionState.Bound,
            snapshot.Production.Status==ClientExecutionStatus.Completed,
            snapshot.Roi is not null);
    }

    public static ClientInspectionExecutionSurface Create(
        ClientInspectionWorkspaceSnapshot snapshot,
        ClientWorkspaceCommandRouting commandRouting)
    {
        ArgumentNullException.ThrowIfNull(commandRouting);

        return Create(snapshot) with
        {
            CommandRouting=commandRouting
        };
    }
}
