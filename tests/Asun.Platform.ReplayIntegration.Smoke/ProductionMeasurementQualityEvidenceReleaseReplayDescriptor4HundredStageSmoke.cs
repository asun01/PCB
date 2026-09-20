using Asun.Platform.QualityEvidenceIntegration;
using Asun.Platform.ReleaseIntegration;

public static class ProductionMeasurementQualityEvidenceReleaseReplayDescriptor4HundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}
        var measurementBinding=new ProductionMeasurementQualityEvidenceReleaseBinding(
            7,
            new string('c',64),
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
        var descriptor=ProductionMeasurementQualityEvidenceReleaseReplayDescriptorRuntime.Create(measurementBinding,replayBinding);
        var badManifest=replayBinding with {ReleaseManifestFingerprint=new string('a',64)};
var badReplay=replayBinding with {ReplayBundleFingerprint=new string('b',64)};
for(var i=0;i<10;i++) Check(!ProductionMeasurementQualityEvidenceReleaseReplayDescriptorRuntime.IsValid(measurementBinding,badManifest,descriptor),"Manifest tampering should be rejected.");
for(var i=0;i<10;i++) Check(!ProductionMeasurementQualityEvidenceReleaseReplayDescriptorRuntime.IsValid(measurementBinding,badReplay,descriptor),"Replay bundle tampering should be rejected.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityEvidenceReleaseReplayDescriptorRuntime.Validate(measurementBinding,badManifest,descriptor).Count>0,"Manifest tampering should emit diagnostics.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityEvidenceReleaseReplayDescriptorRuntime.Validate(measurementBinding,badReplay,descriptor).Count>0,"Replay bundle tampering should emit diagnostics.");
for(var i=0;i<10;i++) Check(descriptor.ProductionInputFingerprint==measurementBinding.ProductionInputFingerprint,"Baseline input remains stable.");
for(var i=0;i<10;i++) Check(descriptor.ReleaseManifestFingerprint==replayBinding.ReleaseManifestFingerprint,"Baseline manifest remains stable.");
for(var i=0;i<10;i++) Check(descriptor.ReplayBundleFingerprint==replayBinding.ReplayBundleFingerprint,"Baseline replay bundle remains stable.");
for(var i=0;i<10;i++) Check(descriptor.Sequence==measurementBinding.Sequence,"Baseline sequence remains stable.");
for(var i=0;i<10;i++) Check(descriptor.QualityRunId==replayBinding.QualityRunId,"Baseline Quality run remains stable.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityEvidenceReleaseReplayDescriptorRuntime.IsValid(measurementBinding,replayBinding,descriptor),"Baseline descriptor remains valid.");
        assert(round==100,$"ProductionMeasurementQualityEvidenceReleaseReplayDescriptor4HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
