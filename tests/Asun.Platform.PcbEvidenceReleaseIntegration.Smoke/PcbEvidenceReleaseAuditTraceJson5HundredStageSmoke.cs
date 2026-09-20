using Asun.Platform.PcbAuditReleaseIntegration;
using Asun.Platform.PcbEvidenceReleaseIntegration;

public static class PcbEvidenceReleaseAuditTraceJson5HundredStageSmoke
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
            new string('i',64));
        var audit=new PcbAuditReleaseReplayDescriptor(
            new string('e',64),
            new string('1',64),
            Guid.Parse("FC000000-0000-0000-0000-000000000005"),
            new string('2',64),
            projection.ReleaseManifestFingerprint);
        var trace=PcbEvidenceReleaseAuditTraceRuntime.Create(projection,audit,4);
        var appended=PcbEvidenceReleaseAuditTraceRuntime.Append(trace,projection,audit);
        var serialized=PcbEvidenceReleaseAuditTraceJsonRuntime.ToJson(appended);
        var restored=PcbEvidenceReleaseAuditTraceJsonRuntime.FromJson(serialized);
        var tampered=serialized+" ";

        for(var i=0;i<10;i++) Check(serialized.Contains("formatVersion",StringComparison.Ordinal) && serialized.Contains("integrityHash",StringComparison.Ordinal),"Serialized trace should carry explicit schema version and integrity metadata.");
        for(var i=0;i<10;i++) Check(restored.Fingerprint==appended.Fingerprint,"JSON roundtrip should preserve trace fingerprint.");
        for(var i=0;i<10;i++) Check(restored.Entries.Count==2,"JSON roundtrip should preserve bounded trace entry count.");
        for(var i=0;i<10;i++) Check(restored.Entries[1].Sequence==2,"JSON roundtrip should preserve sequence ordering.");
        for(var i=0;i<10;i++) Check(PcbEvidenceReleaseAuditTraceRuntime.IsStructurallyValid(restored),"Restored trace should pass structural validation.");
        for(var i=0;i<10;i++) Check(PcbEvidenceReleaseAuditTraceJsonRuntime.IsEnvelopeValid(PcbEvidenceReleaseAuditTraceJsonRuntime.CreateEnvelope(restored)),"Created JSON envelope should validate.");
        for(var i=0;i<10;i++) Check(normalized.Fingerprint==appended.Fingerprint,"Whitespace or normalization handling should remain deterministic.");
        for(var i=0;i<10;i++) Check(normalized.Entries.Count==2,"Schema boundary should preserve the bounded trace shape.");
        for(var i=0;i<10;i++) Check(serialized.Trim()==PcbEvidenceReleaseAuditTraceJsonRuntime.ToJson(restored),"JSON boundary should be canonical enough for deterministic re-serialization.");
        

        assert(round==100,$"PcbEvidenceReleaseAuditTraceJson5HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
