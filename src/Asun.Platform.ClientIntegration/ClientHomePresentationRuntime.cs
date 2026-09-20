namespace Asun.Platform.ClientIntegration;

public sealed record ClientHomePresentationSnapshot(
    string ProgramStatus,
    string AcquisitionStatus,
    string ProductionStatus,
    string QualityStatus,
    string ResultsStatus,
    string Fingerprint);

public static class ClientHomePresentationRuntime
{
    public static ClientHomePresentationSnapshot Create(
        ClientInspectionWorkspaceSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var program=snapshot.Program;
        var acquisition=snapshot.Acquisition;
        var production=snapshot.Production;
        var quality=snapshot.Quality;

        var programStatus=program?.Status switch
        {
            ClientProgramLoadStatus.Ready =>
                $"{program.Name} v{program.Version} · {program.StepCount} step(s)",
            ClientProgramLoadStatus.Invalid=>"Program invalid",
            _=>"Program not loaded"
        };

        var acquisitionStatus=acquisition.State switch
        {
            ClientAcquisitionState.Ready =>
                acquisition.Descriptor?.DisplayName ?? "Acquisition Ready",
            ClientAcquisitionState.Faulted =>
                $"Faulted · {acquisition.LastError}",
            _=>"Acquisition not bound"
        };

        var productionStatus=production.Status switch
        {
            ClientExecutionStatus.Ready=>"Ready",
            ClientExecutionStatus.Running =>
                $"Running {production.FramesProcessed}/{production.TargetFrameCount}",
            ClientExecutionStatus.Completed =>
                $"Completed {production.LastFrameCount} frame(s)",
            ClientExecutionStatus.Cancelled=>"Cancelled",
            ClientExecutionStatus.Failed=>"Failed",
            _=>"Idle"
        };

        var qualityStatus=quality.IsBound
            ? $"Run {quality.RunId} · {quality.FindingCount} finding(s)"
            : "Quality Run not attached";

        var resultsStatus=snapshot.History.Entries.Count==0
            ? "No historical Results"
            : $"{snapshot.History.Entries.Count} historical run(s)";

        var canonical=string.Join(
            "|",
            programStatus,
            acquisitionStatus,
            productionStatus,
            qualityStatus,
            resultsStatus,
            snapshot.Production.ActiveSessionId,
            snapshot.Production.LastReportFingerprint,
            quality.Fingerprint,
            string.Join(",",snapshot.History.Entries.Select(entry=>entry.Ordinal)));

        return new ClientHomePresentationSnapshot(
            programStatus,
            acquisitionStatus,
            productionStatus,
            qualityStatus,
            resultsStatus,
            Convert.ToHexString(
                System.Security.Cryptography.SHA256.HashData(
                    System.Text.Encoding.UTF8.GetBytes(canonical)))
                .ToLowerInvariant());
    }
}
