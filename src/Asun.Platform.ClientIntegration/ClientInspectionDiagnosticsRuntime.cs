using System.Security.Cryptography;
using System.Text;

namespace Asun.Platform.ClientIntegration;

public sealed record ClientInspectionDiagnosticSnapshot(
    bool IsCoherent,
    IReadOnlyList<string> Errors,
    IReadOnlyList<string> Warnings,
    string Fingerprint);

public static class ClientInspectionDiagnosticsRuntime
{
    public static ClientInspectionDiagnosticSnapshot Analyze(
        ClientInspectionWorkspaceSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var errors=new List<string>();
        var warnings=new List<string>();

        ValidateProductionState(snapshot,errors,warnings);
        ValidateProgramAlignment(snapshot,errors);
        ValidateReleaseAndReplay(snapshot,errors);
        ValidateRoi(snapshot,errors);
        ValidateHistory(snapshot.History,errors);

        var canonical=string.Join(
            "|",
            snapshot.Production.Status,
            snapshot.Production.ProgramId,
            snapshot.Production.ActiveSessionId,
            snapshot.Production.LastFrameCount,
            snapshot.Production.LastReportFingerprint,
            snapshot.Replay?.ReplayFingerprint,
            snapshot.Release?.ProjectionFingerprint,
            snapshot.Roi?.InteractionFingerprint,
            snapshot.History.NextOrdinal,
            snapshot.History.DroppedCount,
            string.Join(",",snapshot.History.Entries.Select(entry=>entry.Ordinal)));

        return new ClientInspectionDiagnosticSnapshot(
            errors.Count==0,
            errors,
            warnings,
            Convert.ToHexString(
                SHA256.HashData(Encoding.UTF8.GetBytes(canonical)))
                .ToLowerInvariant());
    }

    public static bool IsCoherent(ClientInspectionWorkspaceSnapshot snapshot)=>
        Analyze(snapshot).IsCoherent;

    private static void ValidateProductionState(
        ClientInspectionWorkspaceSnapshot snapshot,
        List<string> errors,
        List<string> warnings)
    {
        switch(snapshot.Production.Status)
        {
            case ClientExecutionStatus.Idle:
            case ClientExecutionStatus.Ready:
            case ClientExecutionStatus.Running:
            case ClientExecutionStatus.Cancelled:
            case ClientExecutionStatus.Failed:
                if(snapshot.Replay is not null)
                    errors.Add("Non-completed client state cannot carry a replay snapshot.");
                if(snapshot.Release is not null)
                    errors.Add("Non-completed client state cannot carry a Release projection.");
                if(snapshot.Roi is not null)
                    warnings.Add("ROI projection is omitted until a completed Production execution is available.");
                break;

            case ClientExecutionStatus.Completed:
                if(snapshot.Replay is null)
                    errors.Add("Completed client state must carry a replay snapshot.");
                if(snapshot.Release is null)
                    errors.Add("Completed client state must carry a Release projection.");
                if(snapshot.Roi is null)
                    errors.Add("Completed client state must carry an ROI interaction projection.");
                break;

            default:
                errors.Add("Unknown client execution status.");
                break;
        }
    }

    private static void ValidateProgramAlignment(
        ClientInspectionWorkspaceSnapshot snapshot,
        List<string> errors)
    {
        if(snapshot.Program is null)
            return;

        if(snapshot.Program.Status==ClientProgramLoadStatus.Invalid)
        {
            errors.Add("Client inspection snapshot cannot expose an invalid Program as the active Program projection.");
            return;
        }

        if(snapshot.Production.ProgramId is Guid productionProgramId &&
           snapshot.Program.ProgramId!=productionProgramId)
            errors.Add("Client Program identity must match the Production workspace Program identity.");

        if(snapshot.Production.ProgramVersion is Version productionVersion &&
           snapshot.Program.Version!=productionVersion)
            errors.Add("Client Program version must match the Production workspace Program version.");

        if(snapshot.Production.Status!=ClientExecutionStatus.Idle &&
           snapshot.Program.Status!=ClientProgramLoadStatus.Ready)
            errors.Add("A non-idle Production workspace requires a Ready Program projection.");
    }

    private static void ValidateReleaseAndReplay(
        ClientInspectionWorkspaceSnapshot snapshot,
        List<string> errors)
    {
        if(snapshot.Replay is null || snapshot.Release is null)
            return;

        if(snapshot.Replay.ProgramId!=snapshot.Release.ProgramId)
            errors.Add("Replay and Release Program identities must match.");
        if(snapshot.Replay.ProductionSessionId!=snapshot.Release.ProductionSessionId)
            errors.Add("Replay and Release Production session identities must match.");
        if(snapshot.Replay.ReplayFingerprint!=snapshot.Release.ReplayFingerprint)
            errors.Add("Replay and Release projection identities must match.");
    }

    private static void ValidateRoi(
        ClientInspectionWorkspaceSnapshot snapshot,
        List<string> errors)
    {
        if(snapshot.Roi is null || snapshot.Replay is null)
            return;

        if(snapshot.Roi.ProductionSessionId!=snapshot.Replay.ProductionSessionId)
            errors.Add("ROI and replay Production session identities must match.");
        if(snapshot.Roi.RoiFingerprint.Length!=64 ||
           !IsLowerHex(snapshot.Roi.RoiFingerprint))
            errors.Add("ROI fingerprint is malformed.");
        if(snapshot.Roi.InteractionFingerprint.Length!=64 ||
           !IsLowerHex(snapshot.Roi.InteractionFingerprint))
            errors.Add("ROI interaction fingerprint is malformed.");
    }

    private static void ValidateHistory(
        ClientProductionRunHistorySnapshot history,
        List<string> errors)
    {
        if(history.Capacity<=0)
            errors.Add("Client history capacity must be positive.");
        if(history.Entries.Count>history.Capacity)
            errors.Add("Client history entries exceed bounded capacity.");
        if(history.DroppedCount<0)
            errors.Add("Client history dropped count cannot be negative.");

        var ordinals=history.Entries.Select(entry=>entry.Ordinal).ToArray();
        if(ordinals.Length>1 && !ordinals.SequenceEqual(ordinals.OrderBy(value=>value)))
            errors.Add("Client history ordinals must be monotonic.");
    }

    private static bool IsLowerHex(string value)=>
        value.All(character=>
            Uri.IsHexDigit(character) &&
            char.ToLowerInvariant(character)==character);
}
