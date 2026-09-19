namespace Asun.Platform.Pipeline;

public static class PipelineExecutionRuntime
{
    public static async ValueTask<PipelineExecutionTrace<T>> ExecuteAsync<T>(
        PipelineDefinition<T> pipeline,
        T input,
        CancellationToken cancellationToken=default)
    {
        ArgumentNullException.ThrowIfNull(pipeline);

        if(pipeline.Stages.Count==0)
            throw new ArgumentException(
                "Pipeline must contain at least one stage.",
                nameof(pipeline));

        var current=input;
        var executed=new List<string>(pipeline.Stages.Count);

        foreach(var stage in pipeline.Stages)
        {
            cancellationToken.ThrowIfCancellationRequested();

            current=stage.Execute(current);
            executed.Add(stage.Name);

            await Task.Yield();
        }

        return new PipelineExecutionTrace<T>(
            input,
            current,
            executed);
    }
}
