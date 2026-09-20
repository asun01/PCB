using Asun.Platform.QualityEvidenceIntegration;
using Asun.Platform.ReleaseIntegration;
using Asun.Release.Core;

public static class ProductionMeasurementQualityEvidenceReleaseBinding4HundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}
        var measurementEvidenceBinding=new ProductionMeasurementQualityEvidenceBinding(
            7,
            new string('c',64),
            Guid.Parse("E7000000-0000-0000-0000-000000000001"),
            Asun.Domain.Quality.QualityFindingId.Create("PCB.PLACEMENT.OBSERVED"),
            "C1",
            new string('a',64),
            1,
            new string('d',64),
            new string('e',64));
        var manifest=ReleaseManifestRuntime.Create(
            new ReleaseIdentity("Asun PCB",new Version(1,0,0),"stable"),
            new[]{new ReleaseArtifact("measurement/quality/evidence",new string('f',64),128)});
        var binding=ProductionMeasurementQualityEvidenceReleaseBindingRuntime.Create(measurementEvidenceBinding,manifest);
        var badReady=binding with {ReleaseReady=!binding.ReleaseReady};
var malformed=binding with {Fingerprint="bad"};
for(var i=0;i<10;i++) Check(!ProductionMeasurementQualityEvidenceReleaseBindingRuntime.IsValid(measurementEvidenceBinding,manifest,badReady),"Readiness tampering should be rejected.");
for(var i=0;i<10;i++) Check(!ProductionMeasurementQualityEvidenceReleaseBindingRuntime.IsValid(measurementEvidenceBinding,manifest,malformed),"Fingerprint tampering should be rejected.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityEvidenceReleaseBindingRuntime.Validate(measurementEvidenceBinding,manifest,badReady).Count>0,"Readiness tampering should emit diagnostics.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityEvidenceReleaseBindingRuntime.Validate(measurementEvidenceBinding,manifest,malformed).Count>0,"Fingerprint tampering should emit diagnostics.");
for(var i=0;i<10;i++) Check(binding.ProductionInputFingerprint==measurementEvidenceBinding.ProductionInputFingerprint,"Baseline input identity remains stable.");
for(var i=0;i<10;i++) Check(binding.ReleaseManifestFingerprint==manifest.Fingerprint,"Baseline manifest identity remains stable.");
for(var i=0;i<10;i++) Check(binding.ReleaseReady==ReleaseReadinessRuntime.Evaluate(manifest).Ready,"Baseline readiness remains stable.");
for(var i=0;i<10;i++) Check(binding.QualityResultId==measurementEvidenceBinding.QualityResultId,"Baseline Quality identity remains stable.");
for(var i=0;i<10;i++) Check(binding.ComponentId==measurementEvidenceBinding.ComponentId,"Baseline component remains stable.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityEvidenceReleaseBindingRuntime.IsValid(measurementEvidenceBinding,manifest,binding),"Baseline binding remains valid.");
        assert(round==100,$"ProductionMeasurementQualityEvidenceReleaseBinding4HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
