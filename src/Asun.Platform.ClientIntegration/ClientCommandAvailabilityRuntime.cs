namespace Asun.Platform.ClientIntegration;

public sealed record ClientCommandAvailability(
    bool CanLoad,
    bool CanRun,
    bool CanCancel,
    bool CanReset,
    bool CanSelectRoi,
    bool CanCreateRoi,
    bool CanUndoRoi,
    bool CanRedoRoi)
{
    public bool CanBindAcquisition { get; init; }
    public bool CanPreviewAcquisition { get; init; }
    public bool CanEvaluateSimulationQuality { get; init; }
};

public static class ClientCommandAvailabilityRuntime
{
    public static ClientCommandAvailability Create(
        ClientInspectionWorkspaceSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var result=snapshot.Production.Status switch
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

        return result with
        {
            CanBindAcquisition=
                snapshot.Production.Status!=ClientExecutionStatus.Running,
            CanPreviewAcquisition=
                snapshot.Acquisition.State==ClientAcquisitionState.Ready &&
                snapshot.Acquisition.CanCapture,
            CanEvaluateSimulationQuality=
                snapshot.Production.Status==ClientExecutionStatus.Completed &&
                !snapshot.Quality.IsBound
        };
    }
}
