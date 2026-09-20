using Asun.Platform.PcbAuditIntegration;
using Asun.Platform.PcbAuditReleaseIntegration;
using Asun.Platform.PcbEvidenceEnvelopeIntegration;
using Asun.Release.Core;

public static class PcbAuditReleaseTransitionHundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var envelope=new PcbProductionEvidenceEnvelope(
            new string('a',64),
            Guid.Parse("CE000000-0000-0000-0000-000000000001"),
            new string('b',64),
            new string('c',64),
            new string('d',64),
            new string('e',64),
            new string('f',64));
        var audit=new PcbExecutionAuditWindowProjection(
            envelope.Fingerprint,
            Guid.Parse("CF000000-0000-0000-0000-000000000001"),
            1,
            new string('1',64),
            new string('2',64));
        var manifest=ReleaseManifestRuntime.Create(
            new ReleaseIdentity("Asun PCB",new Version(1,0,0),"stable"),
            new[]{new ReleaseArtifact("pcb/audit.logical",new string('3',64),20)});
        var projection=PcbAuditReleaseTransitionProjectionRuntime.Create(envelope,audit,manifest);
        var tampered=projection with {AuditCount=0};
        var wrongManifest=projection with {ReleaseManifestFingerprint=new string('4',64)};

        for(var i=0;i<10;i++) Check(projection.EnvelopeFingerprint==envelope.Fingerprint,"Transition should retain evidence envelope identity.");
        for(var i=0;i<10;i++) Check(projection.QualityRunId==audit.QualityRunId,"Transition should retain Quality identity.");
        for(var i=0;i<10;i++) Check(projection.AuditWindowFingerprint==audit.AuditWindowFingerprint,"Transition should retain audit window identity.");
        for(var i=0;i<10;i++) Check(projection.ReleaseManifestFingerprint==manifest.Fingerprint,"Transition should retain Release identity.");
        for(var i=0;i<10;i++) Check(projection.AuditCount==1,"Transition should report one audit envelope.");
        for(var i=0;i<10;i++) Check(ReleaseReadinessRuntime.Evaluate(manifest).Ready,"Transition should report factual Release readiness.");
        for(var i=0;i<10;i++) Check(projection.Fingerprint.Length==64,"Transition fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(PcbAuditReleaseTransitionProjectionValidationRuntime.IsValid(envelope,audit,manifest,projection),"Audit release transition should validate.");
        for(var i=0;i<10;i++) Check(!PcbAuditReleaseTransitionProjectionValidationRuntime.IsValid(envelope,audit,manifest,tampered),"Audit count tampering should be rejected.");
        for(var i=0;i<10;i++) Check(!PcbAuditReleaseTransitionProjectionValidationRuntime.IsValid(envelope,audit,manifest,wrongManifest),"Release identity tampering should be rejected.");

        assert(round==100,$"PCB audit release transition smoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
