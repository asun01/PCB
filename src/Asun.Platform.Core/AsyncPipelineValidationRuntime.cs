namespace Asun.Platform.Core;

public static class AsyncPipelineValidationRuntime
{
    public static IReadOnlyList<string> Validate<TContext>(
        AsyncPipeline<TContext> pipeline)
    {
        ArgumentNullException.ThrowIfNull(pipeline);

        var errors = new List<string>();
        var ids = pipeline.NodeIds;

        if (ids.Count == 0)
            errors.Add("Pipeline must expose at least one node.");

        if (ids.Distinct(StringComparer.Ordinal).Count() != ids.Count)
            errors.Add("Pipeline node ids must be unique.");

        var levels = pipeline.GetExecutionLayers();

        if (levels.SelectMany(level => level)
                .Distinct(StringComparer.Ordinal)
                .Count() != ids.Count)
        {
            errors.Add("Execution layers must cover every pipeline node exactly once.");
        }

        for (var level = 0; level < levels.Count; level++)
        {
            if (!levels[level].SequenceEqual(
                    levels[level].OrderBy(id => id, StringComparer.Ordinal)))
            {
                errors.Add("Execution layer ids must be deterministically ordered.");
            }

            foreach (var id in levels[level])
            {
                foreach (var dependency in pipeline.GetDependencies(id))
                {
                    if (pipeline.GetExecutionLevel(dependency) >= level)
                        errors.Add("Pipeline dependency must execute before its dependent.");
                }
            }
        }

        return errors;
    }

    public static bool IsValid<TContext>(AsyncPipeline<TContext> pipeline) =>
        Validate(pipeline).Count == 0;
}
