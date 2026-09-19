using Asun.Device.Impl;
using Asun.Platform.Pipeline;
using Asun.Program.Core;
using Asun.Production.Runtime;

public static class ProductionCancellationHundredStageSmoke
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
            Guid.NewGuid(),
            "CancelProgram",
            new Version(1,0,0),
            new[]{
                new ProgramStep(Guid.NewGuid(),1,ProgramStepKind.Acquire,"Acquire",Array.Empty<ProgramParameter>())
            });
        var plan=ProgramExecutionPlanRuntime.Create(program);
        var pipeline=PipelineDefinitionRuntime.Create(new[]{
            new PipelineStage<Asun.Device.Contracts.CapturedFrame>(
                1,
                "Acquire",
                frame=>frame)
        });
        var definition=new ProductionSessionDefinition(
            Guid.NewGuid(),
            plan,
            pipeline,
            2);

        for(var i=0;i<10;i++)
        {
            using var cancellation=new CancellationTokenSource();
            cancellation.Cancel();

            var threw=false;
            try
            {
                await ProductionSessionRuntime.RunAsync(
                    definition,
                    new SimulatedFrameSource(4,4),
                    cancellation.Token);
            }
            catch(OperationCanceledException)
            {
                threw=true;
            }

            Check(threw,"Cancelled production session should honor cancellation.");
        }

        for(var i=0;i<10;i++) Check(plan.Fingerprint.Length==64,"Program plan should remain valid during cancellation.");
        for(var i=0;i<10;i++) Check(definition.FrameCount==2,"Production definition should retain requested frame count.");
        for(var i=0;i<10;i++) Check(pipeline.Stages.Count==1,"Production pipeline should retain one stage.");
        for(var i=0;i<10;i++) Check(ProductionSessionValidationRuntime.Validate(
            definition,
            new ProductionSessionReport(
                definition.SessionId,
                definition.ProgramPlan.Fingerprint,
                0,
                Array.Empty<ProductionFrameExecution>(),
                new string('a',64))).Count>0,"Empty production report should not validate as a completed session.");
        for(var i=0;i<10;i++) Check(ProductionSessionValidationRuntime.IsValid(
            definition,
            new ProductionSessionReport(
                definition.SessionId,
                definition.ProgramPlan.Fingerprint,
                0,
                Array.Empty<ProductionFrameExecution>(),
                new string('a',64)))==false,"Incomplete production report should remain invalid.");
        for(var i=0;i<10;i++) Check(definition.SessionId!=Guid.Empty,"Production session id should remain valid.");
        for(var i=0;i<10;i++) Check(plan.Version==new Version(1,0,0),"Production plan version should remain stable.");
        for(var i=0;i<10;i++) Check(pipeline.Stages[0].Name=="Acquire","Production pipeline stage should remain stable.");
        for(var i=0;i<10;i++) Check(ProductionSessionValidationRuntime.Validate(
            definition,
            new ProductionSessionReport(
                definition.SessionId,
                definition.ProgramPlan.Fingerprint,
                0,
                Array.Empty<ProductionFrameExecution>(),
                new string('a',64))).Count>=1,"Invalid production report should contain validation evidence.");

        assert(round==100,$"Production cancellation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
