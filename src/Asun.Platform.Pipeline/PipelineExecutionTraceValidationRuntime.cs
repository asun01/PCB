namespace Asun.Platform.Pipeline;

public static class PipelineExecutionTraceValidationRuntime
{
    public static IReadOnlyList<string> Validate<T>(
        PipelineDefinition<T> pipeline,
        PipelineExecutionTrace<T> trace)
    {
        ArgumentNullException.ThrowIfNull(pipeline);
        ArgumentNullException.ThrowIfNull(trace);

        var errors=new List<string>();

        if(trace.ExecutedStages.Count!=pipeline.Stages.Count)
            errors.Add("Pipeline execution trace stage count must match the pipeline.");

        var expected=pipeline.Stages.Select(stage=>stage.Name).ToArray();

        if(!trace.ExecutedStages.SequenceEqual(expected))
            errors.Add("Pipeline execution trace stage order must match the pipeline.");

        return errors;
    }

    public static bool IsValid<T>(
        PipelineDefinition<T> pipeline,
        PipelineExecutionTrace<T> trace)=>
        Validate(pipeline,trace).Count==0;
}
