namespace Asun.Platform.ClientIntegration;

public enum ClientRunReadinessState
{
    LoadProgram,
    BindAcquisition,
    Ready,
    Running,
    ReadyForNextSession,
    BindAcquisitionForNextSession,
    ReadyAfterCancellation,
    ReadyAfterFailure
}

public sealed record ClientInspectionExecutionPresentation(
    string ProgramSummary,
    string ExecutionStatus,
    string ProgressText,
    string RoiText,
    string AcquisitionText,
    string AcquisitionPreviewText,
    string ResultText,
    string RunReadinessText)
{
    public ClientRunReadinessState RunReadinessState { get; init; }
}

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

        var runReadinessState=snapshot.Program?.Status!=ClientProgramLoadStatus.Ready
            ? ClientRunReadinessState.LoadProgram
            : snapshot.Production.Status switch
            {
                ClientExecutionStatus.Running => ClientRunReadinessState.Running,
                ClientExecutionStatus.Ready when snapshot.Acquisition.CanCapture => ClientRunReadinessState.Ready,
                ClientExecutionStatus.Ready => ClientRunReadinessState.BindAcquisition,
                ClientExecutionStatus.Completed when snapshot.Acquisition.CanCapture => ClientRunReadinessState.ReadyForNextSession,
                ClientExecutionStatus.Completed => ClientRunReadinessState.BindAcquisitionForNextSession,
                ClientExecutionStatus.Cancelled when snapshot.Acquisition.CanCapture => ClientRunReadinessState.ReadyAfterCancellation,
                ClientExecutionStatus.Failed when snapshot.Acquisition.CanCapture => ClientRunReadinessState.ReadyAfterFailure,
                _ => ClientRunReadinessState.BindAcquisition
            };

        var runReadiness=runReadinessState switch
        {
            ClientRunReadinessState.LoadProgram => "Run — load a program",
            ClientRunReadinessState.BindAcquisition => "Run — bind an acquisition source",
            ClientRunReadinessState.Ready => "Run — ready",
            ClientRunReadinessState.Running => "Run — execution in progress",
            ClientRunReadinessState.ReadyForNextSession => "Run — ready for next session",
            ClientRunReadinessState.BindAcquisitionForNextSession => "Run — bind an acquisition source for the next session",
            ClientRunReadinessState.ReadyAfterCancellation => "Run — ready after cancellation",
            ClientRunReadinessState.ReadyAfterFailure => "Run — ready after failure",
            _ => throw new ArgumentOutOfRangeException()
        };

        return new ClientInspectionExecutionPresentation(
            programSummary,
            progress.StatusText,
            progress.ProgressText,
            roi,
            acquisition,
            preview,
            result,
            runReadiness)
        {
            RunReadinessState=runReadinessState
        };
    }
}
