namespace Asun.Platform.ClientIntegration;

public sealed record ClientInspectionExecutionPresentation(
    string ProgramSummary,
    string ExecutionStatus,
    string ProgressText,
    string RoiText,
    string AcquisitionText,
    string ResultText);

public static class ClientInspectionExecutionPresentationRuntime
{
    public static ClientInspectionExecutionPresentation Create(
        ClientInspectionWorkspaceSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var program=snapshot.Program;
        var programSummary=program is null
            ? "Program — unbound"
            : $"Program — {program.Name} v{program.Version} · {program.StepCount} steps";

        var progress=ClientProductionProgressPresentationRuntime.Create(snapshot.Production);

        var roi=snapshot.Roi is null
            ? "ROI — unavailable until Production completes"
            : "ROI — available";

        var acquisition=snapshot.Acquisition.State switch
        {
            ClientAcquisitionState.Ready =>
                $"Acquisition — {snapshot.Acquisition.Descriptor?.DisplayName ?? snapshot.Acquisition.Descriptor?.SourceId ?? "ready"}",
            ClientAcquisitionState.Faulted =>
                $"Acquisition — faulted: {snapshot.Acquisition.LastError ?? "unknown fault"}",
            _ =>
                "Acquisition — unbound"
        };

        var result=snapshot.Production.Status==ClientExecutionStatus.Completed
            ? $"Result — {snapshot.Production.LastFrameCount} frames"
            : "Result — pending";

        return new ClientInspectionExecutionPresentation(
            programSummary,
            progress.StatusText,
            progress.ProgressText,
            roi,
            acquisition,
            result);
    }
}
