using Asun.Platform.ReleaseIntegration;
using Asun.Platform.ReplayIntegration;
using Asun.Production.Runtime;

public static class ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptor5HundredStageSmoke
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
var badDescriptor=descriptor with {ReleaseReplayDescriptorFingerprint=new string('c',64)};
var badFingerprint=descriptor with {ProvenanceFingerprint=new string('c',64)};
for(var i=0;i<10;i++) Check(!ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptorRuntime.IsValid(provenance,replayDescriptor,badDescriptor),"Replay reference tampering should be rejected.");
for(var i=0;i<10;i++) Check(!ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptorRuntime.IsValid(provenance,replayDescriptor,badFingerprint),"Descriptor fingerprint tampering should be rejected.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptorRuntime.Validate(provenance,replayDescriptor,badDescriptor).Count>0,"Replay reference tampering should emit diagnostics.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptorRuntime.Validate(provenance,replayDescriptor,badFingerprint).Count>0,"Descriptor fingerprint tampering should emit diagnostics.");
for(var i=0;i<10;i++) Check(descriptor.CapturedAtUtc==provenance.CapturedAtUtc,"Capture timestamp remains stable.");
for(var i=0;i<10;i++) Check(descriptor.PixelFormat==provenance.PixelFormat,"Pixel format remains stable.");
for(var i=0;i<10;i++) Check(descriptor.ProductionInputFingerprint==provenance.PayloadFingerprint,"Input identity remains stable.");
for(var i=0;i<10;i++) Check(descriptor.Width==provenance.Width && descriptor.Height==provenance.Height,"Dimensions remain stable.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptorRuntime.IsValid(provenance,replayDescriptor,descriptor),"Canonical provenance descriptor remains valid.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptorRuntime.Create(provenance,replayDescriptor).ProvenanceFingerprint==descriptor.ProvenanceFingerprint,"Descriptor remains deterministic.");
        assert(round==100,$"ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptor5HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
