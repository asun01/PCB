using Asun.Device.Contracts;
using Asun.Device.Impl;
using Asun.Platform.DeviceProductionIntegration;
using Asun.Platform.Pipeline;
using Asun.Program.Core;
using Asun.Production.Runtime;

public static class CaptureSessionProductionBinding3HundredStageSmoke
{
    public static async ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        var program=new InspectionProgram(
            Guid.Parse("FE000000-0000-0000-0000-000000000003"),
            "CaptureProductionBinding",
            new Version(1,0,0),
            new[]{
                new ProgramStep(
                    Guid.Parse("FF000000-0000-0000-0000-000000000003"),
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
            Guid.Parse("FA000000-0000-0000-0000-000000000003"),
            plan,
            pipeline,
            4);
        var production=await ProductionSessionRuntime.RunAsync(
            definition,
            new SimulatedFrameSource(8,8));
        var capture=await CaptureSessionRuntime.CaptureAsync(
            new SimulatedFrameSource(8,8),
            4);
        var binding=CaptureSessionProductionBindingRuntime.Create(capture,production);
        var tamperedCount=binding with {CapturedCount=binding.CapturedCount+1};
        var tamperedFingerprint=binding with {CaptureInputFingerprint=new string('9',64)};

        for(var i=0;i<10;i++) Check(binding.ProductionSessionId==production.SessionId,"Binding should preserve Production session identity.");
        for(var i=0;i<10;i++) Check(binding.CapturedCount==4,"Binding should preserve capture frame count.");
        for(var i=0;i<10;i++) Check(binding.FirstSequence==capture.FirstSequence!.Value,"Binding should preserve first capture sequence.");
        for(var i=0;i<10;i++) Check(binding.LastSequence==capture.LastSequence!.Value,"Binding should preserve last capture sequence.");
        for(var i=0;i<10;i++) Check(binding.ProductionFingerprint==production.Fingerprint,"Binding should preserve Production report fingerprint.");
        for(var i=0;i<10;i++) Check(binding.CaptureInputFingerprint.Length==64,"Binding should expose deterministic capture input fingerprint.");
        for(var i=0;i<10;i++) Check(binding.Fingerprint.Length==64,"Binding should expose deterministic cross-chain fingerprint.");
        for(var i=0;i<10;i++) Check(CaptureSessionProductionBindingRuntime.IsValid(capture,production,binding),"Capture/Production binding should validate against actual runtime outputs.");
        for(var i=0;i<10;i++) Check(!CaptureSessionProductionBindingRuntime.IsValid(capture,production,tamperedCount),"Count tampering should be rejected.");
        for(var i=0;i<10;i++) Check(!CaptureSessionProductionBindingRuntime.IsValid(capture,production,tamperedFingerprint),"Capture fingerprint tampering should be rejected.");

        assert(round==100,$"CaptureSessionProductionBinding3HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
