using Asun.Device.Impl;
using Asun.Domain.Quality;
using Asun.Platform.Evidence;
using Asun.Platform.Pipeline;
using Asun.Platform.ReplayIntegration;
using Asun.Program.Core;
using Asun.Production.Runtime;
using Asun.Release.Core;

public static class ProductionQualityEvidenceReleaseReplayBinding5HundredStageSmoke
{
    public static async ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}
        var program=new InspectionProgram(
            Guid.Parse("8A000000-0000-0000-0000-000000000001"),
            "ReleaseReplayBindingProgram",
            new Version(1,0,0),
            new[]{
                new ProgramStep(Guid.Parse("8B000000-0000-0000-0000-000000000001"),1,ProgramStepKind.Acquire,"Acquire",Array.Empty<ProgramParameter>()),
                new ProgramStep(Guid.Parse("8B000000-0000-0000-0000-000000000002"),2,ProgramStepKind.Measure,"Measure",Array.Empty<ProgramParameter>())});
        var plan=ProgramExecutionPlanRuntime.Create(program);
        var pipeline=PipelineDefinitionRuntime.Create(new[]{
            new PipelineStage<Asun.Device.Contracts.CapturedFrame>(1,"Acquire",frame=>frame),
            new PipelineStage<Asun.Device.Contracts.CapturedFrame>(2,"Measure",frame=>frame)});
        var definition=new ProductionSessionDefinition(Guid.Parse("8C000000-0000-0000-0000-000000000001"),plan,pipeline,2);
        var production=await ProductionSessionRuntime.RunAsync(definition,new SimulatedFrameSource(4,4));
        var qualityRun=QualityInspectionRunRuntime.Create(
            Guid.Parse("8D000000-0000-0000-0000-000000000001"),
            new[]{
                new QualityInspectionResult(Guid.Parse("8E000000-0000-0000-0000-000000000001"),
                    new QualityInspectionSnapshot(Guid.Parse("8F000000-0000-0000-0000-000000000001"),1,
                        new QualityFindingSet(new[]{new QualityFinding(QualityFindingId.Create("REPLAY.INFO"),QualityOutcome.Informational,QualitySeverity.Information,"Frame 1")}),
                        new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))),
                new QualityInspectionResult(Guid.Parse("8E000000-0000-0000-0000-000000000002"),
                    new QualityInspectionSnapshot(Guid.Parse("8F000000-0000-0000-0000-000000000002"),2,
                        new QualityFindingSet(new[]{new QualityFinding(QualityFindingId.Create("REPLAY.INFO"),QualityOutcome.Informational,QualitySeverity.Information,"Frame 2")}),
                        new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>())))
            });
        var evidence=ProductionEvidenceReferenceProjectionRuntime.Create(production,new[]{
            new ProductionEvidenceFrameReference(1,new[]{EvidenceHandle.Create("release/1")}),
            new ProductionEvidenceFrameReference(2,new[]{EvidenceHandle.Create("release/2")})});
        var bundle=ProductionQualityEvidenceReplayBundleRuntime.Create(definition,production,qualityRun,evidence);
        var manifest=ReleaseManifestRuntime.Create(
            new ReleaseIdentity("Asun PCB",new Version(1,0,0),"stable"),
            new[]{new ReleaseArtifact("replay/bundle",new string('a',64),128)});
        var binding=ProductionQualityEvidenceReleaseReplayBindingRuntime.Create(definition,production,qualityRun,evidence,bundle,manifest);
        var changed=ReleaseManifestRuntime.Create(manifest.Identity,new[]{new ReleaseArtifact("replay/changed",new string('d',64),128)});
var changedBinding=ProductionQualityEvidenceReleaseReplayBindingRuntime.Create(definition,production,qualityRun,evidence,bundle,changed);
for(var i=0;i<10;i++) Check(changed.Fingerprint!=manifest.Fingerprint,"Changed Release manifest should alter manifest identity.");
for(var i=0;i<10;i++) Check(changedBinding.Fingerprint!=binding.Fingerprint,"Changed Release manifest should alter replay binding identity.");
for(var i=0;i<10;i++) Check(!ProductionQualityEvidenceReleaseReplayBindingRuntime.IsValid(definition,production,qualityRun,evidence,bundle,manifest,changedBinding),"Changed binding should not validate against original manifest.");
for(var i=0;i<10;i++) Check(ProductionQualityEvidenceReleaseReplayBindingRuntime.IsValid(definition,production,qualityRun,evidence,bundle,changed,changedBinding),"Changed binding should validate against changed source.");
for(var i=0;i<10;i++) Check(changedBinding.ReleaseManifestFingerprint==changed.Fingerprint,"Changed binding should preserve changed manifest identity.");
for(var i=0;i<10;i++) Check(changedBinding.ReplayBundleFingerprint==bundle.Fingerprint,"Changed binding should preserve bundle identity.");
for(var i=0;i<10;i++) Check(binding.ReleaseManifestFingerprint==manifest.Fingerprint,"Original manifest identity should remain stable.");
for(var i=0;i<10;i++) Check(ProductionQualityEvidenceReleaseReplayBindingRuntime.Create(definition,production,qualityRun,evidence,bundle,manifest).Fingerprint==binding.Fingerprint,"Original binding creation should remain deterministic.");
for(var i=0;i<10;i++) Check(binding.Fingerprint.Length==64,"Original binding fingerprint should remain fixed width.");
for(var i=0;i<10;i++) Check(binding.ReleaseReady==ReleaseReadinessRuntime.Evaluate(manifest).Ready,"Original release readiness should remain stable.");

        assert(round==100,$"ProductionQualityEvidenceReleaseReplayBinding5HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
