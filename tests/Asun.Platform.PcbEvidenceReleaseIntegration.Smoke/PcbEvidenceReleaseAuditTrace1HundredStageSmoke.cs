using Asun.Platform.PcbAuditReleaseIntegration;
using Asun.Platform.PcbEvidenceReleaseIntegration;

public static class PcbEvidenceReleaseAuditTrace1HundredStageSmoke
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
            new string('f',64));
        var audit=new PcbAuditReleaseReplayDescriptor(
            new string('f',64),
            new string('1',64),
            Guid.Parse("FB000000-0000-0000-0000-000000000001"),
            new string('2',64),
            projection.ReleaseManifestFingerprint);
        var trace=PcbEvidenceReleaseAuditTraceRuntime.Create(projection,audit,capacity:4);
        var appended=PcbEvidenceReleaseAuditTraceRuntime.Append(trace,projection,audit);
        var tampered=appended with
        {
            ReleaseManifestFingerprint=new string('9',64)
        };
        var tamperedEntry=appended with
        {
            Entries=appended.Entries.Select(entry=>entry with
            {
                MissingCount=1,
                AllRequestedResolved=false
            }).ToArray()
        };

        for(var i=0;i<10;i++) Check(trace.Entries.Count==1,"Initial release audit trace should contain one bounded fact.");
        for(var i=0;i<10;i++) Check(trace.Capacity==4,"Trace capacity should remain explicitly bounded.");
        for(var i=0;i<10;i++) Check(trace.ReleaseManifestFingerprint==projection.ReleaseManifestFingerprint,"Trace should preserve Release manifest identity.");
        for(var i=0;i<10;i++) Check(trace.Entries[0].EvidenceReleaseFactFingerprint==projection.Fingerprint,"Trace should preserve evidence release fact identity.");
        for(var i=0;i<10;i++) Check(trace.Entries[0].AuditTransitionFingerprint==audit.TransitionFingerprint,"Trace should preserve audit transition identity.");
        for(var i=0;i<10;i++) Check(trace.Entries[0].QualityRunId==audit.QualityRunId,"Trace should preserve Quality identity.");
        for(var i=0;i<10;i++) Check(PcbEvidenceReleaseAuditTraceRuntime.IsValid(trace,projection,audit),"Initial trace should validate.");
        for(var i=0;i<10;i++) Check(appended.Entries.Count==2,"Append should add one bounded trace entry.");
        for(var i=0;i<10;i++) Check(appended.Entries[1].Sequence==2,"Appended trace sequence should remain consecutive.");
        for(var i=0;i<10;i++) Check(!PcbEvidenceReleaseAuditTraceRuntime.IsValid(tampered,projection,audit) && !PcbEvidenceReleaseAuditTraceRuntime.IsValid(tamperedEntry,projection,audit),"Trace identity or count tampering should be rejected.");

        assert(round==100,$"PcbEvidenceReleaseAuditTrace1HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
