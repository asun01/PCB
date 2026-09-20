using System.Security.Cryptography;
using System.Text;
using Asun.Platform.PcbEvidenceReleaseIntegration;

namespace Asun.Platform.ReplayIntegration;

public sealed record ProductionCaptureEvidenceReleaseAuditTraceReplayBinding(
    Guid ProductionSessionId,
    string CaptureEvidenceReleaseReplayFingerprint,
    string AuditTraceFingerprint,
    string ReleaseManifestFingerprint,
    long AuditSequence,
    string Fingerprint);

public static class ProductionCaptureEvidenceReleaseAuditTraceReplayBindingRuntime
{
    public static ProductionCaptureEvidenceReleaseAuditTraceReplayBinding Create(
        ProductionCaptureEvidenceReleaseReplayBinding releaseReplay,
        PcbEvidenceReleaseAuditTrace auditTrace)
    {
        ArgumentNullException.ThrowIfNull(releaseReplay);
        ArgumentNullException.ThrowIfNull(auditTrace);

        var errors=Validate(releaseReplay,auditTrace);
        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors));

        var latest=auditTrace.Entries[^1];
        var fingerprint=CreateFingerprint(
            releaseReplay.ProductionSessionId,
            releaseReplay.Fingerprint,
            auditTrace,
            latest.Sequence);

        return new ProductionCaptureEvidenceReleaseAuditTraceReplayBinding(
            releaseReplay.ProductionSessionId,
            releaseReplay.Fingerprint,
            auditTrace.Fingerprint,
            auditTrace.ReleaseManifestFingerprint,
            latest.Sequence,
            fingerprint);
    }

    public static IReadOnlyList<string> Validate(
        ProductionCaptureEvidenceReleaseReplayBinding releaseReplay,
        PcbEvidenceReleaseAuditTrace auditTrace,
        ProductionCaptureEvidenceReleaseAuditTraceReplayBinding binding)
    {
        ArgumentNullException.ThrowIfNull(releaseReplay);
        ArgumentNullException.ThrowIfNull(auditTrace);
        ArgumentNullException.ThrowIfNull(binding);

        var errors=Validate(releaseReplay,auditTrace);
        if(errors.Count>0)
            return errors;

        var latest=auditTrace.Entries[^1];

        if(binding.ProductionSessionId!=releaseReplay.ProductionSessionId)
            errors.Add("Audit replay binding Production session identity must match.");
        if(binding.CaptureEvidenceReleaseReplayFingerprint!=releaseReplay.Fingerprint)
            errors.Add("Audit replay binding Release replay identity must match.");
        if(binding.AuditTraceFingerprint!=auditTrace.Fingerprint)
            errors.Add("Audit replay binding audit trace identity must match.");
        if(binding.ReleaseManifestFingerprint!=auditTrace.ReleaseManifestFingerprint)
            errors.Add("Audit replay binding manifest identity must match audit trace.");
        if(binding.AuditSequence!=latest.Sequence)
            errors.Add("Audit replay binding sequence must match latest trace entry.");
        if(binding.Fingerprint.Length!=64 || !IsLowerHex(binding.Fingerprint))
            errors.Add("Audit replay binding fingerprint must be 64 lowercase hexadecimal characters.");

        if(errors.Count==0)
        {
            var expected=CreateFingerprint(
                releaseReplay.ProductionSessionId,
                releaseReplay.Fingerprint,
                auditTrace,
                latest.Sequence);

            if(expected!=binding.Fingerprint)
                errors.Add("Audit replay binding fingerprint does not match canonical content.");
        }

        return errors;
    }

    public static IReadOnlyList<string> Validate(
        ProductionCaptureEvidenceReleaseReplayBinding releaseReplay,
        PcbEvidenceReleaseAuditTrace auditTrace)
    {
        ArgumentNullException.ThrowIfNull(releaseReplay);
        ArgumentNullException.ThrowIfNull(auditTrace);

        var errors=new List<string>();
        if(releaseReplay.ProductionSessionId==Guid.Empty)
            errors.Add("Release replay Production session identity cannot be empty.");
        if(releaseReplay.Fingerprint.Length!=64 || !IsLowerHex(releaseReplay.Fingerprint))
            errors.Add("Release replay fingerprint is malformed.");
        if(!PcbEvidenceReleaseAuditTraceRuntime.IsStructurallyValid(auditTrace))
            errors.Add("Evidence release audit trace is structurally invalid.");
        if(auditTrace.ReleaseManifestFingerprint!=releaseReplay.ReleaseManifestFingerprint)
            errors.Add("Audit trace Release manifest identity must match Release replay.");

        return errors;
    }

    public static bool IsValid(
        ProductionCaptureEvidenceReleaseReplayBinding releaseReplay,
        PcbEvidenceReleaseAuditTrace auditTrace,
        ProductionCaptureEvidenceReleaseAuditTraceReplayBinding binding)=>
        Validate(releaseReplay,auditTrace,binding).Count==0;

    public static bool IsEquivalent(
        ProductionCaptureEvidenceReleaseAuditTraceReplayBinding left,
        ProductionCaptureEvidenceReleaseAuditTraceReplayBinding right)=>
        left.Fingerprint==right.Fingerprint;

    private static string CreateFingerprint(
        Guid productionSessionId,
        string replayFingerprint,
        PcbEvidenceReleaseAuditTrace trace,
        long latestSequence)
    {
        var canonical=string.Join(
            "|",
            productionSessionId,
            replayFingerprint.Length,
            replayFingerprint,
            trace.ReleaseManifestFingerprint.Length,
            trace.ReleaseManifestFingerprint,
            trace.Fingerprint.Length,
            trace.Fingerprint,
            latestSequence);

        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical)))
            .ToLowerInvariant();
    }

    private static bool IsLowerHex(string value)=>
        value.Length==64 &&
        value.All(c=>Uri.IsHexDigit(c) && char.ToLowerInvariant(c)==c);
}
