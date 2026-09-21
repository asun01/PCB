namespace Asun.Platform.ClientIntegration;

public sealed record ClientInspectionExecutionSurface(
    ClientInspectionExecutionPresentation Presentation,
    bool HasAcquisition,
    bool HasCompletedResult,
    bool CanInteractWithRoi)
{
    public ClientWorkspaceCommandRouting? CommandRouting { get; init; }

    public ClientInspectionResultDisplay ResultDisplay { get; init; } =
        new("Idle",0,"Session not loaded","Replay not available","Release not evaluated",false);

    public bool HasAcquisitionPreview { get; init; }

    public string AcquisitionPreviewText { get; init; }="Acquisition Preview — unavailable";

    public bool CanPreviewAcquisition => CommandRouting?.CanPreviewAcquisition==true;

    public bool CanRunInspection => CommandRouting?.CanRunInspection==true;

    public bool CanCancelInspection => CommandRouting?.CanCancelInspection==true;

    public bool CanEditRoi => CanInteractWithRoi && CommandRouting?.CanEditRoi==true;
}

public static class ClientInspectionExecutionSurfaceRuntime
{
    public static ClientInspectionExecutionSurface Create(
        ClientInspectionWorkspaceSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var presentation=ClientInspectionExecutionPresentationRuntime.Create(snapshot);
        var resultDisplay=ClientResultsPresentationRuntime.CreateCurrent(snapshot);
        var preview=snapshot.Acquisition.Preview;

        return new ClientInspectionExecutionSurface(
            presentation,
            snapshot.Acquisition.State==ClientAcquisitionState.Ready,
            snapshot.Production.Status==ClientExecutionStatus.Completed,
            snapshot.Roi is not null)
        {
            ResultDisplay=resultDisplay,
            HasAcquisitionPreview=preview is not null,
            AcquisitionPreviewText=preview is null
                ? "Acquisition Preview — unavailable"
                : $"Acquisition Preview — {preview.Width}×{preview.Height} · {preview.PixelFormat} · {preview.Sequence}"
        };
    }

    public static ClientInspectionExecutionSurface Create(
        ClientInspectionWorkspaceSnapshot snapshot,
        ClientWorkspaceCommandRouting commandRouting)
    {
        ArgumentNullException.ThrowIfNull(commandRouting);

        return Create(snapshot) with
        {
            CommandRouting=commandRouting
        };
    }
}
