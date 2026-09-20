namespace Asun.Platform.PipelineProductionIntegration;

public sealed record ProductionPipelineReplayFrameAudit(
    long Sequence,
    string InputFingerprint,
    int StageCount,
    IReadOnlyList<string> ExecutedStages,
    string PipelineReportFingerprint);
