using Asun.Platform.PcbAuditReleaseIntegration;
using Asun.Platform.PcbEvidenceReleaseIntegration;

public static class PcbEvidenceReleaseAuditTraceReplayDescriptor5HundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        var projection=new PcbEvidenceReleaseFactProjection(
            new string('a',64),
            new string('b',64),
            new string('c',64),
            new string('d',64),
            4,
            4,
            0,
            true,
            new string('n',64));
        var audit=new PcbAuditReleaseReplayDescriptor(
            new string('e',64),
            new string('f',64),
            Guid.Parse("FD000000-0000-0000-0000-000000000005"),
            new string('2',64),
            projection.ReleaseManifestFingerprint);
        var trace=PcbEvidenceReleaseAuditTraceRuntime.Create(projection,audit,4);
        var descriptor=PcbEvidenceReleaseAuditTraceReplayDescriptorRuntime.Create(trace,audit);
        var tamperedTrace=trace with {ReleaseManifestFingerprint=new string('9',64)};
        var tamperedJson=descriptor with {SerializedJsonFingerprint=new string('8',64)};
        var tamperedQuality=descriptor with {QualityRunId=Guid.Parse("FD000000-0000-0000-0000-000000000009")};

        for(var i=0;i<10;i++) Check(descriptor.FormatVersion==1,"Replay descriptor should expose the explicit trace schema version.");
        for(var i=0;i<10;i++) Check(descriptor.TraceFingerprint==trace.Fingerprint,"Replay descriptor should bind the source trace fingerprint.");
        for(var i=0;i<10;i++) Check(descriptor.SerializedJsonFingerprint.Length==64,"Replay descriptor should bind the serialized JSON payload fingerprint.");
        for(var i=0;i<10;i++) Check(descriptor.ReleaseManifestFingerprint==trace.ReleaseManifestFingerprint,"Replay descriptor should preserve Release manifest identity.");
        for(var i=0;i<10;i++) Check(descriptor.QualityRunId==audit.QualityRunId,"Replay descriptor should preserve Quality identity.");
        for(var i=0;i<10;i++) Check(descriptor.EntryCount==trace.Entries.Count,"Replay descriptor should preserve bounded trace entry count.");
        for(var i=0;i<10;i++) Check(PcbEvidenceReleaseAuditTraceReplayDescriptorRuntime.IsValid(trace,audit,descriptor),"Replay descriptor should validate against the source trace and audit replay.");
        for(var i=0;i<10;i++) Check(!PcbEvidenceReleaseAuditTraceReplayDescriptorRuntime.IsValid(tamperedTrace,audit,descriptor),"Manifest tampering should be rejected at the replay boundary.");
        for(var i=0;i<10;i++) Check(!PcbEvidenceReleaseAuditTraceReplayDescriptorRuntime.IsValid(trace,audit,tamperedJson),"Serialized payload tampering should be rejected.");
        for(var i=0;i<10;i++) Check(!PcbEvidenceReleaseAuditTraceReplayDescriptorRuntime.IsValid(trace,audit,tamperedQuality),"Quality identity tampering should be rejected.");

        assert(round==100,$"PcbEvidenceReleaseAuditTraceReplayDescriptor5HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
