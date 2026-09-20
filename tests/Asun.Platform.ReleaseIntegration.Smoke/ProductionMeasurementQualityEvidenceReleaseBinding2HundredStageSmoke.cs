using Asun.Platform.QualityEvidenceIntegration;
using Asun.Platform.ReleaseIntegration;
using Asun.Release.Core;

public static class ProductionMeasurementQualityEvidenceReleaseBinding2HundredStageSmoke
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
        var badEvidence=binding with {EvidenceFingerprint=new string('a',64)};
var badManifest=binding with {ReleaseManifestFingerprint=new string('b',64)};
for(var i=0;i<10;i++) Check(!ProductionMeasurementQualityEvidenceReleaseBindingRuntime.IsValid(measurementEvidenceBinding,manifest,badEvidence),"Evidence tampering should be rejected.");
for(var i=0;i<10;i++) Check(!ProductionMeasurementQualityEvidenceReleaseBindingRuntime.IsValid(measurementEvidenceBinding,manifest,badManifest),"Manifest tampering should be rejected.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityEvidenceReleaseBindingRuntime.Validate(measurementEvidenceBinding,manifest,badEvidence).Count>0,"Evidence tampering should produce diagnostics.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityEvidenceReleaseBindingRuntime.Validate(measurementEvidenceBinding,manifest,badManifest).Count>0,"Manifest tampering should produce diagnostics.");
for(var i=0;i<10;i++) Check(binding.EvidenceFingerprint==measurementEvidenceBinding.EvidenceFingerprint,"Baseline Evidence identity should remain stable.");
for(var i=0;i<10;i++) Check(binding.ReleaseManifestFingerprint==manifest.Fingerprint,"Baseline Manifest identity should remain stable.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityEvidenceReleaseBindingRuntime.IsValid(measurementEvidenceBinding,manifest,binding),"Baseline Release binding should remain valid.");
for(var i=0;i<10;i++) Check(binding.Fingerprint.Length==64,"Baseline Release fingerprint should remain fixed width.");
        assert(round==100,$"ProductionMeasurementQualityEvidenceReleaseBinding2HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
