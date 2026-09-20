using Asun.Device.Impl;
using Asun.Device.Contracts;
using Asun.Platform.CaptureEvidenceIntegration;
using Asun.Platform.Evidence;
using Asun.Platform.Pipeline;
using Asun.Program.Core;
using Asun.Production.Runtime;

public static class ProductionCaptureEvidenceCanonical5HundredStageSmoke
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
        var duplicate=references.Concat(new[]{references[0]}).ToArray();
        var missing=references.Skip(1).ToArray();
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceCanonicalRuntime.ValidateCanonical(production,references).Count==0,"Baseline canonical validation should pass.");
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceCanonicalRuntime.ValidateCanonical(production,duplicate).Count>0,"Duplicate frame should be rejected.");
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceCanonicalRuntime.ValidateCanonical(production,missing).Count>0,"Missing frame should be rejected.");
        for(var i=0;i<10;i++) Check(!ProductionCaptureEvidenceCanonicalRuntime.IsEquivalent(references,duplicate),"Duplicate frame should not be equivalent.");
        for(var i=0;i<10;i++) Check(!ProductionCaptureEvidenceCanonicalRuntime.IsEquivalent(references,missing),"Missing frame should not be equivalent.");
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceCanonicalRuntime.CreateFingerprintList(duplicate).Count==3,"Duplicate descriptor should expose extra frame.");
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceCanonicalRuntime.CreateFingerprintList(missing).Count==1,"Missing descriptor should expose truncation.");
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceCanonicalRuntime.CreateProjectionFingerprint(references)!=ProductionCaptureEvidenceCanonicalRuntime.CreateProjectionFingerprint(duplicate),"Duplicate should alter projection fingerprint.");
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceCanonicalRuntime.CreateProjectionFingerprint(references)!=ProductionCaptureEvidenceCanonicalRuntime.CreateProjectionFingerprint(missing),"Missing frame should alter projection fingerprint.");
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceCanonicalRuntime.IsCanonical(production,references),"Valid projection should remain canonical.");
        assert(round==100,$"ProductionCaptureEvidenceCanonical5HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
