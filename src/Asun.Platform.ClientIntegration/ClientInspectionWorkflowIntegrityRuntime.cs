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

        if(snapshot.Production.Status==ClientExecutionStatus.Completed &&
           !snapshot.Quality.IsBound &&
           (snapshot.Replay is not null || snapshot.Release is not null))
        {
            errors.Add("Quality-pending Production cannot expose Replay or Release evidence.");
        }

        if(snapshot.Production.Status==ClientExecutionStatus.Completed &&
           snapshot.Quality.IsBound)
        {
            if(snapshot.Replay is null)
                errors.Add("Bound Quality must finalize Replay context.");
            if(snapshot.Release is null)
                errors.Add("Bound Quality must finalize Release projection.");
        }

        if(snapshot.Release is not null && snapshot.Replay is null)
        {
            errors.Add("Release projection requires an authoritative Replay context.");
        }

        if(snapshot.Replay is not null)
        {
            if(!snapshot.Quality.IsBound)
                errors.Add("Replay context requires an authoritative Quality result.");
            if(snapshot.Replay.Status!=ClientExecutionStatus.Completed)
                errors.Add("Replay context must represent completed execution.");
            if(snapshot.Replay.QualityFingerprint!=snapshot.Quality.Fingerprint)
                errors.Add("Replay Quality fingerprint must match the authoritative Quality result.");

            if(snapshot.Release is not null)
            {
                if(snapshot.Replay.ProgramId!=snapshot.Release.ProgramId)
                    errors.Add("Replay and Release Program identities must match.");
                if(snapshot.Replay.ProductionSessionId!=snapshot.Release.ProductionSessionId)
                    errors.Add("Replay and Release session identities must match.");
                if(snapshot.Replay.ReplayFingerprint!=snapshot.Release.ReplayFingerprint)
                    errors.Add("Replay and Release fingerprints must match.");
                if(snapshot.Replay.QualityFingerprint!=snapshot.Release.QualityFingerprint)
                    errors.Add("Replay and Release Quality fingerprints must match.");
            }
        }

        if(snapshot.Quality.IsBound &&
           snapshot.Production.Status!=ClientExecutionStatus.Completed)
        {
            errors.Add("Quality cannot be bound before completed Production.");
        }

        foreach(var entry in snapshot.History.Entries)
        {
            if(!IsLowerHex(entry.ReplayFingerprint))
                errors.Add($"History entry {entry.Ordinal} has an invalid Replay fingerprint.");

            if(!IsLowerHex(entry.QualityFingerprint))
                errors.Add($"History entry {entry.Ordinal} has an invalid Quality fingerprint.");
        }

        if(snapshot.Replay is not null &&
           snapshot.Release is not null &&
           snapshot.Quality.IsBound)
        {
            var currentHistory=snapshot.History.Entries.LastOrDefault(entry=>
                entry.ProductionSessionId==snapshot.Replay.ProductionSessionId &&
                entry.ReplayFingerprint==snapshot.Replay.ReplayFingerprint);

            if(currentHistory is null)
                errors.Add("Finalized Replay/Release authority must be represented in Run History.");
            else if(currentHistory.QualityFingerprint!=snapshot.Quality.Fingerprint)
                errors.Add("Current Run History Quality fingerprint must match authoritative Quality.");
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

    private static bool IsLowerHex(string value)=>
        value is not null &&
        value.Length==64 &&
        value.All(c=>Uri.IsHexDigit(c) && char.ToLowerInvariant(c)==c);
}
