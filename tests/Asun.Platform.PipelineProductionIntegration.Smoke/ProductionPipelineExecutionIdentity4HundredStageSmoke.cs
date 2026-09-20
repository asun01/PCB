using Asun.Device.Impl;
using Asun.Platform.Pipeline;
using Asun.Platform.PipelineProductionIntegration;
using Asun.Program.Core;
using Asun.Production.Runtime;

public static class ProductionPipelineExecutionIdentity4HundredStageSmoke
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
        var malformed=identity with {Fingerprint="bad"};
        var emptySession=identity with {SessionId=Guid.Empty};
        for(var i=0;i<10;i++) Check(!ProductionPipelineExecutionIdentityRuntime.IsValid(definition,production,malformed),"Malformed fingerprint should be rejected.");
        for(var i=0;i<10;i++) Check(!ProductionPipelineExecutionIdentityRuntime.IsValid(definition,production,emptySession),"Session identity drift should be rejected.");
        for(var i=0;i<10;i++) Check(ProductionPipelineExecutionIdentityRuntime.Validate(definition,production,malformed).Count>0,"Malformed fingerprint should produce diagnostics.");
        for(var i=0;i<10;i++) Check(ProductionPipelineExecutionIdentityRuntime.Validate(definition,production,emptySession).Count>0,"Session drift should produce diagnostics.");
        for(var i=0;i<10;i++) Check(identity.SessionId==production.SessionId,"Baseline session should remain aligned.");
        for(var i=0;i<10;i++) Check(identity.ProgramFingerprint==production.ProgramFingerprint,"Baseline program should remain aligned.");
        for(var i=0;i<10;i++) Check(identity.Fingerprint.All(Uri.IsHexDigit),"Execution fingerprint should be hexadecimal.");
        for(var i=0;i<10;i++) Check(identity.PipelineFingerprint.All(Uri.IsHexDigit),"Pipeline fingerprint should be hexadecimal.");
        for(var i=0;i<10;i++) Check(ProductionPipelineExecutionIdentityRuntime.IsValid(definition,production,identity),"Baseline identity should remain valid.");
for(var i=0;i<10;i++) Check(ProductionPipelineExecutionIdentityRuntime.Create(definition,production).SessionId==production.SessionId,"Recreated execution identity should preserve the session identity.");

        assert(round==100,$"ProductionPipelineExecutionIdentity4HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
