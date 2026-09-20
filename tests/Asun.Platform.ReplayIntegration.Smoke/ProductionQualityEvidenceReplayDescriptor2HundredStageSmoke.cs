using Asun.Device.Impl;
using Asun.Domain.Quality;
using Asun.Platform.Evidence;
using Asun.Platform.Pipeline;
using Asun.Platform.ReplayIntegration;
using Asun.Program.Core;
using Asun.Production.Runtime;

public static class ProductionQualityEvidenceReplayDescriptor2HundredStageSmoke
{
    public static async ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}
        var program=new InspectionProgram(
            Guid.Parse("8A000000-0000-0000-0000-000000000001"),
            "ReplayDescriptorProgram",
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
            new ProductionEvidenceFrameReference(1,new[]{EvidenceHandle.Create("replay/1")}),
            new ProductionEvidenceFrameReference(2,new[]{EvidenceHandle.Create("replay/2")})});
        var bundle=ProductionQualityEvidenceReplayBundleRuntime.Create(definition,production,qualityRun,evidence);
        var descriptor=ProductionQualityEvidenceReplayDescriptorRuntime.Create(definition,production,qualityRun,evidence,bundle);
        var bad=descriptor with {ProductionFingerprint=new string('a',64)};
var badQuality=descriptor with {QualityRunId=Guid.NewGuid()};
for(var i=0;i<10;i++) Check(!ProductionQualityEvidenceReplayDescriptorRuntime.IsValid(definition,production,qualityRun,evidence,bundle,bad),"Production fingerprint tampering should be rejected.");
for(var i=0;i<10;i++) Check(!ProductionQualityEvidenceReplayDescriptorRuntime.IsValid(definition,production,qualityRun,evidence,bundle,badQuality),"Quality identity tampering should be rejected.");
for(var i=0;i<10;i++) Check(ProductionQualityEvidenceReplayDescriptorRuntime.Validate(definition,production,qualityRun,evidence,bundle,bad).Count>0,"Production tampering should produce diagnostics.");
for(var i=0;i<10;i++) Check(ProductionQualityEvidenceReplayDescriptorRuntime.Validate(definition,production,qualityRun,evidence,bundle,badQuality).Count>0,"Quality tampering should produce diagnostics.");
for(var i=0;i<10;i++) Check(descriptor.ProductionFingerprint==production.Fingerprint,"Baseline production identity should remain stable.");
for(var i=0;i<10;i++) Check(descriptor.QualityRunId==qualityRun.RunId,"Baseline Quality identity should remain stable.");
for(var i=0;i<10;i++) Check(ProductionQualityEvidenceReplayDescriptorRuntime.IsValid(definition,production,qualityRun,evidence,bundle,descriptor),"Baseline descriptor should remain valid.");
for(var i=0;i<10;i++) Check(descriptor.DescriptorFingerprint.Length==64,"Baseline descriptor fingerprint should remain fixed width.");
for(var i=0;i<10;i++) Check(descriptor.BundleFingerprint==bundle.Fingerprint,"Baseline bundle identity should remain stable.");
for(var i=0;i<10;i++) Check(ProductionQualityEvidenceReplayDescriptorRuntime.Create(definition,production,qualityRun,evidence,bundle).BundleFingerprint==descriptor.BundleFingerprint,"Recreated descriptor should preserve bundle identity.");

        assert(round==100,$"ProductionQualityEvidenceReplayDescriptor2HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
