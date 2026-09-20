using System.Security.Cryptography;
using System.Text;
using Asun.Release.Core;

namespace Asun.Platform.PipelineProductionIntegration;

public sealed record ProductionPipelineReleaseHandoff(
    Guid SessionId,
    string ProgramFingerprint,
    string PipelineReplayFingerprint,
    string ReleaseManifestFingerprint,
    bool ReleaseReady,
    string Fingerprint);

public static class ProductionPipelineReleaseHandoffRuntime
{
    public static ProductionPipelineReleaseHandoff Create(
        ProductionPipelineReplayAudit audit,
        ReleaseManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(audit);
        ArgumentNullException.ThrowIfNull(manifest);

        if(!ReleaseManifestValidationRuntime.IsValid(manifest))
            throw new ArgumentException("Release manifest is invalid.",nameof(manifest));

        if(audit.SessionId==Guid.Empty ||
           string.IsNullOrWhiteSpace(audit.ProgramFingerprint) ||
           string.IsNullOrWhiteSpace(audit.Fingerprint) ||
           audit.Fingerprint.Length!=64)
            throw new ArgumentException("Pipeline replay audit is invalid.",nameof(audit));

        var readiness=ReleaseReadinessRuntime.Evaluate(manifest);
        var fingerprint=CreateFingerprint(audit,manifest,readiness.Ready);

        return new ProductionPipelineReleaseHandoff(
            audit.SessionId,
            audit.ProgramFingerprint,
            audit.Fingerprint,
            manifest.Fingerprint,
            readiness.Ready,
            fingerprint);
    }

    public static IReadOnlyList<string> Validate(
        ProductionPipelineReplayAudit audit,
        ReleaseManifest manifest,
        ProductionPipelineReleaseHandoff handoff)
    {
        ArgumentNullException.ThrowIfNull(audit);
        ArgumentNullException.ThrowIfNull(manifest);
        ArgumentNullException.ThrowIfNull(handoff);

        var errors=new List<string>();
        if(!ReleaseManifestValidationRuntime.IsValid(manifest))
            errors.Add("Release manifest is invalid.");
        if(audit.SessionId==Guid.Empty)
            errors.Add("Pipeline replay session id is invalid.");
        if(string.IsNullOrWhiteSpace(audit.ProgramFingerprint))
            errors.Add("Pipeline replay program fingerprint is required.");
        if(audit.Fingerprint.Length!=64)
            errors.Add("Pipeline replay fingerprint must be 64 characters.");
        if(handoff.SessionId!=audit.SessionId)
            errors.Add("Release handoff session id must match the replay audit.");
        if(handoff.ProgramFingerprint!=audit.ProgramFingerprint)
            errors.Add("Release handoff program fingerprint must match the replay audit.");
        if(handoff.PipelineReplayFingerprint!=audit.Fingerprint)
            errors.Add("Release handoff replay fingerprint must match the replay audit.");
        if(handoff.ReleaseManifestFingerprint!=manifest.Fingerprint)
            errors.Add("Release handoff manifest fingerprint must match the release manifest.");
        if(handoff.ReleaseReady!=ReleaseReadinessRuntime.Evaluate(manifest).Ready)
            errors.Add("Release handoff readiness must match manifest readiness.");

        if(errors.Count>0)
            return errors;

        var expected=CreateFingerprint(audit,manifest,handoff.ReleaseReady);
        if(expected!=handoff.Fingerprint)
            errors.Add("Release handoff fingerprint does not match canonical content.");

        return errors;
    }

    public static bool IsValid(
        ProductionPipelineReplayAudit audit,
        ReleaseManifest manifest,
        ProductionPipelineReleaseHandoff handoff)=>
        Validate(audit,manifest,handoff).Count==0;

    internal static string CreateFingerprint(
        ProductionPipelineReplayAudit audit,
        ReleaseManifest manifest,
        bool releaseReady)
    {
        var canonical=string.Join(
            "|",
            audit.SessionId,
            audit.ProgramFingerprint,
            audit.Fingerprint,
            manifest.Fingerprint,
            releaseReady);

        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical)))
            .ToLowerInvariant();
    }
}
