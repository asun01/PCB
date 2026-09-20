using Asun.Device.Impl;
using Asun.Platform.Pipeline;
using Asun.Platform.PipelineProductionIntegration;
using Asun.Program.Core;
using Asun.Production.Runtime;

public static class ProductionPipelineExecutionIdentity5HundredStageSmoke
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
        var recreated=ProductionPipelineExecutionIdentityRuntime.Create(definition,production);
        var changedDefinition=new ProductionSessionDefinition(definition.SessionId,definition.ProgramPlan,
            PipelineDefinitionRuntime.Create(new[]
            {
                new PipelineStage<CapturedFrame>(1,"Acquire",frame=>frame),
                new PipelineStage<CapturedFrame>(2,"Measure",frame=>frame),
                new PipelineStage<CapturedFrame>(3,"Inspect",frame=>frame)
            }),2);
        for(var i=0;i<10;i++) Check(recreated.Fingerprint==identity.Fingerprint,"Recreated identity should be equivalent.");
        for(var i=0;i<10;i++) Check(ProductionPipelineExecutionIdentityRuntime.CreatePipelineFingerprint(definition.Pipeline)==identity.PipelineFingerprint,"Pipeline fingerprint should match canonical source.");
        for(var i=0;i<10;i++) Check(ProductionPipelineExecutionIdentityRuntime.CreatePipelineFingerprint(changedDefinition.Pipeline)!=identity.PipelineFingerprint,"Pipeline topology changes should alter pipeline identity.");
        for(var i=0;i<10;i++) Check(ProductionPipelineExecutionIdentityRuntime.Validate(definition,production,identity).Count==0,"Canonical validation should remain clean.");
        for(var i=0;i<10;i++) Check(ProductionPipelineExecutionIdentityRuntime.Create(definition,production).ProgramFingerprint==definition.ProgramPlan.Fingerprint,"Program fingerprint should remain canonical.");
        for(var i=0;i<10;i++) Check(ProductionPipelineExecutionIdentityRuntime.Create(definition,production).ProductionFingerprint==production.Fingerprint,"Production fingerprint should remain canonical.");
        for(var i=0;i<10;i++) Check(ProductionPipelineExecutionIdentityRuntime.Create(definition,production).FrameCount==production.FrameCount,"Frame count should remain canonical.");
        for(var i=0;i<10;i++) Check(identity.SessionId!=Guid.Empty,"Session identity should remain valid.");
        for(var i=0;i<10;i++) Check(identity.ProgramFingerprint.Length==64,"Program fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(identity.ReplayAuditFingerprint.Length==64,"Replay audit identity should be fixed width.");
        assert(round==100,$"ProductionPipelineExecutionIdentity5HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
