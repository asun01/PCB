using System.Security.Cryptography;
using System.Text;
using Asun.Platform.PcbAuditReleaseIntegration;

namespace Asun.Platform.PcbEvidenceReleaseIntegration;

public readonly record struct PcbEvidenceReleaseAuditTraceEntry(
    long Sequence,
    string EvidenceReleaseFactFingerprint,
    string ReleaseManifestFingerprint,
    string AuditTransitionFingerprint,
    Guid QualityRunId,
    int RequestedCount,
    int FoundCount,
    int MissingCount,
    bool AllRequestedResolved);

public sealed record PcbEvidenceReleaseAuditTrace(
    int Capacity,
    IReadOnlyList<PcbEvidenceReleaseAuditTraceEntry> Entries,
    string ReleaseManifestFingerprint,
    string Fingerprint);

public static class PcbEvidenceReleaseAuditTraceRuntime
{
    public static PcbEvidenceReleaseAuditTrace Create(
        PcbEvidenceReleaseFactProjection projection,
        PcbAuditReleaseReplayDescriptor auditReplay,
        int capacity=32)
    {
        ArgumentNullException.ThrowIfNull(projection);
        ArgumentNullException.ThrowIfNull(auditReplay);
        ValidateCapacity(capacity);

        var entry=CreateEntry(1,projection,auditReplay);
        return CreateSnapshot(capacity,new[]{entry});
    }

    public static PcbEvidenceReleaseAuditTrace Append(
        PcbEvidenceReleaseAuditTrace current,
        PcbEvidenceReleaseFactProjection projection,
        PcbAuditReleaseReplayDescriptor auditReplay)
    {
        ArgumentNullException.ThrowIfNull(current);
        ArgumentNullException.ThrowIfNull(projection);
        ArgumentNullException.ThrowIfNull(auditReplay);

        var errors=Validate(current,projection,auditReplay);
        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors),nameof(projection));

        if(current.Entries.Count>=current.Capacity)
            throw new InvalidOperationException("The evidence release audit trace is at capacity.");

        var entry=CreateEntry(current.Entries[^1].Sequence+1,projection,auditReplay);
        return CreateSnapshot(
            current.Capacity,
            current.Entries.Concat(new[]{entry}).ToArray());
    }

    public static IReadOnlyList<string> Validate(
        PcbEvidenceReleaseAuditTrace trace,
        PcbEvidenceReleaseFactProjection projection,
        PcbAuditReleaseReplayDescriptor auditReplay)
    {
        ArgumentNullException.ThrowIfNull(trace);
        ArgumentNullException.ThrowIfNull(projection);
        ArgumentNullException.ThrowIfNull(auditReplay);

        var errors=new List<string>();
        try
        {
            ValidateCapacity(trace.Capacity);
        }
        catch(ArgumentOutOfRangeException)
        {
            errors.Add("Trace capacity must be between 1 and 64.");
        }

        if(trace.Entries.Count==0)
            errors.Add("Evidence release audit trace must contain at least one entry.");
        if(trace.Entries.Count>trace.Capacity)
            errors.Add("Evidence release audit trace exceeds capacity.");

        for(var i=0;i<trace.Entries.Count;i++)
        {
            var entry=trace.Entries[i];
            if(entry.Sequence!=i+1)
                errors.Add("Trace sequences must be consecutive starting at one.");
            if(!IsLowerHex(entry.EvidenceReleaseFactFingerprint))
                errors.Add("Trace evidence fact fingerprint is malformed.");
            if(!IsLowerHex(entry.ReleaseManifestFingerprint))
                errors.Add("Trace Release manifest fingerprint is malformed.");
            if(!IsLowerHex(entry.AuditTransitionFingerprint))
                errors.Add("Trace audit transition fingerprint is malformed.");
            if(entry.QualityRunId==Guid.Empty)
                errors.Add("Trace Quality run identity is invalid.");
            if(entry.RequestedCount<0 || entry.FoundCount<0 || entry.MissingCount<0)
                errors.Add("Trace evidence counts must be non-negative.");
            if(entry.RequestedCount!=entry.FoundCount+entry.MissingCount)
                errors.Add("Trace evidence counts must reconcile.");
            if(entry.AllRequestedResolved!=(entry.MissingCount==0))
                errors.Add("Trace resolution state must match missing count.");
        }

        if(trace.ReleaseManifestFingerprint!=projection.ReleaseManifestFingerprint)
            errors.Add("Trace manifest identity must match the supplied evidence release projection.");
        if(auditReplay.ReleaseManifestFingerprint!=projection.ReleaseManifestFingerprint)
            errors.Add("Audit replay manifest identity must match the evidence release projection.");
        if(trace.Entries.Count>0)
        {
            var last=trace.Entries[^1];
            if(last.EvidenceReleaseFactFingerprint!=projection.Fingerprint)
                errors.Add("Trace latest evidence fact identity must match supplied projection.");
            if(last.ReleaseManifestFingerprint!=projection.ReleaseManifestFingerprint)
                errors.Add("Trace latest manifest identity must match supplied projection.");
            if(last.AuditTransitionFingerprint!=auditReplay.TransitionFingerprint)
                errors.Add("Trace latest audit transition identity must match supplied replay descriptor.");
            if(last.QualityRunId!=auditReplay.QualityRunId)
                errors.Add("Trace latest Quality identity must match supplied replay descriptor.");
        }

        if(trace.Fingerprint.Length!=64 || !IsLowerHex(trace.Fingerprint))
            errors.Add("Trace fingerprint must be 64 lowercase hexadecimal characters.");

        if(errors.Count==0 &&
           CreateFingerprint(trace.Capacity,trace.Entries,trace.ReleaseManifestFingerprint)!=trace.Fingerprint)
            errors.Add("Trace fingerprint does not match canonical content.");

        return errors;
    }

    public static bool IsValid(
        PcbEvidenceReleaseAuditTrace trace,
        PcbEvidenceReleaseFactProjection projection,
        PcbAuditReleaseReplayDescriptor auditReplay)=>
        Validate(trace,projection,auditReplay).Count==0;

    public static IReadOnlyList<string> ValidateStructure(
        PcbEvidenceReleaseAuditTrace trace)
    {
        ArgumentNullException.ThrowIfNull(trace);

        var errors=new List<string>();
        try
        {
            ValidateCapacity(trace.Capacity);
        }
        catch(ArgumentOutOfRangeException)
        {
            errors.Add("Trace capacity must be between 1 and 64.");
        }

        if(trace.Entries.Count==0)
            errors.Add("Evidence release audit trace must contain at least one entry.");
        if(trace.Entries.Count>trace.Capacity)
            errors.Add("Evidence release audit trace exceeds capacity.");

        for(var i=0;i<trace.Entries.Count;i++)
        {
            var entry=trace.Entries[i];

            if(entry.Sequence!=i+1)
                errors.Add("Trace sequences must be consecutive starting at one.");
            if(!IsLowerHex(entry.EvidenceReleaseFactFingerprint))
                errors.Add("Trace evidence fact fingerprint is malformed.");
            if(!IsLowerHex(entry.ReleaseManifestFingerprint))
                errors.Add("Trace Release manifest fingerprint is malformed.");
            if(!IsLowerHex(entry.AuditTransitionFingerprint))
                errors.Add("Trace audit transition fingerprint is malformed.");
            if(entry.QualityRunId==Guid.Empty)
                errors.Add("Trace Quality run identity is invalid.");
            if(entry.RequestedCount<0 || entry.FoundCount<0 || entry.MissingCount<0)
                errors.Add("Trace evidence counts must be non-negative.");
            if(entry.RequestedCount!=entry.FoundCount+entry.MissingCount)
                errors.Add("Trace evidence counts must reconcile.");
            if(entry.AllRequestedResolved!=(entry.MissingCount==0))
                errors.Add("Trace resolution state must match missing count.");
        }

        if(trace.Entries.Count>0)
        {
            var manifest=trace.Entries[0].ReleaseManifestFingerprint;
            if(trace.ReleaseManifestFingerprint!=manifest)
                errors.Add("Trace manifest identity must match its entries.");
            if(trace.Entries.Any(entry=>entry.ReleaseManifestFingerprint!=manifest))
                errors.Add("All trace entries must use one Release manifest fingerprint.");
        }

        if(trace.Fingerprint.Length!=64 || !IsLowerHex(trace.Fingerprint))
            errors.Add("Trace fingerprint must be 64 lowercase hexadecimal characters.");

        if(errors.Count==0 &&
           CreateFingerprint(trace.Capacity,trace.Entries,trace.ReleaseManifestFingerprint)!=trace.Fingerprint)
            errors.Add("Trace fingerprint does not match canonical content.");

        return errors;
    }

    public static bool IsStructurallyValid(PcbEvidenceReleaseAuditTrace trace)=>
        ValidateStructure(trace).Count==0;

    private static PcbEvidenceReleaseAuditTrace CreateSnapshot(
        int capacity,
        IReadOnlyList<PcbEvidenceReleaseAuditTraceEntry> entries)
    {
        var ordered=entries
            .OrderBy(entry=>entry.Sequence)
            .ToArray();

        if(ordered.Length==0 || ordered.Length>capacity)
            throw new ArgumentException("Trace entry count must be within the configured capacity.",nameof(entries));

        var manifest=ordered[0].ReleaseManifestFingerprint;
        if(ordered.Any(entry=>entry.ReleaseManifestFingerprint!=manifest))
            throw new ArgumentException("All trace entries must use one Release manifest fingerprint.",nameof(entries));

        var fingerprint=CreateFingerprint(capacity,ordered,manifest);
        return new PcbEvidenceReleaseAuditTrace(
            capacity,
            ordered,
            manifest,
            fingerprint);
    }

    private static PcbEvidenceReleaseAuditTraceEntry CreateEntry(
        long sequence,
        PcbEvidenceReleaseFactProjection projection,
        PcbAuditReleaseReplayDescriptor auditReplay)=>
        new(
            sequence,
            projection.Fingerprint,
            projection.ReleaseManifestFingerprint,
            auditReplay.TransitionFingerprint,
            auditReplay.QualityRunId,
            projection.RequestedCount,
            projection.FoundCount,
            projection.MissingCount,
            projection.AllRequestedResolved);

    private static string CreateFingerprint(
        int capacity,
        IEnumerable<PcbEvidenceReleaseAuditTraceEntry> entries,
        string releaseManifestFingerprint)
    {
        var builder=new StringBuilder();
        builder.Append(capacity).Append('|').Append(releaseManifestFingerprint).Append('|');

        foreach(var entry in entries.OrderBy(item=>item.Sequence))
        {
            builder.Append(entry.Sequence).Append('|')
                .Append(entry.EvidenceReleaseFactFingerprint).Append('|')
                .Append(entry.AuditTransitionFingerprint).Append('|')
                .Append(entry.QualityRunId).Append('|')
                .Append(entry.RequestedCount).Append('|')
                .Append(entry.FoundCount).Append('|')
                .Append(entry.MissingCount).Append('|')
                .Append(entry.AllRequestedResolved).Append('|');
        }

        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(builder.ToString())))
            .ToLowerInvariant();
    }

    private static void ValidateCapacity(int capacity)
    {
        if(capacity<1 || capacity>64)
            throw new ArgumentOutOfRangeException(nameof(capacity));
    }

    private static bool IsLowerHex(string value)=>
        value.Length==64 &&
        value.All(character=>
            Uri.IsHexDigit(character) &&
            char.ToLowerInvariant(character)==character);
}
