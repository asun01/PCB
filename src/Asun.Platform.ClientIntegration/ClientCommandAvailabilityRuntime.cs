namespace Asun.Platform.ClientIntegration;

public sealed record ClientCommandAvailability(
    bool CanLoad,
    bool CanRun,
    bool CanCancel,
    bool CanReset,
    bool CanSelectRoi,
    bool CanCreateRoi,
    bool CanUndoRoi,
    bool CanRedoRoi);

public static class ClientCommandAvailabilityRuntime
{
    public static ClientCommandAvailability Create(
        ClientInspectionWorkspaceSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        return snapshot.Production.Status switch
        {
            ClientExecutionStatus.Idle =>
                new(true,false,false,false,false,false,false,false),

            ClientExecutionStatus.Ready =>
                new(true,snapshot.Acquisition.CanCapture,false,true,false,false,false,false),

            ClientExecutionStatus.Running =>
                new(false,false,true,false,false,false,false,false),

            ClientExecutionStatus.Completed =>
                new(true,snapshot.Acquisition.CanCapture,false,true,true,true,snapshot.CanUndoRoi,snapshot.CanRedoRoi),

            ClientExecutionStatus.Cancelled =>
                new(true,snapshot.Acquisition.CanCapture,false,true,false,false,false,false),

            ClientExecutionStatus.Failed =>
                new(true,snapshot.Acquisition.CanCapture,false,true,false,false,false,false),

            _ =>
                new(false,false,false,false,false,false,false,false)
        };
    }
}
