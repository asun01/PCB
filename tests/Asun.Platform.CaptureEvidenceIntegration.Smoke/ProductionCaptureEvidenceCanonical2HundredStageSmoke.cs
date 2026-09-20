using Asun.Device.Impl;
using Asun.Device.Contracts;
using Asun.Platform.CaptureEvidenceIntegration;
using Asun.Platform.Evidence;
using Asun.Platform.Pipeline;
using Asun.Program.Core;
using Asun.Production.Runtime;

public static class ProductionCaptureEvidenceCanonical2HundredStageSmoke
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
        var tampered=references.Select(x=>x.Sequence==1?x with {PixelFormat="}:bad":x}).ToArray();
        var blank=references.Select(x=>x.Sequence==2?x with {PixelFormat=""}:x).ToArray();
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceCanonicalRuntime.ValidateCanonical(production,references).Count==0,"Baseline canonical validation should pass.");
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceCanonicalRuntime.ValidateCanonical(production,tampered).Count>0,"Pixel format tampering should be rejected.");
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceCanonicalRuntime.ValidateCanonical(production,blank).Count>0,"Blank pixel format should be rejected.");
        for(var i=0;i<10;i++) Check(!ProductionCaptureEvidenceCanonicalRuntime.IsEquivalent(references,tampered),"Pixel format mutation should change projection identity.");
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceCanonicalRuntime.CreateProjectionFingerprint(references)!=ProductionCaptureEvidenceCanonicalRuntime.CreateProjectionFingerprint(tampered),"Pixel format mutation should change fingerprint.");
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceCanonicalRuntime.ValidateCanonical(production,references).Count==0,"Baseline should remain valid after negative cases.");
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceCanonicalRuntime.CreateFingerprintList(references).Count==2,"Fingerprint list cardinality should remain stable.");
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceCanonicalRuntime.CreateProjectionFingerprint(references).Length==64,"Projection fingerprint width should remain stable.");
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceCanonicalRuntime.IsEquivalent(references,references.ToArray()),"Equivalent copies should remain equivalent.");
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceCanonicalRuntime.CreateFingerprintList(references).SequenceEqual(ProductionCaptureEvidenceCanonicalRuntime.CreateFingerprintList(references.Reverse().ToArray())),"Frame fingerprints should be order-canonical.");
        assert(round==100,$"ProductionCaptureEvidenceCanonical2HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
