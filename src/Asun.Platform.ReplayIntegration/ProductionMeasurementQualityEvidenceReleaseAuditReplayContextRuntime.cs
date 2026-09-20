using System.Security.Cryptography;
using System.Text;

namespace Asun.Platform.ReplayIntegration;

public sealed record ProductionMeasurementQualityEvidenceReleaseAuditReplayContext(
    Guid ProductionSessionId,
    Guid QualityRunId,
    long Sequence,
    string ProductionInputFingerprint,
    Guid QualityResultId,
    string EvidenceFingerprint,
    string ReleaseManifestFingerprint,
    long AuditSequence,
    string ReplayDescriptorFingerprint,
    string AuditReplayBindingFingerprint,
    string BindingFingerprint);

public static class ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime
{
    public static ProductionMeasurementQualityEvidenceReleaseAuditReplayContext Create(
        ProductionMeasurementQualityEvidenceReleaseReplayDescriptor replayDescriptor,
        ProductionCaptureEvidenceReleaseAuditTraceReplayBinding auditBinding)
    {
        ArgumentNullException.ThrowIfNull(replayDescriptor);
        ArgumentNullException.ThrowIfNull(auditBinding);

        var errors=Validate(replayDescriptor,auditBinding);
        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors));

        var canonical=string.Join("|",
            replayDescriptor.ProductionSessionId,
            replayDescriptor.QualityRunId,
            replayDescriptor.Sequence,
            replayDescriptor.ProductionInputFingerprint,
            replayDescriptor.QualityResultId,
            replayDescriptor.EvidenceFingerprint,
            replayDescriptor.ReleaseManifestFingerprint,
            auditBinding.AuditSequence,
            replayDescriptor.DescriptorFingerprint,
            auditBinding.Fingerprint);

        return new ProductionMeasurementQualityEvidenceReleaseAuditReplayContext(
            replayDescriptor.ProductionSessionId,
            replayDescriptor.QualityRunId,
            replayDescriptor.Sequence,
            replayDescriptor.ProductionInputFingerprint,
            replayDescriptor.QualityResultId,
            replayDescriptor.EvidenceFingerprint,
            replayDescriptor.ReleaseManifestFingerprint,
            auditBinding.AuditSequence,
            replayDescriptor.DescriptorFingerprint,
            auditBinding.Fingerprint,
            Hash(canonical));
    }

    public static IReadOnlyList<string> Validate(
        ProductionMeasurementQualityEvidenceReleaseReplayDescriptor replayDescriptor,
        ProductionCaptureEvidenceReleaseAuditTraceReplayBinding auditBinding)
    {
        ArgumentNullException.ThrowIfNull(replayDescriptor);
        ArgumentNullException.ThrowIfNull(auditBinding);

        var errors=new List<string>();
        if(replayDescriptor.ProductionSessionId==Guid.Empty)
            errors.Add("Replay descriptor Production session identity cannot be empty.");
        if(replayDescriptor.QualityRunId==Guid.Empty)
            errors.Add("Replay descriptor Quality run identity cannot be empty.");
        if(replayDescriptor.Sequence<=0)
            errors.Add("Replay descriptor sequence must be positive.");
        if(!IsLowerHex(replayDescriptor.ProductionInputFingerprint))
            errors.Add("Replay descriptor Production input fingerprint is malformed.");
        if(replayDescriptor.QualityResultId==Guid.Empty)
            errors.Add("Replay descriptor Quality result identity cannot be empty.");
        if(!IsLowerHex(replayDescriptor.EvidenceFingerprint))
            errors.Add("Replay descriptor Evidence fingerprint is malformed.");
        if(!IsLowerHex(replayDescriptor.ReleaseManifestFingerprint))
            errors.Add("Replay descriptor Release manifest fingerprint is malformed.");
        if(!IsLowerHex(replayDescriptor.DescriptorFingerprint))
            errors.Add("Replay descriptor fingerprint is malformed.");

        if(auditBinding.ProductionSessionId!=replayDescriptor.ProductionSessionId)
            errors.Add("Audit replay Production session identity must match the replay descriptor.");
        if(auditBinding.AuditSequence<=0)
            errors.Add("Audit replay sequence must be positive.");
        if(!IsLowerHex(auditBinding.AuditTraceFingerprint))
            errors.Add("Audit trace fingerprint is malformed.");
        if(!IsLowerHex(auditBinding.CaptureEvidenceReleaseReplayFingerprint))
            errors.Add("Capture Evidence release replay fingerprint is malformed.");
        if(!IsLowerHex(auditBinding.ReleaseManifestFingerprint))
            errors.Add("Audit replay Release manifest fingerprint is malformed.");
        if(!IsLowerHex(auditBinding.Fingerprint))
            errors.Add("Audit replay binding fingerprint is malformed.");
        if(auditBinding.ReleaseManifestFingerprint!=replayDescriptor.ReleaseManifestFingerprint)
            errors.Add("Audit replay Release manifest identity must match the replay descriptor.");

        return errors;
    }

    public static bool IsValid(
        ProductionMeasurementQualityEvidenceReleaseReplayDescriptor replayDescriptor,
        ProductionCaptureEvidenceReleaseAuditTraceReplayBinding auditBinding)=>
        Validate(replayDescriptor,auditBinding).Count==0;

    public static bool IsEquivalent(
        ProductionMeasurementQualityEvidenceReleaseAuditReplayContext left,
        ProductionMeasurementQualityEvidenceReleaseAuditReplayContext right)=>
        left.BindingFingerprint==right.BindingFingerprint;

    private static bool IsLowerHex(string value)=>
        value.Length==64 && value.All(c=>Uri.IsHexDigit(c) && char.ToLowerInvariant(c)==c);

    private static string Hash(string value)=>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();
}
