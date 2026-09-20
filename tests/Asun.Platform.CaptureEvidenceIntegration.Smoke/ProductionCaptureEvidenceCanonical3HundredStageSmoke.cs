using Asun.Device.Impl;
using Asun.Device.Contracts;
using Asun.Platform.CaptureEvidenceIntegration;
using Asun.Platform.Evidence;
using Asun.Platform.Pipeline;
using Asun.Program.Core;
using Asun.Production.Runtime;

public static class ProductionCaptureEvidenceCanonical3HundredStageSmoke
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
        var handlesChanged=references.Select(x=>x.Sequence==1?x with {Handles=new[]{EvidenceHandle.Create("capture/1/other")}:x}).ToArray();
        var dimensionsChanged=references.Select(x=>x.Sequence==2?x with {Width=7}:x).ToArray();
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceCanonicalRuntime.IsCanonical(production,references),"Baseline projection should remain canonical.");
        for(var i=0;i<10;i++) Check(!ProductionCaptureEvidenceCanonicalRuntime.IsEquivalent(references,handlesChanged),"Evidence handle mutation should be observable.");
        for(var i=0;i<10;i++) Check(!ProductionCaptureEvidenceCanonicalRuntime.IsEquivalent(references,dimensionsChanged),"Dimension mutation should be observable.");
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceCanonicalRuntime.CreateProjectionFingerprint(references)!=ProductionCaptureEvidenceCanonicalRuntime.CreateProjectionFingerprint(handlesChanged),"Handle mutation should change fingerprint.");
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceCanonicalRuntime.CreateProjectionFingerprint(references)!=ProductionCaptureEvidenceCanonicalRuntime.CreateProjectionFingerprint(dimensionsChanged),"Dimension mutation should change fingerprint.");
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceCanonicalRuntime.ValidateCanonical(production,handlesChanged).Count==0,"Opaque handle mutation remains structurally valid.");
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceCanonicalRuntime.ValidateCanonical(production,dimensionsChanged).Count==0,"Positive dimension mutation remains structurally valid.");
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceCanonicalRuntime.CreateFingerprintList(handlesChanged)[0]!=ProductionCaptureEvidenceCanonicalRuntime.CreateFingerprintList(references)[0],"Changed handle frame fingerprint should differ.");
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceCanonicalRuntime.CreateFingerprintList(dimensionsChanged)[1]!=ProductionCaptureEvidenceCanonicalRuntime.CreateFingerprintList(references)[1],"Changed dimension frame fingerprint should differ.");
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceCanonicalRuntime.CreateProjectionFingerprint(references).Length==64,"Baseline projection fingerprint should remain valid.");
        assert(round==100,$"ProductionCaptureEvidenceCanonical3HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
