using System.Security.Cryptography;
using System.Text;
using Asun.Platform.PcbAuditReleaseIntegration;
using Asun.Platform.PcbEvidenceReleaseIntegration;

namespace Asun.Platform.ReplayIntegration;

public sealed record PcbEvidenceReleaseAuditTraceReplayDescriptor(
    int FormatVersion,
    string TraceFingerprint,
    string SerializedJsonFingerprint,
    string ReleaseManifestFingerprint,
    Guid QualityRunId,
    int EntryCount,
    string Fingerprint);

public static class PcbEvidenceReleaseAuditTraceReplayDescriptorRuntime
{
    public static PcbEvidenceReleaseAuditTraceReplayDescriptor Create(
        PcbEvidenceReleaseAuditTrace trace,
        PcbAuditReleaseReplayDescriptor auditReplay)
    {
        ArgumentNullException.ThrowIfNull(trace);
        ArgumentNullException.ThrowIfNull(auditReplay);

        var json=PcbEvidenceReleaseAuditTraceJsonRuntime.ToJson(trace);
        var restored=PcbEvidenceReleaseAuditTraceJsonRuntime.FromJson(json);

        if(!PcbEvidenceReleaseAuditTraceRuntime.IsStructurallyValid(restored))
            throw new ArgumentException("Evidence release trace failed structural validation.",nameof(trace));

        if(auditReplay.ReleaseManifestFingerprint!=trace.ReleaseManifestFingerprint)
            throw new ArgumentException("Audit replay manifest must match the evidence release trace.",nameof(auditReplay));
        if(trace.Entries.Count==0)
            throw new ArgumentException("Evidence release trace must contain entries.",nameof(trace));
        if(auditReplay.QualityRunId!=trace.Entries[^1].QualityRunId)
            throw new ArgumentException("Audit replay Quality identity must match the latest trace entry.",nameof(auditReplay));

        var serializedHash=CreateSerializedJsonFingerprint(json);
        var fingerprint=CreateFingerprint(
            PcbEvidenceReleaseAuditTraceJsonRuntime.CurrentFormatVersion,
            restored.Fingerprint,
            serializedHash,
            restored.ReleaseManifestFingerprint,
            auditReplay.QualityRunId,
            restored.Entries.Count);

        return new PcbEvidenceReleaseAuditTraceReplayDescriptor(
            PcbEvidenceReleaseAuditTraceJsonRuntime.CurrentFormatVersion,
            restored.Fingerprint,
            serializedHash,
            restored.ReleaseManifestFingerprint,
            auditReplay.QualityRunId,
            restored.Entries.Count,
            fingerprint);
    }

    public static IReadOnlyList<string> Validate(
        PcbEvidenceReleaseAuditTrace trace,
        PcbAuditReleaseReplayDescriptor auditReplay,
        PcbEvidenceReleaseAuditTraceReplayDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(trace);
        ArgumentNullException.ThrowIfNull(auditReplay);
        ArgumentNullException.ThrowIfNull(descriptor);

        var errors=new List<string>();
        var json=PcbEvidenceReleaseAuditTraceJsonRuntime.ToJson(trace);
        var restored=PcbEvidenceReleaseAuditTraceJsonRuntime.FromJson(json);
        var serializedHash=CreateSerializedJsonFingerprint(json);

        if(descriptor.FormatVersion!=PcbEvidenceReleaseAuditTraceJsonRuntime.CurrentFormatVersion)
            errors.Add("Replay descriptor schema version must match the trace schema version.");
        if(descriptor.TraceFingerprint!=trace.Fingerprint)
            errors.Add("Replay descriptor trace fingerprint must match.");
        if(descriptor.SerializedJsonFingerprint!=serializedHash)
            errors.Add("Replay descriptor serialized JSON fingerprint must match.");
        if(descriptor.ReleaseManifestFingerprint!=trace.ReleaseManifestFingerprint)
            errors.Add("Replay descriptor Release manifest fingerprint must match.");
        if(descriptor.QualityRunId!=auditReplay.QualityRunId)
            errors.Add("Replay descriptor Quality identity must match audit replay.");
        if(descriptor.EntryCount!=trace.Entries.Count)
            errors.Add("Replay descriptor entry count must match trace.");
        if(auditReplay.ReleaseManifestFingerprint!=trace.ReleaseManifestFingerprint)
            errors.Add("Audit replay manifest must match trace.");
        if(trace.Entries.Count>0 &&
           auditReplay.QualityRunId!=trace.Entries[^1].QualityRunId)
            errors.Add("Audit replay Quality identity must match trace latest entry.");
        if(restored.Fingerprint!=trace.Fingerprint)
            errors.Add("Trace JSON roundtrip fingerprint must remain stable.");
        if(descriptor.Fingerprint.Length!=64 || !IsLowerHex(descriptor.Fingerprint))
            errors.Add("Replay descriptor fingerprint must be 64 lowercase hexadecimal characters.");

        if(errors.Count==0)
        {
            var expected=CreateFingerprint(
                descriptor.FormatVersion,
                trace.Fingerprint,
                serializedHash,
                trace.ReleaseManifestFingerprint,
                auditReplay.QualityRunId,
                trace.Entries.Count);

            if(expected!=descriptor.Fingerprint)
                errors.Add("Replay descriptor fingerprint does not match canonical content.");
        }

        return errors;
    }

    public static bool IsValid(
        PcbEvidenceReleaseAuditTrace trace,
        PcbAuditReleaseReplayDescriptor auditReplay,
        PcbEvidenceReleaseAuditTraceReplayDescriptor descriptor)=>
        Validate(trace,auditReplay,descriptor).Count==0;

    private static string CreateSerializedJsonFingerprint(string json)=>
        Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(json)))
            .ToLowerInvariant();

    private static string CreateFingerprint(
        int formatVersion,
        string traceFingerprint,
        string serializedJsonFingerprint,
        string releaseManifestFingerprint,
        Guid qualityRunId,
        int entryCount)=>
        Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(
                    string.Join(
                        "|",
                        formatVersion,
                        traceFingerprint,
                        serializedJsonFingerprint,
                        releaseManifestFingerprint,
                        qualityRunId,
                        entryCount))))
            .ToLowerInvariant();

    private static bool IsLowerHex(string value)=>
        value.Length==64 &&
        value.All(character=>
            Uri.IsHexDigit(character) &&
            char.ToLowerInvariant(character)==character);
}
