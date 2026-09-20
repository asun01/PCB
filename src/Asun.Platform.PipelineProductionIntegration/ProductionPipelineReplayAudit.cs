namespace Asun.Platform.PipelineProductionIntegration;

public sealed record ProductionPipelineReplayAudit(
    Guid SessionId,
    string ProgramFingerprint,
    IReadOnlyList<ProductionPipelineReplayFrameAudit> Frames,
    string Fingerprint);
