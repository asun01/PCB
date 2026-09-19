namespace Asun.Platform.Pipeline;

public static class PipelineDefinitionRuntime
{
    public static PipelineDefinition<T> Create<T>(
        IEnumerable<PipelineStage<T>> stages)
    {
        ArgumentNullException.ThrowIfNull(stages);

        var materialized=stages.ToArray();

        foreach(var stage in materialized)
        {
            if(!stage.IsValid)
                throw new ArgumentException(
                    "Pipeline contains an invalid stage.",
                    nameof(stages));
        }

        if(materialized.GroupBy(stage=>stage.Order).Any(group=>group.Count()>1))
            throw new ArgumentException(
                "Pipeline stage orders must be unique.",
                nameof(stages));

        if(materialized.GroupBy(stage=>stage.Name,StringComparer.Ordinal).Any(group=>group.Count()>1))
            throw new ArgumentException(
                "Pipeline stage names must be unique.",
                nameof(stages));

        return new PipelineDefinition<T>(
            materialized
                .OrderBy(stage=>stage.Order)
                .ThenBy(stage=>stage.Name,StringComparer.Ordinal)
                .ToArray());
    }
}
