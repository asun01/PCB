using Asun.Device.Impl;
using Asun.Platform.Pipeline;
using Asun.Platform.PipelineProductionIntegration;
using Asun.Program.Core;
using Asun.Production.Runtime;

public static class ProductionPipelineExecutionIdentity2HundredStageSmoke
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
        var bad=identity with {ProgramFingerprint=new string('a',64)};
        var badPipeline=identity with {PipelineFingerprint=new string('b',64)};
        for(var i=0;i<10;i++) Check(ProductionPipelineExecutionIdentityRuntime.IsValid(definition,production,identity),"Baseline execution identity should validate.");
        for(var i=0;i<10;i++) Check(!ProductionPipelineExecutionIdentityRuntime.IsValid(definition,production,bad),"Program fingerprint tampering should be rejected.");
        for(var i=0;i<10;i++) Check(!ProductionPipelineExecutionIdentityRuntime.IsValid(definition,production,badPipeline),"Pipeline fingerprint tampering should be rejected.");
        for(var i=0;i<10;i++) Check(ProductionPipelineExecutionIdentityRuntime.Validate(definition,production,bad).Count>0,"Program tampering should produce validation errors.");
        for(var i=0;i<10;i++) Check(ProductionPipelineExecutionIdentityRuntime.Validate(definition,production,badPipeline).Count>0,"Pipeline tampering should produce validation errors.");
        for(var i=0;i<10;i++) Check(identity.ProgramFingerprint==definition.ProgramPlan.Fingerprint,"Baseline program identity should remain stable.");
        for(var i=0;i<10;i++) Check(identity.PipelineFingerprint==ProductionPipelineExecutionIdentityRuntime.Create(definition,production).PipelineFingerprint,"Baseline pipeline identity should remain stable.");
        for(var i=0;i<10;i++) Check(identity.Fingerprint.Length==64,"Baseline identity fingerprint should remain valid.");
        for(var i=0;i<10;i++) Check(ProductionPipelineExecutionIdentityRuntime.IsValid(definition,production,identity),"Negative cases must not mutate the baseline.");
        assert(round==100,$"ProductionPipelineExecutionIdentity2HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
