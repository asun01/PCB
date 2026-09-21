namespace Asun.Platform.ClientIntegration;

public static class ClientInspectionWorkflowIntegrityRuntime
{
    public static IReadOnlyList<string> Validate(
        ClientInspectionWorkspaceSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var errors=new List<string>();

        if(snapshot.Acquisition.State==ClientAcquisitionState.Faulted)
        {
            if(snapshot.Acquisition.CanCapture)
                errors.Add("Faulted Acquisition cannot advertise capture readiness.");
            if(snapshot.Acquisition.Preview is not null)
                errors.Add("Faulted Acquisition cannot retain a Preview.");
        }

        var program=snapshot.Program;
        if(program?.Status==ClientProgramLoadStatus.Ready)
        {
            if(snapshot.ProgramItems.Count!=program.StepCount)
                errors.Add("Ready Program step count must match ProgramItems count.");

            if(program.SelectedStepId is Guid selected &&
               !snapshot.ProgramItems.Any(item=>item.StepId==selected))
            {
                errors.Add("Selected Program step must exist in ProgramItems.");
            }
        }

        if(program?.Status==ClientProgramLoadStatus.Invalid &&
           snapshot.ProgramItems.Count!=0)
        {
            errors.Add("Invalid Program cannot expose ProgramItems.");
        }

        if(snapshot.Production.Status==ClientExecutionStatus.Completed)
        {
            if(snapshot.Replay is null)
                errors.Add("Completed Production must have Replay context.");
            if(snapshot.Release is null)
                errors.Add("Completed Production must have Release projection.");
        }

        if(snapshot.Replay is not null)
        {
            if(snapshot.Replay.Status!=ClientExecutionStatus.Completed)
                errors.Add("Replay context must represent completed execution.");

            if(snapshot.Release is not null)
            {
                if(snapshot.Replay.ProgramId!=snapshot.Release.ProgramId)
                    errors.Add("Replay and Release Program identities must match.");
                if(snapshot.Replay.ProductionSessionId!=snapshot.Release.ProductionSessionId)
                    errors.Add("Replay and Release session identities must match.");
                if(snapshot.Replay.ReplayFingerprint!=snapshot.Release.ReplayFingerprint)
                    errors.Add("Replay and Release fingerprints must match.");
            }
        }

        if(snapshot.Quality.IsBound &&
           snapshot.Production.Status!=ClientExecutionStatus.Completed)
        {
            errors.Add("Quality cannot be bound before completed Production.");
        }

        if(snapshot.SelectedHistoryOrdinal is long selectedOrdinal &&
           !snapshot.History.Entries.Any(entry=>entry.Ordinal==selectedOrdinal))
        {
            errors.Add("Selected historical run must exist in bounded Run History.");
        }

        return errors;
    }

    public static bool IsValid(
        ClientInspectionWorkspaceSnapshot snapshot)=>
        Validate(snapshot).Count==0;
}
