using Asun.Platform.Pipeline;

public static class PipelineExecutionHundredStageSmoke
{
    public static async ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var pipeline=PipelineDefinitionRuntime.Create(new[]{
            new PipelineStage<string>(1,"Normalize",value=>value.Trim()),
            new PipelineStage<string>(2,"Append",value=>value+"|A"),
            new PipelineStage<string>(3,"Measure",value=>value.Length.ToString()),
            new PipelineStage<string>(4,"Finalize",value=>"R:"+value)
        });

        var trace=await PipelineExecutionRuntime.ExecuteAsync(pipeline,"  pcb  ");
        var report=PipelineExecutionReportRuntime.Create(pipeline,trace);
        var invalid=report with {StageCount=3};

        for(var i=0;i<10;i++) Check(trace.Output=="R:5","Pipeline should apply all four stages.");
        for(var i=0;i<10;i++) Check(trace.ExecutedStages.SequenceEqual(new[]{"Normalize","Append","Measure","Finalize"}),"Pipeline trace should preserve stage order.");
        for(var i=0;i<10;i++) Check(PipelineExecutionTraceValidationRuntime.IsValid(pipeline,trace),"Pipeline trace should validate.");
        for(var i=0;i<10;i++) Check(report.StageCount==4,"Pipeline report should preserve four stages.");
        for(var i=0;i<10;i++) Check(PipelineExecutionReportValidationRuntime.IsValid(report),"Pipeline report should validate.");
        for(var i=0;i<10;i++) Check(!PipelineExecutionReportValidationRuntime.IsValid(invalid),"Tampered pipeline report should be rejected.");
        for(var i=0;i<10;i++) Check(report.Fingerprint.Length==64,"Pipeline report fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(report.Fingerprint.All(Uri.IsHexDigit),"Pipeline report fingerprint should be hexadecimal.");
        for(var i=0;i<10;i++) Check(PipelineDefinitionRuntime.Create(pipeline.Stages).Stages.SequenceEqual(pipeline.Stages),"Pipeline definition recreation should be deterministic.");
        for(var i=0;i<10;i++) Check(pipeline.Stages.Count==4,"Pipeline should retain four stages.");

        assert(round==100,$"Pipeline execution smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
