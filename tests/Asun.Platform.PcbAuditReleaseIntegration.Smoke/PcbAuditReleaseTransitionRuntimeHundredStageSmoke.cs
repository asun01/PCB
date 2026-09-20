using Asun.Platform.PcbAuditReleaseIntegration;

public static class PcbAuditReleaseTransitionRuntimeHundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var projection=new PcbAuditReleaseTransitionProjection(
            new string('a',64),
            Guid.Parse("CE000000-0000-0000-0000-000000000001"),
            new string('b',64),
            new string('c',64),
            2,
            true,
            new string('d',64));
        var copy=projection with {};
        var tampered=projection with {ReleaseManifestFingerprint=new string('e',64)};
        var descriptor=PcbAuditReleaseTransitionRuntime.CreateReplayDescriptor(projection);

        for(var i=0;i<10;i++) Check(PcbAuditReleaseTransitionRuntime.CreateTransitionKey(projection).Length==64,"Transition key should be fixed width.");
        for(var i=0;i<10;i++) Check(PcbAuditReleaseTransitionRuntime.CreateTransitionKey(projection)==PcbAuditReleaseTransitionRuntime.CreateTransitionKey(copy),"Equivalent projections should have identical transition keys.");
        for(var i=0;i<10;i++) Check(PcbAuditReleaseTransitionRuntime.CreateCanonicalIdentity(projection).Contains(projection.Fingerprint,StringComparison.Ordinal),"Canonical identity should include transition fingerprint.");
        for(var i=0;i<10;i++) Check(PcbAuditReleaseTransitionRuntime.IsEquivalent(projection,copy),"Equivalent projections should be recognized.");
        for(var i=0;i<10;i++) Check(!PcbAuditReleaseTransitionRuntime.IsEquivalent(projection,tampered),"Tampered projections should not be equivalent.");
        for(var i=0;i<10;i++) Check(descriptor.TransitionFingerprint==projection.Fingerprint,"Replay descriptor should preserve transition identity.");
        for(var i=0;i<10;i++) Check(descriptor.EnvelopeFingerprint==projection.EnvelopeFingerprint,"Replay descriptor should preserve envelope identity.");
        for(var i=0;i<10;i++) Check(descriptor.QualityRunId==projection.QualityRunId,"Replay descriptor should preserve Quality identity.");
        for(var i=0;i<10;i++) Check(descriptor.AuditWindowFingerprint==projection.AuditWindowFingerprint,"Replay descriptor should preserve audit identity.");
        for(var i=0;i<10;i++) Check(descriptor.ReleaseManifestFingerprint==projection.ReleaseManifestFingerprint,"Replay descriptor should preserve Release identity.");

        assert(round==100,$"Audit release transition runtime smoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
