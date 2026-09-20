using Asun.Platform.ReleaseIntegration;
using Asun.Platform.ReplayIntegration;
using Asun.Production.Runtime;

public static class ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptor3HundredStageSmoke
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
var badPixel=provenance with {PixelFormat="Mono16"};
var badDimensions=provenance with {Width=0};
for(var i=0;i<10;i++) Check(!ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptorRuntime.IsValid(badPixel,replayDescriptor,descriptor),"Pixel format tampering should be rejected.");
for(var i=0;i<10;i++) Check(!ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptorRuntime.IsValid(badDimensions,replayDescriptor,descriptor),"Dimension tampering should be rejected.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptorRuntime.Validate(badPixel,replayDescriptor,descriptor).Count>0,"Pixel format tampering should emit diagnostics.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptorRuntime.Validate(badDimensions,replayDescriptor,descriptor).Count>0,"Dimension tampering should emit diagnostics.");
for(var i=0;i<10;i++) Check(descriptor.PixelFormat==provenance.PixelFormat,"Baseline pixel format remains stable.");
for(var i=0;i<10;i++) Check(descriptor.Width==provenance.Width,"Baseline width remains stable.");
for(var i=0;i<10;i++) Check(descriptor.Height==provenance.Height,"Baseline height remains stable.");
for(var i=0;i<10;i++) Check(descriptor.Sequence==provenance.Sequence.Value,"Baseline sequence remains stable.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptorRuntime.IsValid(provenance,replayDescriptor,descriptor),"Baseline descriptor remains valid.");
for(var i=0;i<10;i++) Check(descriptor.ProvenanceFingerprint==ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptorRuntime.Create(provenance,replayDescriptor).ProvenanceFingerprint,"Creation remains deterministic.");
        assert(round==100,$"ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptor3HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
