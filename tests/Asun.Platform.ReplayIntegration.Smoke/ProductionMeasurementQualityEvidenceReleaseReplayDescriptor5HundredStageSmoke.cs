using Asun.Platform.QualityEvidenceIntegration;
using Asun.Platform.ReleaseIntegration;

public static class ProductionMeasurementQualityEvidenceReleaseReplayDescriptor5HundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}
        var measurementBinding=new ProductionMeasurementQualityEvidenceReleaseBinding(
            7,
            new string('c',64),
            Guid.Parse("E7000000-0000-0000-0000-000000000001"),
            Asun.Domain.Quality.QualityFindingId.Create("PCB.PLACEMENT.OBSERVED"),
            "C1",
            new string('a',64),
            1,
            new string('d',64),
            new string('e',64));
        var replayBinding=new ProductionQualityEvidenceReleaseReplayBinding(
            Guid.Parse("E7000000-0000-0000-0000-000000000010"),
            Guid.Parse("E7000000-0000-0000-0000-000000000011"),
            new string('d',64),
            new string('e',64),
            true,
            new string('f',64));
        var descriptor=ProductionMeasurementQualityEvidenceReleaseReplayDescriptorRuntime.Create(
            measurementBinding,replayBinding);
var badDescriptor=descriptor with {ReleaseReplayBindingFingerprint=new string('b',64)};
var badDescriptorFingerprint=descriptor with {DescriptorFingerprint=new string('b',64)};
for(var i=0;i<10;i++) Check(!ProductionMeasurementQualityEvidenceReleaseReplayDescriptorRuntime.IsValid(measurementBinding,replayBinding,badDescriptor),"Descriptor replay-binding identity tampering should be rejected.");
for(var i=0;i<10;i++) Check(!ProductionMeasurementQualityEvidenceReleaseReplayDescriptorRuntime.IsValid(measurementBinding,replayBinding,badDescriptorFingerprint),"Descriptor fingerprint tampering should be rejected.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityEvidenceReleaseReplayDescriptorRuntime.Validate(measurementBinding,replayBinding,badDescriptor).Count>0,"Replay identity tampering should emit diagnostics.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityEvidenceReleaseReplayDescriptorRuntime.Validate(measurementBinding,replayBinding,badDescriptorFingerprint).Count>0,"Fingerprint tampering should emit diagnostics.");
for(var i=0;i<10;i++) Check(descriptor.DescriptorFingerprint.Length==64,"Descriptor fingerprint width remains fixed.");
for(var i=0;i<10;i++) Check(descriptor.DescriptorFingerprint.All(Uri.IsHexDigit),"Descriptor fingerprint remains hexadecimal.");
for(var i=0;i<10;i++) Check(descriptor.ReleaseManifestFingerprint==measurementBinding.ReleaseManifestFingerprint,"Release manifest identity remains stable.");
for(var i=0;i<10;i++) Check(descriptor.EvidenceFingerprint==measurementBinding.EvidenceFingerprint,"Evidence identity remains stable.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityEvidenceReleaseReplayDescriptorRuntime.IsValid(measurementBinding,replayBinding,descriptor),"Canonical descriptor remains valid.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityEvidenceReleaseReplayDescriptorRuntime.Create(measurementBinding,replayBinding).DescriptorFingerprint==descriptor.DescriptorFingerprint,"Descriptor remains deterministic.");
        assert(round==100,$"ProductionMeasurementQualityEvidenceReleaseReplayDescriptor5HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
