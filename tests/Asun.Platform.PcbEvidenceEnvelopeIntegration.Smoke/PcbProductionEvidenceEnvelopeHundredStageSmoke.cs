using Asun.Platform.PcbExecutionIntegration;
using Asun.Platform.PcbEvidenceEnvelopeIntegration;
using Asun.Platform.PcbEvidenceResolutionIntegration;
using Asun.Platform.PcbReleaseIntegration;
using Asun.Platform.PcbReplayIntegration;

public static class PcbProductionEvidenceEnvelopeHundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var execution=new PcbExecutionSnapshot(
            new string('a',64),
            Guid.Parse("C5000000-0000-0000-0000-000000000001"),
            new string('b',64),
            new string('c',64),
            2,
            2,
            Guid.Parse("C6000000-0000-0000-0000-000000000001"),
            new string('d',64),
            new string('e',64));
        var replay=new PcbEndToEndReplaySnapshot(
            execution.Fingerprint,
            new string('1',64),
            new string('2',64),
            new string('3',64),
            new string('4',64),
            new string('5',64));
        var resolution=new PcbExecutionEvidenceResolution(
            execution.Fingerprint,
            new string('6',64),
            new string('7',64),
            Array.Empty<Asun.Platform.Evidence.EvidenceHandle>(),
            Array.Empty<Asun.Platform.Evidence.EvidenceHandle>(),
            Array.Empty<Asun.Platform.Evidence.EvidenceHandle>(),
            new string('8',64));
        var release=new PcbExecutionReleaseProjection(
            execution.AssemblyFingerprint,
            execution.ProductionSessionId,
            execution.Fingerprint,
            new string('9',64),
            1,
            true,
            new string('0',64));
        var envelope=PcbProductionEvidenceEnvelopeRuntime.Create(
            execution,
            replay,
            resolution,
            release);
        var tampered=envelope with
        {
            ReleaseProjectionFingerprint=new string('a',64)
        };
        var wrongSession=envelope with
        {
            ProductionSessionId=Guid.Parse("C7000000-0000-0000-0000-000000000001")
        };

        for(var i=0;i<10;i++) Check(envelope.AssemblyFingerprint==execution.AssemblyFingerprint,"Envelope should retain assembly identity.");
        for(var i=0;i<10;i++) Check(envelope.ProductionSessionId==execution.ProductionSessionId,"Envelope should retain production identity.");
        for(var i=0;i<10;i++) Check(envelope.ExecutionSnapshotFingerprint==execution.Fingerprint,"Envelope should retain execution identity.");
        for(var i=0;i<10;i++) Check(envelope.ReplaySnapshotFingerprint==replay.Fingerprint,"Envelope should retain replay identity.");
        for(var i=0;i<10;i++) Check(envelope.EvidenceResolutionFingerprint==resolution.Fingerprint,"Envelope should retain evidence resolution identity.");
        for(var i=0;i<10;i++) Check(envelope.ReleaseProjectionFingerprint==release.Fingerprint,"Envelope should retain release projection identity.");
        for(var i=0;i<10;i++) Check(PcbProductionEvidenceEnvelopeValidationRuntime.IsValid(execution,replay,resolution,release,envelope),"Evidence envelope should validate.");
        for(var i=0;i<10;i++) Check(!PcbProductionEvidenceEnvelopeValidationRuntime.IsValid(execution,replay,resolution,release,tampered),"Release identity tampering should be rejected.");
        for(var i=0;i<10;i++) Check(!PcbProductionEvidenceEnvelopeValidationRuntime.IsValid(execution,replay,resolution,release,wrongSession),"Production identity tampering should be rejected.");
        for(var i=0;i<10;i++) Check(envelope.Fingerprint.Length==64,"Envelope fingerprint should be fixed width.");

        assert(round==100,$"PCB production evidence envelope smoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
