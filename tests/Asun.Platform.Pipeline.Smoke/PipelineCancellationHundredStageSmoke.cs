using Asun.Platform.Pipeline;

public static class PipelineCancellationHundredStageSmoke
{
    public static async ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        for(var i=0;i<10;i++)
        {
            using var cancellation=new CancellationTokenSource();
            cancellation.Cancel();
            var pipeline=PipelineDefinitionRuntime.Create(new[]{
                new PipelineStage<string>(1,"A",value=>value+"A"),
                new PipelineStage<string>(2,"B",value=>value+"B")
            });
            var threw=false;
            try
            {
                await PipelineExecutionRuntime.ExecuteAsync(pipeline,"",cancellation.Token);
            }
            catch(OperationCanceledException)
            {
                threw=true;
            }
            Check(threw,"Cancelled pipeline should honor cancellation.");
        }

        for(var i=0;i<10;i++)
        {
            var threw=false;
            try
            {
                PipelineDefinitionRuntime.Create(new[]{
                    new PipelineStage<string>(1,"A",value=>value),
                    new PipelineStage<string>(1,"B",value=>value)
                });
            }
            catch(ArgumentException)
            {
                threw=true;
            }
            Check(threw,"Duplicate stage order should be rejected.");
        }

        for(var i=0;i<10;i++)
        {
            var threw=false;
            try
            {
                PipelineDefinitionRuntime.Create(new[]{
                    new PipelineStage<string>(1,"A",value=>value),
                    new PipelineStage<string>(2,"A",value=>value)
                });
            }
            catch(ArgumentException)
            {
                threw=true;
            }
            Check(threw,"Duplicate stage name should be rejected.");
        }

        for(var i=0;i<10;i++)
        {
            var pipeline=PipelineDefinitionRuntime.Create(new[]{
                new PipelineStage<string>(2,"B",value=>value+"B"),
                new PipelineStage<string>(1,"A",value=>value+"A")
            });
            Check(pipeline.Stages[0].Name=="A" && pipeline.Stages[1].Name=="B","Pipeline creation should canonicalize stage order.");
        }

        for(var i=0;i<10;i++)
        {
            var pipeline=PipelineDefinitionRuntime.Create(new[]{
                new PipelineStage<string>(1,"A",value=>value+"A")
            });
            var trace=await PipelineExecutionRuntime.ExecuteAsync(pipeline,"");
            Check(trace.Output=="A","Single-stage pipeline should execute its transform.");
        }

        for(var i=0;i<10;i++)
        {
            var pipeline=PipelineDefinitionRuntime.Create(new[]{
                new PipelineStage<string>(1,"A",value=>value+"A"),
                new PipelineStage<string>(2,"B",value=>value+"B")
            });
            var trace=await PipelineExecutionRuntime.ExecuteAsync(pipeline,"");
            Check(trace.ExecutedStages.Count==2,"Trace should contain every executed stage.");
        }

        for(var i=0;i<10;i++)
        {
            var pipeline=PipelineDefinitionRuntime.Create(new[]{
                new PipelineStage<string>(1,"A",value=>value+"A")
            });
            var trace=await PipelineExecutionRuntime.ExecuteAsync(pipeline,"");
            Check(PipelineExecutionTraceValidationRuntime.IsValid(pipeline,trace),"Trace should validate for a successful execution.");
        }

        for(var i=0;i<10;i++)
        {
            var pipeline=PipelineDefinitionRuntime.Create(new[]{
                new PipelineStage<string>(1,"A",value=>value+"A")
            });
            var trace=await PipelineExecutionRuntime.ExecuteAsync(pipeline,"");
            var report=PipelineExecutionReportRuntime.Create(pipeline,trace);
            Check(PipelineExecutionReportValidationRuntime.IsValid(report),"Execution report should validate.");
        }

        for(var i=0;i<10;i++)
        {
            var pipeline=PipelineDefinitionRuntime.Create(new[]{
                new PipelineStage<string>(1,"A",value=>value+"A"),
                new PipelineStage<string>(2,"B",value=>value+"B"),
                new PipelineStage<string>(3,"C",value=>value+"C")
            });
            var trace=await PipelineExecutionRuntime.ExecuteAsync(pipeline,"");
            Check(trace.Output=="ABC","Three-stage pipeline should compose transforms.");
        }

        for(var i=0;i<10;i++)
        {
            var pipeline=PipelineDefinitionRuntime.Create(new[]{
                new PipelineStage<string>(1,"A",value=>value+"A")
            });
            var trace=await PipelineExecutionRuntime.ExecuteAsync(pipeline,"");
            Check(trace.ExecutedStages.SequenceEqual(new[]{"A"}),"Single-stage trace should preserve the stage name.");
        }

        assert(round==100,$"Pipeline cancellation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
