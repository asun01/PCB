using System.Security.Cryptography;
using System.Text;
using Asun.Release.Core;

namespace Asun.Platform.ClientIntegration;

public sealed record ClientReleaseProjection(
    Guid ProgramId,
    Guid ProductionSessionId,
    string ReplayFingerprint,
    bool ReleaseReady,
    string ArtifactPath,
    string ReleaseManifestFingerprint,
    string ProjectionFingerprint);

public static class ClientReleaseProjectionRuntime
{
    public static ClientReleaseProjection Create(
        ClientProductionReplaySnapshot replaySnapshot,
        ReleaseManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(replaySnapshot);
        ArgumentNullException.ThrowIfNull(manifest);

        var errors=Validate(replaySnapshot,manifest);
        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors));

        var readiness=ReleaseReadinessRuntime.Evaluate(manifest);
        var artifact=manifest.Artifacts[0];

        var canonical=string.Join("|",
            replaySnapshot.ProgramId,
            replaySnapshot.ProductionSessionId,
            replaySnapshot.ReplayFingerprint,
            readiness.Ready,
            artifact.Path,
            manifest.Fingerprint);

        return new ClientReleaseProjection(
            replaySnapshot.ProgramId,
            replaySnapshot.ProductionSessionId,
            replaySnapshot.ReplayFingerprint,
            readiness.Ready,
            artifact.Path,
            manifest.Fingerprint,
            Hash(canonical));
    }

    public static IReadOnlyList<string> Validate(
        ClientProductionReplaySnapshot replaySnapshot,
        ReleaseManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(replaySnapshot);
        ArgumentNullException.ThrowIfNull(manifest);

        var errors=new List<string>();

        if(replaySnapshot.Status!=ClientExecutionStatus.Completed)
            errors.Add("Client release projection requires completed execution.");
        if(replaySnapshot.ProgramId==Guid.Empty)
            errors.Add("Client Program identity cannot be empty.");
        if(replaySnapshot.ProductionSessionId==Guid.Empty)
            errors.Add("Client Production session identity cannot be empty.");
        if(!IsLowerHex(replaySnapshot.ReplayFingerprint))
            errors.Add("Client replay fingerprint is malformed.");
        if(!ReleaseManifestValidationRuntime.IsValid(manifest))
            errors.Add("Release manifest is invalid.");
        if(manifest.Artifacts.Count!=1)
            errors.Add("Client release projection requires exactly one logical artifact.");

        return errors;
    }

    public static bool IsValid(
        ClientProductionReplaySnapshot replaySnapshot,
        ReleaseManifest manifest)=>
        Validate(replaySnapshot,manifest).Count==0;

    public static bool IsEquivalent(
        ClientReleaseProjection left,
        ClientReleaseProjection right)=>
        left.ProjectionFingerprint==right.ProjectionFingerprint;

    private static bool IsLowerHex(string value)=>
        value is not null &&
        value.Length==64 &&
        value.All(c=>Uri.IsHexDigit(c) && char.ToLowerInvariant(c)==c);

    private static string Hash(string value)=>
        Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(value)))
        .ToLowerInvariant();
}
