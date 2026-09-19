namespace Asun.Platform.Pipeline;

public sealed record PipelineExecutionTrace<T>(
    T Input,
    T Output,
    IReadOnlyList<string> ExecutedStages);
