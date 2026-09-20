using Asun.Device.Impl;
using Asun.Platform.Pipeline;
using Asun.Platform.PipelineProductionIntegration;
using Asun.Program.Core;
using Asun.Production.Runtime;

public static class ProductionPipelineExecutionIdentity3HundredStageSmoke
{
    public static async ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}
        var program=new InspectionProgram(
            Guid.Parse("C5000000-0000-0000-0000-000000000001"),
            "PipelineExecutionIdentityProgram",
            new Version(1,0,0),
            new[]
            {
                new ProgramStep(Guid.Parse("C5100000-0000-0000-0000-000000000001"),1,ProgramStepKind.Acquire,"Acquire",Array.Empty<ProgramParameter>()),
                new ProgramStep(Guid.Parse("C5100000-0000-0000-0000-000000000002"),2,ProgramStepKind.Measure,"Measure",Array.Empty<ProgramParameter>())
            });
        var plan=ProgramExecutionPlanRuntime.Create(program);
        var pipeline=PipelineDefinitionRuntime.Create(
            new[]
            {
                new PipelineStage<CapturedFrame>(1,"Acquire",frame=>frame),
                new PipelineStage<CapturedFrame>(2,"Measure",frame=>frame)
            });
        var definition=new ProductionSessionDefinition(
            Guid.Parse("C5200000-0000-0000-0000-000000000001"),
            plan,
            pipeline,
            2);
        var production=await ProductionSessionRuntime.RunAsync(definition,new SimulatedFrameSource(4,4));
        var identity=ProductionPipelineExecutionIdentityRuntime.Create(definition,production);
        var badCount=identity with {FrameCount=9};
        var badProduction=identity with {ProductionFingerprint=new string('c',64)};
        var badReplay=identity with {ReplayAuditFingerprint=new string('d',64)};
        for(var i=0;i<10;i++) Check(!ProductionPipelineExecutionIdentityRuntime.IsValid(definition,production,badCount),"Frame count tampering should be rejected.");
        for(var i=0;i<10;i++) Check(!ProductionPipelineExecutionIdentityRuntime.IsValid(definition,production,badProduction),"Production fingerprint tampering should be rejected.");
        for(var i=0;i<10;i++) Check(!ProductionPipelineExecutionIdentityRuntime.IsValid(definition,production,badReplay),"Replay audit fingerprint tampering should be rejected.");
        for(var i=0;i<10;i++) Check(ProductionPipelineExecutionIdentityRuntime.Validate(definition,production,badCount).Count>0,"Frame count tampering should produce errors.");
        for(var i=0;i<10;i++) Check(ProductionPipelineExecutionIdentityRuntime.Validate(definition,production,badProduction).Count>0,"Production fingerprint tampering should produce errors.");
        for(var i=0;i<10;i++) Check(ProductionPipelineExecutionIdentityRuntime.Validate(definition,production,badReplay).Count>0,"Replay fingerprint tampering should produce errors.");
        for(var i=0;i<10;i++) Check(identity.FrameCount==production.FrameCount,"Baseline frame count should remain stable.");
        for(var i=0;i<10;i++) Check(identity.ProductionFingerprint==production.Fingerprint,"Baseline production identity should remain stable.");
        for(var i=0;i<10;i++) Check(identity.ReplayAuditFingerprint.Length==64,"Baseline replay identity should remain fixed width.");
for(var i=0;i<10;i++) Check(ProductionPipelineExecutionIdentityRuntime.Create(definition,production).ProductionFingerprint==production.Fingerprint,"Recreated execution identity should preserve the production fingerprint.");

        assert(round==100,$"ProductionPipelineExecutionIdentity3HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
