using System.Security.Cryptography;
using System.Text;
using Asun.Platform.PcbAuditReleaseIntegration;
using Asun.Platform.PcbEvidenceReleaseIntegration;

namespace Asun.Platform.ReplayIntegration;

public sealed record ProductionCaptureEvidenceAuditReplayConvergence(
    Guid ProductionSessionId,
    string CaptureEvidenceReleaseReplayFingerprint,
    string TraceFingerprint,
    string SerializedJsonFingerprint,
    string ReleaseManifestFingerprint,
    Guid QualityRunId,
    long AuditSequence,
    int EntryCount,
    int FormatVersion,
    string Fingerprint);

public static class ProductionCaptureEvidenceAuditReplayConvergenceRuntime
{
    public static ProductionCaptureEvidenceAuditReplayConvergence Create(
        ProductionCaptureEvidenceReleaseAuditTraceReplayBinding binding,
        PcbEvidenceReleaseAuditTraceReplayDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(binding);
        ArgumentNullException.ThrowIfNull(descriptor);

        var errors=Validate(binding,descriptor);
        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors));

        var fingerprint=CreateFingerprint(
            binding.ProductionSessionId,
            binding.CaptureEvidenceReleaseReplayFingerprint,
            descriptor.TraceFingerprint,
            descriptor.SerializedJsonFingerprint,
            descriptor.ReleaseManifestFingerprint,
            descriptor.QualityRunId,
            binding.AuditSequence,
            descriptor.EntryCount,
            descriptor.FormatVersion);

        return new ProductionCaptureEvidenceAuditReplayConvergence(
            binding.ProductionSessionId,
            binding.CaptureEvidenceReleaseReplayFingerprint,
            descriptor.TraceFingerprint,
            descriptor.SerializedJsonFingerprint,
            descriptor.ReleaseManifestFingerprint,
            descriptor.QualityRunId,
            binding.AuditSequence,
            descriptor.EntryCount,
            descriptor.FormatVersion,
            fingerprint);
    }

    public static IReadOnlyList<string> Validate(
        ProductionCaptureEvidenceReleaseAuditTraceReplayBinding binding,
        PcbEvidenceReleaseAuditTraceReplayDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(binding);
        ArgumentNullException.ThrowIfNull(descriptor);

        var errors=new List<string>();

        if(binding.ProductionSessionId==Guid.Empty)
            errors.Add("Convergence Production session identity cannot be empty.");
        if(!IsLowerHex(binding.CaptureEvidenceReleaseReplayFingerprint))
            errors.Add("Convergence capture/evidence replay fingerprint is malformed.");
        if(!IsLowerHex(binding.AuditTraceFingerprint))
            errors.Add("Convergence binding audit trace fingerprint is malformed.");
        if(!IsLowerHex(binding.ReleaseManifestFingerprint))
            errors.Add("Convergence binding Release manifest fingerprint is malformed.");
        if(binding.AuditSequence<=0)
            errors.Add("Convergence audit sequence must be positive.");

        if(descriptor.FormatVersion<=0)
            errors.Add("Convergence descriptor format version must be positive.");
        if(!IsLowerHex(descriptor.TraceFingerprint))
            errors.Add("Convergence descriptor trace fingerprint is malformed.");
        if(!IsLowerHex(descriptor.SerializedJsonFingerprint))
            errors.Add("Convergence serialized JSON fingerprint is malformed.");
        if(!IsLowerHex(descriptor.ReleaseManifestFingerprint))
            errors.Add("Convergence descriptor Release manifest fingerprint is malformed.");
        if(descriptor.QualityRunId==Guid.Empty)
            errors.Add("Convergence Quality identity cannot be empty.");
        if(descriptor.EntryCount<=0)
            errors.Add("Convergence descriptor entry count must be positive.");
        if(!IsLowerHex(descriptor.Fingerprint))
            errors.Add("Convergence descriptor fingerprint is malformed.");

        if(binding.AuditTraceFingerprint!=descriptor.TraceFingerprint)
            errors.Add("Binding and descriptor trace identities must match.");
        if(binding.ReleaseManifestFingerprint!=descriptor.ReleaseManifestFingerprint)
            errors.Add("Binding and descriptor Release manifest identities must match.");
        if(binding.AuditSequence<=0 || descriptor.EntryCount<1 || binding.AuditSequence>descriptor.EntryCount)
            errors.Add("Binding audit sequence must be within descriptor entry bounds.");
        if(errors.Count==0)
        {
            if(binding.AuditSequence!=descriptor.EntryCount)
                errors.Add("Binding latest audit sequence must equal descriptor entry count.");
        }

        return errors;
    }

    public static bool IsValid(
        ProductionCaptureEvidenceReleaseAuditTraceReplayBinding binding,
        PcbEvidenceReleaseAuditTraceReplayDescriptor descriptor)=>
        Validate(binding,descriptor).Count==0;

    public static bool IsEquivalent(
        ProductionCaptureEvidenceAuditReplayConvergence left,
        ProductionCaptureEvidenceAuditReplayConvergence right)=>
        left.Fingerprint==right.Fingerprint;

    private static string CreateFingerprint(
        Guid productionSessionId,
        string replayFingerprint,
        string traceFingerprint,
        string serializedJsonFingerprint,
        string releaseManifestFingerprint,
        Guid qualityRunId,
        long auditSequence,
        int entryCount,
        int formatVersion)
    {
        var canonical=string.Join(
            "|",
            productionSessionId,
            replayFingerprint.Length,
            replayFingerprint,
            traceFingerprint.Length,
            traceFingerprint,
            serializedJsonFingerprint.Length,
            serializedJsonFingerprint,
            releaseManifestFingerprint.Length,
            releaseManifestFingerprint,
            qualityRunId,
            auditSequence,
            entryCount,
            formatVersion);

        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical)))
            .ToLowerInvariant();
    }

    private static bool IsLowerHex(string value)=>
        value.Length==64 &&
        value.All(character=>
            Uri.IsHexDigit(character) &&
            char.ToLowerInvariant(character)==character);
}
