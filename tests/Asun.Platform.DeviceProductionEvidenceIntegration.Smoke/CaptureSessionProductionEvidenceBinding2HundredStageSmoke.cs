using Asun.Device.Contracts;
using Asun.Device.Impl;
using Asun.Platform.CaptureEvidenceIntegration;
using Asun.Platform.DeviceProductionEvidenceIntegration;
using Asun.Platform.DeviceProductionIntegration;
using Asun.Platform.Pipeline;
using Asun.Platform.Evidence;
using Asun.Program.Core;
using Asun.Production.Runtime;

public static class CaptureSessionProductionEvidenceBinding2HundredStageSmoke
{
    public static async ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        var program=new InspectionProgram(
            Guid.Parse("F1000000-0000-0000-0000-000000000002"),
            "CaptureEvidenceBinding",
            new Version(1,0,0),
            new[]{
                new ProgramStep(
                    Guid.Parse("F2000000-0000-0000-0000-000000000002"),
                    1,
                    ProgramStepKind.Acquire,
                    "Acquire",
                    new[]{new ProgramParameter("source","simulated")})
            });
        var plan=ProgramExecutionPlanRuntime.Create(program);
        var pipeline=PipelineDefinitionRuntime.Create(new[]{
            new PipelineStage<CapturedFrame>(
                1,
                "Acquire",
                frame=>CapturedFrame.Create(frame.Metadata,frame.Payload.ToArray()))
        });
        var definition=new ProductionSessionDefinition(
            Guid.Parse("F3000000-0000-0000-0000-000000000002"),
            plan,
            pipeline,
            3);
        var production=await ProductionSessionRuntime.RunAsync(
            definition,
            new SimulatedFrameSource(8,8));
        var captureSession=await CaptureSessionRuntime.CaptureAsync(
            new SimulatedFrameSource(8,8),
            3);

        var frameSource=new SimulatedFrameSource(8,8);
        var frame1=await frameSource.CaptureAsync();
        var frame2=await frameSource.CaptureAsync();
        var frame3=await frameSource.CaptureAsync();
        var frames=new[]{frame1,frame2,frame3};
        var provenance=ProductionFrameProvenanceRuntime.Create(production,frames);
        var evidenceReferences=new ProductionCaptureEvidenceFrameReference[]
        {
            new(1,frames[0].PayloadFingerprint,frames[0].Metadata.Width,frames[0].Metadata.Height,frames[0].Metadata.PixelFormat,new[]{EvidenceHandle.Create("capture/1/a")}),
            new(2,frames[1].PayloadFingerprint,frames[1].Metadata.Width,frames[1].Metadata.Height,frames[1].Metadata.PixelFormat,new[]{EvidenceHandle.Create("capture/2/a")}),
            new(3,frames[2].PayloadFingerprint,frames[2].Metadata.Width,frames[2].Metadata.Height,frames[2].Metadata.PixelFormat,new[]{EvidenceHandle.Create("capture/3/a")}),
        };
        var productionBinding=CaptureSessionProductionBindingRuntime.Create(
            captureSession,
            production);
        var binding=CaptureSessionProductionEvidenceBindingRuntime.Create(
            productionBinding,
            production,
            provenance,
            evidenceReferences);
        var tamperedBinding=binding with
        {
            ProductionBindingFingerprint=new string('9',64)
        };
        var tamperedReferences=evidenceReferences
            .Select(reference=>reference.Sequence==1
                ? reference with {PayloadFingerprint=new string('8',64)}
                : reference)
            .ToArray();

        for(var i=0;i<10;i++) Check(binding.ProductionSessionId==production.SessionId,"Binding should preserve Production session identity.");
        for(var i=0;i<10;i++) Check(binding.ProductionBindingFingerprint==productionBinding.Fingerprint,"Binding should preserve Device/Production binding identity.");
        for(var i=0;i<10;i++) Check(binding.FrameCount==3,"Binding should preserve Evidence frame count.");
        for(var i=0;i<10;i++) Check(binding.EvidenceCaptureProjectionFingerprint.Length==64,"Evidence projection fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(binding.Fingerprint.Length==64,"Cross-chain Device/Production/Evidence fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(provenance.Count==3,"Every Production frame should have matching provenance.");
        for(var i=0;i<10;i++) Check(evidenceReferences.Select(reference=>reference.Sequence).SequenceEqual(Enumerable.Range(1,3)),"Evidence reference sequences should remain contiguous.");
        for(var i=0;i<10;i++) Check(CaptureSessionProductionEvidenceBindingRuntime.IsValid(productionBinding,production,provenance,evidenceReferences,binding),"Device/Production/Evidence binding should validate against actual runtime facts.");
        for(var i=0;i<10;i++) Check(!CaptureSessionProductionEvidenceBindingRuntime.IsValid(productionBinding,production,provenance,evidenceReferences,tamperedBinding),"Binding identity tampering should be rejected.");
        for(var i=0;i<10;i++) Check(!CaptureSessionProductionEvidenceBindingRuntime.IsValid(productionBinding,production,provenance,tamperedReferences,binding),"Evidence payload tampering should be rejected.");

        assert(round==100,$"CaptureSessionProductionEvidenceBinding2HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
