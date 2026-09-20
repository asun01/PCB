using Asun.Device.Impl;
using Asun.Domain.Quality;
using Asun.Platform.Evidence;
using Asun.Platform.Pipeline;
using Asun.Platform.ReplayIntegration;
using Asun.Program.Core;
using Asun.Production.Runtime;

public static class ProductionQualityEvidenceReplayDescriptor5HundredStageSmoke
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
        var reversed=evidence.Frames.Reverse().ToArray();
var reversedProjection=new ProductionEvidenceReferenceProjection(evidence.ProductionSessionId,evidence.ProductionFingerprint,reversed,evidence.Fingerprint);
var equivalentBundle=ProductionQualityEvidenceReplayBundleRuntime.Create(definition,production,qualityRun,reversedProjection);
var equivalent=ProductionQualityEvidenceReplayDescriptorRuntime.Create(definition,production,qualityRun,reversedProjection,equivalentBundle);
for(var i=0;i<10;i++) Check(equivalent.ProductionSessionId==descriptor.ProductionSessionId,"Reordered evidence should preserve production identity.");
for(var i=0;i<10;i++) Check(equivalent.QualityRunId==descriptor.QualityRunId,"Reordered evidence should preserve Quality identity.");
for(var i=0;i<10;i++) Check(equivalent.EvidenceFrameCount==descriptor.EvidenceFrameCount,"Reordered evidence should preserve count.");
for(var i=0;i<10;i++) Check(equivalent.BundleFingerprint==descriptor.BundleFingerprint,"Reordered evidence should preserve canonical bundle identity.");
for(var i=0;i<10;i++) Check(equivalent.DescriptorFingerprint==descriptor.DescriptorFingerprint,"Reordered evidence should preserve descriptor identity.");
for(var i=0;i<10;i++) Check(ProductionQualityEvidenceReplayDescriptorRuntime.IsEquivalent(descriptor,equivalent),"Equivalent reordered descriptors should compare equal.");
for(var i=0;i<10;i++) Check(ProductionQualityEvidenceReplayDescriptorRuntime.IsValid(definition,production,qualityRun,reversedProjection,equivalentBundle,equivalent),"Reordered replay descriptor should validate.");
for(var i=0;i<10;i++) Check(descriptor.DescriptorFingerprint==ProductionQualityEvidenceReplayDescriptorRuntime.Create(definition,production,qualityRun,evidence,bundle).DescriptorFingerprint,"Original descriptor should remain deterministic.");
for(var i=0;i<10;i++) Check(descriptor.DescriptorFingerprint.All(Uri.IsHexDigit),"Original descriptor fingerprint should remain hexadecimal.");
        assert(round==100,$"ProductionQualityEvidenceReplayDescriptor5HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
