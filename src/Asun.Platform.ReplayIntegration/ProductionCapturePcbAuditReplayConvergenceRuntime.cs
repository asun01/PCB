using System.Security.Cryptography;
using System.Text;

namespace Asun.Platform.ReplayIntegration;

public sealed record ProductionCapturePcbAuditReplayConvergence(
    Guid ProductionSessionId,
    Guid QualityRunId,
    string ReleaseManifestFingerprint,
    string CaptureAuditReplayFingerprint,
    string PcbAuditReplayClosureFingerprint,
    bool ReleaseReady,
    string ArtifactPath,
    string Fingerprint);

public static class ProductionCapturePcbAuditReplayConvergenceRuntime
{
    public static ProductionCapturePcbAuditReplayConvergence Create(
        ProductionCaptureEvidenceAuditReplayConvergence captureAuditReplay,
        ProductionReleaseCandidatePcbAuditReplayClosure pcbAuditClosure)
    {
        ArgumentNullException.ThrowIfNull(captureAuditReplay);
        ArgumentNullException.ThrowIfNull(pcbAuditClosure);

        var errors=Validate(captureAuditReplay,pcbAuditClosure);
        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors));

        var fingerprint=CreateFingerprint(
            captureAuditReplay.ProductionSessionId,
            captureAuditReplay.QualityRunId,
            captureAuditReplay.ReleaseManifestFingerprint,
            captureAuditReplay.Fingerprint,
            pcbAuditClosure.Fingerprint,
            pcbAuditClosure.ReleaseReady,
            pcbAuditClosure.ArtifactPath);

        return new ProductionCapturePcbAuditReplayConvergence(
            captureAuditReplay.ProductionSessionId,
            captureAuditReplay.QualityRunId,
            captureAuditReplay.ReleaseManifestFingerprint,
            captureAuditReplay.Fingerprint,
            pcbAuditClosure.Fingerprint,
            pcbAuditClosure.ReleaseReady,
            pcbAuditClosure.ArtifactPath,
            fingerprint);
    }

    public static IReadOnlyList<string> Validate(
        ProductionCaptureEvidenceAuditReplayConvergence captureAuditReplay,
        ProductionReleaseCandidatePcbAuditReplayClosure pcbAuditClosure)
    {
        ArgumentNullException.ThrowIfNull(captureAuditReplay);
        ArgumentNullException.ThrowIfNull(pcbAuditClosure);

        var errors=new List<string>();

        if(captureAuditReplay.ProductionSessionId==Guid.Empty)
            errors.Add("Capture audit replay Production session identity cannot be empty.");
        if(captureAuditReplay.QualityRunId==Guid.Empty)
            errors.Add("Capture audit replay Quality identity cannot be empty.");
        if(!IsLowerHex(captureAuditReplay.ReleaseManifestFingerprint))
            errors.Add("Capture audit replay Release manifest fingerprint is malformed.");
        if(!IsLowerHex(captureAuditReplay.Fingerprint))
            errors.Add("Capture audit replay fingerprint is malformed.");

        if(pcbAuditClosure.ProductionSessionId==Guid.Empty)
            errors.Add("PCB audit replay Production session identity cannot be empty.");
        if(pcbAuditClosure.QualityRunId==Guid.Empty)
            errors.Add("PCB audit replay Quality identity cannot be empty.");
        if(!IsLowerHex(pcbAuditClosure.ReleaseManifestFingerprint))
            errors.Add("PCB audit replay Release manifest fingerprint is malformed.");
        if(!IsLowerHex(pcbAuditClosure.Fingerprint))
            errors.Add("PCB audit replay closure fingerprint is malformed.");
        if(string.IsNullOrWhiteSpace(pcbAuditClosure.ArtifactPath))
            errors.Add("PCB audit replay logical artifact path cannot be blank.");

        if(captureAuditReplay.ProductionSessionId!=pcbAuditClosure.ProductionSessionId)
            errors.Add("Capture and PCB audit replay Production session identities must match.");
        if(captureAuditReplay.QualityRunId!=pcbAuditClosure.QualityRunId)
            errors.Add("Capture and PCB audit replay Quality identities must match.");
        if(captureAuditReplay.ReleaseManifestFingerprint!=pcbAuditClosure.ReleaseManifestFingerprint)
            errors.Add("Capture and PCB audit replay Release manifest identities must match.");

        return errors;
    }

    public static bool IsValid(
        ProductionCaptureEvidenceAuditReplayConvergence captureAuditReplay,
        ProductionReleaseCandidatePcbAuditReplayClosure pcbAuditClosure)=>
        Validate(captureAuditReplay,pcbAuditClosure).Count==0;

    public static bool IsEquivalent(
        ProductionCapturePcbAuditReplayConvergence left,
        ProductionCapturePcbAuditReplayConvergence right)=>
        left.Fingerprint==right.Fingerprint;

    private static string CreateFingerprint(
        Guid sessionId,
        Guid qualityRunId,
        string manifestFingerprint,
        string captureFingerprint,
        string pcbAuditFingerprint,
        bool releaseReady,
        string artifactPath)=>
        Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(
                    string.Join(
                        "|",
                        sessionId,
                        qualityRunId,
                        manifestFingerprint.Length,
                        manifestFingerprint,
                        captureFingerprint.Length,
                        captureFingerprint,
                        pcbAuditFingerprint.Length,
                        pcbAuditFingerprint,
                        releaseReady,
                        artifactPath.Length,
                        artifactPath))))
            .ToLowerInvariant();

    private static bool IsLowerHex(string value)=>
        value.Length==64 &&
        value.All(character=>
            Uri.IsHexDigit(character) &&
            char.ToLowerInvariant(character)==character);
}
