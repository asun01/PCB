namespace Asun.Platform.Pipeline;

public sealed class PipelineDefinition<T>
{
    public PipelineDefinition(
        IReadOnlyList<PipelineStage<T>> stages)
    {
        ArgumentNullException.ThrowIfNull(stages);
        Stages=stages.ToArray();
    }

    public IReadOnlyList<PipelineStage<T>> Stages { get; }
}
