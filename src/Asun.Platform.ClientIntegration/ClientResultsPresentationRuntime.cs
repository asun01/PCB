namespace Asun.Platform.ClientIntegration;

public sealed record ClientInspectionResultDisplay(
    string Status,
    int FrameCount,
    string SessionText,
    string ReplayText,
    string ReleaseText,
    bool ReleaseReady);

public static class ClientResultsPresentationRuntime
{
    public static ClientInspectionResultDisplay CreateCurrent(
        ClientInspectionWorkspaceSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var production=snapshot.Production;
        var replay=snapshot.Replay;
        var release=snapshot.Release;

        var status=production.Status switch
        {
            ClientExecutionStatus.Completed=>"Completed",
            ClientExecutionStatus.Running=>"Running",
            ClientExecutionStatus.Cancelled=>"Cancelled",
            ClientExecutionStatus.Failed=>"Failed",
            ClientExecutionStatus.Ready=>"Ready",
            _=>"Idle"
        };

        return new ClientInspectionResultDisplay(
            status,
            production.LastFrameCount,
            production.ActiveSessionId is Guid session
                ? $"Session {session}"
                : "Session not loaded",
            replay is null
                ? "Replay not available"
                : replay.ReplayFingerprint.Length>=12
                    ? $"Replay {replay.ReplayFingerprint[..12]}..."
                    : "Replay fingerprint invalid",
            release is null
                ? "Release not evaluated"
                : release.ReleaseReady
                    ? $"Ready · {release.ArtifactPath}"
                    : "Not Ready",
            release?.ReleaseReady==true);
    }

    public static IReadOnlyList<ClientRunHistoryDisplayItem> CreateHistory(
        ClientInspectionWorkspaceSnapshot snapshot,
        int maxItems=10)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        return ClientRunHistoryPresentationRuntime.CreateItems(snapshot.History,maxItems);
    }
}
