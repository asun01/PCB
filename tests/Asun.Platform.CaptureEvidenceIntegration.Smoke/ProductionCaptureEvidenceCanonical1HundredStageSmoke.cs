using Asun.Device.Impl;
using Asun.Device.Contracts;
using Asun.Platform.CaptureEvidenceIntegration;
using Asun.Platform.Evidence;
using Asun.Platform.Pipeline;
using Asun.Program.Core;
using Asun.Production.Runtime;

public static class ProductionCaptureEvidenceCanonical1HundredStageSmoke
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
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceCanonicalRuntime.IsCanonical(production,references),"Canonical capture evidence projection should validate.");
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceCanonicalRuntime.CreateFingerprintList(references).Count==2,"Frame fingerprint list should cover both frames.");
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceCanonicalRuntime.CreateFingerprintList(references).All(x=>x.Length==64),"Frame fingerprints should be SHA-256 width.");
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceCanonicalRuntime.CreateProjectionFingerprint(references).Length==64,"Projection fingerprint should be SHA-256 width.");
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceCanonicalRuntime.IsEquivalent(references,references),"Projection should be equivalent to itself.");
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceCanonicalRuntime.IsEquivalent(references,references.Reverse().ToArray()),"Projection fingerprint should be sequence-canonical.");
        for(var i=0;i<10;i++) Check(references.All(x=>x.PayloadFingerprint.Length==64),"Payload identities should remain fixed width.");
        for(var i=0;i<10;i++) Check(references.All(x=>x.Handles.All(h=>h.IsValid)),"Opaque evidence handles should remain valid.");
        for(var i=0;i<10;i++) Check(references.All(x=>x.Width>0 && x.Height>0),"Capture dimensions should remain positive.");
        for(var i=0;i<10;i++) Check(references.All(x=>!string.IsNullOrWhiteSpace(x.PixelFormat)),"Pixel formats should remain explicit.");
        assert(round==100,$"ProductionCaptureEvidenceCanonical1HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
