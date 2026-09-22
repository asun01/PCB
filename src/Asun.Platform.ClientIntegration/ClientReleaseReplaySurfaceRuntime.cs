namespace Asun.Platform.ClientIntegration;

public sealed record ClientReleaseReplaySurface(
    bool ReplayAvailable,
    Guid? ReplayProgramId,
    Guid? ReplayProductionSessionId,
    int ReplayFrameCount,
    string ReplayFingerprint,
    bool ReleaseAvailable,
    bool ReleaseReady,
    string ReleaseArtifactPath,
    string ReleaseManifestFingerprint)
{
    public string QualityFingerprint { get; init; }="";
}

public static class ClientReleaseReplaySurfaceRuntime
{
    public static ClientReleaseReplaySurface Create(
        ClientInspectionWorkspaceSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var replay=snapshot.Replay;
        var release=snapshot.Release;

        return new ClientReleaseReplaySurface(
            replay is not null,
            replay?.ProgramId,
            replay?.ProductionSessionId,
            replay?.FrameCount ?? 0,
            replay?.ReplayFingerprint ?? string.Empty,
            release is not null,
            release?.ReleaseReady==true,
            release?.ArtifactPath ?? string.Empty,
            release?.ReleaseManifestFingerprint ?? string.Empty)
        {
            QualityFingerprint=snapshot.Quality.Fingerprint ?? string.Empty
        };
    }
}
