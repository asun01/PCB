namespace Asun.Platform.ClientIntegration;

public enum ClientWorkflowStep
{
    LoadProgram,
    BindAcquisition,
    RunInspection,
    ReviewQuality,
    ReviewResults,
    Ready
}

public sealed record ClientInspectionWorkflowSnapshot(
    ClientWorkflowStep Step,
    bool CanAct,
    string Message,
    ClientExecutionStatus ProductionStatus,
    bool AcquisitionReady,
    bool QualityBound,
    int HistoryCount,
    string Fingerprint);

public static class ClientInspectionWorkflowRuntime
{
    public static ClientInspectionWorkflowSnapshot Evaluate(
        ClientInspectionWorkspaceSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        ClientWorkflowStep step;
        bool canAct;
        string message;

        if(snapshot.Program?.Status!=ClientProgramLoadStatus.Ready)
        {
            step=ClientWorkflowStep.LoadProgram;
            canAct=true;
            message="Load a valid inspection Program.";
        }
        else if(!snapshot.Acquisition.CanCapture)
        {
            step=ClientWorkflowStep.BindAcquisition;
            canAct=true;
            message="Bind a ready Acquisition source.";
        }
        else if(snapshot.Production.Status==ClientExecutionStatus.Ready)
        {
            step=ClientWorkflowStep.RunInspection;
            canAct=true;
            message="Run the inspection session.";
        }
        else if(snapshot.Production.Status==ClientExecutionStatus.Cancelled)
        {
            step=ClientWorkflowStep.RunInspection;
            canAct=false;
            message="Production was cancelled; reset the session before starting another execution.";
        }
        else if(snapshot.Production.Status==ClientExecutionStatus.Failed)
        {
            step=ClientWorkflowStep.RunInspection;
            canAct=false;
            message="Production failed; reset the session before retrying execution.";
        }
        else if(snapshot.Production.Status==ClientExecutionStatus.Running)
        {
            step=ClientWorkflowStep.RunInspection;
            canAct=false;
            message=$"Inspection is running: {snapshot.Production.FramesProcessed}/{snapshot.Production.TargetFrameCount} frame(s).";
        }
        else if(snapshot.Production.Status==ClientExecutionStatus.Completed && !snapshot.Quality.IsBound)
        {
            step=ClientWorkflowStep.ReviewQuality;
            canAct=false;
            message="Production completed; waiting for an authoritative Quality Run.";
        }
        else if(snapshot.Production.Status==ClientExecutionStatus.Completed &&
                snapshot.Quality.IsBound &&
                snapshot.History.Entries.Count==0)
        {
            step=ClientWorkflowStep.ReviewResults;
            canAct=false;
            message="Quality is available; Results history has not yet been populated.";
        }
        else
        {
            step=ClientWorkflowStep.ReviewResults;
            canAct=true;
            message="Review the current Result or historical runs.";
        }

        var canonical=string.Join(
            "|",
            step,
            canAct,
            message,
            snapshot.Production.Status,
            snapshot.Production.FramesProcessed,
            snapshot.Production.TargetFrameCount,
            snapshot.Acquisition.State,
            snapshot.Quality.IsBound,
            snapshot.History.Entries.Count,
            snapshot.Production.ActiveSessionId,
            snapshot.Production.LastReportFingerprint);

        var fingerprint=Convert.ToHexString(
            System.Security.Cryptography.SHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes(canonical)))
            .ToLowerInvariant();

        return new ClientInspectionWorkflowSnapshot(
            step,
            canAct,
            message,
            snapshot.Production.Status,
            snapshot.Acquisition.State==ClientAcquisitionState.Ready,
            snapshot.Quality.IsBound,
            snapshot.History.Entries.Count,
            fingerprint);
    }
}
