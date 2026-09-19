namespace Asun.Release.Core;

public sealed record ReleaseManifest(
    ReleaseIdentity Identity,
    IReadOnlyList<ReleaseArtifact> Artifacts,
    string Fingerprint);
