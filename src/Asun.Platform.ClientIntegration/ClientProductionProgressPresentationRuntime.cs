namespace Asun.Platform.ClientIntegration;

public sealed record ClientProductionProgressPresentation(
    string StatusText,
    string SessionText,
    string ProgressText,
    bool IsIndeterminate);

public static class ClientProductionProgressPresentationRuntime
{
    public static ClientProductionProgressPresentation Create(
        ClientWorkspaceSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var session=snapshot.ActiveSessionId?.ToString("N") ?? "—";

        if(snapshot.Status==ClientExecutionStatus.Running &&
           snapshot.TargetFrameCount>0)
        {
            return new ClientProductionProgressPresentation(
                "Running",
                $"Session {session}",
                $"Running — Session {session} · {snapshot.FramesProcessed}/{snapshot.TargetFrameCount} frames",
                false);
        }

        var progress=snapshot.TargetFrameCount>0
            ? $"{snapshot.FramesProcessed}/{snapshot.TargetFrameCount} frames"
            : "0 frames";

        return new ClientProductionProgressPresentation(
            snapshot.Status.ToString(),
            $"Session {session}",
            progress,
            snapshot.Status==ClientExecutionStatus.Running);
    }
}
