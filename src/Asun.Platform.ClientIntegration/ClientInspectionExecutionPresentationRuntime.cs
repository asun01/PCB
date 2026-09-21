namespace Asun.Platform.ClientIntegration;

public sealed record ClientInspectionExecutionPresentation(
    string ProgramSummary,
    string ExecutionStatus,
    string ProgressText,
    string RoiText,
    string AcquisitionText,
    string AcquisitionPreviewText,
    string ResultText,
    string RunReadinessText);

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

        var preview=snapshot.Acquisition.Preview is null
            ? "Preview — not captured"
            : $"Preview — {snapshot.Acquisition.Preview.Width}×{snapshot.Acquisition.Preview.Height} · {snapshot.Acquisition.Preview.PixelFormat} · Frame {snapshot.Acquisition.Preview.Sequence.Value}";

        var result=snapshot.Production.Status==ClientExecutionStatus.Completed
            ? $"Result — {snapshot.Production.LastFrameCount} frames"
            : "Result — pending";

        var runReadiness=snapshot.Production.Status switch
        {
            ClientExecutionStatus.Running => "Run — execution in progress",
            ClientExecutionStatus.Ready when snapshot.Acquisition.CanCapture => "Run — ready",
            ClientExecutionStatus.Ready => "Run — bind an acquisition source",
            ClientExecutionStatus.Completed when snapshot.Acquisition.CanCapture => "Run — ready for next session",
            ClientExecutionStatus.Completed => "Run — bind an acquisition source for the next session",
            ClientExecutionStatus.Cancelled when snapshot.Acquisition.CanCapture => "Run — ready after cancellation",
            ClientExecutionStatus.Failed when snapshot.Acquisition.CanCapture => "Run — ready after failure",
            _ when snapshot.Program is null => "Run — load a program",
            _ => "Run — load a program and bind an acquisition source"
        };

        return new ClientInspectionExecutionPresentation(
            programSummary,
            progress.StatusText,
            progress.ProgressText,
            roi,
            acquisition,
            preview,
            result,
            runReadiness);
    }
}
