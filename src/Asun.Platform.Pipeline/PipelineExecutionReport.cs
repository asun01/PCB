namespace Asun.Platform.Pipeline;

public sealed record PipelineExecutionReport(
    int StageCount,
    IReadOnlyList<string> ExecutedStages,
    string Fingerprint);
