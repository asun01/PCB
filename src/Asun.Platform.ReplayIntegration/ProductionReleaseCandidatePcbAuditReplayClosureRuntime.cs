using System.Security.Cryptography;
using System.Text;
using Asun.Platform.PcbAuditReleaseIntegration;
using Asun.Release.Core;

namespace Asun.Platform.ReplayIntegration;

public sealed record ProductionReleaseCandidatePcbAuditReplayClosure(
    Guid ProductionSessionId,
    string ReleaseCandidateAuditClosureFingerprint,
    string PcbAuditReleaseTransitionFingerprint,
    string PcbAuditEnvelopeFingerprint,
    Guid QualityRunId,
    string AuditWindowFingerprint,
    string ReleaseManifestFingerprint,
    bool ReleaseReady,
    string ArtifactPath,
    string Fingerprint);

public static class ProductionReleaseCandidatePcbAuditReplayClosureRuntime
{
    public static ProductionReleaseCandidatePcbAuditReplayClosure Create(
        ProductionExecutionReleaseCandidateAuditClosure releaseCandidateAudit,
        PcbAuditReleaseReplayDescriptor pcbAuditReplay,
        ReleaseManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(releaseCandidateAudit);
        ArgumentNullException.ThrowIfNull(pcbAuditReplay);
        ArgumentNullException.ThrowIfNull(manifest);

        var errors=ValidateInputs(releaseCandidateAudit,pcbAuditReplay,manifest);
        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors),nameof(manifest));

        var readiness=ReleaseReadinessRuntime.Evaluate(manifest);
        var artifactPath=manifest.Artifacts[0].Path;
        var fingerprint=CreateFingerprint(
            releaseCandidateAudit.ProductionSessionId,
            releaseCandidateAudit.Fingerprint,
            pcbAuditReplay.TransitionFingerprint,
            pcbAuditReplay.EnvelopeFingerprint,
            pcbAuditReplay.QualityRunId,
            pcbAuditReplay.AuditWindowFingerprint,
            manifest.Fingerprint,
            readiness.Ready,
            artifactPath);

        return new ProductionReleaseCandidatePcbAuditReplayClosure(
            releaseCandidateAudit.ProductionSessionId,
            releaseCandidateAudit.Fingerprint,
            pcbAuditReplay.TransitionFingerprint,
            pcbAuditReplay.EnvelopeFingerprint,
            pcbAuditReplay.QualityRunId,
            pcbAuditReplay.AuditWindowFingerprint,
            manifest.Fingerprint,
            readiness.Ready,
            artifactPath,
            fingerprint);
    }

    public static IReadOnlyList<string> Validate(
        ProductionExecutionReleaseCandidateAuditClosure releaseCandidateAudit,
        PcbAuditReleaseReplayDescriptor pcbAuditReplay,
        ReleaseManifest manifest,
        ProductionReleaseCandidatePcbAuditReplayClosure closure)
    {
        ArgumentNullException.ThrowIfNull(releaseCandidateAudit);
        ArgumentNullException.ThrowIfNull(pcbAuditReplay);
        ArgumentNullException.ThrowIfNull(manifest);
        ArgumentNullException.ThrowIfNull(closure);

        var errors=ValidateInputs(releaseCandidateAudit,pcbAuditReplay,manifest);
        if(errors.Count>0)
            return errors;

        var readiness=ReleaseReadinessRuntime.Evaluate(manifest);
        if(closure.ProductionSessionId!=releaseCandidateAudit.ProductionSessionId)
            errors.Add("PCB audit replay closure Production session identity must match Release candidate audit.");
        if(closure.ReleaseCandidateAuditClosureFingerprint!=releaseCandidateAudit.Fingerprint)
            errors.Add("PCB audit replay closure candidate-audit identity must match.");
        if(closure.PcbAuditReleaseTransitionFingerprint!=pcbAuditReplay.TransitionFingerprint)
            errors.Add("PCB audit replay closure transition identity must match.");
        if(closure.PcbAuditEnvelopeFingerprint!=pcbAuditReplay.EnvelopeFingerprint)
            errors.Add("PCB audit replay closure evidence-envelope identity must match.");
        if(closure.QualityRunId!=pcbAuditReplay.QualityRunId)
            errors.Add("PCB audit replay closure Quality run identity must match.");
        if(closure.AuditWindowFingerprint!=pcbAuditReplay.AuditWindowFingerprint)
            errors.Add("PCB audit replay closure audit-window identity must match.");
        if(closure.ReleaseManifestFingerprint!=manifest.Fingerprint)
            errors.Add("PCB audit replay closure Release manifest identity must match.");
        if(closure.ReleaseReady!=readiness.Ready)
            errors.Add("PCB audit replay closure readiness must match canonical Release readiness.");
        if(closure.ArtifactPath!=manifest.Artifacts[0].Path)
            errors.Add("PCB audit replay closure artifact path must match logical Release artifact.");
        if(closure.Fingerprint.Length!=64 || !IsLowerHex(closure.Fingerprint))
            errors.Add("PCB audit replay closure fingerprint must be 64 lowercase hexadecimal characters.");

        if(errors.Count==0)
        {
            var expected=CreateFingerprint(
                releaseCandidateAudit.ProductionSessionId,
                releaseCandidateAudit.Fingerprint,
                pcbAuditReplay.TransitionFingerprint,
                pcbAuditReplay.EnvelopeFingerprint,
                pcbAuditReplay.QualityRunId,
                pcbAuditReplay.AuditWindowFingerprint,
                manifest.Fingerprint,
                readiness.Ready,
                manifest.Artifacts[0].Path);

            if(expected!=closure.Fingerprint)
                errors.Add("PCB audit replay closure fingerprint does not match canonical content.");
        }

        return errors;
    }

    public static bool IsValid(
        ProductionExecutionReleaseCandidateAuditClosure releaseCandidateAudit,
        PcbAuditReleaseReplayDescriptor pcbAuditReplay,
        ReleaseManifest manifest,
        ProductionReleaseCandidatePcbAuditReplayClosure closure)=>
        Validate(releaseCandidateAudit,pcbAuditReplay,manifest,closure).Count==0;

    internal static string CreateFingerprint(
        Guid productionSessionId,
        string releaseCandidateAuditClosureFingerprint,
        string pcbAuditReleaseTransitionFingerprint,
        string pcbAuditEnvelopeFingerprint,
        Guid qualityRunId,
        string auditWindowFingerprint,
        string releaseManifestFingerprint,
        bool releaseReady,
        string artifactPath)
    {
        var canonical=string.Join(
            "|",
            productionSessionId,
            releaseCandidateAuditClosureFingerprint,
            pcbAuditReleaseTransitionFingerprint,
            pcbAuditEnvelopeFingerprint,
            qualityRunId,
            auditWindowFingerprint,
            releaseManifestFingerprint,
            releaseReady,
            artifactPath.Length,
            artifactPath);

        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical)))
            .ToLowerInvariant();
    }

    private static List<string> ValidateInputs(
        ProductionExecutionReleaseCandidateAuditClosure releaseCandidateAudit,
        PcbAuditReleaseReplayDescriptor pcbAuditReplay,
        ReleaseManifest manifest)
    {
        var errors=new List<string>();

        if(releaseCandidateAudit.ProductionSessionId==Guid.Empty)
            errors.Add("Release candidate audit Production session identity is invalid.");
        if(!IsLowerHex(releaseCandidateAudit.Fingerprint))
            errors.Add("Release candidate audit fingerprint is malformed.");
        if(!ReleaseManifestValidationRuntime.IsValid(manifest))
            errors.Add("Release manifest is invalid.");
        if(manifest.Artifacts.Count!=1)
            errors.Add("Release candidate replay closure requires exactly one logical artifact.");
        if(manifest.Artifacts.Count==1 &&
           releaseCandidateAudit.ReleaseManifestFingerprint!=manifest.Fingerprint)
            errors.Add("Release candidate audit manifest identity must match the supplied Release manifest.");
        if(!IsLowerHex(pcbAuditReplay.TransitionFingerprint))
            errors.Add("PCB audit replay transition fingerprint is malformed.");
        if(!IsLowerHex(pcbAuditReplay.EnvelopeFingerprint))
            errors.Add("PCB audit replay envelope fingerprint is malformed.");
        if(pcbAuditReplay.QualityRunId==Guid.Empty)
            errors.Add("PCB audit replay Quality run identity is invalid.");
        if(!IsLowerHex(pcbAuditReplay.AuditWindowFingerprint))
            errors.Add("PCB audit replay audit-window fingerprint is malformed.");
        if(!IsLowerHex(pcbAuditReplay.ReleaseManifestFingerprint))
            errors.Add("PCB audit replay Release manifest fingerprint is malformed.");
        if(manifest.Artifacts.Count==1 &&
           pcbAuditReplay.ReleaseManifestFingerprint!=manifest.Fingerprint)
            errors.Add("PCB audit replay manifest identity must match the supplied Release manifest.");

        var readiness=ReleaseReadinessRuntime.Evaluate(manifest);
        if(releaseCandidateAudit.ReleaseReady!=readiness.Ready)
            errors.Add("Release candidate audit readiness must match canonical Release readiness.");
        if(manifest.Artifacts.Count==1 &&
           releaseCandidateAudit.ArtifactPath!=manifest.Artifacts[0].Path)
            errors.Add("Release candidate audit artifact path must match logical Release artifact.");

        return errors;
    }

    private static bool IsLowerHex(string value)=>
        value.Length==64 &&
        value.All(c=>Uri.IsHexDigit(c) && char.ToLowerInvariant(c)==c);
}
