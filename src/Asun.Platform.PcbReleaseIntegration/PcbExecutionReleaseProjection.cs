namespace Asun.Platform.PcbReleaseIntegration;

public sealed record PcbExecutionReleaseProjection(
    string AssemblyFingerprint,
    Guid ProductionSessionId,
    string ExecutionSnapshotFingerprint,
    string ReleaseManifestFingerprint,
    int ArtifactCount,
    bool ReleaseReady,
    string Fingerprint);
