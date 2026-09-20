using Asun.Platform.ReleaseIntegration;
using Asun.Platform.ReplayIntegration;
using Asun.Production.Runtime;

public static class ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptor4HundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}
        var provenance=new ProductionFrameProvenance(
            new Asun.Device.Contracts.FrameSequence(7),
            640,
            480,
            "Mono8",
            DateTimeOffset.Parse("2026-09-20T05:00:00Z"),
            new string('b',64));
        var releaseBinding=new ProductionMeasurementQualityEvidenceReleaseBinding(
            7,
            new string('b',64),
            Guid.Parse("E7000000-0000-0000-0000-000000000001"),
            "C1",
            new string('d',64),
            new string('e',64),
            true,
            new string('f',64));
        var replayBinding=new ProductionQualityEvidenceReleaseReplayBinding(
            Guid.Parse("E7000000-0000-0000-0000-000000000010"),
            Guid.Parse("E7000000-0000-0000-0000-000000000011"),
            new string('d',64),
            new string('e',64),
            true,
            new string('f',64));
        var replayDescriptor=ProductionMeasurementQualityEvidenceReleaseReplayDescriptorRuntime.Create(releaseBinding,replayBinding);
        var descriptor=ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptorRuntime.Create(provenance,replayDescriptor);
var badReplay=replayDescriptor with {ProductionInputFingerprint=new string('c',64)};
var badReplayFingerprint=replayDescriptor with {DescriptorFingerprint=new string('c',64)};
for(var i=0;i<10;i++) Check(!ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptorRuntime.IsValid(provenance,badReplay,descriptor),"Replay input identity tampering should be rejected.");
for(var i=0;i<10;i++) Check(!ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptorRuntime.IsValid(provenance,badReplayFingerprint,descriptor),"Replay fingerprint tampering should be rejected.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptorRuntime.Validate(provenance,badReplay,descriptor).Count>0,"Replay input tampering should emit diagnostics.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptorRuntime.Validate(provenance,badReplayFingerprint,descriptor).Count>0,"Replay fingerprint tampering should emit diagnostics.");
for(var i=0;i<10;i++) Check(descriptor.ReleaseReplayDescriptorFingerprint==replayDescriptor.DescriptorFingerprint,"Baseline replay identity remains stable.");
for(var i=0;i<10;i++) Check(descriptor.ProductionInputFingerprint==provenance.PayloadFingerprint,"Baseline input identity remains stable.");
for(var i=0;i<10;i++) Check(descriptor.Sequence==provenance.Sequence.Value,"Baseline sequence remains stable.");
for(var i=0;i<10;i++) Check(descriptor.ProvenanceFingerprint.Length==64,"Baseline fingerprint remains fixed width.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptorRuntime.IsValid(provenance,replayDescriptor,descriptor),"Baseline descriptor remains valid.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptorRuntime.Create(provenance,replayDescriptor).ProvenanceFingerprint==descriptor.ProvenanceFingerprint,"Creation remains deterministic.");
        assert(round==100,$"ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptor4HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
