using Asun.Device.Impl;
using Asun.Device.Contracts;
using Asun.Platform.CaptureEvidenceIntegration;
using Asun.Platform.Evidence;
using Asun.Platform.Pipeline;
using Asun.Program.Core;
using Asun.Production.Runtime;

public static class ProductionCaptureEvidenceCanonical4HundredStageSmoke
{
    public static async ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}
        var program=new InspectionProgram(
            Guid.Parse("97000000-0000-0000-0000-000000000001"),"CaptureEvidenceCanonicalProgram",new Version(1,0,0),
            new[]{new ProgramStep(Guid.Parse("98000000-0000-0000-0000-000000000001"),1,ProgramStepKind.Acquire,"Acquire",Array.Empty<ProgramParameter>())});
        var plan=ProgramExecutionPlanRuntime.Create(program);
        var pipeline=PipelineDefinitionRuntime.Create(new[]{new PipelineStage<CapturedFrame>(1,"Acquire",frame=>frame)});
        var definition=new ProductionSessionDefinition(Guid.Parse("99000000-0000-0000-0000-000000000001"),plan,pipeline,2);
        var recorder=new RecordingFrameSource(new SimulatedFrameSource(5,5,"Gray8"));
        var production=await ProductionSessionRuntime.RunAsync(definition,recorder);
        var provenance=ProductionFrameProvenanceRuntime.Create(production,recorder.Frames);
        var evidenceFrames=new[]
        {
            new ProductionEvidenceFrameReference(1,new[]{EvidenceHandle.Create("capture/1/raw"),EvidenceHandle.Create("capture/1/metadata")}),
            new ProductionEvidenceFrameReference(2,new[]{EvidenceHandle.Create("capture/2/raw"),EvidenceHandle.Create("capture/2/metadata")})
        };
        var references=ProductionCaptureEvidenceProjectionRuntime.Create(production,provenance,evidenceFrames);
        var reversedHandles=references.Select(x=>x.Sequence==1?x with {Handles=x.Handles.Reverse().ToArray()}:x).ToArray();
        var reversedFrames=references.Reverse().ToArray();
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceCanonicalRuntime.IsCanonical(production,references),"Baseline should be canonical.");
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceCanonicalRuntime.IsEquivalent(references,reversedFrames),"Frame ordering should not change canonical identity.");
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceCanonicalRuntime.IsEquivalent(references,reversedHandles),"Opaque handle ordering should not change canonical identity.");
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceCanonicalRuntime.CreateProjectionFingerprint(references)==ProductionCaptureEvidenceCanonicalRuntime.CreateProjectionFingerprint(reversedFrames),"Projection fingerprint should be frame-order independent.");
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceCanonicalRuntime.CreateFingerprintList(references).SequenceEqual(ProductionCaptureEvidenceCanonicalRuntime.CreateFingerprintList(reversedFrames)),"Fingerprint list should be frame-order independent.");
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceCanonicalRuntime.CreateFrameFingerprint(references[0])==ProductionCaptureEvidenceCanonicalRuntime.CreateFrameFingerprint(reversedHandles[0]),"Handle order should not affect frame fingerprint.");
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceCanonicalRuntime.IsCanonical(production,references),"Repeated canonical validation should remain stable.");
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceCanonicalRuntime.CreateProjectionFingerprint(references).All(char.IsAsciiHexDigit),"Projection fingerprint should be hexadecimal.");
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceCanonicalRuntime.CreateFingerprintList(references).All(x=>x.All(char.IsAsciiHexDigit)),"Frame fingerprints should be hexadecimal.");
        for(var i=0;i<10;i++) Check(references.Count==2,"Final reference cardinality");
  assert(round==100,$"ProductionCaptureEvidenceCanonical4HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
