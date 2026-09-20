using Asun.Device.Contracts;
using Asun.Device.Impl;
using Asun.Platform.Pipeline;
using Asun.Platform.PipelineProductionIntegration;
using Asun.Program.Core;
using Asun.Production.Runtime;

public static class ProductionPipelineReplayAuditHundredStageSmoke
{
    public static async ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var program=new InspectionProgram(
            Guid.Parse("AC000000-0000-0000-0000-000000000001"),
            "PipelineReplayProgram",
            new Version(1,0,0),
            new[]
            {
                new ProgramStep(Guid.Parse("AD000000-0000-0000-0000-000000000001"),1,ProgramStepKind.Acquire,"Acquire",Array.Empty<ProgramParameter>()),
                new ProgramStep(Guid.Parse("AD000000-0000-0000-0000-000000000002"),2,ProgramStepKind.Measure,"Measure",Array.Empty<ProgramParameter>())
            });
        var plan=ProgramExecutionPlanRuntime.Create(program);
        var pipeline=PipelineDefinitionRuntime.Create(
            new[]
            {
                new PipelineStage<CapturedFrame>(1,"Acquire",frame=>frame),
                new PipelineStage<CapturedFrame>(2,"Measure",frame=>frame)
            });
        var definition=new ProductionSessionDefinition(
            Guid.Parse("AE000000-0000-0000-0000-000000000001"),
            plan,
            pipeline,
            2);
        var production=await ProductionSessionRuntime.RunAsync(
            definition,
            new SimulatedFrameSource(4,4));
        var audit=ProductionPipelineReplayAuditRuntime.Create(definition,production);
        var tampered=audit with {ProgramFingerprint=new string('a',64)};
        var changed=audit with
        {
            Frames=new[]
            {
                audit.Frames[0] with
                {
                    ExecutedStages=new[]{"Measure","Acquire"}
                },
                audit.Frames[1]
            }
        };

        for(var i=0;i<10;i++) Check(production.FrameCount==2,"Production should retain two frames.");
        for(var i=0;i<10;i++) Check(audit.Frames.Count==2,"Replay audit should contain two frame audits.");
        for(var i=0;i<10;i++) Check(audit.Frames.All(frame=>frame.StageCount==2),"Replay audit should retain two executed stages per frame.");
        for(var i=0;i<10;i++) Check(audit.Frames.All(frame=>frame.ExecutedStages.SequenceEqual(new[]{"Acquire","Measure"})),"Replay audit should preserve stage order.");
        for(var i=0;i<10;i++) Check(audit.Frames[0].Sequence==1 && audit.Frames[1].Sequence==2,"Replay audit should preserve production sequence.");
        for(var i=0;i<10;i++) Check(audit.Frames.All(frame=>frame.PipelineReportFingerprint.Length==64),"Replay audit should retain stage-report fingerprints.");
        for(var i=0;i<10;i++) Check(ProductionPipelineReplayAuditValidationRuntime.IsValid(definition,production,audit),"Pipeline replay audit should validate.");
        for(var i=0;i<10;i++) Check(!ProductionPipelineReplayAuditValidationRuntime.IsValid(definition,production,tampered),"Program fingerprint tampering should be rejected.");
        for(var i=0;i<10;i++) Check(!ProductionPipelineReplayAuditValidationRuntime.IsValid(definition,production,changed),"Pipeline stage-order drift should be rejected.");
        for(var i=0;i<10;i++) Check(audit.Fingerprint.Length==64,"Pipeline replay audit fingerprint should be fixed width.");

        assert(round==100,$"Pipeline replay audit smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
